using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class Tests
{
    [Test]
    public void BankAccountBalanceCanBeRead()
    {
        if (GetBalanceField() != null)
        {
            Assert.Fail(
                message: 
                    "We surely don't want to allow any potantial hackers to manipulate the balance." 
                    + " Make the balance field inaccessible from outside.");
        }

        var balanceProperty = GetBalanceProperty();
        var getBalanceMethod = GetBalanceMethod();

        Assert.That(
            balanceProperty != null || getBalanceMethod != null, 
            Is.True,
            message: 
                "We still want to be able to >read< the balance, duh... ;) Make sure that the "
                + "BankAccount class has either a public property with the name \"Balance\" or a " 
                + "public method called \"GetBalance()\". Either must return a value of type " 
                + "decimal.");
        
        if (balanceProperty != null)
        {
            CheckBalanceProperty(balanceProperty);
        }

        CheckBalanceField();
    }

    private FieldInfo GetBalanceField()
    {
        return (typeof (BankAccount))
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => f.Name.ToLower() == "balance")
            .Where(f => f.FieldType == typeof (decimal))
            .FirstOrDefault();
    }

    private PropertyInfo GetBalanceProperty()
    {
        return (typeof (BankAccount))
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name == "Balance")
            .Where(p => p.PropertyType == typeof (decimal))
            .SingleOrDefault();
    }

    private MethodInfo GetBalanceMethod()
    {
        return (typeof (BankAccount))
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name == "GetBalance")
            .Where(m => m.ReturnType == typeof (decimal))
            .Where(m => m.GetParameters().Count() == 0)
            .SingleOrDefault();
    }

    private void CheckBalanceProperty(PropertyInfo property)
    {
        var getter = property.GetGetMethod(nonPublic: true);
        Assert.That(
            getter, 
            Is.Not.Null.And.Matches((MethodInfo m) => m.IsPublic), 
            message: "Make sure the \"Balance\" property can be publicly read.");

        var setter = property.GetSetMethod(nonPublic: true);
        Assert.That(
            setter,
            Is.Null.Or.Not.Matches((MethodInfo m) => m.IsPublic),
            message: "The setter of the \"Balance\" property should not be publicly accessible."
        );
        Assert.That(
            setter,
            Is.Null.Or.Not.Matches((MethodInfo m) => m.IsAssembly),
            message: 
                "Making the getter of the \"Balance\" property internal is not enough. The hacker " 
                + "could still manipulate it since he is within the same assembly."
        );
    }

    private void CheckBalanceField()
    {
        var fields = (typeof (BankAccount))
            .GetFields()
            .Where(f => f.Name.ToLower() == "balance")
            .ToList();

        foreach (var field in fields)
        {
            Assert.That(
                field.IsAssembly,
                Is.False,
                message: 
                    "The \"Balance\" field must not be accessible from outside the BankAccount " 
                    + "class.");
        }
    }

    [Test]
    public void TransferFunds_TwoEnabledAccounts_FundsAreTransfered()
    {
        // Given
        var balanceAccessor = GetBalanceAccessor();
        var sender = new BankAccount("Sender");
        var recepient = new BankAccount("Recepient");
        var senderInitialBalance = balanceAccessor(sender);
        var recepientInitialBalance = balanceAccessor(sender);
        var transferAmount = senderInitialBalance;

        // When
        sender.TransferTo(transferAmount, recepient);

        // Then
        Assert.That(
            () => balanceAccessor(recepient), 
            Is.EqualTo(recepientInitialBalance + transferAmount), 
            message: 
                "Incorrect amount received. Your changes seem to have broken the TransferFunds(...)" 
                + " method.");

        Assert.That(
            () => balanceAccessor(sender), 
            Is.EqualTo(senderInitialBalance - transferAmount), 
            message: 
                "Incorrect amount subtracted. Your changes seem to have broken the " 
                + "TransferFunds(...) method.");
    }

    private Func<BankAccount, decimal> GetBalanceAccessor()
    {
        var property = GetBalanceProperty();
        var method = GetBalanceMethod();
        var field = GetBalanceField();
        return bankAccount => 
        {
            if (property != null)
            {
                return (decimal)property.GetValue(bankAccount);
            }
            else if (method != null)
            {
                return (decimal)method.Invoke(bankAccount, parameters: new object[] {});
            }
            else if (field != null)
            {
                return (decimal)field.GetValue(bankAccount);
            }
            else
            {
                Assert.Fail(
                    message: 
                        "Provide a public \"Balance\" property or a \"GetBalance()\" method " 
                        + "first.");
                return default;
            }
        };
    }

    [Test]
    public void TransferFunds_SenderIsFraud_TransferProhibited()
    {
        // Given
        var balanceAccessor = GetBalanceAccessor();
        var sender = new BankAccount("Fraudster");
        var recepient = new BankAccount("Recipient");
        var transferAmount = balanceAccessor(sender);

        // When
        var transferResult = sender.TransferTo(transferAmount, recepient);

        // TWhen & hen
        Assert.That(transferResult.Match(() => true, () => false), Is.False);
    }

    [Test]
    public void TransferFunds_RecipientIsFraud_TransferProhibited()
    {
        // Given
        var balanceAccessor = GetBalanceAccessor();
        var sender = new BankAccount("Sender");
        var recepient = new BankAccount("Fraudster");
        var transferAmount = balanceAccessor(sender);

        // When
        var transferResult = sender.TransferTo(transferAmount, recepient);

        // TWhen & hen
        Assert.That(transferResult.Match(() => true, () => false), Is.False);
    }

    [Test]
    public void BankAccountCannotBeEnabled()
    {
        // Given
        var balanceAccessor = GetBalanceAccessor();
        var sender = new BankAccount("Sender");
        var recepient = new BankAccount("Fraudster");
        var initialBalance = balanceAccessor(sender);
        TryEnableAccount(recepient);

        // When
        var result = sender.TransferTo(initialBalance, recepient);

        // Then
        Assert.That(
            () => result.Match(() => true, () => false), 
            Is.False,
            message: 
                "Althogh the hackers account is being disabled automatically, it can be " 
                + "re-enabled. Make sure the \"IsEnabled\" field cannot be manipulated by " 
                + "the hacker.");
    }

    private void TryEnableAccount(BankAccount account)
    {
        (typeof (BankAccount)).GetFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(e => e.IsPublic || e.IsAssembly)
            .Where(e => e.Name.ToLower() == "isenabled")
            .Where(e => e.FieldType == typeof (bool))
            .FirstOrDefault()
            ?.SetValue(account, true);
    }

    [Test]
    public void BankholderCannotBeChanged()
    {
        var accountHolderIsPublic = (typeof (BankAccount)).GetFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => f.Name.ToLower() == "accountholder")
            .Any(f => (f.IsPublic || f.IsAssembly) && !f.IsInitOnly);

        Assert.That(
            accountHolderIsPublic, 
            Is.False,
            message: 
                "Too many idiots have overwritten the account holders name by accident. To "
                + "prevent this in the future, make sure the account holder cannot be changed.");
    }
}