using Content.Shared.FixedPoint;
using Robust.Shared.Random;

namespace Content.Server.Economy.Components;

/// <summary>
/// Added to the abstract representation of a station to track its money.
/// </summary>
[RegisterComponent]
public sealed class BankAccountComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField("balance")]
    public FixedPoint2 Balance { get; set; } = 0;
    [ViewVariables(VVAccess.ReadOnly), DataField("currencyType")]
    public string CurrencyType { get; } = "SpaceCredits";
    public BankAccountComponent(int minBalance, int maxBalance)
    {
       var random = IoCManager.Resolve<IRobustRandom>();
       Balance = FixedPoint2.New(random.Next(minBalance, maxBalance));
    }
}
