using System;

/*
Exercise 2
----------

Oh no, it looks like a hacker has bypassed the security systems of an accounting system and uses 
vulnerabilities in order to steal funds. Fix the issues by setting appropriate access levels and 
ensure the security features that are present in the BankAccount class cannot be circumvented.

There also seems to be a second problem. A developer has accidently inserted a bug which leads the
account holders name to be overwritten in certain cases. Make it impossible to overwrite the inital
value.
 */
public class Hacker
{
    public static void Main(string[] args)
    {
        var hackersAccount = new BankAccount("Fraudster Black");
        var victimAccount = new BankAccount("Elsa Fröhlich");
        hackersAccount.IsEnabled = true;
        var transferResult = victimAccount
            .TransferTo(100m, hackersAccount)
            .Match(
                () => "Transfer succeeded. The Hacker won.", 
                () => "Transfer declined. The hacker could not steal money from the victim.");
        hackersAccount.Balance *= 2;

        Console.WriteLine(transferResult);
        Console.WriteLine($"The hackers new balance is {hackersAccount.Balance:0.00} (was 100.00)");
    }
}

public abstract class Account
{
    // (1) Change the access level of Balance so that it can only be changed by Account or 
    // BankAccount.
    public decimal Balance;

    public Account()
    {
        Balance = 100m;
    }
}

public class BankAccount : Account
{
    // (2) Make it impossible to overrwrite the initial value assigned to AccountHolder.
    public string AccountHolder;

    // (3) Hide also the IsEnabled flag so that it cannot be manipulated from the outside.
    public bool IsEnabled;

    public BankAccount(string accountHolder)
    {
        AccountHolder = accountHolder;
        IsEnabled = !accountHolder.ToLower().StartsWith("fraudster");
    }

    public Result TransferTo(decimal amount, BankAccount destination)
    {
        if (IsEnabled && destination.IsEnabled && Balance - amount >= 0)
        {
            Balance -= amount;
            destination.Balance += amount;
            return Result.Success();
        }
        else
        {
            AccountHolder = "Oups...";
            return Result.Failure();
        }
    }
}

#region Helpers
// This section contains supporting code that can be ignored for the purposes
// of this exercise.

public class Result
{
    public static Result Success() => new Result(true);
    public static Result Failure() => new Result(false);

    private bool isSuccess;

    private Result(bool isSuccess)
    {
        this.isSuccess = isSuccess;
    }

    public TOut Match<TOut>(Func<TOut> successMatcher, Func<TOut> failureMatcher)
    {
        return isSuccess ? successMatcher() : failureMatcher();
    }
}

#endregion