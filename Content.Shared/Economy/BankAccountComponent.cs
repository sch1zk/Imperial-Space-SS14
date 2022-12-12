using Content.Shared.FixedPoint;
using Robust.Shared.Random;

namespace Content.Shared.Economy;

[RegisterComponent]
public sealed class BankAccountComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly), DataField("accountNumber")]
    public string AccountNumber { get; }
    [ViewVariables(VVAccess.ReadOnly), DataField("accountPin")]
    public string AccountPin { get; }

    [ViewVariables(VVAccess.ReadWrite), DataField("balance")]
    public FixedPoint2 Balance { get; set; }
    [ViewVariables(VVAccess.ReadOnly), DataField("currencyType")]
    public string CurrencyType { get; } = "SpaceCredits";
    public BankAccountComponent(string accountNumber, string accountPin)
    {
        AccountNumber = accountNumber;
        AccountPin = accountPin;
        Balance = 0;
    }
    public bool TryChangeBalanceBy(FixedPoint2 amount)
    {
        if (Balance + amount < 0)
            return false;
        SetBalance(Balance + amount);
        return true;
    }
    public void SetBalance(FixedPoint2 newValue)
    {
        Balance = FixedPoint2.Clamp(newValue, 0, FixedPoint2.MaxValue);
    }
}
