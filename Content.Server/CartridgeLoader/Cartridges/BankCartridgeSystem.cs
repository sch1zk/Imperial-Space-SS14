using Content.Server.Store.Components;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Content.Shared.Store;
using Robust.Shared.Prototypes;

namespace Content.Server.CartridgeLoader.Cartridges;

public sealed class BankCartridgeSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly CartridgeLoaderSystem? _cartridgeLoaderSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BankCartridgeComponent, CartridgeMessageEvent>(OnUiMessage);
        SubscribeLocalEvent<BankCartridgeComponent, CartridgeUiReadyEvent>(OnUiReady);
    }
    private void OnUiReady(EntityUid uid, BankCartridgeComponent component, CartridgeUiReadyEvent args)
    {
        UpdateUiState(uid, args.Loader, component);
    }
    private void OnUiMessage(EntityUid uid, BankCartridgeComponent component, CartridgeMessageEvent args)
    {
        UpdateUiState(uid, args.LoaderUid, component);
    }
    private void UpdateUiState(EntityUid uid, EntityUid loaderUid, BankCartridgeComponent? component)
    {
        if (!Resolve(uid, ref component))
            return;

        var state = new BankUiState();
        if (component.LinkedBankAccount!= null)
        {
            state.LinkedAccountNumber = component.LinkedBankAccount.AccountNumber;
            state.LinkedAccountBalance = component.LinkedBankAccount.Balance;
            if (component.LinkedBankAccount.CurrencyType != null && _prototypeManager.TryIndex(component.LinkedBankAccount.CurrencyType, out CurrencyPrototype? p))
                state.CurrencySymbol = Loc.GetString(p.CurrencySymbol);
        }
        _cartridgeLoaderSystem?.UpdateCartridgeUiState(loaderUid, state);
    }
}
