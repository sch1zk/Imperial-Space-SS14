using Content.Shared.CCVar;
using Content.Shared.FixedPoint;
using Content.Shared.Roles;
using Robust.Shared.Configuration;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Server.Economy.Wage
{
    public sealed class WageManagerSystem : EntitySystem
    {
        [Dependency] private readonly IConfigurationManager _configurationManager = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly BankManagerSystem _bankManagerSystem = default!;
        private static List<Payout> PayoutsList = new();
        public bool WagesEnabled { get; private set; }
        //private void SetEnabled(bool value) => WagesEnabled = value;
        private void SetEnabled(bool value)
        {
            WagesEnabled = value;
        }
        public override void Initialize()
        {
            base.Initialize();
            _configurationManager.OnValueChanged(CCVars.EconomyWagesEnabled, SetEnabled, true);
        }
        public override void Shutdown()
        {
            base.Shutdown();
            _configurationManager.UnsubValueChanged(CCVars.EconomyWagesEnabled, SetEnabled);
        }
        public void Payday()
        {
            foreach (var payout in PayoutsList)
            {
                _bankManagerSystem.TryTransferFromToBankAccount(
                    payout.FromAccountNumber,
                    payout.FromAccountPin,
                    payout.ToAccountNumber,
                    payout.PayoutAmount);
            }
        }
        public bool TryAddAccountToWagePayoutList(BankAccount bankAccount, JobPrototype jobPrototype)
        {
            if (jobPrototype.Department == null || !_prototypeManager.TryIndex(jobPrototype.Department, out DepartmentPrototype? department))
                return false;
            if (department == null || !_bankManagerSystem.TryGetBankAccount(department.AccountNumber.ToString(), out var departmentBankAccount))
                return false;
            var newPayout = new Payout(
                departmentBankAccount.AccountNumber,
                departmentBankAccount.AccountPin,
                bankAccount.AccountNumber,
                jobPrototype.Wage);
            PayoutsList.Add(newPayout);
            return true;
        }
        private sealed class Payout
        {
            public string FromAccountNumber { get; }
            public string FromAccountPin { get; }
            public string ToAccountNumber { get; }
            public FixedPoint2 PayoutAmount { get; }
            public Payout(string fromAccountNumber, string fromAccountPin, string toAccountNumber, FixedPoint2 payoutAmount)
            {
                FromAccountNumber = fromAccountNumber;
                FromAccountPin = fromAccountPin;
                ToAccountNumber = toAccountNumber;
                PayoutAmount = payoutAmount;
            }
        }
    }
}
