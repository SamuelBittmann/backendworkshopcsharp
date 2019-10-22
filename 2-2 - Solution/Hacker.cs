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
        var transferResult = victimAccount
            .TransferTo(100m, hackersAccount)
            .Match(
                () => "Transfer succeeded. The Hacker won.", 
                () => "Transfer declined. The hacker could not steal money from the victim.");

        Console.WriteLine(transferResult);
        Console.WriteLine($"The hackers new balance is {hackersAccount.Balance:0.00} (was 100.00)");
    }
}

public abstract class Account
{
    // The balance is completely protected from outside access and serves as a backing field for the
    // publicly accessible read only property "Balance".
    protected decimal balance;

    public decimal Balance => balance;

    // Alternatively you could use an auto property which allows you to set read and write access
    // levels individually.
    public decimal BalanceAlternative { get; protected set; }

    public Account()
    {
        balance = 100m;
    }
}

public class BankAccount : Account
{
    // The readonly keyword ensures the value can only be set from within the constructor. Making it
    // private is not necessary for completing this exercise, but it is always a good idea to 
    // restrict access as much as possible / reasonable.
    private readonly string AccountHolder;

    // There is no reason for making the IsEnabled flag accessible from outside, so you should make
    // it private.
    private bool IsEnabled;

    public BankAccount(string accountHolder)
    {
        AccountHolder = accountHolder;
        IsEnabled = !accountHolder.ToLower().StartsWith("fraudster");
    }

    public Result TransferTo(decimal amount, BankAccount destination)
    {
        if (IsEnabled && destination.IsEnabled && balance - amount >= 0)
        {
            balance -= amount;
            destination.balance += amount;
            return Result.Success();
        }
        else
        {
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