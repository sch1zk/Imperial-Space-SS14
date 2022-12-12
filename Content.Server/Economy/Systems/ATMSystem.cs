using Content.Server.Economy.Components;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Access.Components;
using Content.Shared.Economy;
using Content.Shared.Economy.ATM;
using Content.Shared.FixedPoint;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Content.Server.Stack;
using static Content.Shared.Economy.ATM.SharedATMComponent;
using Content.Shared.Hands.EntitySystems;
using System.Linq;
using Content.Server.Store.Components;
using Content.Shared.Interaction;
using Robust.Shared.Player;
using Content.Shared.Popups;

namespace Content.Server.Economy.Systems
{
    public sealed class ATMSystem : SharedATMSystem
    {
        [Dependency] private readonly IEntityManager _entities = default!;
        [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;
        [Dependency] private readonly BankManagerSystem _bankManagerSystem = default!;
        [Dependency] private readonly StackSystem _stack = default!;
        [Dependency] private readonly SharedHandsSystem _hands = default!;
        [Dependency] private readonly SharedPopupSystem _popup = default!;
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
            bool haveAccessToBankAccount = false;
            string? bankAccountBalance = null;
            if (component.IdCardSlot.Item is { Valid: true } idCardEntityUid)
            {
                if (_entities.TryGetComponent<IdCardComponent>(idCardEntityUid, out var idCardComponent))
                {
                    idCardFullName = idCardComponent.FullName;
                    if (
                        idCardComponent.StoredBankAccountNumber != null &&
                        _bankManagerSystem.TryGetBankAccount(idCardComponent.StoredBankAccountNumber, out var bankAccountComponent))
                    {
                        idCardStoredBankAccountNumber = idCardComponent.StoredBankAccountNumber;
                        if (bankAccountComponent.AccountPin.Equals(idCardComponent.StoredBankAccountPin))
                        {
                            haveAccessToBankAccount = true;
                            bankAccountBalance = bankAccountComponent.Balance.ToString();
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
            if (msg.Amount <= 0) return;
            if (!TryGetBankAccounNumberFromStoredIdCard(component, out var bankAccountNumber))
                return;
            if (!_bankManagerSystem.TryWithdrawFromBankAccount(bankAccountNumber, msg.Amount))
                return;
            var coordinates = Transform(buyer).Coordinates;
            var ents = _stack.SpawnMultiple("SpaceCredit", msg.Amount, coordinates);
            _hands.PickupOrDrop(buyer, ents.First());
            UpdateComponentUserInterface(component);
        }
        public bool TryInsert(int amount, ATMComponent component)
        {
            if(amount <= 0) return false;
            if(!TryGetBankAccounNumberFromStoredIdCard(component, out var bankAccountNumber))
                return false;
            if (!_bankManagerSystem.TryInsertToBankAccount(bankAccountNumber, amount))
                return false;
            UpdateComponentUserInterface(component);
            return true;
        }
        public int GetCurrencyValue(CurrencyComponent component)
        {
            TryComp<StackComponent>(component.Owner, out var stack);
            var amount = stack?.Count ?? 1;
            if(component.Price.TryGetValue("SpaceCredits", out FixedPoint2 result))
                return result.Int() * amount;
            return 0;
        }
        private bool TryGetBankAccounNumberFromStoredIdCard(ATMComponent component, out string storedBankAccountNumber)
        {
            storedBankAccountNumber = string.Empty;
            if (component.IdCardSlot.Item is not { Valid: true } idCardEntityUid)
                return false;
            if (!_entities.TryGetComponent<IdCardComponent>(idCardEntityUid, out var idCardComponent))
                return false;
            if (idCardComponent.StoredBankAccountNumber == null)
                return false;
            storedBankAccountNumber = idCardComponent.StoredBankAccountNumber;
            return true;
        }
    }
}
