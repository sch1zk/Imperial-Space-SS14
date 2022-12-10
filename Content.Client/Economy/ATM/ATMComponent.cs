using Content.Shared.Economy.ATM;

namespace Content.Client.Economy.ATM
{
    [RegisterComponent]
    [ComponentReference(typeof(SharedATMComponent))]
    [Access(typeof(ATMSystem))]
    public sealed class ATMComponent : SharedATMComponent
    {
        [DataField("offState")]
        public string? OffState;
        [DataField("normalState")]
        public string? NormalState;
    }
}
