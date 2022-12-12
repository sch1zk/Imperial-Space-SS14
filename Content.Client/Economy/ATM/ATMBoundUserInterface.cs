using Content.Client.Economy.ATM.UI;
using Content.Shared.Economy.ATM;
using Content.Shared.Containers.ItemSlots;
using Robust.Client.GameObjects;
using static Content.Shared.Economy.ATM.SharedATMComponent;

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

            _menu.IdCardButton.OnPressed += _ => SendMessage(new ItemSlotButtonPressedEvent(IdCardSlotId));
            _menu.OnWithdrawAttempt += (_, amount) => SendMessage(new ATMRequestWithdrawMessage(amount));

            _menu.OnClose += Close;
            _menu.OpenCentered();
        }
        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
            var castState = (ATMBoundUserInterfaceState) state;
            _menu?.UpdateState(castState);
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
