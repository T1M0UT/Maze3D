using System;
using System.Linq;
using static Maze3D.Values;
using static Maze3D.SelectionMenu;
using System.Collections.Generic;
using System.Threading;

namespace Maze3D
{
    

    class Game
    {
        private MyVector newPlayerVector;
        private MyVector delta;
        private Maze maze;
        private User user;
        private SelectionMenu shop;
        private SelectionMenu accounts;
        private List<User> users;
        private string currentWindow;
        private GameState gameState;


        public static readonly List<(string Name, int Price)> products = new List<(string Name, int Price)>
        {
            ("Life points extension", 2),
            ("Additional life", 1),
            ("Oxygen tank extension", 1),
            ("Refuel oxygen tank", 1),
            ("new secret item", 3)
        };

        public static Game GetGame(List<User> users)
        {
            Game game = new Game();
            FieldCreationParams parameters = StartReader();
            game.maze = Maze.MakeMaze(parameters);
            game.shop = new SelectionMenu("Shop", products);
            game.users = users;
            List<(string username, int gameCounter)> accounts = new List<(string username, int gameCounter)>();
            users.ForEach((User user) => accounts.Add((user.Username, user.GamesCounter)));
            game.accounts = new SelectionMenu("Accounts", accounts);
            game.gameState = GameState.InProgress;
            return game;
        }

        public void Run()
        {
            Init();
            while (IsEndGame() == GameState.InProgress)
            {
                Draw();
                do Input();
                while (!ActionOnInputHappened());
            }
            GameOutcome();
        }

        private void Init()
        {
            
            ChooseUser();
            SetupUser();
        }

        private void ChooseUser()
        {
            currentWindow = "maze";
            
            if (users.Count == 0)
            {
                SignUp();
            }
            else
            {
                user = users[0];
                SayWelcomeBack();
            }
  
            Console.WriteLine($"To change an account press P");
            if (Console.ReadKey().Key == ConsoleKey.P)
            {
                currentWindow = "accounts";
            }
        }
        
        private void SetupUser()
        {
            user.CurrentHearts = user.MaxHearts;
            int startingFee = random.Next(0, 20);
            user.CurrentOxygen = user.MaxOxygen - startingFee;
        }

        public static void Rules()
        {
            Console.WriteLine($"1.YOU play as '{player}', first find a key '{key}' and then reach '{finish}'.");
            Console.WriteLine("2.There is a chance that there will be no passage.");
            Console.WriteLine("3.This maze is three-dimensional (imagine a cube). Use WASD+RF to move.");
            Console.WriteLine($"4.There are some '{wall}' - walls.");
            Console.WriteLine($"5.There are several visual pointers like '{wallTop}', '{wallBottom}', '{wallBoth}' that show in which direction the '{wall}” is in the third dimension.");
            Console.WriteLine("6.There is oxygen counter, you spend one per each move.");
            Console.WriteLine($"7.Spikes '{spike}' are critical.");
            Console.WriteLine($"8.The sign '{hazard}' is awaring you of a spike '{spike}' in third dimension.");
            Console.WriteLine($"9.There are oxygen bubbles: '{oxygen}', coins: '{coin}'");
            Console.WriteLine("Extend your health bar and such to survive longer.");
            Console.WriteLine("Use 'b' key to enter the shop during the game.");
            Console.WriteLine("Use ARROWS to navigate in menus and ENTER to purchase goods.");
            Console.WriteLine("Use 'i' key to open and close rules during the game.");
            Console.WriteLine("Use 'k' to kick the bucket if needed.");
            Console.WriteLine("Enjoy! Press any key to start.");
            Console.ReadKey();
        }

        public static FieldCreationParams StartReader()
        {
            int frequency = 10;
            Console.WriteLine("Press 'i' to read the rules. Or any other key to start.");
            var key = Console.ReadKey().Key;
            if (key == ConsoleKey.I)
            {
                Rules();
            }
            Console.Write("Walls percent (5-10 optimally) = ");

            bool parseSuccess = int.TryParse(Console.ReadLine(), out frequency);
            while (!parseSuccess || frequency > 100 || frequency < 0)
            {
                Console.Write("Incorrect value! TryAgain: ");
                parseSuccess = int.TryParse(Console.ReadLine(), out frequency);
            }
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.CursorVisible = false;

            MyVector size = new MyVector(42, 22, 6);
            return new FieldCreationParams(size, frequency);
        }

        #region Draw

