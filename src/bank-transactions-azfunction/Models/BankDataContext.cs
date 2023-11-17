using System.Collections.Generic;

namespace Models
{
    public class BankDataContext
    {
        private static BankDataContext? instance;
        private static readonly object padlock = new();

        public ICollection<Account> Accounts { get; } = new List<Account>();
        public ICollection<TransactionRecord> TransactionHistory { get; } = new List<TransactionRecord>();

        public static BankDataContext Instance
        {
            get
            {
                lock (padlock)
                {
                    return instance ??= new BankDataContext();
                }
            }
        }
    }
}
