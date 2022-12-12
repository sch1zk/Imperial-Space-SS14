using Robust.Shared.Serialization;

namespace Content.Shared.Economy.ATM
{
    [Serializable, NetSerializable]
    public enum ATMUiKey : byte
    {
        Key
    }
    [Serializable, NetSerializable]
    public sealed class ATMRequestWithdrawMessage : BoundUserInterfaceMessage
    {
        public int Amount;
        public ATMRequestWithdrawMessage(int amount)
        {
            Amount = amount;
        }
    }
}
