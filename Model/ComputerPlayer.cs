using System; 
using System.Collections.Generic;
using System.Linq;

namespace SeaBattle
{
    public class ComputerPlayer //make public for UnitTests
    {
        public delegate void ShotState(Board b);
        public delegate bool ShotDirection(Board b);

        private List<ShotDirection> _shotOnDirection;

        private int _luckyShotRow, _luckyShotCol;
        private int _curShotRow, _curShotCol;
        private int _indxDirection;

        private Random _rand = new Random(DateTime.Now.Millisecond);

        public ComputerPlayer()
        {
            _shotOnDirection = new List<ShotDirection>(4);
            _indxDirection = 0;
            Shot = ShotFinding;
        }
         
        public ShotState Shot;
         
        public ComputerPlayer(Board userBoard) : this()
        { 
            CheckOnExistDamagetShip(userBoard);
        }

        private void CheckOnExistDamagetShip(Board board)
        {
            for (int row = 1; row < Settings.boardSize + 1; row++)
                for (int col = 1; col < Settings.boardSize + 1; col++)
                    if (board[row, col].Type == TypeCell.DamagetShip)
                    {
                        _luckyShotRow = row;
                        _luckyShotCol = col;
                        Shot = ShotFinishing;
                        return;
                    } 
        }

        /// <summary>
        /// Random shooting on ships
        /// </summary>
        /// <param name="userBoard">board in which ships located</param>
        private void ShotFinding(Board userBoard)
        {
            do
            {
                _luckyShotRow = _rand.Next(1, Settings.boardSize + 1);
                _luckyShotCol = _rand.Next(1, Settings.boardSize + 1);

            } while (userBoard[_luckyShotRow, _luckyShotCol].Type == TypeCell.AreaAroundShip ||
                     userBoard[_luckyShotRow, _luckyShotCol].Type == TypeCell.KilledShip ||
                     userBoard[_luckyShotRow, _luckyShotCol].Type == TypeCell.MuffShot);
 
            if (userBoard.Shot(_luckyShotRow, _luckyShotCol))
            {
                Shot = ShotFinishing;
                Shot(userBoard);
            }
        }

        /// <summary>
        /// Finishing killing some ship after the first lucky shot
        /// </summary>
        /// <param name="userBoard">board in which ship located</param>
        private void ShotFinishing(Board userBoard)
        {
            if (_shotOnDirection.Count == 0) 
                AddPossibleDirection(userBoard);

            _curShotRow = _luckyShotRow;
            _curShotCol = _luckyShotCol;

            bool shotSuccess = true;
            while (shotSuccess && userBoard[_luckyShotRow, _luckyShotCol].Type != TypeCell.KilledShip) 
            {
                shotSuccess = _shotOnDirection[_indxDirection](userBoard);

                if (!shotSuccess) _indxDirection++; 
            } 
             
            if (userBoard[_luckyShotRow, _luckyShotCol].Type == TypeCell.KilledShip) 
            {
                Shot = ShotFinding;
                _indxDirection = 0;
                _shotOnDirection.Clear();
            } 
        }

        /// <summary>
        /// Adds directions for shooting after first lucky shot
        /// </summary>
        private void AddPossibleDirection(Board board)
        { 
            if (_luckyShotRow > 1 && board[_luckyShotRow - 1, _luckyShotCol].Type != TypeCell.MuffShot
                                  && board[_luckyShotRow - 1, _luckyShotCol].Type != TypeCell.AreaAroundShip)
                _shotOnDirection.Add(UpDirection);

            if (_luckyShotRow < Settings.boardSize + 1 && board[_luckyShotRow + 1, _luckyShotCol].Type != TypeCell.MuffShot
                                                       && board[_luckyShotRow + 1, _luckyShotCol].Type != TypeCell.AreaAroundShip)
                _shotOnDirection.Add(DownDirection);

            if (_luckyShotCol > 1 && board[_luckyShotRow, _luckyShotCol - 1].Type != TypeCell.MuffShot
                                  && board[_luckyShotRow, _luckyShotCol - 1].Type != TypeCell.AreaAroundShip)
                _shotOnDirection.Add(LeftDirection);

            if (_luckyShotCol < Settings.boardSize + 1 && board[_luckyShotRow, _luckyShotCol + 1].Type != TypeCell.MuffShot
                                                       && board[_luckyShotRow, _luckyShotCol + 1].Type != TypeCell.AreaAroundShip)
                _shotOnDirection.Add(RigthDirection);

            RandomizeDirections();
        }

        private bool UpDirection(Board userBoard)
        {
            if (_curShotRow > 1 && userBoard[_curShotRow - 1, _luckyShotCol].Type != TypeCell.MuffShot)
            {
                _curShotRow -= 1;
                return userBoard.Shot(_curShotRow, _luckyShotCol);
            }
            return false;
        }

        private bool DownDirection(Board userBoard)
        {
            if (_curShotRow < Settings.boardSize + 1 && userBoard[_curShotRow + 1, _luckyShotCol].Type != TypeCell.MuffShot)
            { 
                _curShotRow += 1;
                return userBoard.Shot(_curShotRow, _luckyShotCol);
            }
            return false;
        }

        private bool LeftDirection(Board userBoard)
        {
            if (_curShotCol > 1 && userBoard[_luckyShotRow, _curShotCol - 1].Type != TypeCell.MuffShot)
            {
                _curShotCol -= 1;
                return userBoard.Shot(_luckyShotRow, _curShotCol);
            }
            return false;
        }

        private bool RigthDirection(Board userBoard)
        {
            if (_curShotCol < Settings.boardSize + 1 && userBoard[_luckyShotRow, _curShotCol + 1].Type != TypeCell.MuffShot)
            {
                _curShotCol += 1;
                return userBoard.Shot(_luckyShotRow, _curShotCol);
            }
            return false;
        }

        private void RandomizeDirections() => 
            _shotOnDirection = _shotOnDirection.OrderBy(x => _rand.Next()).ToList();  
    }
}
