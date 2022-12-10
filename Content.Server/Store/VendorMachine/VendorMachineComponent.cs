using Content.Shared.Store.VendorMachine;
using Content.Shared.VendingMachines;

namespace Content.Server.Store.VendorMachine
{
    [RegisterComponent]
    [ComponentReference(typeof(SharedVendorMachineComponent))]
    [Access(typeof(VendorMachineSystem))]
    public sealed class VendorMachineComponent : SharedVendorMachineComponent
    {
        public bool Ejecting;
        public bool Denying;
        public bool Broken;
    }
}
