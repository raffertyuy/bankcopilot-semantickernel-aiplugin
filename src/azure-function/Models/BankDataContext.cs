using System.Collections.Generic;

namespace Models;

public class BankDataContext
{
    private static BankDataContext instance = null;
    private static readonly object padlock = new object();

    private List<Account> accounts;

    BankDataContext()
    {
        accounts = new List<Account>();
    }

    public static BankDataContext Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new BankDataContext();
                }
                return instance;
            }
        }
    }

    public List<Account> Accounts
    {
        get { return accounts; }
    }
}
