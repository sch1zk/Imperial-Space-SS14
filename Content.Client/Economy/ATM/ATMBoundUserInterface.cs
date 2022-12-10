using Content.Client.Economy.ATM.UI;
using Content.Client.VendingMachines.UI;
using Robust.Client.GameObjects;

namespace Content.Client.Economy.ATM
{
    public sealed class ATMBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private ATMMenu? _menu;

        public ATMBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
        {
        }
        protected override void Open()
        {
            base.Open();
            _menu = new ATMMenu { Title = IoCManager.Resolve<IEntityManager>().GetComponent<MetaDataComponent>(Owner.Owner).EntityName };
            _menu.OnClose += Close;
            _menu.OpenCentered();
        }
        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;

            if (_menu == null)
                return;

            _menu.OnClose -= Close;
            _menu.Dispose();
        }
    }
}
