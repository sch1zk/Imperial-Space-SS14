using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Stack;
using Content.Shared.Access.Components;
using Content.Shared.Economy.ATM;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Store;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;
using System.Linq;
using static Content.Shared.Economy.ATM.SharedATMComponent;

namespace Content.Server.Economy.ATM
{
    public sealed class ATMSystem : SharedATMSystem
    {
        [Dependency] private readonly IPrototypeManager _proto = default!;
        [Dependency] private readonly IEntityManager _entities = default!;
        [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;
        [Dependency] private readonly BankManagerSystem _bankManagerSystem = default!;
        [Dependency] private readonly StackSystem _stack = default!;
        [Dependency] private readonly SharedHandsSystem _hands = default!;
        [Dependency] private readonly AudioSystem _audioSystem = default!;
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<ATMComponent, PowerChangedEvent>(OnPowerChanged);
            SubscribeLocalEvent<ATMComponent, ComponentStartup>((_, comp, _) => UpdateComponentUserInterface(comp));
            SubscribeLocalEvent<ATMComponent, EntInsertedIntoContainerMessage>((_, comp, _) => UpdateComponentUserInterface(comp));
            SubscribeLocalEvent<ATMComponent, EntRemovedFromContainerMessage>((_, comp, _) => UpdateComponentUserInterface(comp));
            SubscribeLocalEvent<ATMComponent, ATMRequestWithdrawMessage>(OnRequestWithdraw);
        }
        private void OnPowerChanged(EntityUid uid, ATMComponent component, ref PowerChangedEvent args)
        {
            TryUpdateVisualState(uid, component);
        }
        public void TryUpdateVisualState(EntityUid uid, ATMComponent? component = null)
        {
            if (!Resolve(uid, ref component))
                return;

            var finalState = ATMVisualState.Normal;
            if (!this.IsPowered(uid, EntityManager))
            {
                finalState = ATMVisualState.Off;
            }
            if (TryComp<AppearanceComponent>(component.Owner, out var appearance))
            {
                _appearanceSystem.SetData(uid, ATMVisuals.VisualState, finalState, appearance);
            }
        }
        private void UpdateComponentUserInterface(ATMComponent component)
        {
            string? idCardFullName = null;
            string? idCardEntityName = null;
            string? idCardStoredBankAccountNumber = null;
            var haveAccessToBankAccount = false;
            FixedPoint2? bankAccountBalance = null;
            if (component.IdCardSlot.Item is { Valid: true } idCardEntityUid)
            {
                if (_entities.TryGetComponent<IdCardComponent>(idCardEntityUid, out var idCardComponent))
                {
                    idCardFullName = idCardComponent.FullName;
                    if (_bankManagerSystem.TryGetBankAccount(idCardComponent.StoredBankAccountNumber, idCardComponent.StoredBankAccountPin, out var bankAccount))
                    {
                        idCardStoredBankAccountNumber = idCardComponent.StoredBankAccountNumber;
                        if (bankAccount.AccountPin.Equals(idCardComponent.StoredBankAccountPin))
                        {
                            haveAccessToBankAccount = true;
                            bankAccountBalance = bankAccount.Balance;
                        }
                    }
                }
                idCardEntityName = _entities.GetComponent<MetaDataComponent>(idCardEntityUid)?.EntityName;
            }
            var newState = new ATMBoundUserInterfaceState(
                component.IdCardSlot.HasItem,
                idCardFullName,
                idCardEntityName,
                idCardStoredBankAccountNumber,
                haveAccessToBankAccount,
                bankAccountBalance
                );
            component.UpdateUserInterface(newState);
        }
        private void OnRequestWithdraw(EntityUid uid, ATMComponent component, ATMRequestWithdrawMessage msg)
        {
            if (msg.Session.AttachedEntity is not { Valid: true } buyer)
                return;
            if (msg.Amount <= 0)
            {
                Deny(component);
                return;
            }
            if (!TryGetBankAccountDetailsFromStoredIdCard(component, out var bankAccountNumber, out var bankAccountPin))
            {
                Deny(component);
                return;
            }
            if (component.CurrencyWhitelist.Count == 0)
            {
                Deny(component);
                return;
            }
            var currency = component.CurrencyWhitelist.First();
            if (!_proto.TryIndex<CurrencyPrototype>(currency, out var proto))
            {
                Deny(component);
                return;
            }
            if (proto.Cash == null || !proto.CanWithdraw)
            {
                Deny(component);
                return;
            }

            var amountRemaining = msg.Amount;
            if (!_bankManagerSystem.TryWithdrawFromBankAccount(
                bankAccountNumber, bankAccountPin,
                new KeyValuePair<string, FixedPoint2>(currency, amountRemaining)))
            {
                Deny(component);
                return;
            }

            //FixedPoint2 amountRemaining = msg.Amount;
            var coordinates = Transform(buyer).Coordinates;
            var sortedCashValues = proto.Cash.Keys.OrderByDescending(x => x).ToList();
            foreach (var value in sortedCashValues)
            {
                var cashId = proto.Cash[value];
                var amountToSpawn = (int) MathF.Floor((float) (amountRemaining / value));
                var ents = _stack.SpawnMultiple(cashId, amountToSpawn, coordinates);
                _hands.PickupOrDrop(buyer, ents.First());
                amountRemaining -= value * amountToSpawn;
            }
            Apply(component);
            _audioSystem.PlayPvs(component.SoundWithdrawCurrency, component.Owner, AudioParams.Default.WithVolume(-2f));
            UpdateComponentUserInterface(component);
        }
        public bool TryAddCurrency(Dictionary<string, FixedPoint2> currency, ATMComponent component)
        {
            foreach (var type in currency)
            {
                if (!component.CurrencyWhitelist.Contains(type.Key))
                    return false;
            }
            if (!TryGetBankAccountDetailsFromStoredIdCard(component, out var bankAccountNumber, out var bankAccountPin))
                return false;

            foreach (var type in currency)
            {
                if (!_bankManagerSystem.TryInsertToBankAccount(bankAccountNumber, bankAccountPin, type))
                    return false;
            }
            _audioSystem.PlayPvs(component.SoundInsertCurrency, component.Owner, AudioParams.Default.WithVolume(-2f));
            UpdateComponentUserInterface(component);
            return true;
        }
        private bool TryGetBankAccountDetailsFromStoredIdCard(ATMComponent component, out string storedBankAccountNumber, out string storedBankAccountPin)
        {
            storedBankAccountNumber = string.Empty;
            storedBankAccountPin = string.Empty;
            if (component.IdCardSlot.Item is not { Valid: true } idCardEntityUid)
                return false;
            if (!_entities.TryGetComponent<IdCardComponent>(idCardEntityUid, out var idCardComponent))
                return false;
            if (idCardComponent.StoredBankAccountNumber == null || idCardComponent.StoredBankAccountPin == null)
                return false;
            storedBankAccountNumber = idCardComponent.StoredBankAccountNumber;
            storedBankAccountPin = idCardComponent.StoredBankAccountPin;
            return true;
        }
        private void Deny(ATMComponent component)
        {
            _audioSystem.PlayPvs(component.SoundDeny, component.Owner, AudioParams.Default.WithVolume(-2f));
        }
        private void Apply(ATMComponent component)
        {
            _audioSystem.PlayPvs(component.SoundApply, component.Owner, AudioParams.Default.WithVolume(-2f));
        }
    }
}
