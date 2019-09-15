using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

/*
Exercise 3b
-----------

You are a game developer and you have just finished your next masterpiece.
Two game publishers from two different countries are interested in publishing
your work in their respective country. However, they can't seem to agree on
two minor details: Publisher A wants all text, that is being displayed, to be
preceeded by a '>' character. And publisher B wants the units converted to
the imperial system. Make your game customizable using old fashioned delegates
And test the variants for both publishers in the Program class.

Publisher B has provided you with an implementation for the conversion.

You can ignore everything inside the region GameInternal.
*/

#region GameInternal

public struct Coordinate : IEquatable<Coordinate>
{
    public readonly int X;
    public readonly int Y;

    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Coordinate operator +(Coordinate left, Coordinate right)
    {
        return new Coordinate(left.X + right.X, left.Y + right.Y);
    }

    public double Distance(Coordinate other)
    {
        return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
    }

    public int ManhattanDistance(Coordinate other)
    {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }

    public override bool Equals(object other)
    {
        return other is Coordinate
            ? Equals((Coordinate) other)
            : false;
    }

    public bool Equals(Coordinate other)
    {
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return X ^ Y;
    }

    public static bool operator ==(Coordinate left, Coordinate right) => left.Equals(right);
    public static bool operator!=(Coordinate left, Coordinate right) => !left.Equals(right);

    public static Coordinate Up = new Coordinate(0, -1);
    public static Coordinate Down = new Coordinate(0, 1);
    public static Coordinate Left = new Coordinate(-1, 0);
    public static Coordinate Right = new Coordinate(1, 0);
    public static Coordinate Zero = new Coordinate(0, 0);
}

public class GameController
{
    private const int mapSizeX = 10;
    private const int mapSizeY = 10;

    private static IDictionary<int, Coordinate> directions = new Dictionary<int, Coordinate> {
        { 0, Coordinate.Up },
        { 1, Coordinate.Right },
        { 2, Coordinate.Down },
        { 3, Coordinate.Left }
    };

    private Coordinate player;
    private Coordinate target;
    private readonly Random random = new Random();
    public int StepCount { get; private set; } = 0;
    public double Distance => player.Distance(target);

    public GameController()
    {
        player = new Coordinate(random.Next(minValue: 0, maxValue: mapSizeX), random.Next(minValue: 0, maxValue: mapSizeY));

        do 
        {
            target = new Coordinate(random.Next(minValue: 0, maxValue: mapSizeX), random.Next(minValue: 0, maxValue: mapSizeY));
        } while (target.ManhattanDistance(player) <= Math.Floor(mapSizeX / 2d));
    }

    public (bool Moved, bool Won) Move(Coordinate direction)
    {
        var newPosition = player + direction;

        if (!ValidatePosition(newPosition))
        {
            return (Moved: false, Won: false);
        }
        
        player = newPosition;
        StepCount++;

        if (player == target)
        {
            return (Moved: true, Won: true);
        }
        else
        {
            Coordinate newTargetPosition;
            do 
            {
                newTargetPosition = target + directions[random.Next(0, 4)];
            } while(!ValidatePosition(newTargetPosition) || newTargetPosition == player);
            target = newTargetPosition;

            return (Moved: true, Won: false);
        }
    }

    private bool ValidatePosition(Coordinate position)
    {
        return position.X >= 0 
            && position.X < mapSizeX 
            && position.Y >= 0 
            && position.Y < mapSizeY;
    }
}


#endregion


public class Game
{
    private static IDictionary<ConsoleKey, Coordinate> keyMap = new Dictionary<ConsoleKey, Coordinate>
    {
        { ConsoleKey.UpArrow, Coordinate.Up },
        { ConsoleKey.RightArrow, Coordinate.Right },
        { ConsoleKey.DownArrow, Coordinate.Down },
        { ConsoleKey.LeftArrow, Coordinate.Left }
    };

    //(1) Add a constructor which receives delegate methods for formatting the output.
    //    Make one for formatting a line and one for converting the distance in meters into
    //    a display string.

    public void Run()
    {
        var game = new GameController();

        //(2) Format all strings before writing them to the console
        Console.WriteLine("Welcome to 'Fishing in the dark'");
        Console.WriteLine();
        Console.WriteLine("Use the arrow keys and try to catch the robber.");
        Console.WriteLine("You can move straight by pressing the same arrow key twice or diagonally by pressing two different arrow keys subsequently.");
        Console.WriteLine("After each step you will be told how far away you are from the robber.");
        Console.WriteLine();
        WriteDistance();
        Console.WriteLine();

        var result = (Moved: true, Won: false);
        do {
            Console.Write("Your move: ");
            var keys = new List<ConsoleKey>();
            keys.Add(Console.ReadKey().Key);
            keys.Add(Console.ReadKey().Key);
            Console.Write("\n");

            var movement = keys
                .Where(k => keyMap.Keys.Contains(k))
                .Distinct()
                .Select(k => keyMap[k])
                .Aggregate(Coordinate.Zero, (accu, v) => accu + v);

            if (movement != Coordinate.Zero)
            {
                result = game.Move(movement);
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }

            if (!result.Moved)
            {
                Console.WriteLine("Oops, looks like you hit a wall...");
            }
            else
            {
                WriteDistance();
                Console.WriteLine();
            }
        } while (!result.Won);

        Console.WriteLine($"Yay!! You caught the bad guy in {game.StepCount} steps :)");
    }

    private void WriteDistance()
    {
        //(3) Call the delegate for formatting the distance
        Console.WriteLine($"You are {game.Distance:0.00} meters away.");
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        var game = new Game();
        game.Run();
    }

    private static string FormatDistanceImperial(double distanceInMeters)
    {
        var feet = distanceInMeters / 0.3048;
        return $"{feet:0.00} feet";
    }
}