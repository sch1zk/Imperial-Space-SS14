using Content.Server.DeviceLinking.Components;
using Content.Server.MachineLinking.System;
using Content.Shared.Audio;
using Content.Shared.Interaction;
using Content.Shared.Toggleable;
using Content.Shared.MachineLinking;
using Robust.Shared.Audio;
using Robust.Shared.Player;

namespace Content.Server.DeviceLinking.Systems
{
    public sealed class SignalSwitchSystem : EntitySystem
    {
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
        [Dependency] private readonly DeviceLinkSystem _signalSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<SignalSwitchComponent, ComponentInit>(OnInit);
            SubscribeLocalEvent<SignalSwitchComponent, ActivateInWorldEvent>(OnActivated);
        }

        private void OnInit(EntityUid uid, SignalSwitchComponent component, ComponentInit args)
        {
            _signalSystem.EnsureSourcePorts(uid, component.OnPort, component.OffPort);
        }

        private void OnActivated(EntityUid uid, SignalSwitchComponent component, ActivateInWorldEvent args)
        {
            if (args.Handled)
                return;

            component.State = !component.State;
            _signalSystem.InvokePort(uid, component.State ? component.OnPort : component.OffPort);
            SoundSystem.Play(component.ClickSound.GetSound(), Filter.Pvs(component.Owner), component.Owner,
                AudioHelpers.WithVariation(0.125f).WithVolume(8f));

            args.Handled = true;

            UpdateVisuals(uid, component);
        }

       private void UpdateVisuals(EntityUid uid, SignalSwitchComponent? component = null, AppearanceComponent? appearance = null)
        {
            if (!Resolve(uid, ref component, ref appearance, false))
                return;

            _appearance.SetData(uid, SignalSwitchVisuals.On, component.State, appearance);
        }
    }
}
