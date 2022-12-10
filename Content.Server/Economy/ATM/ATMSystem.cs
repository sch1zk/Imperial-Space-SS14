using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Economy.ATM;
using Robust.Server.GameObjects;
using static Content.Shared.Store.VendorMachine.SharedVendorMachineComponent;

namespace Content.Server.Economy.ATM
{
    public sealed class ATMSystem : EntitySystem
    {
        [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<ATMComponent, PowerChangedEvent>(OnPowerChanged);

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


    }
}
