using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;


namespace ConsoleSnakeGame
{
    class Program
    {
        private delegate void Buff();
        private delegate void BuffInitialize();



        private static int score = 0;
        private static int points = 0;


        private static int MaxHeight = 20;
        private static int MaxWidth = 40;
        static void Main(string[] args)
        {
            InitializeProgram();
            Thread thread = new Thread(() => {
                InitializeGame();
                mainGame();
            });
            thread.Start();
        }
        private static void InitializeProgram()
        {
            Console.BufferHeight = 500;
            Console.BufferWidth = 500;

            Console.WindowWidth = 500;
            Console.WindowHeight = 500;
        }
        private static void InitializeGame()
        {
            PaintBorder();
            SetNumberRecord();
            SetBuffList();
        }
        private static void SetBuffList()
        {
            buffList.Add(('E', ConsoleKey.E, new Buff(BuffIncreaseLength),new BuffInitialize(BuffIncreaseLengthInitialize)));


            foreach (var item in buffList)
            {
                item.Item4.Invoke(); //初始化
            }
        }
        private static void PaintBorder()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < MaxHeight; i++)
            {
                Console.SetCursorPosition(0, i);

                Console.WriteLine("|");
                Console.SetCursorPosition(MaxWidth - 1, i);
                Console.WriteLine("|");
            }
            for (int i = 0; i < MaxWidth; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("-");
                Console.SetCursorPosition(i, MaxHeight - 1);
                Console.Write("-");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        private static void SetNumberRecord()
        {
            Console.SetCursorPosition(MaxWidth + 10, 1);
            Console.Write($"Score: {score}       ");

            Console.SetCursorPosition(MaxWidth + 10, 2);
            Console.Write($"Points: {points}     ");
        }
        private static void SetInfor(string info, ConsoleColor bacoground = ConsoleColor.Black)
        {
            Console.SetCursorPosition(MaxWidth + 10, 0);
            Console.Write("                                                                      ");

            Console.SetCursorPosition(MaxWidth + 10, 0);
            ConsoleColor temp = Console.BackgroundColor;
            Console.BackgroundColor = bacoground;
            Console.Write(info);
            Console.BackgroundColor = temp;
        }


        //***
        private static List<Point> snake = new List<Point> { new Point(20, 10) };
        private static List<(char, ConsoleKey, Buff, BuffInitialize)> buffList = new List<(char, ConsoleKey, Buff, BuffInitialize)>();
        private static void mainGame()
        {


            Point food = GenerateFood();
            char direction = 'R';
            char buff = ' ';
            


            while (true)
            {
                if (HasCollidedWithSelf(snake))
                {
                    GameOver();
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



                    // **** Adding buff
                    foreach (var bf in buffList) {
                        if(key == bf.Item2)
                        {
                            bf.Item3.Invoke();
                        }
                    }
                    // **** Adding buff



                }
                Point nextHead = GetNextHead(snake[0], direction);
                snake.Insert(0, nextHead);

                if (nextHead.Equals(food))
                {
                    food = GenerateFood();
                    score += 1;
                    points += 3;
                }
                else
                {
                    snake.RemoveAt(snake.Count - 1);
                }
                Draw(snake, food);


                SetNumberRecord();
                Thread.Sleep((200 - score * 10 > 100) ? (200 - score * 10) : 100);
            }
        }
        //***


        private static void GameOver()
        {
            Console.SetCursorPosition(0, 0);
            //有一点小问题，不知道怎么回事。
            //那就按别的思路走！
            SetInfor("Game over!",ConsoleColor.Red);
            Thread.Sleep(30000);


            return;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Game Over!");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Score: ");

            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(score);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Environment.Exit(0);
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
                //case 'U':
                //    newY = (head.Y - 1 + (MaxHeight - 1)) % (MaxHeight - 1); 
                //    break; //MaxHeight - 1 ------- -1 避免触碰边框
                //case 'D': 
                //    newY = (head.Y + 1) % (MaxHeight - 1); 
                //    break;
                //case 'L': 
                //    newX = (head.X - 1 + (MaxWidth - 1)) % (MaxWidth - 1);
                //    break;
                //case 'R': 
                //    newX = (head.X + 1) % (MaxWidth - 1); 
                //    break;
                case 'U':
                    newY = (newY - 1 == 0) ? MaxHeight - 2 : newY - 1;
                    break;
                case 'D':
                    newY = (newY + 1 == MaxHeight - 2) ? 1 : newY + 1;
                    break;
                case 'L':
                    newX = (newX - 1 == 0) ? MaxWidth - 2 : newX - 1;
                    break;
                case 'R':
                    newX = (newX + 1 == MaxWidth - 2) ? 1 : newX + 1;
                    break;
            }




            return new Point(newX, newY);
        }
        static Point GenerateFood()
        {
            Random rand = new Random();
            return new Point(rand.Next(1, MaxWidth - 1 - 1 * 2), rand.Next(1, MaxHeight - 1 - 1 * 2)); // - 1 * 2 禁止边框
        }

        private static List<Point> lastList;
        private static Point lastFood = new Point(1, 1);
        static void Draw(List<Point> snake, Point food)
        {
            if (lastList != null)
            {
                foreach (var point in lastList)
                {
                    Console.SetCursorPosition(point.X, point.Y);
                    Console.Write(" ");
                }
            }

            Console.SetCursorPosition(lastFood.X, lastFood.Y);
            Console.Write(" ");

            lastList = new List<Point>(snake);  // 在清除蛇之前先复制一份当前的蛇，不能直接赋值，否则shallow copy
            lastFood = food;


            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var point in snake)
            {
                Console.SetCursorPosition(point.X, point.Y);
                Console.Write("O");
            }
            Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(food.X, food.Y);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("X");
            Console.ForegroundColor = ConsoleColor.White;
        }









        //******************************







        private static void decreasePoints(int p)
        {
            points -= p;
            SetNumberRecord();
        }
        private static object Lock = new object();
        private static void BuffIncreaseLength()
        {
            lock(Lock) //防止重新按了
            {
                if (points < 10)
                {
                    SetInfor($"You do not have enough points to use this - it costs 10");
                    return;
                }
                else
                {
                    decreasePoints(10);
                }
            }
            

            Random rd = new Random();
            int len = rd.Next(1, 8 + 1);
            SetInfor($"Buff - increasing the length of the snake. Length: {len}");
            for (int i = 0; i < len; i++)
            {
                snake.Add(snake[snake.Count - 1]);
            }
            
        }
        //***************Buff的提示从3开始***************
        private static void BuffIncreaseLengthInitialize()
        {
            Console.SetCursorPosition(MaxWidth + 10, 3);
            Console.Write("Increase the lenth of the snake randomly - press E - it costs 10 points");
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

}