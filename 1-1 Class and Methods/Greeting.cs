using System;

/*
Exercise 1
----------

Modify the code below so that the program returns either a formal or an
informal greeting according to the following logic:
If the entered age is below 18 the user should be greeted with
"Hi, <first name>". If the age is 18 or above, the greeting should read
"Hello, mr./mrs. <last name>". No distinction between genders is necessary.
 */
public class Greeting
{
    public static void Main(string[] args)
    {
        int age;
        if (args.Length <= 0 || !Int32.TryParse(args[2], out age))
        {
            Console.WriteLine("Please provide the following arguments: First name, last name and age.");
            return;
        }
        var firstName = args[0];
        var lastName = args[1];

        var greeting = GetGreeting(firstName: firstName, lastName: lastName, age: age);  // (3) Call method using named arguments
        Console.WriteLine(greeting);
    }

    public static string GetGreeting(string firstName, string lastName, int age )   // (1) Change method signature
    {
        if (age >= 18)
        {
            return $"Hello, mr./mrs. {lastName}";
        } else {
            return $"Hi, {firstName}";
        }
    }
}