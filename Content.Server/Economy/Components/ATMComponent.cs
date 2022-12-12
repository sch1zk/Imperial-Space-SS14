using Content.Server.Economy.Systems;
using Content.Server.UserInterface;
using Content.Shared.Access.Components;
using Content.Shared.Economy;
using Content.Shared.Economy.ATM;
using Content.Shared.Store;
using Robust.Server.GameObjects;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.Economy.Components
{
    [RegisterComponent]
    [ComponentReference(typeof(SharedATMComponent))]
    [Access(typeof(ATMSystem))]
    public sealed class ATMComponent : SharedATMComponent
    {
        
        //[Dependency] private readonly BankManagerSystem _bankManagerSystem = default!;
        [ViewVariables] private BoundUserInterface? UserInterface => Owner.GetUIOrNull(ATMUiKey.Key);
        [ViewVariables(VVAccess.ReadOnly), DataField("currencyWhitelist", customTypeSerializer: typeof(PrototypeIdHashSetSerializer<CurrencyPrototype>))]
        public HashSet<string> CurrencyWhitelist = new();
        protected override void Initialize()
        {
            base.Initialize();
            Owner.EnsureComponentWarn<ServerUserInterfaceComponent>();
        }
        public void UpdateUserInterface(ATMBoundUserInterfaceState state)
        {
            if (!Initialized || UserInterface == null)
                return;
            UserInterface.SetState(state);
        }
    }
}
