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
                bankAccount.AccountName = Loc.GetString($"department-{department.ID}");
            }
        }
    }
}
