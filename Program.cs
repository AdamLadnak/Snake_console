using System;
using System.Collections.Generic;
using System.Timers;

class Program
{
    static int windowWidth = 30;
    static int windowHeight = 20;
    static int velkostBunky = 1;
    static bool koniecHry = true;
    static System.Timers.Timer gameTimer;
    static bool isTimerRunning = false;
    static bool menuIsOpen = false;
    static int skore = 0;
    static Difficulty obtiaznost = Difficulty.Medium;

    static Snake snake;
    static Food jedlo;
    static GameEnvironment herneProstredie;

    enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    static void Main()
    {
        Console.CursorVisible = false;
        herneProstredie = new GameEnvironment(windowWidth, windowHeight);

        while (true)
        {
            hlavneMenu();
        }
    }

    static void hlavneMenu()
    {
        menuIsOpen = true;
        Console.Clear();
        herneProstredie.nakresliOhranicenie();

        Console.SetCursorPosition(windowWidth / 2 - 3, windowHeight / 2 - 1);
        Console.Write("1 PLAY");
        Console.SetCursorPosition(windowWidth / 2 - 3, windowHeight / 2 + 1);
        Console.Write("2 EXIT");
        Console.SetCursorPosition(0, windowHeight + 1);

        while (menuIsOpen)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.NumPad1 || key == ConsoleKey.D1)
            {
                menuIsOpen = false;
                vyberObtiaznost();
            }
            else if (key == ConsoleKey.NumPad2 || key == ConsoleKey.D2)
            {
                Environment.Exit(0);
            }
        }
    }

    static void vyberObtiaznost()
    {
        menuIsOpen = true;
        Console.Clear();
        herneProstredie.nakresliOhranicenie();

        Console.SetCursorPosition(windowWidth / 2 - 4, windowHeight / 2 - 2);
        Console.Write("1 EASY");
        Console.SetCursorPosition(windowWidth / 2 - 4, windowHeight / 2 - 1);
        Console.Write("2 MEDIUM");
        Console.SetCursorPosition(windowWidth / 2 - 4, windowHeight / 2);
        Console.Write("3 HARD");
        Console.SetCursorPosition(windowWidth / 2 - 4, windowHeight / 2 + 2);
        Console.Write("4 BACK");
        Console.SetCursorPosition(0, windowHeight + 1);

        while (menuIsOpen)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.NumPad1 || key == ConsoleKey.D1)
            {
                obtiaznost = Difficulty.Easy;
                menuIsOpen = false;
                novaHra();
            }
            else if (key == ConsoleKey.NumPad2 || key == ConsoleKey.D2)
            {
                obtiaznost = Difficulty.Medium;
                menuIsOpen = false;
                novaHra();
            }
            else if (key == ConsoleKey.NumPad3 || key == ConsoleKey.D3)
            {
                obtiaznost = Difficulty.Hard;
                menuIsOpen = false;
                novaHra();
            }
            else if (key == ConsoleKey.NumPad4 || key == ConsoleKey.D4)
            {
                menuIsOpen = false;
                hlavneMenu();
            }
        }
    }

    static void novaHra()
    {
        Console.Clear();
        snake = new Snake(windowWidth / 2, windowHeight / 2);
        jedlo = new Food(windowWidth, windowHeight, snake);
        herneProstredie.nakresliOhranicenie();
        jedlo.Nakresli();
        Console.SetCursorPosition(0, windowHeight + 1);
        Console.Write("Skóre: 0");
        koniecHry = false;
        skore = 0;

        gameTimer = new System.Timers.Timer(GetGameSpeed());
        gameTimer.Elapsed += OnTimedEvent;
        gameTimer.Start();
        isTimerRunning = true;

        while (!koniecHry)
        {
            var key = Console.ReadKey(true).Key;
            OnKeyPress(key);
        }
    }

    static int GetGameSpeed()
    {
        switch (obtiaznost)
        {
            case Difficulty.Easy:
                return 150;
            case Difficulty.Medium:
                return 100;
            case Difficulty.Hard:
                return 50;
            default:
                return 100;
        }
    }

    static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        if (snake.StavKoniecHry(windowWidth, windowHeight))
        {
            Console.SetCursorPosition(0, windowHeight + 2);
            Console.WriteLine("GAME OVER");
            Console.WriteLine($"Skóre: {skore}");
            Console.WriteLine("Stlačte akúkoľvek klávesu pre návrat do menu...");
            koniecHry = true;

            gameTimer.Stop();
            isTimerRunning = false;
            return;
        }

        (int oldX, int oldY) = snake.Posun();
        snake.ResetMove();

        Console.SetCursorPosition(oldX, oldY);
        Console.Write(" ");

        if (snake.HlavaX == jedlo.X && snake.HlavaY == jedlo.Y)
        {
            jedlo.NovaPozicia();
            jedlo.Nakresli();
            skore++;
            Console.SetCursorPosition(7, windowHeight + 1);
            Console.Write(skore);
        }
        else
        {
            (int tailX, int tailY) = snake.Chvost[0];
            Console.SetCursorPosition(tailX, tailY);
            Console.Write(" ");
            snake.Chvost.RemoveAt(0);
        }

        Console.SetCursorPosition(snake.HlavaX, snake.HlavaY);
        Console.Write("S");

        Console.SetCursorPosition(snake.Chvost[^1].Item1, snake.Chvost[^1].Item2);
        Console.Write("O");
    }


    static void OnKeyPress(ConsoleKey key)
    {
        snake.Ovladaj(key, velkostBunky);

        if (key == ConsoleKey.P)
        {
            if (isTimerRunning)
            {
                gameTimer.Stop();
                isTimerRunning = false;
                Console.SetCursorPosition(0, windowHeight + 2);
                Console.WriteLine("P - Resume");
                Console.WriteLine("Q - Quit to Main Menu");
            }
            else
            {
                Console.SetCursorPosition(0, windowHeight + 2);
                for (int i = 0; i < 3; i++)
                {
                    Console.Write(new string(' ', Console.WindowWidth));
                }
                gameTimer.Start();
                isTimerRunning = true;
            }
        }
        else if (key == ConsoleKey.Q)
        {
            koniecHry = true;
            return;
        }
    }
}

