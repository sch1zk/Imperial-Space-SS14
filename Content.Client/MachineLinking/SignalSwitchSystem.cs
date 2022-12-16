using Content.Client.Store.VendorMachine;
using Content.Shared.MachineLinking;
using Content.Shared.Toggleable;
using Robust.Client.GameObjects;

namespace Content.Client.MachineLinking;
public sealed class SignalSwitchSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SignalSwitchComponent, AppearanceChangeEvent>(OnAppearanceChange);
    }

    private void OnAppearanceChange(EntityUid uid, SignalSwitchComponent? component, ref AppearanceChangeEvent args)
    {
        if (!Resolve(uid, ref component))
            return;

        if (args.Sprite == null)
            return;

        if (!args.Component.TryGetData(SignalSwitchVisuals.On, out bool enabled))
            return;

        args.Sprite.LayerSetState(SignalSwitchVisualLayers.BaseUnshaded, enabled ? "on_unshaded" : "off_unshaded");
    }
}

public enum SignalSwitchVisualLayers : byte
{
    Base,
    BaseUnshaded
}
