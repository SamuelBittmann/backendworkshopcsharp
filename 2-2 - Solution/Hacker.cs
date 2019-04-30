using System;
using AtmSystem;

/*
Exercise 2 (Solution)
----------

Oh no, it looks like a hacker has bypassed the security systems of an
ATM and accesses the account directly. Fix the issues and force the
attacker to use the intended method "Withdraw(...)" of the ATM class.

There also seems to be a second problem. An unknowing developer has
disabled the accessability check because he did not know what it was
used for. Move the check inside the Account class and prevent
developers of the ATM class from even seeing the accessability status.
 */
public class Hacker
{
    public static void Main(string[] args)
    {
        decimal amount;
        if (args.Length < 1 || !decimal.TryParse(args[0], out amount))
        {
            Console.WriteLine("Please provide a valid floating point number for the first argument.");
            return;
        }

        var account = new Account();
        var atm = new ATM(account);
        var result = atm.Withdraw(amount);
        Console.WriteLine(result);
    }
}

namespace AtmSystem
{
    public class ATM
    {
        private readonly Account account;

        public ATM(Account account)
        {
            this.account = account;
        }

        public string Withdraw(decimal amount)
        {
            if (account.Balance >= amount)
            {
                account.Withdraw(amount);
                return $"Your new balance is {account.Balance} Fr.";
            }
            else
            {
                return $"Unable to withdraw {amount} Fr. Your current balance is {account.Balance} Fr.";
            }
        }
    }

    public class Account
    {
        public decimal Balance { get; private set; }

        public Account()
        {
            Balance = 100m;
        }
        
        private bool IsAccessible() 
        {
            return true;
        }

        internal void Withdraw(decimal amouont)
        {
            if (IsAccessible())
            {
                Balance -= amouont;
            }
        }
    }
}
