using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;

namespace ASCII_Orbit
{
    class Program
    {
        static Stopwatch Playtime = new Stopwatch();
        static int GameWidth = 60;
        static int GameHeight = 25;
        static int AreaLeft = ((Console.WindowWidth - GameWidth) / 2) + 1;
        static int AreaTop = (Console.WindowHeight - GameHeight) / 2 + 1;
        static float Invaderspeed = 2f;
        static Random RandomNumber = new Random();
        static int Lives = 3;
        static int InvaderBeat = 0;
        static Timer InvaderSpawnTimer;
        static char[] InvaderTypes = Enumerable.Range('A', 26).Select(i => (char)i).ToArray();
        static ConsoleColor[] Colors = { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.Blue, ConsoleColor.DarkYellow, ConsoleColor.DarkMagenta, ConsoleColor.DarkCyan };
        static List<Invader> Invaders = new List<Invader>();
        static int PlayerPosition;
        static List<(int shotX, int shotY)> Projectiles = new List<(int, int)>();
        static int Score = 0;
        static int NextExtraLife = 10000;
        static int Level = 1;
        static float Min = 3;
        static float Max = 6;
        static Stopwatch FireCooldown = new Stopwatch();

        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x00010000;
        private const int WS_THICKFRAME = 0x00040000;

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        // Main game loop controlling player movement and actions
        static async Task Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.WindowWidth = 80;
            Console.BufferWidth = 80;
            Console.WindowHeight = 30;
            Console.BufferHeight = 30;
            AreaLeft = ((Console.WindowWidth - GameWidth) / 2) + 1;
            AreaTop = (Console.WindowHeight - GameHeight) / 2 + 1;
            IntPtr consoleHandle = GetConsoleWindow();
            int windowStyle = GetWindowLong(consoleHandle, GWL_STYLE);
            windowStyle &= ~WS_MAXIMIZEBOX;
            windowStyle &= ~WS_THICKFRAME;
            SetWindowLong(consoleHandle, GWL_STYLE, windowStyle);

            DisplayTitleScreen();
            Console.Clear();
            bool isRunning = true;
            PlayerPosition = GameWidth / 2;
            ConsoleKeyInfo keyInfo;
            Console.CursorVisible = false;
            DrawBorder();
            DisplayScoreAndLives(Score, Lives, Level);
            await Task.Delay(2000);
            Playtime.Start();
            InvaderSpawnTimer = new Timer(OnInvaderspawn, null, TimeSpan.FromSeconds(Min), TimeSpan.FromSeconds(Max));
            FireCooldown.Start();

            while (isRunning && Lives > 0)
            {
                if (Console.KeyAvailable)
                {
                    keyInfo = Console.ReadKey(true);
                    ClearPosition(PlayerPosition + AreaLeft, GameHeight - 2 + AreaTop);
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            PlayerPosition = Math.Max(1, PlayerPosition - 1);
                            break;
                        case ConsoleKey.RightArrow:
                            PlayerPosition = Math.Min(GameWidth - 2, PlayerPosition + 1);
                            break;
                        case ConsoleKey.Spacebar:
                            if (FireCooldown.ElapsedMilliseconds >= 750)
                            {
                                StartShoot(PlayerPosition + AreaLeft, GameHeight - 3 + AreaTop);
                                FireCooldown.Restart();
                            }
                            break;
                    }
                }
                if (Projectiles.Count > 0)
                    UpdateProjectiles(ref Score);

                MoveInvaders();
                DrawPlayer(PlayerPosition, GameHeight - 2);
                DisplayScoreAndLives(Score, Lives, Level);

                foreach (var invader in Invaders)
                {
                    if (invader.PositionX == PlayerPosition && invader.PositionY == GameHeight - 2)
                    {
                        Lives--;
                        if (Lives <= 0)
                            break;
                    }
                }
                Invaders.RemoveAll(invader => invader.PositionY >= GameHeight - 2);