class GameEnvironment
{
    private int windowWidth;
    private int windowHeight;

    public GameEnvironment(int windowWidth, int windowHeight)
    {
        this.windowWidth = windowWidth;
        this.windowHeight = windowHeight;
    }

    public void nakresliOhranicenie()
    {
        for (int x = 0; x < windowWidth; x++)
        {
            Console.SetCursorPosition(x, 0);
            Console.Write("#");
            Console.SetCursorPosition(x, windowHeight - 1);
            Console.Write("#");
        }

        for (int y = 0; y < windowHeight; y++)
        {
            Console.SetCursorPosition(0, y);
            Console.Write("#");
            Console.SetCursorPosition(windowWidth - 1, y);
            Console.Write("#");
        }
    }
}

class Snake
{
    public int HlavaX { get; private set; }
    public int HlavaY { get; private set; }
    public int Dx { get; private set; }
    public int Dy { get; private set; }
    public List<(int, int)> Chvost { get; private set; }
    private bool hasMoved;
    private int lastDx;
    private int lastDy;

    public Snake(int startX, int startY)
    {
        HlavaX = startX;
        HlavaY = startY;
        Dx = 0;
        Dy = 0;
        Chvost = new List<(int, int)>();
        hasMoved = false;
        lastDx = 0;
        lastDy = 0;
    }

    public (int, int) Posun()
    {
        Chvost.Add((HlavaX, HlavaY));
        int oldX = HlavaX;
        int oldY = HlavaY;

        HlavaX += Dx;
        HlavaY += Dy;

        hasMoved = true;
        return (oldX, oldY);
    }

    public void Ovladaj(ConsoleKey key, int velkostBunky)
    {
        if (hasMoved)
            return;

        if (key == ConsoleKey.LeftArrow && Dx == 0)
        {
            if (lastDx != velkostBunky)
            {
                Dx = -velkostBunky;
                Dy = 0;
            }
        }
        else if (key == ConsoleKey.RightArrow && Dx == 0)
        {
            if (lastDx != -velkostBunky)
            {
                Dx = velkostBunky;
                Dy = 0;
            }
        }
        else if (key == ConsoleKey.UpArrow && Dy == 0)
        {
            if (lastDy != velkostBunky)
            {
                Dy = -velkostBunky;
                Dx = 0;
            }
        }
        else if (key == ConsoleKey.DownArrow && Dy == 0)
        {
            if (lastDy != -velkostBunky)
            {
                Dy = velkostBunky;
                Dx = 0;
            }
        }
    }

    public void ResetMove()
    {
        hasMoved = false;
        lastDx = Dx;
        lastDy = Dy;
    }

    public bool StavKoniecHry(int windowWidth, int windowHeight)
    {
        return HlavaX <= 0 || HlavaX >= windowWidth - 1 || HlavaY <= 0 || HlavaY >= windowHeight - 1 || Chvost.Contains((HlavaX, HlavaY));
    }
}

class Food
{
    public int X { get; private set; }
    public int Y { get; private set; }
    private Random rand = new Random();
    private int windowWidth;
    private int windowHeight;
    private Snake snake;

    public Food(int windowWidth, int windowHeight, Snake snake)
    {
        this.windowWidth = windowWidth;
        this.windowHeight = windowHeight;
        this.snake = snake;
        NovaPozicia();
    }

    public void NovaPozicia()
    {
        do
        {
            X = rand.Next(1, windowWidth - 1);
            Y = rand.Next(1, windowHeight - 1);
        } while (snake.Chvost.Contains((X, Y)) || (X == snake.HlavaX && Y == snake.HlavaY));
    }

    public void Nakresli()
    {
        Console.SetCursorPosition(X, Y);
        Console.Write("J");
    }
}
