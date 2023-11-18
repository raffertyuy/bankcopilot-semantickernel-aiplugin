using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Models
{
    public class Customer
    {
        public string IdNumber { get; private set; }
        public string FullName { get; set; }
        public string BillingAddress { get; set; } = string.Empty;

        public ICollection<Account> Accounts { get; } = new List<Account>();

        public Customer(string idnumber, string fullname)
        {
            IdNumber = idnumber ?? throw new ArgumentNullException(nameof(idnumber));
            FullName = fullname ?? throw new ArgumentNullException(nameof(fullname));
        }
    }
}