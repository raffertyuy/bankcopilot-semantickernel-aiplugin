using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Models
{
    public class Account
    {
        private string _accountNumber;
        private string _accountName;
        private decimal _accountBalance;

        public string AccountNumber 
        { 
            get => _accountNumber; 
            set => _accountNumber = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException("Account number cannot be null or empty."); 
        }

        public string AccountName 
        { 
            get => _accountName; 
            set => _accountName = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException("Account name cannot be null or empty."); 
        }

        public decimal AccountBalance
        {
            get => _accountBalance;
            set => _accountBalance = value >= 0 ? value : throw new ArgumentException("Account balance cannot be less than zero.");
        }

        public Account(string accountNumber, string accountName, decimal accountBalance)
        {
            _accountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            _accountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
            AccountBalance = accountBalance;
        }
    }
}