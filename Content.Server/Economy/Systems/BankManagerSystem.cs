using Content.Shared.Economy;
using Content.Shared.FixedPoint;
using Content.Shared.Roles;
using Robust.Shared.Random;
using System.Diagnostics.CodeAnalysis;

namespace Content.Server.Economy.Systems
{
    public sealed class BankManagerSystem : EntitySystem
    {
        [Dependency] private readonly IRobustRandom _robustRandom = default!;

        private static Dictionary<string, BankAccount> _activeBankAccounts = new();
        public override void Initialize()
        {
            base.Initialize();
            Logger.InfoS("bankmanager", $"Initializing bankmanager, active bank accounts: {_activeBankAccounts.Count}");
        }
        public bool TryGetBankAccount(string? bankAccountNumber, string? bankAccountPin, [MaybeNullWhen(false)] out BankAccount bankAccount)
        {
            bankAccount = null;
            if (bankAccountNumber == null || bankAccountPin == null)
                return false;
            _activeBankAccounts.TryGetValue(bankAccountNumber, out bankAccount);
            if (bankAccount == null)
                return false;
            if (bankAccountNumber != bankAccount.AccountNumber || bankAccountPin != bankAccount.AccountPin)
                return false;
            return true;
        }
        public BankAccount? CreateNewBankAccount()
        {
            int bankAccountNumber;
            do
            {
                bankAccountNumber = _robustRandom.Next(111111, 999999);
            } while (_activeBankAccounts.ContainsKey(bankAccountNumber.ToString()));
            string bankAccountPin = GenerateBankAccountPin();
            string bankAccountNumberStr = bankAccountNumber.ToString();
            BankAccount bankAccount = new BankAccount(bankAccountNumberStr, bankAccountPin);
            Logger.InfoS("bankmanager", $"Creating new account, active bank accounts: {_activeBankAccounts.Count}");
            return _activeBankAccounts.TryAdd(bankAccountNumberStr, bankAccount)
                ? bankAccount
                : null;
        }
        private string GenerateBankAccountPin()
        {
            var pin = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                pin += _robustRandom.Next(0, 9).ToString();
            }
            return pin;
        }
        public bool TryWithdrawFromBankAccount(string? bankAccountNumber, string? bankAccountPin, KeyValuePair<string, FixedPoint2> currency)
        {
            if (!TryGetBankAccount(bankAccountNumber, bankAccountPin, out var bankAccount))
                return false;
            if (currency.Key != bankAccount.CurrencyType)
                return false;
            return bankAccount.TryChangeBalanceBy(-currency.Value);
        }
        public bool TryInsertToBankAccount(string? bankAccountNumber, string? bankAccountPin, KeyValuePair<string, FixedPoint2> currency)
        {
            if (!TryGetBankAccount(bankAccountNumber, bankAccountPin, out var bankAccount))
                return false;
            if (currency.Key != bankAccount.CurrencyType)
                return false;
            return bankAccount.TryChangeBalanceBy(currency.Value);
        }
        public void TryGenerateStartingBalance(BankAccount bankAccount, JobPrototype jobPrototype)
        {
            if (jobPrototype.MaxBankBalance > 0)
            {
                var newBalance = FixedPoint2.New(_robustRandom.Next(jobPrototype.MinBankBalance, jobPrototype.MaxBankBalance));
                bankAccount.SetBalance(newBalance);
            }
        }
        public void Clear()
        {
            _activeBankAccounts.Clear();
        }
    }
}
