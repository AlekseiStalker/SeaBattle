using System; 

namespace SeaBattle
{ 
    interface IDraw  
    {
        void PrintUserBoard();
        void PrintComputerBoard();
        void PrintUserShootingBoard();
    }

    interface IGameActions<T> 
    {
        void UserInput(T keyInfo);
        bool WinnerExist();
        void Restart();
        void ResetComputerPlayer();
    }
    public class ViewManager : IDraw, IGameActions<ConsoleKeyInfo> //make public for UnitTests
    { 
        private static int _paddingLeft = BoardFactory.GetCells();
        private static int _paddingTop = BoardFactory.GetCells() / 2;

        private static string _curLeftUP    = "╔",
                              _curRigthUP   = "╗",
                              _curLeftDown  = "╚",
                              _curRigthDown = "╝";

        static GameController _game = new GameController();   

        public void Restart()
        {
            _paddingLeft = BoardFactory.GetCells();
            _paddingTop = BoardFactory.GetCells() / 2;

            _game.InitNewGame();
        }

        public void ResetComputerPlayer()
        {
            _game.ResetComputerPlayer();
        }

        public void PrintUserBoard()
        {
            PrintBorder(_game.UserShipBoard, BoardFactory.GetCells() * 2);
        }

        public void PrintComputerBoard()
        {
            PrintBorder(_game.ComputerShipBoard, BoardFactory.GetCells() * 4);
        } 

        public void PrintUserShootingBoard()
        {
            PrintBorder(_game.UserShootingBoard);
            SetPosCustumCursor();
        }

        /// <summary>
        /// Print board on console
        /// </summary>
        /// <param name="board">board which need print</param>
        /// <param name="padding">padding from left edge console</param>
        private void PrintBorder(Board board, int padding = 0)
        {
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < BoardFactory.GetCells(); i++)
            {
                Console.SetCursorPosition(padding, i);

                for (int j = 0; j < BoardFactory.GetCells(); j++)
                {
                    string cell_Symbol = GetSymbolByCellType(board[i, j].Type);
                    Console.Write(cell_Symbol);

                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
            }
        }
         
        public void UserInput(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.LeftArrow:
                    MoveCursor(Movement.Left);
                    break;
                case ConsoleKey.RightArrow:
                    MoveCursor(Movement.Rigth);
                    break;
                case ConsoleKey.UpArrow:
                    MoveCursor(Movement.Up);
                    break;
                case ConsoleKey.DownArrow:
                    MoveCursor(Movement.Down);
                    break;
                case ConsoleKey.Spacebar:
                    _game.UserShot(_paddingTop, _paddingLeft / 2);
                    break;
                default:
                    break;
            }
        }

