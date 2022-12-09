using Content.Server.Mind;

namespace Content.Server.Economy.Components;

/// <summary>
/// Added to the abstract representation of a station to track its money.
/// </summary>
[RegisterComponent, Access(typeof(MindSystem))]
public sealed class BankAccountComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField("balance")]
    public int Balance;

    public BankAccountComponent(int balance = 0)
    {
        Balance = balance;
    }
}
