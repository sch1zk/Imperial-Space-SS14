using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using static Content.Shared.Store.VendorMachine.SharedVendorMachineComponent;
using Robust.Server.GameObjects;

namespace Content.Server.Store.VendorMachine;

public sealed class VendorMachineSystem : EntitySystem
{
    [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<VendorMachineComponent, PowerChangedEvent>(OnPowerChanged);
    }
    private void OnPowerChanged(EntityUid uid, VendorMachineComponent component, ref PowerChangedEvent args)
    {
        TryUpdateVisualState(uid, component);
    }
    public void TryUpdateVisualState(EntityUid uid, VendorMachineComponent? vendComponent = null)
    {
        if (!Resolve(uid, ref vendComponent))
            return;

        var finalState = VendorMachineVisualState.Normal;
        if (vendComponent.Broken)
        {
            finalState = VendorMachineVisualState.Broken;
        }
        else if (vendComponent.Ejecting)
        {
            finalState = VendorMachineVisualState.Eject;
        }
        else if (vendComponent.Denying)
        {
            finalState = VendorMachineVisualState.Deny;
        }
        else if (!this.IsPowered(uid, EntityManager))
        {
            finalState = VendorMachineVisualState.Off;
        }

        if (TryComp<AppearanceComponent>(vendComponent.Owner, out var appearance))
        {
            _appearanceSystem.SetData(uid, VendorMachineVisuals.VisualState, finalState, appearance);
        }
    }
}
