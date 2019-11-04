using NUnit.Framework;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

[TestFixture]
public class Tests
{
    [Test]
    public void Poor_NoPricesLowerThan1()
    {
        // Given
        var output = new StringWriter();
        Console.SetOut(output);

        // When
        DiscriminatingShop.Main(new string[] {});

        // Then
        foreach (var price in ExtractPrices(output.ToString()))
        {
            Assert.That(
                price, 
                Is.GreaterThanOrEqualTo(1m),
                message: "Make sure you don't show prices below 1.00 to poor people (2 * 0.5).");
        }
    }

    [Test]
    public void Rich_NoPricesLowerThan10()
    {
        // Given
        var output = new StringWriter();
        Console.SetOut(output);

        // When
        DiscriminatingShop.Main(new string[] { "--rich" });

        // Then
        foreach (var price in ExtractPrices(output.ToString()))
        {
            Assert.That(
                price, 
                Is.GreaterThanOrEqualTo(10m),
                message: "Make sure you don't show prices below 10.00 to rich people (2 * 5).");
        }
    }

    [Test]
    public void PricesAreDifferentForPoorAndRich()
    {
        // Given
        var outputPoor = new StringWriter();
        var outputRich = new StringWriter();

        // When
        Console.SetOut(outputPoor);
        DiscriminatingShop.Main(new string[] {});
        Console.SetOut(outputRich);
        DiscriminatingShop.Main(new string[] { "--rich" });

        // Then
        var pricesPoor = ExtractPrices(outputPoor.ToString());
        var pricesRich = ExtractPrices(outputRich.ToString());
        
        Assert.That(
            pricesPoor.Count, 
            Is.EqualTo(pricesRich.Count), 
            message: 
                "Make sure your shop returns the same amount of prices for rich and poor " 
                + "people");

        for (var i = 0; i < pricesRich.Count; i++)
        {
            Assert.That(
                pricesPoor[i] * 10, 
                Is.EqualTo(pricesRich[i]), 
                message:
                    "Use the price calculator to provide custom tailored prices to poor and to " 
                    + "rich people.");
        }
    }

    private IList<decimal> ExtractPrices(string input)
    {
        var matches = Regex.Matches(input, "(\\d+\\.?\\d*)");
        var prices = new List<decimal> ();
        foreach (Match match in matches)
        {
            prices.Add(decimal.Parse(match.Value));
        }

        return prices;
    }

    [Test]
    public void ReturnsAtLeastOnePrice()
    {
        // Given
        var output = new StringWriter();
        Console.SetOut(output);

        // When
        DiscriminatingShop.Main(new string[] {});

        // Then
        var prices = ExtractPrices(output.ToString());
        Assert.That(
            prices.Count, 
            Is.GreaterThanOrEqualTo(3),
            message: "Add at least three prices to your shop.");
    }
}