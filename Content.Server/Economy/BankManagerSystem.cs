using Content.Shared.Economy;
using Content.Shared.FixedPoint;
using Content.Shared.Roles;
using Robust.Shared.Random;
using System.Diagnostics.CodeAnalysis;

namespace Content.Server.Economy
{
    public sealed class BankManagerSystem : EntitySystem
    {
        [Dependency] private readonly IRobustRandom _robustRandom = default!;

        private static Dictionary<string, BankAccount> _activeBankAccounts = new();
        public override void Initialize()
        {
            base.Initialize();
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
        public bool IsBankAccountExists(string? bankAccountNumber)
        {
            if (bankAccountNumber == null)
                return false;
            return _activeBankAccounts.ContainsKey(bankAccountNumber);
        }
        public BankAccount? CreateNewBankAccount(int? bankAccountNumber = null)
        {
            if(bankAccountNumber == null)
            {
                int p;
                do
                {
                    p = _robustRandom.Next(111111, 999999);
                } while (_activeBankAccounts.ContainsKey(p.ToString()));
                bankAccountNumber = p;
            }
            var bankAccountPin = GenerateBankAccountPin();
            var bankAccountNumberStr = bankAccountNumber.ToString();
            var bankAccount = new BankAccount(bankAccountNumberStr, bankAccountPin);
            return _activeBankAccounts.TryAdd(bankAccountNumberStr, bankAccount)
                ? bankAccount
                : null;
        }
        private string GenerateBankAccountPin()
        {
            var pin = string.Empty;
            for (var i = 0; i < 4; i++)
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
        public bool TryTransferFromToBankAccount(string? bankAccountFromNumber, string? bankAccountFromPin, string? bankAccountNumberTo, FixedPoint2 amount)
        {
            if (bankAccountFromNumber == null || bankAccountNumberTo == null)
                return false;
            if (!TryGetBankAccount(bankAccountFromNumber, bankAccountFromPin, out var bankAccountFrom))
                return false;
            if (!_activeBankAccounts.TryGetValue(bankAccountNumberTo, out var bankAccountTo))
                return false;
            if (bankAccountFrom.CurrencyType != bankAccountTo.CurrencyType)
                return false;
            if (bankAccountFrom.TryChangeBalanceBy(-amount))
            {
                return bankAccountTo.TryChangeBalanceBy(amount);
            }
            return false;
        }
        public bool TryGetBankAccountCurrencyType(string? bankAccountNumber, out string? currencyType)
        {
            currencyType = null;
            if (bankAccountNumber == null)
                return false;
            if (!_activeBankAccounts.TryGetValue(bankAccountNumber, out var bankAccount))
                return false;
            currencyType = bankAccount.CurrencyType;
            return true;
        }
        public string? GetBankAccountName(string? bankAccountNumber)
        {
            if (bankAccountNumber == null)
                return null;
            if (!_activeBankAccounts.TryGetValue(bankAccountNumber, out var bankAccount))
                return null;
            return bankAccount.AccountName;
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