                if (Lives <= 0)
                {
                    DisplayGameOverScreen(Score, Level);
                    isRunning = false;
                    Projectiles.Clear();
                    Invaders.Clear();
                    Invaderspeed = 10;
                    break;
                }
                await Task.Delay(20);
            }
        }

        // Display the game title screen with instructions
        static void DisplayTitleScreen()
        {
            Console.Clear();
            Console.CursorVisible = false;
            string[] titleLines = {
                @"   __    ___   ___  ____  ____    _____  ____  ____  ____  ____ ",
                @"  /__\  / __) / __)(_  _)(_  _)  (  _  )(  _ \(  _ \(_  _)(_  _)",
                @" /(__)\ \__ \( (__  _)(_  _)(_    )(_)(  )   / ) _ < _)(_   )(  ",
                @"(__)(__)(___/ \___)(____)(____)  (_____)(_)\_)(____/(____) (__) "
            };

            string instructions = "PRESS SPACE TO START THE GAME";
            int screenWidth = Console.WindowWidth;
            int screenHeight = Console.WindowHeight;
            int titleHeight = titleLines.Length;
            int startY = ((screenHeight - titleHeight) / 2) - 2;
            int instructionsY = startY + titleHeight + 2;
            Console.ForegroundColor = ConsoleColor.Cyan;

            foreach (string line in titleLines)
            {
                Console.SetCursorPosition((screenWidth - line.Length) / 2, startY++);
                Console.WriteLine(line);
            }
            Console.ResetColor();
            BlinkInstructions(instructions, screenWidth, instructionsY);
            Console.Clear();
        }

        // Displays blinking instructions until spacebar is pressed
        static void BlinkInstructions(string instructions, int screenWidth, int instructionsY)
        {
            bool showInstructions = true;
            while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Spacebar)
            {
                Console.SetCursorPosition((screenWidth - instructions.Length) / 2, instructionsY);
                if (showInstructions)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(instructions);
                    Console.ResetColor();
                }
                else
                    Console.WriteLine(new string(' ', instructions.Length));
                showInstructions = !showInstructions;
                Thread.Sleep(500);
            }
        }

        // Draws a border around the game area
        static void DrawBorder()
        {
            string horizontalBorder = new string('═', GameWidth - 2);
            string verticalBorder = "║";
            Console.SetCursorPosition(AreaLeft, AreaTop);
            Console.Write("╔" + horizontalBorder + "╗");
            for (int y = 1; y < GameHeight - 1; y++)
            {
                Console.SetCursorPosition(AreaLeft, AreaTop + y);
                Console.Write(verticalBorder);
                Console.SetCursorPosition(AreaLeft + GameWidth - 1, AreaTop + y);
                Console.Write(verticalBorder);
            }
            Console.SetCursorPosition(AreaLeft, AreaTop + GameHeight - 1);
            Console.Write("╚" + horizontalBorder + "╝");
        }

        // Clears a specific position within the game area
        static void ClearPosition(int x, int y)
        {
            if (x > AreaLeft && x < AreaLeft + GameWidth - 1)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(" ");
            }
        }

        // Displays score, lives and level information
        static void DisplayScoreAndLives(int Score, int Lives, int Level)
        {
            Console.SetCursorPosition(AreaLeft + 1, AreaTop - 1);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Lives: ");
            Console.ResetColor();
            Console.Write($"{Lives:D2}            ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Score: ");
            Console.ResetColor();
            Console.Write($"{Score:D9}            ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Level: ");
            Console.ResetColor();
            Console.Write($"{Level:D2}");
        }

        // Initiates a projectile from the player's position
        static void StartShoot(int startX, int startY)
        {
            Task.Run(() => Console.Beep(200, 100));
            Projectiles.Add((startX, startY));
        }

        // Handles invader spawn and timer adjustment
        static void OnInvaderspawn(object state)
        {
            int xPos = RandomNumber.Next(1, GameWidth - 1);
            int yPos = 1;
            char enemyChar = InvaderTypes[RandomNumber.Next(InvaderTypes.Length)];
            ConsoleColor color = Colors[RandomNumber.Next(Colors.Length)];
            Invader newInvader = new Invader(xPos, yPos, enemyChar, color, Invaderspeed);
            Invaders.Add(newInvader);
            Console.Beep(300, 100);
            double nextSpawnTimeInSeconds = RandomNumber.NextDouble() * (Max - Min) + Min;
            if (InvaderSpawnTimer != null)
                InvaderSpawnTimer.Change(TimeSpan.FromSeconds(nextSpawnTimeInSeconds), Timeout.InfiniteTimeSpan);
        }

        // Updates projectile positions and handles collision detection
        static void UpdateProjectiles(ref int Score)
        {
            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                var (shotX, shotY) = Projectiles[i];
                ClearPosition(shotX, shotY);
                shotY--;

                bool shotRemoved = false;
                if (shotY >= AreaTop + 1)
                {
                    foreach (var invader in Invaders.ToList())
                    {
                        if ((shotX == AreaLeft + invader.PositionX && shotY == AreaTop + invader.PositionY) ||
                            (shotX == AreaLeft + invader.PositionX && shotY == AreaTop + invader.PositionY - 1))
                        {
                            Score += 50 + (Level * 50);
                            InvaderBeat++;
                            ClearPosition(shotX, shotY);
                            Console.Beep(800, 100);
                            Invaders.Remove(invader);
                            shotRemoved = true;
                            DisplayScoreAndLives(Score, Lives, Level);
                            if (Score >= NextExtraLife)
                            {
                                NextExtraLife = NextExtraLife * 2;
                                Lives++;
                                Console.Beep(1200, 200);
                                Thread.Sleep(100);
                                Console.Beep(800, 200);
                            }
                            if (InvaderBeat == 5 + (2 * (Level - 1)))
                            {
                                InvaderBeat = 0;
                                Level++;
                                Console.Beep(500, 100);
                                Console.Beep(600, 100);
                                Console.Beep(700, 100);
                                Console.Beep(800, 100);
                                if (Min > 1.0f) Min -= 0.2f;
                                if (Max > 2.5f) Max -= 0.2f;
                            }
                            break;
                        }
                    }
                    if (!shotRemoved)
                    {
                        Projectiles[i] = (shotX, shotY);
                        Console.SetCursorPosition(shotX, shotY);
                        Console.Write("|");
                    }
                    else
                        Projectiles.RemoveAt(i);
                }
                else
                    Projectiles.RemoveAt(i);
            }
        }

        // Moves invaders downwards and handles collisions
        static void MoveInvaders()
        {
            for (int i = Invaders.Count - 1; i >= 0; i--)
            {
                Invader invader = Invaders[i];
                int previousY = invader.PositionY;

                invader.Move(Level);

                if (invader.PositionY != previousY)
                {
                    ClearPosition(AreaLeft + invader.PositionX, AreaTop + previousY);
                    DrawInvader(invader.PositionX, invader.PositionY, invader.Char, invader.Color);
                }

                if (invader.PositionY >= GameHeight - 2)
                {
                    Console.Beep(500, 200);
                    Console.Beep(300, 200);
                    Invaders.RemoveAt(i);
                    Lives--;
                    if (Lives <= 0)
                        break;
                }
            }
        }

        // Draws the player's character on screen
        static void DrawPlayer(int positionX, int positionY)
        {
            Console.SetCursorPosition(positionX + AreaLeft, positionY + AreaTop);
            Console.Write("▲");
        }

        // Draws an invader at the specified position
        static void DrawInvader(int positionX, int positionY, char character, ConsoleColor color)
        {
            Console.SetCursorPosition(positionX + AreaLeft, positionY + AreaTop);
            Console.ForegroundColor = color;
            Console.Write(character);
            Console.ResetColor();
        }

        // Displays the game over screen with player statistics
        static void DisplayGameOverScreen(int Score, int Level)
        {
            Console.Clear();
            Console.CursorVisible = false;
            int centerX = Console.WindowWidth / 2;
            int centerY = Console.WindowHeight / 2 - 8;
            string[] gameOverString = new string[] {
                @"  ____    _    __  __ _____    _____     _______ ____  ",
                @" / ___|  / \  |  \/  | ____|  / _ \ \   / / ____|  _ \ ",
                @"| |  _  / _ \ | |\/| |  _|   | | | \ \ / /|  _| | |_) |",
                @"| |_| |/ ___ \| |  | | |___  | |_| |\ V / | |___|  _ < ",
                @" \____/_/   \_\_|  |_|_____|  \___/  \_/  |_____|_| \_\",
            };

            if (InvaderSpawnTimer != null)
            {
                InvaderSpawnTimer.Change(Timeout.Infinite, Timeout.Infinite);
                InvaderSpawnTimer.Dispose();
                InvaderSpawnTimer = null;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var line in gameOverString)
            {
                Console.SetCursorPosition(centerX - line.Length / 2, centerY++);
                Console.WriteLine(line);
            }
            Console.ResetColor();
            int infoY = centerY + 3;
            string longestLine = $"Playtime:\t{59:D2}m {59:D2}s";
            int totalWidth = longestLine.Length;
            int infoX = centerX - (totalWidth / 2) - 2;

            Console.SetCursorPosition(infoX, infoY);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Score:\t  ");
            Console.ResetColor();
            Console.WriteLine($"{Score,9:D9}");

            Console.SetCursorPosition(infoX, infoY + 1);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Level:\t  ");
            Console.ResetColor();
            Console.WriteLine($"{Level,9:D2}");

            Console.SetCursorPosition(infoX, infoY + 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Time:\t  ");
            Console.ResetColor();
            Console.WriteLine($"{Playtime.Elapsed.Minutes,6:D2}:{Playtime.Elapsed.Seconds:D2}");

            Console.SetCursorPosition(infoX, infoY + 3);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Rank:\t  ");
            Console.ResetColor();
            Console.WriteLine($"{GetRank(Score),9}");

            Console.SetCursorPosition(centerX - 16, infoY + 7);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("PRESS THE SPACE BUTTON TO CONTINUE");
            while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Spacebar)
            {
                Thread.Sleep(500);
                Console.SetCursorPosition(centerX - 16, infoY + 7);
                Console.Write(new string(' ', "PRESS THE SPACE BUTTON TO CONTINUE".Length));
                Thread.Sleep(500);
                Console.SetCursorPosition(centerX - 16, infoY + 7);
                Console.Write("PRESS THE SPACE BUTTON TO CONTINUE");
            }
            ResetGameState(ref Score, ref Lives, ref Level, ref InvaderBeat);
            DisplayTitleScreen();
            Invaderspeed = 10;
        }

        // Resets game state for a new session
        static void ResetGameState(ref int score, ref int lives, ref int level, ref int invaderBeat)
        {
            score = 0;
            lives = 3;
            level = 1;
            invaderBeat = 0;
            PlayerPosition = GameWidth / 2;
            Projectiles.Clear();
            Invaders.Clear();
            Console.Clear();
            DrawBorder();
            DisplayScoreAndLives(score, lives, level);
            Playtime.Reset();
            Playtime.Start();
            FireCooldown.Reset();
        }

        // DeterMines player's rank based on Score
        static string GetRank(int Score)
        {
            int rank = 0;
            int currentThreshold = 3000;
            int result = Score;

            while (result >= currentThreshold)
            {
                result -= currentThreshold;
                rank++;
                currentThreshold = 3000 + (750 * rank);
            }

            if (rank == 0) return "F";
            else if (rank == 1) return "F+";
            else if (rank == 2) return "E-";
            else if (rank == 3) return "E";
            else if (rank == 4) return "E+";
            else if (rank == 5) return "D-";
            else if (rank == 6) return "D";
            else if (rank == 7) return "D+";
            else if (rank == 8) return "C-";
            else if (rank == 9) return "C";
            else if (rank == 10) return "C+";
            else if (rank == 11) return "B-";
            else if (rank == 12) return "B";
            else if (rank == 13) return "B+";
            else if (rank == 14) return "A-";
            else if (rank == 15) return "A";
            else if (rank == 16) return "A+";
            else if (rank == 17) return "S-";
            else if (rank == 18) return "S";
            else return "S+";
        }
    }

    class Invader
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public char Char { get; set; }
        public ConsoleColor Color { get; set; }
        public double MoveSpeedInSeconds { get; set; }

        private Stopwatch moveTimer = new Stopwatch();

        public Invader(int posX, int posY, char character, ConsoleColor color, double moveSpeedInSeconds)
        {
            PositionX = posX;
            PositionY = posY;
            Char = character;
            Color = color;
            MoveSpeedInSeconds = moveSpeedInSeconds;
            moveTimer.Start();
        }

        public void Move(int Level)
        {
            double reductionPercentage = 0.20;
            double MaxReductionLimit = MoveSpeedInSeconds / 5;
            double adjustedMoveSpeed = MoveSpeedInSeconds * Math.Pow(1 - reductionPercentage, Level - 1);
            adjustedMoveSpeed = Math.Max(adjustedMoveSpeed, MaxReductionLimit);
            if (moveTimer.Elapsed.TotalSeconds >= adjustedMoveSpeed)
            {
                PositionY++;
                moveTimer.Restart();
            }
        }
    }
}