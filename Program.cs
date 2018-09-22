using System; 

namespace SeaBattle
{ 
    class Program
    { 
        static void Main(string[] args)
        { 
            Console.Title = "Sea Battle";
            Console.CursorVisible = false;


            TypeSerialization typeSerializ = TypeSerialization.Xml;


            IDraw gameView = new ViewManager();
            IGameActions<ConsoleKeyInfo> gameState = new ViewManager();

            bool InGame = false;
            while (true)
            {
                ViewInputMenu();

                if (_numMenu == 1)
                {
                    gameState.Restart();
                    Console.Clear();
                    InGame = true;

                    ViewGameControlInfo(); 
                }

                if (_numMenu == 2)
                {
                    if (ViewManager.Serialize(typeSerializ))
                    {
                        Console.SetCursorPosition(13, 2);
                        Console.Write("(game saved)");
                        Console.ReadKey(true);
                    }
                }

                if (_numMenu == 3)
                { 
                    ViewManager.Deserialize(typeSerializ);
                    gameState.ResetComputerPlayer();
                    Console.Clear();
                    InGame = true;

                    ViewGameControlInfo(); 
                }

                while (InGame)
                {
                    gameView.PrintUserBoard();
                    gameView.PrintUserShootingBoard();

                    bool viewComputerBoard = true;

                    while (true)
                    {
                        ConsoleKeyInfo inputKey = Console.ReadKey(true);
                        gameState.UserInput(inputKey);

                        if (inputKey.Key == ConsoleKey.Q && viewComputerBoard)
                        {
                            gameView.PrintComputerBoard();
                            viewComputerBoard = false;
                        }
                        else if (inputKey.Key == ConsoleKey.Q && !viewComputerBoard)
                        {
                            Console.Clear();
                            viewComputerBoard = true;
                        }

                        if (inputKey.Key == ConsoleKey.Spacebar || inputKey.Key == ConsoleKey.Q)
                        {
                            gameView.PrintUserBoard();
                            gameView.PrintUserShootingBoard();
                        }

                        if (gameState.WinnerExist())
                        {
                            Console.ReadKey(true);
                            return;
                        }

                        if (inputKey.Key == ConsoleKey.Escape)
                        {
                            Console.Clear();
                            InGame = false;
                            break;
                        } 
                    }
                }

                if (_numMenu == 4)
                {
                    Console.Clear();
                    Console.WriteLine();
                    ViewGameResults();
                    Console.ReadKey(true);
                    Console.Clear();
                }

                if (_numMenu == 5)
                {
                    Console.Clear();
                    Console.WriteLine("\nThis game was developed by Alexey Ovsanicoff thanks to DBBest." +
                                      "\n\nArchitecture feature  of application is the fact that due to realization" +
                                      "\nof Xml serialization the encapsulation was completely killed." +
                                      "\n\n (Press \'Esc\' for back to main menu)");
                    Console.ReadKey(true);
                    Console.Clear();
                }
            }
        }

        static int _numMenu = 1;

        private static int ViewInputMenu()
        { 
            Console.SetCursorPosition(0, 1); 
            Console.WriteLine("   New game");
            Console.WriteLine("   Save game");
            Console.WriteLine("   Continue game");
            Console.WriteLine("   Game results");
            Console.WriteLine("   Info");

            _numMenu = 1;
            Console.SetCursorPosition(0, _numMenu);
            Console.Write(">>");

            while (true)
            { 
                ConsoleKeyInfo input = Console.ReadKey(true);
                if (input.Key == ConsoleKey.DownArrow)
                    MoveCursor(false);
                else if (input.Key == ConsoleKey.UpArrow)
                    MoveCursor(true);
                else if (input.Key == ConsoleKey.Enter)
                    break;
            }

            return _numMenu;
        }

        private static void MoveCursor(bool up)
        {
            Console.SetCursorPosition(0, _numMenu);
            Console.Write("  ");

            if(up && _numMenu == 1) 
                _numMenu = 5; 
            else if (!up && _numMenu == 5) 
                _numMenu = 1; 
            else 
                _numMenu = up ? _numMenu - 1 : _numMenu + 1;
             
            Console.SetCursorPosition(0, _numMenu);
            Console.Write(">>");
        }

        private static void ViewGameResults()
        { 
            ViewManager.ViewCountShipsLeft("User");
            ViewManager.ViewCountShipsLeft("Computer");
            Console.WriteLine("\n (Press \'Esc\' for back to main menu)"); 
        }

        private static void ViewGameControlInfo()
        {
            Console.SetCursorPosition(2, 15);
            Console.WriteLine("\t\t-= GAME CONTROL =-");
            Console.SetCursorPosition(2,17);
            Console.WriteLine("- Press (left/rigth/up/down) arrow keys for move the cursor.");
            Console.SetCursorPosition(2, 19); 
            Console.WriteLine("- Press  'Space'  for shooting.");
            Console.SetCursorPosition(2, 21);
            Console.WriteLine("- Press 'q'  for check computer board.");
            Console.SetCursorPosition(2, 23);
            Console.WriteLine("- Press  'Esc'  for back to main menu.");
        }
    }
}
