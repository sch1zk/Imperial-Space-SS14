using Content.Shared.Economy.ATM;
using Robust.Shared.GameObjects;
using Robust.Shared.Random;
using System.Diagnostics.CodeAnalysis;

namespace Content.Shared.Economy
{
    public sealed class BankManagerSystem : EntitySystem
    {
        private static Dictionary<string, BankAccountComponent> _activeBankAccounts = new();
        public override void Initialize()
        {
            base.Initialize();
        }
        public bool TryGetBankAccount(string storedBankAccountNumber, [MaybeNullWhen(false)] out BankAccountComponent bankAccountComponent)
        {
            _activeBankAccounts.TryGetValue(storedBankAccountNumber, out bankAccountComponent);
            return bankAccountComponent != null ? true : false;
        }
        public BankAccountComponent? CreateNewBankAccount()
        {
            int bankAccountNumber;
            var random = IoCManager.Resolve<IRobustRandom>();
            do
            {
                bankAccountNumber = random.Next(111111, 999999);
            } while (_activeBankAccounts.ContainsKey(bankAccountNumber.ToString()));
            string bankAccountPin = GenerateBankAccountPin(random);
            string bankAccountNumberStr = bankAccountNumber.ToString();
            BankAccountComponent bankAccountComponent = new BankAccountComponent(bankAccountNumberStr, bankAccountPin);
            return _activeBankAccounts.TryAdd(bankAccountNumberStr, bankAccountComponent)
                ? bankAccountComponent
                : null;
        }
        private string GenerateBankAccountPin(IRobustRandom random)
        {
            var pin = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                pin += random.Next(0, 9).ToString();
            }
            return pin;
        }
    }
}
