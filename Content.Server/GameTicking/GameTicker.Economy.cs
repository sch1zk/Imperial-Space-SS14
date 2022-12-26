using Content.Server.Access.Systems;
using Content.Server.CartridgeLoader.Cartridges;
using Content.Server.Economy.Wage;
using Content.Server.Mind;
using Content.Shared.Inventory;
using Content.Shared.Roles;
using Content.Shared.CartridgeLoader;

namespace Content.Server.GameTicking
{
    public sealed partial class GameTicker
    {
        [Dependency] private readonly WageManagerSystem _wageManagerSystem = default!;
        [Dependency] private readonly BankCartridgeSystem _bankCartridgeSystem = default!;
        [Dependency] private readonly InventorySystem _inventorySystem = default!;
        [Dependency] private readonly IdCardSystem _cardSystem = default!;

        private void CreateDepartmentsBankAccounts()
        {
            foreach (var department in _prototypeManager.EnumeratePrototypes<DepartmentPrototype>())
            {
                var bankAccount = _bankManagerSystem.CreateNewBankAccount(department.AccountNumber, true);
                if (bankAccount == null) continue;
                bankAccount.AccountName = department.ID;
                bankAccount.Balance = 100000;
            }
        }
        private void CreateBankAccountAndStoreInMob(EntityUid entity, Mind.Mind mind, JobPrototype jobPrototype)
        {
            if (!_cardSystem.TryFindIdCard(entity, out var idCardComponent))
                return;

            if (_cardSystem.TryStoreNewBankAccount(idCardComponent.Owner, idCardComponent, out var bankAccount) && bankAccount != null)
            {
                if (mind != null && mind.Session != null)
                {
                    mind.AddMemory(new Memory("memory-account-number", bankAccount.AccountNumber));
                    mind.AddMemory(new Memory("memory-account-pin", bankAccount.AccountPin));
                }

                _bankManagerSystem.TryGenerateStartingBalance(bankAccount, jobPrototype);
                _wageManagerSystem.TryAddAccountToWagePayoutList(bankAccount, jobPrototype);
                if (!_inventorySystem.TryGetSlotEntity(entity, "id", out var idUid))
                    return;

                if (EntityManager.TryGetComponent(idUid, out CartridgeLoaderComponent? cartrdigeLoaderComponent))
                {
                    foreach (var uid in cartrdigeLoaderComponent.InstalledPrograms)
                    {
                        if (!EntityManager.TryGetComponent(uid, out BankCartridgeComponent? bankCartrdigeComponent))
                            continue;

                        _bankCartridgeSystem.LinkBankAccountToCartridge(bankCartrdigeComponent, bankAccount);
                    }
                }
            }
        }
    }
}
