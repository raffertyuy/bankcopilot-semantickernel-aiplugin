using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Models
{
    public class Account
    {
        private decimal _accountBalance;

        public string AccountNumber { get; private set; }
        public string AccountName { get; private set; }

        public decimal AccountBalance
        {
            get => _accountBalance;
            set => _accountBalance = value >= 0 ? value : throw new ArgumentException("Account balance cannot be less than zero.");
        }

        public Account(string accountNumber, string accountName, decimal accountBalance)
        {
            AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            AccountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
            AccountBalance = accountBalance;
        }
    }
}