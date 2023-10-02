using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Console.WindowHeight = 20;
        Console.WindowWidth = 40;

        List<Point> snake = new List<Point> { new Point(20, 10) };
        Point food = GenerateFood();
        char direction = 'R';

        while (true)
        {
            if (HasCollidedWithSelf(snake))
            {
                GameOver();
                break;  // Exit the game loop
            }

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                direction = key switch
                {
                    ConsoleKey.UpArrow => 'U',
                    ConsoleKey.DownArrow => 'D',
                    ConsoleKey.LeftArrow => 'L',
                    ConsoleKey.RightArrow => 'R',
                    _ => direction
                };
            }

            Point nextHead = GetNextHead(snake[0], direction);
            snake.Insert(0, nextHead);

            if (nextHead.Equals(food))
            {
                food = GenerateFood();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }

            Console.Clear();
            Draw(snake, food);

            Thread.Sleep(100);
        }
    }

    private static void GameOver()
    {
        Console.Clear();
        Console.WriteLine("Game Over!");
    }

    static bool HasCollidedWithSelf(List<Point> snake)
    {
        Point head = snake[0];
        for (int i = 1; i < snake.Count; i++)
        {
            if (snake[i].Equals(head))
            {
                return true;
            }
        }
        return false;
    }

    static Point GetNextHead(Point head, char direction)
    {
        int newX = head.X;
        int newY = head.Y;

        switch (direction)
        {
            case 'U': newY = (head.Y - 1 + Console.WindowHeight) % Console.WindowHeight; break;
            case 'D': newY = (head.Y + 1) % Console.WindowHeight; break;
            case 'L': newX = (head.X - 1 + Console.WindowWidth) % Console.WindowWidth; break;
            case 'R': newX = (head.X + 1) % Console.WindowWidth; break;
        }

        return new Point(newX, newY);
    }

    static Point GenerateFood()
    {
        Random rand = new Random();
        return new Point(rand.Next(0, Console.WindowWidth - 1), rand.Next(0, Console.WindowHeight - 1));
    }

    static void Draw(List<Point> snake, Point food)
    {
        foreach (var point in snake)
        {
            Console.SetCursorPosition(point.X, point.Y);
            Console.Write("O");
        }

        Console.SetCursorPosition(food.X, food.Y);
        Console.Write("X");
    }
}

struct Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object obj)
    {
        return obj is Point point && X == point.X && Y == point.Y;
    }
}
