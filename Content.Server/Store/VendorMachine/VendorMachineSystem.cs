using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using static Content.Shared.Store.VendorMachine.SharedVendorMachineComponent;
using Robust.Server.GameObjects;
using Content.Server.Store.Components;
using Content.Shared.Interaction;
using Robust.Shared.Audio;

namespace Content.Server.Store.VendorMachine;

public sealed class VendorMachineSystem : EntitySystem
{
    [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<VendorMachineComponent, PowerChangedEvent>(OnPowerChanged);
        SubscribeLocalEvent<StoreComponent, StoreOnDenyEvent>(OnDeny);
        SubscribeLocalEvent<StoreComponent, StoreOnEjectEvent>(OnEject);
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
    private void OnDeny(EntityUid uid, StoreComponent component, StoreOnDenyEvent args)
    {
        if (TryComp<VendorMachineComponent>(component.Owner, out var vendorComponent))
        {
            ShowDeny(uid, vendorComponent);
        }
    }

    private void OnEject(EntityUid uid, StoreComponent component, StoreOnEjectEvent args)
    {
        if (TryComp<VendorMachineComponent>(component.Owner, out var vendorComponent))
        {
            ShowEject(uid, vendorComponent);
        }
    }

    private void ShowDeny(EntityUid uid, VendorMachineComponent? component)
    {
        if (!Resolve(uid, ref component))
            return;
        if (component.Denying)
            return;

        component.Denying = true;
        TryUpdateVisualState(uid, component);
    }

    private void ShowEject(EntityUid uid, VendorMachineComponent? component)
    {
        if (!Resolve(uid, ref component))
            return;
        if (component.Ejecting)
            return;

        component.Ejecting = true;
        TryUpdateVisualState(uid, component);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        foreach (var comp in EntityQuery<VendorMachineComponent>())
        {
            if (comp.Ejecting)
            {
                comp.EjectAccumulator += frameTime;
                if (comp.EjectAccumulator >= comp.EjectDelay)
                {
                    comp.EjectAccumulator = 0f;
                    comp.Ejecting = false;
                    TryUpdateVisualState(comp.Owner, comp);
                }
            }

            if (comp.Denying)
            {
                comp.DenyAccumulator += frameTime;
                if (comp.DenyAccumulator >= comp.DenyDelay)
                {
                    comp.DenyAccumulator = 0f;
                    comp.Denying = false;
                    TryUpdateVisualState(comp.Owner, comp);
                }
            }
        }
    }
}