        void Draw()
        {
            Console.Clear();
            if (currentWindow == "maze") MazeDraw();
            else if (currentWindow == "shop") ShopDraw();
            else if (currentWindow == "accounts") AccountsDraw();
            Console.ResetColor();
        }

        private void MazeDraw()
        {
            for (int j = 0; j <= maze.Size.Y; j++)
            {
                for (int i = 0; i <= maze.Size.X; i++)
                {
                    Console.BackgroundColor = GetMazeBackgroundColor(i, j);
                    Console.Write(maze[i, j, maze.PlayerVector.Z]);
                }
                Console.WriteLine();
            }
            if (user.CurrentOxygen <= 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else if (user.CurrentOxygen <= 10) Console.BackgroundColor = ConsoleColor.Red;
            else if (user.CurrentOxygen <= 25) Console.BackgroundColor = ConsoleColor.DarkYellow;
            else Console.BackgroundColor = ConsoleColor.DarkGreen;

            Console.WriteLine('\n' + $"-- Oxygen level {user.CurrentOxygen}/{user.MaxOxygen} -- "
                + $"{(user.HasKey ? "You have the key!" : "Find a key")} -- "
                + $"Layer {maze.PlayerVector.Z}/{maze.Size.Z - 1} --");
            Console.ResetColor();
            Console.WriteLine($" {user.Username} -- {GetHearts()} -- Coins: {user.CoinCounter} --");
        }

        private string GetHearts()
        {
            if (user.MaxHearts <= 8)
            {
                return new String(emptyHeart, user.MaxHearts - user.CurrentHearts) +
                new String(fullHeart, user.CurrentHearts);
            }
            return $"{user.CurrentHearts}/{user.MaxHearts} Hearts";
        }

        private ConsoleColor GetMazeBackgroundColor(int i, int j)
        {
            if (i == maze.PlayerVector.X && j == maze.PlayerVector.Y)
            {
                return ConsoleColor.Red;
            }
            if (new MyVector(i, j, maze.PlayerVector.Z) ==  maze.FinishVector)
            {
                return ConsoleColor.Green;
            }
            return Values.BackgroundColor;

        }

        private void ShopDraw()
        {
            Console.WriteLine(shop.ToString() + "\n");
            int productIndex = 0;
            string message = "";
            foreach ((string productName, int productPrice) in shop.items)
            {
                string pluralCoinForm = (productPrice == 1 ? "" : "s");
                string cursorOrSpace = "   ";
                
                ConsoleColor color = Values.BackgroundColor;
                SelectionState itemSelectionState = SelectionState.None;
                if (shop.CurrentSelectedIndex == productIndex)
                {
                    itemSelectionState = shop.Selection;
                    cursorOrSpace = " > ";
                }
                    
                switch (itemSelectionState)
                {
                    case SelectionState.Select:
                        color = ConsoleColor.Blue;
                        break;
                    case SelectionState.Success:
                        color = ConsoleColor.DarkGreen;
                        message = $"{productName} purchased!";
                        break;
                    case SelectionState.Fail2:
                        color = ConsoleColor.DarkRed;
                        message = "Bar is full.";
                        break;
                    case SelectionState.Fail1:
                        color = ConsoleColor.DarkRed;
                        message = "Not enough coins.";
                        break;
                }

                Console.BackgroundColor = color;

                Console.WriteLine(String
                    .Join(" ", cursorOrSpace, productName, Values.verticalWall, productPrice, "coin") 
                    + pluralCoinForm
                  );

                Console.ResetColor();
                productIndex++;
            }
            Console.WriteLine("\n " + message);
        }

        private void AccountsDraw()
        {
            Console.WriteLine(accounts.ToString() + "\n");
            string cursorOrSpace = "   ";
            SelectionState currentItemSelection = SelectionState.None;

            for (int userIndex = 0; userIndex <= accounts.MaxIndex; userIndex++)
            {
                string username = accounts[userIndex].item;
                int gameCounter = accounts[userIndex].value;

                currentItemSelection = SelectionState.None;

                if (accounts.CurrentSelectedIndex == userIndex)
                {
                    currentItemSelection = accounts.Selection;
                    cursorOrSpace = " > ";
                    if(accounts.Selection == SelectionState.Success)
                    {
                        Login(userIndex);
                        return;
                    }
                }

                Console.BackgroundColor = SelectColor(currentItemSelection);
                Console.WriteLine(cursorOrSpace + username + " --- Games played: " + gameCounter + " ");
                cursorOrSpace = "   ";
            }

            DisplayAdditionalItem(
                accounts.CurrentSelectedIndex,
                accounts.MaxIndex + 1,
                "(Create new account)",
                () => SignUp()
              );
            DisplayAdditionalItem(
                accounts.CurrentSelectedIndex,
                accounts.MaxIndex + 2,
                "(Play as guest)",
                () => GuestMode()
                );

            Console.BackgroundColor = Values.BackgroundColor;
        }

        private void GuestMode()
        {
            user = new User();
            currentWindow = "maze";
            Thread.Sleep(300);
            Console.ResetColor();
            Console.Write("");
            Draw();
        }

        private void DisplayAdditionalItem(int selectedIndex, int additionalIndex, string text, Action action)
        {
            SelectionState currentItemSelection = SelectionState.None;
            string cursorOrSpace = "   ";

            if (selectedIndex == additionalIndex)
            {
                currentItemSelection = accounts.Selection;
                cursorOrSpace = " > ";
            }

            Console.BackgroundColor = SelectColor(currentItemSelection);
            Console.WriteLine(cursorOrSpace + text);
            if (selectedIndex == additionalIndex && accounts.Selection == SelectionState.Success)
            {
                action.Invoke();
            }
        }

        private void SayWelcomeBack()
        {
            Console.WriteLine($"Welcome back, {user.Username} -- Coins: {user.CoinCounter}");
        }

        private void Login(int accountIndex)
        {
            user = users[accountIndex];
            currentWindow = "maze";
            Draw();
        }

        private void SignUp()
        {
            Console.ResetColor();
            Console.Clear();
            Console.Write("Hi! What's your name?: ");
            string username = Console.ReadLine();
            user = new User(username);
            Console.WriteLine($"Welcome, {user.Username} -- Coins: {user.CoinCounter}");
            users.Add(user);
            Thread.Sleep(1700);
            currentWindow = "maze";
            Draw();
        }

        private ConsoleColor SelectColor(SelectionState selectionState)
        {
            ConsoleColor color = selectionState switch
            {
                SelectionState.Select => ConsoleColor.Blue,
                SelectionState.Success => ConsoleColor.DarkGreen,
                _ => Values.BackgroundColor
            };
            return color;
        }

        #endregion

        #region Movement
        private void Input()
        {
            if (currentWindow == "maze")
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.W:
                        delta.Y = -1;
                        break;
                    case ConsoleKey.S:
                        delta.Y = 1;
                        break;
                    case ConsoleKey.A:
                        delta.X = -1;
                        break;
                    case ConsoleKey.D:
                        delta.X = 1;
                        break;
                    case ConsoleKey.R:
                        delta.Z = 1;
                        break;
                    case ConsoleKey.F:
                        delta.Z = -1;
                        break;
                    case ConsoleKey.B:
                        currentWindow = "shop";
                        shop.Selection = SelectionState.Select;
                        Draw();
                        break;
                    case ConsoleKey.K:
                        user.CurrentHearts = 0;
                        currentWindow = "maze";
                        break;
                    case ConsoleKey.I:
                        Console.Clear();
                        Rules();
                        Draw();
                        break;
                }
            }
            else if (currentWindow == "shop")
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.B:
                        currentWindow = "maze";
                        break;
                    case ConsoleKey.UpArrow:
                        if (shop.CurrentSelectedIndex > 0) shop.CurrentSelectedIndex--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (shop.CurrentSelectedIndex < shop.MaxIndex) shop.CurrentSelectedIndex++;
                        break;
                    case ConsoleKey.Enter:
                        TryPurchase();
                        break;
                    case ConsoleKey.I:
                        Console.Clear();
                        Rules();
                        break;
                }
                Draw();
            }
            else if (currentWindow == "accounts")
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        if (accounts.CurrentSelectedIndex > 0) accounts.CurrentSelectedIndex--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (accounts.CurrentSelectedIndex < accounts.MaxIndex + 2) accounts.CurrentSelectedIndex++;
                        break;
                    case ConsoleKey.Enter:
                        accounts.Selection = SelectionState.Success;
                        break;
                }
                Draw();
            }

            while (Console.KeyAvailable)
            {
                Console.ReadKey(false);
            }
        }


        private void TryPurchase()
        {
            var shopItem = shop.items[shop.CurrentSelectedIndex];
            string item = shopItem.item;
            int price = shopItem.value;
            if (price <= user.CoinCounter)
            {
                if (ApplyPurchase(item))
                {
                    user.CoinCounter -= price;
                    shop.Selection = SelectionState.Success;
                }
                else shop.Selection = SelectionState.Fail2;
            }
            else shop.Selection = SelectionState.Fail1;
        }

        private bool ApplyPurchase(string item)
        {
            bool success = false;
            switch (item)
            {
                case "Life points extension":
                    user.MaxHearts++;
                    success = true;
                    break;
                case "Additional life":
                    if (user.CurrentHearts < user.MaxHearts)
                    {
                        user.CurrentHearts++;
                        success = true;
                    }
                    break;
                case "Oxygen tank extension":
                    user.MaxOxygen += 5;
                    success = true;
                    break;
                case "Refuel oxygen tank":
                    if(user.CurrentOxygen < user.MaxOxygen)
                    {
                        user.CurrentOxygen = user.MaxOxygen;
                        success = true;
                    }
                    break;
            }
            return success;
        }



        private bool ActionOnInputHappened()
        {
            if (DidMove())
            {
                IsEndGame();
                return true;
            }
            return false;
        }

        private bool DidMove()
        {
            newPlayerVector = maze.PlayerVector + delta;
            bool movementOccured = false;
            if (delta == 0 && user.CurrentHearts != 0) return false;

            if (IsWalkable())
            {
                Goto();
                movementOccured = true;
            }
            delta = 0;
            return movementOccured;
        }

        private bool IsWalkable()
        {
            if ((newPlayerVector > 0) && (newPlayerVector < maze.Size))
            {
                if (maze[newPlayerVector] != wall)
                    return true;
            }
            return false;
        }

        private void Goto()
        {
            if (maze.PlayerVector == maze.FinishVector)
            {
                maze[maze.PlayerVector] = finish;
            }
            else maze[maze.PlayerVector] = space;

            maze[maze.PlayerVector] = maze.GetMapPointer(maze.PlayerVector);

            maze.PlayerVector = newPlayerVector;
            user.CurrentOxygen -= 1;
            if (maze[newPlayerVector] == oxygen)
            {
                if (user.MaxOxygen < user.CurrentOxygen + user.BonusOxygen)
                {
                    user.CurrentOxygen = user.MaxOxygen;
                }
                else user.CurrentOxygen += user.BonusOxygen;
            }
            else if (maze[newPlayerVector] == key)
            {
                user.HasKey = true;
            }
            else if (maze[newPlayerVector] == spike)
            {
                user.CurrentHearts--;
            }
            else if (maze[newPlayerVector] == coin)
            {
                user.CoinCounter++;
            }
            else if (maze[newPlayerVector] == finish && user.HasKey)
            {
                user.CoinCounter += 5;
            }
            maze[newPlayerVector] = player;
        }
        #endregion

        private GameState IsEndGame()
        {
            if (maze[maze.PlayerVector] == maze[maze.FinishVector] && user.HasKey)
            {
                return GameState.Win;
            }
            if (user.CurrentOxygen <= 0)
            {
                return GameState.OutOfOxygen;
            }
            if (user.CurrentHearts == 0)
            {
                return GameState.DiedOnSpike;
            }
            return GameState.InProgress;
        }

        private void GameOutcome()
        {
            Draw();
            string endgameText = gameState switch
            {
                GameState.Win => "Winni macaroni!\n\n" + String.Join("\n",
                  "╲╲╲╲╲╲╭╯╯╮╱╱╱╱╱╱",
                  "╲╭╰╰╮╲┃00┃╱╭┳┳╮╱",
                  "╲┃oo┃┗┫╰╯┣┛┃oo┃",
                  "╲┃╭╮┃╲╰┳┳╯╱┃━━┃╱",
                  "╲╰┳┳╯┏━┻┻━┓╰┳┳╯╱",
                  "┏━┻┻━┫┊┊┊┊┣━┻┻━┓"
                ),
                GameState.OutOfOxygen => "No oxygen left..",
                GameState.DiedOnSpike => "You died..",
                GameState.Suicide => "You kicked the bucket..\n\n" + String.Join("\n",
                    "╲╲╭━┻┻┻┻━╮╱╱┏┓╱╱",
                    "╲╭┫╭━╮╭━┓┣╮╱┣┫╱╱",
                    "┈┃┃┃▆┃┃▋┃┃┃╭╯╰╮┈",
                    "┈╰┫┗━╭╮━╯┣╯┃╭╮┃┈",
                    "┈┈┃╰┳┳━━╯┃┈┃╳╳┃┈",
                    "╱╱┃╲╰╯╱╱╱┃╲┃╰╯┃╲",
                    "╱╱╰━┳━━┳━╯╲┗━━┛╲"
                ),
                _ => throw new NotImplementedException(),
            };
            user.GamesCounter++;
            Console.WriteLine(endgameText);
        }
    }
}
