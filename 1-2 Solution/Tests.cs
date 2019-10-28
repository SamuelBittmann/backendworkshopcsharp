using NUnit.Framework;
using System;
using System.IO;

[TestFixture]
public class Tests
{
    [Test]
    public void Main_YoungerThan18_InformalGreeting()
    {
        // Given
        var output = new StringWriter();
        Console.SetOut(output);
        var firstName = "First";
        var lastName = "Last";
        var age = 17;

        // When
        Greeting.Main(new string[] { firstName, lastName, age.ToString() });

        // Then
        Assert.That(
            output.ToString(), 
            Does.Match($"\\s*Hi, {firstName}\\.?\\s*"),
            message: "If the age is below 18, the greeting should be \"Hi, FirstName\".");
    }

    [Test]
    public void Main_18orOlder_InformalGreeting()
    {
        // Given
        var output = new StringWriter();
        Console.SetOut(output);
        var firstName = "First";
        var lastName = "Last";
        var age = 18;

        // When
        Greeting.Main(new string[] { firstName, lastName, age.ToString() });

        // Then
        Assert.That(
            output.ToString(), 
            Does.Match($"\\s*Hello, mr./mrs. {lastName}\\.?\\s*"),
            message: 
                "If the age is below 18, the greeting should be \"Hello, mr./mrs., LastName\".");
    }
}