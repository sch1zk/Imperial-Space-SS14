using Content.Shared.MachineLinking;
using Content.Server.MachineLinking.System;

namespace Content.Server.DeviceLinking.Components
{
    /// <summary>
    ///     Simple switch that will fire ports when toggled on or off. A button is jsut a switch that signals on the
    ///     same port regardless of its state.
    /// </summary>
    [RegisterComponent]
    [ComponentReference(typeof(SharedSignalSwitchComponent))]
    [Access(typeof(SignalSwitchSystem))]
    public sealed class SignalSwitchComponent : SharedSignalSwitchComponent
    {
        /// <summary>
        ///     The port that gets signaled when the switch turns on.
        /// </summary>
        //[DataField("onPort", customTypeSerializer: typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
        //public string OnPort = "On";

        /// <summary>
        ///     The port that gets signaled when the switch turns off.
        /// </summary>
        //[DataField("offPort", customTypeSerializer: typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
        //public string OffPort = "Off";

        //[DataField("state")]
        //public bool State;

        //[DataField("clickSound")]
        //public SoundSpecifier ClickSound { get; set; } = new SoundPathSpecifier("/Audio/Machines/lightswitch.ogg");
    }
}
