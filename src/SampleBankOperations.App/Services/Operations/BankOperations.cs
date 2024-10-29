using SampleBankOperations.App.Interfaces;
using SampleBankOperations.Application.Interfaces;
using SampleBankOperations.Application.Validations;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Interfaces;

namespace SampleBankOperations.App.Services.Operations;

public class BankOperations : IBankOperations
{
    private readonly IAccountService _accountService;
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger _logger;

    public BankOperations(IAccountService accountService, IAccountRepository accountRepository, ILogger logger)
    {
        _accountService = accountService;
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public void OpenAccount()
    {
        Console.Write("Enter Account Number: ");
        var accountNumber = Console.ReadLine();
        Console.Write("Enter Initial Balance: ");
        if (decimal.TryParse(Console.ReadLine(), out var initialBalance))
        {
            var account = new Account(accountNumber!, initialBalance, Core.Enums.AccountType.Checking);
            _accountRepository.Add(account);
            _logger.Log($"Account {accountNumber} opened with balance {initialBalance:C}.");
        }
        else
        {
            Console.WriteLine("Invalid amount.");
        }
    }

    public Account GetAccountByNumber(string accountNumber)
    {
        return _accountRepository.GetByAccountNumber(accountNumber);
    }

    public void ViewBalance(Account account)
    {
        var balance = _accountService.GetBalance(account);
        Console.WriteLine($"Current balance for account {account.AccountNumber}: {balance:C}");
    }

    public void Deposit(Account account)
    {
        Console.Write("Enter Deposit Amount: ");
        if (decimal.TryParse(Console.ReadLine(), out var amount))
        {
            _accountService.Deposit(account, amount, amt => Console.WriteLine($"Deposited: {amt:C}"));
        }
        else
        {
            Console.WriteLine("Invalid amount.");
        }
    }

    public void Withdraw(Account account)
    {
        Console.Write("Enter Withdraw Amount: ");
        if (decimal.TryParse(Console.ReadLine(), out var amount))
        {
            bool success = _accountService.Withdraw(account, amount, AccountValidator.MinimumBalanceValidator(0));
            Console.WriteLine(success ? $"Withdrawn: {amount:C}" : "Insufficient balance.");
        }
        else
        {
            Console.WriteLine("Invalid amount.");
        }
    }

    public void Transfer(Account fromAccount, Account toAccount)
    {
        Console.Write("Enter Transfer Amount: ");
        if (decimal.TryParse(Console.ReadLine(), out var amount))
        {
            bool success = _accountService.Transfer(fromAccount, toAccount, amount, 
                AccountValidator.MinimumBalanceValidator(0), AccountValidator.RequestedAmountValidator(amount));

            Console.WriteLine(success ? $"Transferred: {amount:C} from {fromAccount.AccountNumber} to {toAccount.AccountNumber}" 
                : "Transfer failed due to insufficient balance.");
        }
        else
        {
            Console.WriteLine("Invalid amount.");
        }
    }

    public void CalculateInterest(Account account)
    {
        Console.Write("Enter Interest Rate (%): ");
        if (decimal.TryParse(Console.ReadLine(), out var rate))
        {
            decimal interest = _accountService.CalculateInterest(account, (balance, r) => balance * (r / 100), rate);
            Console.WriteLine($"Calculated Interest: {interest:C}");
        }
        else
        {
            Console.WriteLine("Invalid rate.");
        }
    }
}
