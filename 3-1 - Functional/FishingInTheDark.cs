using System.Threading;
using System;
using System.Collections.Generic;

public class Game
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
    private Random random;

    public Game()
    {
        random = new Random();
        player = new Coordinate(random.Next(minValue: 0, maxValue: mapSizeX), random.Next(minValue: 0, maxValue: mapSizeY));

        do 
        {
            target = new Coordinate(random.Next(minValue: 0, maxValue: mapSizeX), random.Next(minValue: 0, maxValue: mapSizeY));
        } while (target.ManhattanDistance(player) <= Math.Floor(mapSizeX / 2d));
    }

    public bool Move(Coordinate direction)
    {
        var newPosition = player.Add(direction);
        player = new Coordinate(
            Math.Min(mapSizeX - 1, Math.Max(0, newPosition.X)),
            Math.Min(mapSizeY - 1, Math.Max(0, newPosition.Y)));

        if (player.X == target.X && player.Y == target.Y)
        {
            return true;
        }
        else
        {
            Coordinate newTargetPosition;
            do 
            {
                direction = directions[random.Next(0, 4)];
                newTargetPosition = target.Add(direction);
            } while(newTargetPosition.X < 0 || newTargetPosition.X >= mapSizeX || newTargetPosition.Y < 0 || newTargetPosition.Y >= mapSizeY);
            target = newTargetPosition;

            return false;
        }
    }

    public string Distance()
    {
        return $"You are {player.Distance(target):0.00} meters away.";
    }
}

public class Coordinate
{
    public readonly int X;
    public readonly int Y;

    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Coordinate Add(Coordinate other) 
    {
        return new Coordinate(X + other.X, Y + other.Y);
    }

    public double Distance(Coordinate other)
    {
        return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
    }

    public int ManhattanDistance(Coordinate other)
    {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }

    public static Coordinate Up = new Coordinate(0, -1);
    public static Coordinate Down = new Coordinate(0, 1);
    public static Coordinate Left = new Coordinate(-1, 0);
    public static Coordinate Right = new Coordinate(1, 0);
}

public class Program
{
    public static void Main(string[] args)
    {
        var game = new Game();

        Console.WriteLine("Welcome to 'Fishing in the dark'");
        Console.WriteLine();
        Console.WriteLine("Move using the arrow key and try to catch the robber.");
        Console.WriteLine("After each step you will be told how far away you are from the robber.");
        Console.WriteLine();
        Console.WriteLine(game.Distance());
        Console.WriteLine();

        var result = false;
        do {
            Console.Write("Your move: ");
            var key = Console.ReadKey().Key;
            if (key == ConsoleKey.UpArrow) {
                result = game.Move(Coordinate.Up);
            } 
            else if (key == ConsoleKey.RightArrow)
            {
                result = game.Move(Coordinate.Right);
            }
            else if (key == ConsoleKey.DownArrow)
            {
                result = game.Move(Coordinate.Down);
            }
            else if (key == ConsoleKey.LeftArrow)
            {
                result = game.Move(Coordinate.Left);
            }
            else 
            {
                Console.WriteLine("Invalid input.");
            }

            Console.WriteLine(game.Distance());
            Console.WriteLine();
            
        } while (!result);

        Console.WriteLine("Congratulations!! You caught the bad guy :)");
    }
}