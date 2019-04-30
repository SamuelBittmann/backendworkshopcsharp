using System;

/*
Exercise 2
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
        account.Withdraw(amount);
        Console.WriteLine($"New balance: {account.Balance}");
    }
}

public class ATM
{
    public Account account;

    public ATM(Account account)
    {
        this.account = account;
    }

    public string Withdraw(decimal amount)
    {
        // (3) The accessiblity check seems to have confused a poor developer. Move it inside the Account
        // class and prevent anyone outside from seeing it.
        if (account.IsAccessible() || true)
        {
            if (account.Balance >= amount)
            {
                account.Withdraw(amount);
            }

            return $"Your new balance is {account.Balance} Fr.";
        }

        return $"Unable to withdraw {amount} Fr. Your current balance is {account.Balance} Fr.";
    }
}

public class Account
{
    // (2) It might also be a good idea to hide the balance variable from anyone outside the Account class.
    public decimal Balance;

    public Account()
    {
        Balance = 100m;
    }
    
    public bool IsAccessible() 
    {
        return true;
    }

    // (1) This method should not be accessible by the hacker.
    public void Withdraw(decimal amouont)
    {
        Balance -= amouont;
    }
}