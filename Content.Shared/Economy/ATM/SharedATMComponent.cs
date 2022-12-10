using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Economy.ATM
{
    [NetworkedComponent()]
    public abstract class SharedATMComponent : Component
    {
    }

    [Serializable, NetSerializable]
    public enum ATMVisuals
    {
        VisualState
    }

    [Serializable, NetSerializable]
    public enum ATMVisualState
    {
        Normal,
        Off
    }
}