        public bool WinnerExist()
        {
            string nameWinner = _game.CheckOnWinner();
            if (nameWinner != "")
            {
                Console.SetCursorPosition(Settings.boardSize, Settings.boardSize + 2);
                Console.Write(nameWinner + " WON THE GAME! ");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Print on console how many ships left to kill computer and player
        /// </summary>
        /// <param name="who"></param>
        public static void ViewCountShipsLeft(string who)
        {
            int countShipAlive = 0;
            int fourDeck = 0, threeDeck = 0, twoDeck = 0, oneDeck = 0;

            if (_game.UserShipBoard != null && _game.ComputerShipBoard != null)
            { 
                switch (who)
                {
                    case "User":
                        _game.SetCountShipsLeft(_game.ComputerShipBoard, out countShipAlive, out fourDeck, out threeDeck, out twoDeck, out oneDeck);
                        break;
                    case "Computer":
                        _game.SetCountShipsLeft(_game.UserShipBoard, out countShipAlive, out fourDeck, out threeDeck, out twoDeck, out oneDeck);
                        break;
                }
            }

            Console.WriteLine();
            Console.WriteLine(" " + who + " must kill " + countShipAlive + " ships." +
                              "\n FourDeck: " + fourDeck +
                              "\n ThreeDeck: " + threeDeck +
                              "\n TwoDeck: " + twoDeck +
                              "\n oneDeck: " + oneDeck); 
            Console.WriteLine();
        }
         
        private string GetSymbolByCellType(TypeCell type)
        {
            string res = "";
            switch (type)
            {
                case TypeCell.BorderLeftUp:
                    Console.ForegroundColor = ConsoleColor.White;
                    res = "╔";
                    break;
                case TypeCell.BorderRigthUp:
                    Console.ForegroundColor = ConsoleColor.White;
                    res = "╗";
                    break;
                case TypeCell.BorderMidVert:
                    Console.ForegroundColor = ConsoleColor.White;
                    res = "║";
                    break;
                case TypeCell.BorderMidHor:
                    Console.ForegroundColor = ConsoleColor.White;
                    res = "══";
                    break;
                case TypeCell.BorderLeftDown:
                    Console.ForegroundColor = ConsoleColor.White;
                    res = "╚";
                    break;
                case TypeCell.BorderRigthDown:
                    Console.ForegroundColor = ConsoleColor.White;
                    res = "╝";
                    break; 
                case TypeCell.MuffShot:
                    Console.ForegroundColor = ConsoleColor.White;
                    res = "░░";
                    break;
                case TypeCell.HiddenShip:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    res = "██";
                    break;
                case TypeCell.DamagetShip:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    res = "▓▓";
                    break;
                case TypeCell.KilledShip:
                    Console.ForegroundColor = ConsoleColor.Red;
                    res = "██";
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    res = " ~";
                    break;
            }
            return res;
        }
          
        private void MoveCursor(Movement move)
        {
            ClearCurrentCursor();

            switch (move)
            {
                case Movement.Left:
                    if (!(_paddingLeft - 2 < 1)) 
                        _paddingLeft -= 2; 
                    break;
                case Movement.Rigth:
                    if (!(_paddingLeft + 2 >= BoardFactory.GetCells() * 2 - 2)) 
                        _paddingLeft += 2; 
                    break;
                case Movement.Up:
                    if (!(_paddingTop - 1 < 1)) 
                        _paddingTop -= 1; 
                    break;
                case Movement.Down:
                    if (!(_paddingTop + 1 >= BoardFactory.GetCells() - 1)) 
                        _paddingTop += 1; 
                    break; 
            }

            SetPosCustumCursor();
        }

        private void ClearCurrentCursor()
        { 
            string symb = GetSymbolByCellType(_game.UserShootingBoard[_paddingTop - 1, _paddingLeft / 2 - 1].Type);
            //clearLeftUpCursor
            WriteAt(symb, _paddingLeft >= 3 ? _paddingLeft - 3 : _paddingLeft - 2, _paddingTop - 1);
             
            symb = GetSymbolByCellType(_game.UserShootingBoard[_paddingTop - 1, _paddingLeft / 2 + 1].Type);
            //clearRigthUpCursor
            WriteAt(symb, _paddingLeft + 1, _paddingTop - 1);

            symb = GetSymbolByCellType(_game.UserShootingBoard[_paddingTop + 1, _paddingLeft / 2 - 1].Type);
            //clearLeftDownCursor
            WriteAt(symb, _paddingLeft >= 3 ? _paddingLeft - 3 : _paddingLeft - 2, _paddingTop + 1);

            symb = GetSymbolByCellType(_game.UserShootingBoard[_paddingTop + 1, _paddingLeft / 2 + 1].Type);
            //clearRigthDownCursor
            WriteAt(symb, _paddingLeft + 1, _paddingTop + 1);

            symb = GetSymbolByCellType(_game.UserShootingBoard[_paddingTop, _paddingLeft / 2].Type);
            //clearCenterCursor
            WriteAt(symb, _paddingLeft -1, _paddingTop);

            Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(_paddingLeft, _paddingTop);
        }

        private void SetPosCustumCursor()
        {
            WriteAt(_curLeftUP,    _paddingLeft - 2, _paddingTop - 1);
            WriteAt(_curRigthUP,   _paddingLeft + 1, _paddingTop - 1);
             
            WriteAt(_curLeftDown,  _paddingLeft - 2, _paddingTop + 1);
            WriteAt(_curRigthDown, _paddingLeft + 1, _paddingTop + 1);

            Console.SetCursorPosition(_paddingLeft, _paddingTop);
        }

        private void WriteAt(string symb, int row, int col)
        {
            Console.SetCursorPosition(row, col);
            Console.Write(symb);
        } 

        public static bool Serialize(TypeSerialization type)
        {
            if (_game.UserShipBoard != null && _game.UserShootingBoard != null)
            {
                switch (type)
                {
                    case TypeSerialization.Xml:
                        _game.XmlSerialize();
                        break;
                    case TypeSerialization.JSON:
                        _game.JsonSerialize();
                        break;
                    case TypeSerialization.Binary:
                        _game.BinarySerialize();
                        break;
                }
                return true;
            }
            return false;
        }

        public static void Deserialize(TypeSerialization type)
        {
            try
            {
                switch (type)
                {
                    case TypeSerialization.Xml:
                        _game.XmlDeserialize();
                        break;
                    case TypeSerialization.JSON:
                        _game.JsonDeserialize();
                        break;
                    case TypeSerialization.Binary:
                        _game.BinaryDeserialize();
                        break;
                }
            }
            catch {
                Console.Clear();
                throw new SeaBattleException("No data to start a saved application."); 
            }
        }
    }
} 




