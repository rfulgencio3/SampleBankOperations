using SampleBankOperations.App.Interfaces;
using SampleBankOperations.App.Services.Operations;
using SampleBankOperations.Core.Entities;

namespace SampleBankOperations.App.Services.UI;

public class UserInterface
{
    private readonly IBankOperations _bankOperations;
    private readonly Dictionary<string, Action> _actions;

    public UserInterface(IBankOperations bankOperations)
    {
        _bankOperations = bankOperations;

        _actions = new Dictionary<string, Action>
        {
            { "1", () => _bankOperations.OpenAccount() },
            { "2", () => ExecuteWithAccount(_bankOperations.ViewBalance) },
            { "3", () => ExecuteWithAccount(_bankOperations.Deposit) },
            { "4", () => ExecuteWithAccount(_bankOperations.Withdraw) },
            { "5", () => ExecuteWithAccounts(_bankOperations.Transfer) },
            { "6", () => ExecuteWithAccount(_bankOperations.CalculateInterest) },
            { "9", ExitApplication }
        };
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("Welcome to SampleBankOperations! Choose an option:");
            Console.WriteLine("1. Open Account");
            Console.WriteLine("2. Get Balance");
            Console.WriteLine("3. Deposit");
            Console.WriteLine("4. Withdraw");
            Console.WriteLine("5. Transfer Between Accounts");
            Console.WriteLine("6. Calculate Interest");
            Console.WriteLine("9. Exit");

            var choice = Console.ReadLine();

            if (_actions.TryGetValue(choice!, out var action))
            {
                action();
            }
            else
            {
                Console.WriteLine("Invalid choice, please try again.");
            }
        }
    }

    private void ExecuteWithAccount(Action<Account> accountAction)
    {
        Console.Write("Enter Account Number: ");
        var accountNumber = Console.ReadLine();
        var account = _bankOperations.GetAccountByNumber(accountNumber!);
        if (account == null)
        {
            Console.WriteLine("Account not found.");
            return;
        }
        accountAction(account);
    }

    private void ExecuteWithAccounts(Action<Account, Account> transferAction)
    {
        Console.Write("Enter Source Account Number: ");
        var fromAccountNumber = Console.ReadLine();
        var fromAccount = _bankOperations.GetAccountByNumber(fromAccountNumber!);
        if (fromAccount == null)
        {
            Console.WriteLine("Source account not found.");
            return;
        }

        Console.Write("Enter Destination Account Number: ");
        var toAccountNumber = Console.ReadLine();
        var toAccount = _bankOperations.GetAccountByNumber(toAccountNumber!);
        if (toAccount == null)
        {
            Console.WriteLine("Destination account not found.");
            return;
        }
        transferAction(fromAccount, toAccount);
    }

    private void ExitApplication()
    {
        Console.WriteLine("Thank you for using SampleBankOperations!");
        Environment.Exit(0);
    }
}
