using Content.Server.Economy;
using Content.Shared.Roles;

namespace Content.Server.GameTicking
{
    public sealed partial class GameTicker
    {
        private void CreateDepartmentsBankAccounts()
        {
            foreach (var department in _prototypeManager.EnumeratePrototypes<DepartmentPrototype>())
            {
                var bankAccount = _bankManagerSystem.CreateNewBankAccount(department.AccountNumber);
                if (bankAccount == null) continue;
                bankAccount.AccountName = department.ID;
                bankAccount.Balance = 100000;
            }
        }
    }
}
