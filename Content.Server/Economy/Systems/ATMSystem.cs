using Content.Server.Economy.Components;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Access.Components;
using Content.Shared.Economy;
using Content.Shared.Economy.ATM;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using static Content.Shared.Economy.ATM.SharedATMComponent;

namespace Content.Server.Economy.Systems
{
    public sealed class ATMSystem : SharedATMSystem
    {
        [Dependency] private readonly IEntityManager _entities = default!;
        [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;
        [Dependency] private readonly BankManagerSystem _bankManagerSystem = default!;
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<ATMComponent, PowerChangedEvent>(OnPowerChanged);
            SubscribeLocalEvent<ATMComponent, ComponentStartup>((_, comp, _) => UpdateComponentUserInterface(comp));
            SubscribeLocalEvent<ATMComponent, EntInsertedIntoContainerMessage>((_, comp, _) => UpdateComponentUserInterface(comp));
            SubscribeLocalEvent<ATMComponent, EntRemovedFromContainerMessage>((_, comp, _) => UpdateComponentUserInterface(comp));
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


    }
}
