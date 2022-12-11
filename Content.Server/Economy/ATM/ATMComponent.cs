using Content.Server.UserInterface;
using Content.Shared.Access.Components;
using Content.Shared.Economy.ATM;
using Robust.Server.GameObjects;
using static Content.Shared.Access.Components.SharedIdCardConsoleComponent;

namespace Content.Server.Economy.ATM
{
    [RegisterComponent]
    [ComponentReference(typeof(SharedATMComponent))]
    [Access(typeof(ATMSystem))]
    public sealed class ATMComponent : SharedATMComponent
    {
        [ViewVariables] private BoundUserInterface? UserInterface => Owner.GetUIOrNull(ATMUiKey.Key);
        protected override void Initialize()
        {
            base.Initialize();
            Owner.EnsureComponentWarn<ServerUserInterfaceComponent>();
        }
        public void UpdateUserInterface()
        {
            if (!Initialized)
                return;
            if (UserInterface != null)
            {
                var newState = new ATMBoundUserInterfaceState(IdCardSlot.HasItem);
                UserInterface.SetState(newState);
            }
        }
    }
}
