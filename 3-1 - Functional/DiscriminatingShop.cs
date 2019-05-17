using System;
using System.Collections.Generic;
using System.Linq;

/*
Let's make some money by selling some stuff in our store which sets the price
according to the customers wealth. Sounds neat, right? Customer tailored
prices are great because let's face it, if you're rich enough you can't
differentiate between 10 and 100 dollars anyway... Use the given
PriceCalculator and some LINQ Expressions to transform the prices into a
string containing the adjusted values separated by commas.
 */
internal class PriceCalculator
{
    private decimal multiplier;

    public PriceCalculator(bool rich)
    {
        multiplier = rich ? 5m : 0.5m;
    }

    public decimal CalculatePrice(decimal price)
    {
        return price * multiplier;
    }
}

public class DiscriminatingShop
{
    public static void Main(string[] args)
    {
        var rich = args.Any(a => a == "--rich");
        var calculator = new PriceCalculator(rich);
        var prices = new List<decimal> { 1.25m, 551.35m, 1024m, 8.30m };

        //(1) Remove prices that are lower than 2 dollars.
        //(2) Transform the prices using the calculator.
        //(3) Reduce the list into a string.
        string pricesAdjusted;

        Console.WriteLine(pricesAdjusted);
    }
}