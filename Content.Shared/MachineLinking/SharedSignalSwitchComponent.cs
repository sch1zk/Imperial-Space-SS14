using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.MachineLinking
{
    /// <summary>
    ///     Simple switch that will fire ports when toggled on or off. A button is jsut a switch that signals on the
    ///     same port regardless of its state.
    /// </summary>
    [NetworkedComponent()]
    public abstract class SharedSignalSwitchComponent : Component
    {
        /// <summary>
        ///     The port that gets signaled when the switch turns on.
        /// </summary>
        [DataField("onPort", customTypeSerializer: typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
        public string OnPort = "On";

        /// <summary>
        ///     The port that gets signaled when the switch turns off.
        /// </summary>
        [DataField("offPort", customTypeSerializer: typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
        public string OffPort = "Off";

        [DataField("state")]
        public bool State;

        [DataField("clickSound")]
        public SoundSpecifier ClickSound { get; set; } = new SoundPathSpecifier("/Audio/Machines/lightswitch.ogg");
    }

    [Serializable, NetSerializable]
    public enum SignalSwitchVisuals
    {
        On
    }
}
