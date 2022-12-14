using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable, NetSerializable]
public sealed class BankUiState : BoundUserInterfaceState
{
    public string? LinkedAccountNumber;
    public FixedPoint2? LinkedAccountBalance;
    public string? CurrencySymbol;
    public BankUiState(string? linkedAccountNumber = null, FixedPoint2? linkedAccountBalance = null, string? currencySymbol = null)
    {
        LinkedAccountNumber = linkedAccountNumber;
        LinkedAccountBalance = linkedAccountBalance;
        CurrencySymbol = currencySymbol;
    }
}
