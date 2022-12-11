using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Economy.ATM
{
    [NetworkedComponent()]
    public abstract class SharedATMComponent : Component
    {
        public static string IdCardSlotId = "ATM-IdCard";

        [DataField("idCardSlot")]
        public ItemSlot IdCardSlot = new();

        [Serializable, NetSerializable]
        public sealed class ATMBoundUserInterfaceState : BoundUserInterfaceState
        {
            public readonly bool IsCardPresent;
            public ATMBoundUserInterfaceState(bool isCardPresent)
            {
                IsCardPresent = isCardPresent;
            }
        }
        [Serializable, NetSerializable]
        public sealed class InsertIdCardMessage : BoundUserInterfaceMessage { }
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
