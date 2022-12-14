using Content.Server.Economy;
using Robust.Shared.Serialization;

namespace Content.Server.CartridgeLoader.Cartridges
{
    [RegisterComponent]
    public sealed class BankCartridgeComponent : Component
    {
        [ViewVariables]
        public BankAccount? LinkedBankAccount { get; set; }
    }
}
