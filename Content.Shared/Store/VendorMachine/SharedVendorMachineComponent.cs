using Content.Shared.Actions;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Store.VendorMachine
{
    [NetworkedComponent()]
    public abstract class SharedVendorMachineComponent : Component
    {
        /// <summary>
        /// Used by the server to determine how long the vending machine stays in the "Deny" state.
        /// Used by the client to determine how long the deny animation should be played.
        /// </summary>
        [DataField("denyDelay")]
        public float DenyDelay = 2.0f;

        /// <summary>
        /// Used by the server to determine how long the vending machine stays in the "Eject" state.
        /// The selected item is dispensed afer this delay.
        /// Used by the client to determine how long the deny animation should be played.
        /// </summary>
        [DataField("ejectDelay")]
        public float EjectDelay = 1.2f;

        [Serializable, NetSerializable]
        public enum VendorMachineVisuals
        {
            VisualState
        }

        [Serializable, NetSerializable]
        public enum VendorMachineVisualState
        {
            Normal,
            Off,
            Broken,
            Eject,
            Deny
        }
    }
}
