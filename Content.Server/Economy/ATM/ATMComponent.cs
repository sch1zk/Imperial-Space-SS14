using Content.Shared.Economy.ATM;
using Content.Shared.Store.VendorMachine;

namespace Content.Server.Economy.ATM
{
    [RegisterComponent]
    [ComponentReference(typeof(SharedATMComponent))]
    [Access(typeof(ATMSystem))]
    public sealed class ATMComponent : SharedATMComponent
    {
    }
}
