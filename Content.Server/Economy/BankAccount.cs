using Content.Shared.FixedPoint;

namespace Content.Server.Economy;
public sealed class BankAccount
{
    [ViewVariables(VVAccess.ReadOnly)]
    public string AccountNumber { get; }
    [ViewVariables(VVAccess.ReadOnly)]
    public string AccountPin { get; }
    [ViewVariables(VVAccess.ReadWrite)]
    public string? AccountName { get; set; }

    [ViewVariables(VVAccess.ReadWrite)]
    public FixedPoint2 Balance { get; set; }
    [ViewVariables(VVAccess.ReadOnly)]
    public string CurrencyType { get; }
    public BankAccount(string accountNumber, string accountPin, string currencyType = "SpaceCredits", string? accountName = null)
    {
        AccountNumber = accountNumber;
        AccountPin = accountPin;
        AccountName = accountName;
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
