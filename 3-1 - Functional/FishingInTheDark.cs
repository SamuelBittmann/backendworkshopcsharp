using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

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
    private readonly Random random = new Random();
    public int StepCount { get; private set; } = 0;

    public Game()
    {
        player = new Coordinate(random.Next(minValue: 0, maxValue: mapSizeX), random.Next(minValue: 0, maxValue: mapSizeY));

        do 
        {
            target = new Coordinate(random.Next(minValue: 0, maxValue: mapSizeX), random.Next(minValue: 0, maxValue: mapSizeY));
        } while (target.ManhattanDistance(player) <= Math.Floor(mapSizeX / 2d));
    }

    public (bool Moved, bool Won) Move(Coordinate direction)
    {
        var newPosition = player.Add(direction);

        if (ValidatePosition(newPosition))
        {
            player = new Coordinate(
                Math.Min(mapSizeX - 1, Math.Max(0, newPosition.X)),
                Math.Min(mapSizeY - 1, Math.Max(0, newPosition.Y)));
            StepCount++;

            if (player.X == target.X && player.Y == target.Y)
            {
                return (Moved: true, Won: true);
            }
            else
            {
                Coordinate newTargetPosition;
                do 
                {
                    direction = directions[random.Next(0, 4)];
                    newTargetPosition = target.Add(direction);
                } while(!ValidatePosition(newTargetPosition) || (newTargetPosition.X == player.X && newTargetPosition.Y == player.Y));
                target = newTargetPosition;

                return (Moved: true, Won: false);
            }
        }
        else
        {
            return (Moved: false, Won: false);
        }
    }

    private bool ValidatePosition(Coordinate position)
    {
        return position.X >= 0 
            && position.X < mapSizeX 
            && position.Y >= 0 
            && position.Y < mapSizeY;
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
    public static Coordinate UpLeft = Up.Add(Left);
    public static Coordinate UpRight = Up.Add(Right);
    public static Coordinate DownLeft = Down.Add(Left);
    public static Coordinate DownRight = Down.Add(Right);
}

public class Program
{
    public static void Main(string[] args)
    {
        var game = new Game();

        Console.WriteLine("Welcome to 'Fishing in the dark'");
        Console.WriteLine();
        Console.WriteLine("Use the arrow keys and try to catch the robber.");
        Console.WriteLine("You can move straight pressing the same arrow key twice or diagonally by pressing two different arrow keys subsequently.");
        Console.WriteLine("After each step you will be told how far away you are from the robber.");
        Console.WriteLine();
        Console.WriteLine(game.Distance());
        Console.WriteLine();

        var result = (Moved: true, Won: false);
        do {
            Console.Write("Your move: ");
            var keys = new List<ConsoleKey>();
            keys.Add(Console.ReadKey().Key);
            keys.Add(Console.ReadKey().Key);
            if (keys.All(k => k == ConsoleKey.UpArrow)) {
                result = game.Move(Coordinate.Up);
            } 
            else if (keys.All(k => k == ConsoleKey.RightArrow))
            {
                result = game.Move(Coordinate.Right);
            }
            else if (keys.All(k => k == ConsoleKey.DownArrow))
            {
                result = game.Move(Coordinate.Down);
            }
            else if (keys.All(k => k == ConsoleKey.LeftArrow))
            {
                result = game.Move(Coordinate.Left);
            }
            else if (keys.Any(k => k == ConsoleKey.UpArrow) && keys.Any(k => k == ConsoleKey.LeftArrow))
            {
                result = game.Move(Coordinate.UpLeft);
            }
            else if (keys.Any(k => k == ConsoleKey.UpArrow) && keys.Any(k => k == ConsoleKey.RightArrow))
            {
                result = game.Move(Coordinate.UpRight);
            }
            else if (keys.Any(k => k == ConsoleKey.DownArrow) && keys.Any(k => k == ConsoleKey.LeftArrow))
            {
                result = game.Move(Coordinate.DownLeft);
            }
            else if (keys.Any(k => k == ConsoleKey.DownArrow) && keys.Any(k => k == ConsoleKey.RightArrow))
            {
                result = game.Move(Coordinate.DownRight);
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
                Console.WriteLine(game.Distance());
                Console.WriteLine();
            }
        } while (!result.Won);

        Console.WriteLine($"Yay!! You caught the bad guy in {game.StepCount} steps :)");
    }
}