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
    public BankAccountComponent(string accountNumber, string accountPin, int minBalance, int maxBalance)
    {
        AccountNumber = accountNumber;
        AccountPin = accountPin;
        var random = IoCManager.Resolve<IRobustRandom>();
        Balance = FixedPoint2.New(random.Next(minBalance, maxBalance));
    }
}
