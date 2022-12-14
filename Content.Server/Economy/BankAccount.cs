using Content.Shared.FixedPoint;

namespace Content.Server.Economy;
public sealed class BankAccount
{
    public event Action? OnChangeValue;

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
    [ViewVariables(VVAccess.ReadOnly)]
    public bool IsInfinite { get; }
    public BankAccount(string accountNumber, string accountPin, string currencyType = "SpaceCredits", string? accountName = null, bool isInfinite = false)
    {
        AccountNumber = accountNumber;
        AccountPin = accountPin;
        AccountName = accountName;
        Balance = 0;
        CurrencyType = currencyType;
        IsInfinite = isInfinite;
    }
    public bool TryChangeBalanceBy(FixedPoint2 amount)
    {
        if (IsInfinite)
            return true;
        if (Balance + amount < 0)
            return false;
        SetBalance(Balance + amount);
        return true;
    }
    public void SetBalance(FixedPoint2 newValue)
    {
        OnChangeValue?.Invoke();
        Balance = FixedPoint2.Clamp(newValue, 0, FixedPoint2.MaxValue);
    }
}
