using System;
using System.Collections.Generic;
using System.Linq;

/*
Exercise 3a
-----------

Let's make some money by selling some stuff in our store which sets the price
according to the customers wealth. Sounds neat, right? Customer tailored
prices are great because let's face it, if you're rich enough you can't
differentiate between 10 and 100 dollars anyway... 
First populate the prices list with the prices for our five products. The first
item should cost 1 (banana). Each following item should cost five times as much
as the previous one. Then, use the given PriceCalculator and some LINQ Expressions 
to transform the prices into a string containing the adjusted values separated by 
commas.
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

        //(1) Populate the prices list. You can use Enumerable.Range(...)
        //or an iterator. Why not try both? ;)
        var prices = Enumerable.Range(1,5);

        var pricesAdjusted = prices
        //(2) Remove prices that are lower than 2 dollars.
            .Where(p => p >= 2)
        //(3) Transform the prices using the calculator.
            .Select(p => calculator.CalculatePrice(p))
        //(4) Reduce the list into a string.
            .Aggregate((accu, p) => accu + p);

        Console.WriteLine(pricesAdjusted);
    }
}