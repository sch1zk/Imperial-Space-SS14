using Content.Client.Message;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Store.Ui;

[GenerateTypedNameReferences]
public sealed partial class StoreConfirmWindow : DefaultWindow
{
    public StoreConfirmWindow()
    {
        RobustXamlLoader.Load(this);
        ConfirmText.SetMarkup(Loc.GetString("store-ui-confirm-text"));
        ConfirmButton.OnButtonDown += _ => Close();
    }
}
