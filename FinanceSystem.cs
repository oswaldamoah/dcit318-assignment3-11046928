using System;
using System.Collections.Generic;

public static class FinanceSystem
{
    // Transaction record (immutable data structure)
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // Transaction processor interface
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // Concrete processor implementations
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing bank transfer of GHC{transaction.Amount} for {transaction.Category}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing mobile money payment of GHC{transaction.Amount} for {transaction.Category}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing crypto transaction of GHC{transaction.Amount} for {transaction.Category}");
        }
    }

    // Base Account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
        }
    }

    // Sealed SavingsAccount class
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) 
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds - transaction cancelled");
            }
            else
            {
                base.ApplyTransaction(transaction);
                Console.WriteLine($"Transaction successful. New balance: GHC{Balance}");
            }
        }
    }

    // Main Finance Application
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            Console.Clear();
            Console.WriteLine("=== Finance Management System ===");

            // Get account details from user
            Console.Write("Enter account number: ");
            string accountNumber = Console.ReadLine() ?? "ACC001";

            Console.Write("Enter initial balance (GHC): ");
            decimal initialBalance;
            while (!decimal.TryParse(Console.ReadLine(), out initialBalance) || initialBalance < 0)
            {
                Console.Write("Invalid amount. Please enter a positive number: ");
            }

            // Create account
            var account = new SavingsAccount(accountNumber, initialBalance);
            Console.WriteLine($"Account created with balance: GHC{account.Balance}");

            // Create transactions
            var transactions = new List<Transaction>();
            for (int i = 1; i <= 3; i++)
            {
                Console.WriteLine($"\nEnter details for Transaction {i}:");
                
                Console.Write("Amount (GHC): ");
                decimal amount;
                while (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.Write("Invalid amount. Please enter a positive number: ");
                }

                Console.Write("Category (e.g., Groceries, Utilities, Entertainment): ");
                string category = Console.ReadLine() ?? "Miscellaneous";

                transactions.Add(new Transaction(i, DateTime.Now, amount, category));
            }

            // Process transactions
            var processors = new ITransactionProcessor[]
            {
                new MobileMoneyProcessor(),
                new BankTransferProcessor(),
                new CryptoWalletProcessor()
            };

            Console.WriteLine("\nProcessing transactions:");
            for (int i = 0; i < transactions.Count; i++)
            {
                Console.WriteLine($"\nTransaction {i + 1}:");
                processors[i].Process(transactions[i]);
                account.ApplyTransaction(transactions[i]);
                _transactions.Add(transactions[i]);
            }

            // Display summary
            Console.WriteLine("\nTransaction Summary:");
            Console.WriteLine($"Account: {account.AccountNumber}");
            Console.WriteLine($"Final Balance: GHC{account.Balance}");
            Console.WriteLine("\nTransaction History:");
            foreach (var t in _transactions)
            {
                Console.WriteLine($"{t.Date}: GHC{t.Amount} for {t.Category}");
            }
        }
    }

    public static void Run()
    {
        var app = new FinanceApp();
        app.Run();
    }
}