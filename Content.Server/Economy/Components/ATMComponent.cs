using Content.Server.Economy.Systems;
using Content.Server.UserInterface;
using Content.Shared.Access.Components;
using Content.Shared.Economy;
using Content.Shared.Economy.ATM;
using Robust.Server.GameObjects;

namespace Content.Server.Economy.Components
{
    [RegisterComponent]
    [ComponentReference(typeof(SharedATMComponent))]
    [Access(typeof(ATMSystem))]
    public sealed class ATMComponent : SharedATMComponent
    {
        
        //[Dependency] private readonly BankManagerSystem _bankManagerSystem = default!;
        [ViewVariables] private BoundUserInterface? UserInterface => Owner.GetUIOrNull(ATMUiKey.Key);
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
