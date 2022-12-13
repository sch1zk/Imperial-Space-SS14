using Content.Shared.FixedPoint;

namespace Content.Server.Economy;
public sealed class BankAccount
{
    [ViewVariables(VVAccess.ReadOnly), DataField("accountNumber")]
    public string AccountNumber { get; }
    [ViewVariables(VVAccess.ReadOnly), DataField("accountPin")]
    public string AccountPin { get; }

    [ViewVariables(VVAccess.ReadWrite), DataField("balance")]
    public FixedPoint2 Balance { get; set; }
    [ViewVariables(VVAccess.ReadOnly), DataField("currencyType")]
    public string CurrencyType { get; }
    public BankAccount(string accountNumber, string accountPin, string currencyType = "SpaceCredits")
    {
        AccountNumber = accountNumber;
        AccountPin = accountPin;
        Balance = 0;
        CurrencyType = currencyType;
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
