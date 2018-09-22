using System;
using System.Collections.Generic; 
using System.Runtime.Serialization; 

namespace SeaBattle
{
    [DataContract]
    [KnownType(typeof(List<Ship>))]
    [KnownType(typeof(Cell[][]))]
    [Serializable]
    public class Board
    {  
        private static Random _rand = new Random(DateTime.Now.Millisecond);
          
        public Board(int size, int countShips)
        {
            Size = size;
            _cells = new Cell[Size, Size];
            _jaggetArray = new Cell[Size][];
            Ships = new List<Ship>(countShips);
             
            InitBoard();
        }
         
        [DataMember]
        public List<Ship> Ships { get; set; } //add public setter for Xml and JSON Serilizer  

        public Cell[,] _cells; // set public for BinarySerializer
         
        private Cell[][] _jaggetArray;// created for Xml and JSON Serilizer  
        [DataMember]
        public Cell[][] Cells
        {
            get
            {
                for (int row = 0; row < BoardFactory.GetCells(); row++)
                {
                    _jaggetArray[row] = new Cell[BoardFactory.GetCells()];
                    for (int col = 0; col < BoardFactory.GetCells(); col++)
                    {
                        _jaggetArray[row][col] = _cells[row, col];
                    }
                }
                return _jaggetArray;
            }
            set
            {
                _jaggetArray = value;

                for (int row = 0; row < BoardFactory.GetCells(); row++)
                {
                    for (int col = 0; col < BoardFactory.GetCells(); col++)
                    {
                        _cells[row, col] = _jaggetArray[row][col];
                    }
                }
            }
        }

        public int Size { get; set; }

        [OnDeserializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            _cells = new Cell[BoardFactory.GetCells(), BoardFactory.GetCells()];
            Size = BoardFactory.GetCells();
        }

        /// <summary>
        /// Init board by default (set type cells like border or empty)
        /// </summary>
        private void InitBoard()
        {
            //init borders
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (i == 0 && j == 0)
                        _cells[i, j] = new Cell() { Type = TypeCell.BorderLeftUp, IsOccupied = true };
                    else if (j == 0 && i == Settings.boardSize + 1)
                        _cells[i, j] = new Cell() { Type = TypeCell.BorderLeftDown, IsOccupied = true };
                    else if (i == 0 && j == Settings.boardSize + 1)
                        _cells[i, j] = new Cell() { Type = TypeCell.BorderRigthUp, IsOccupied = true };
                    else if (i == Settings.boardSize + 1 && j == Settings.boardSize + 1)
                        _cells[i, j] = new Cell() { Type = TypeCell.BorderRigthDown, IsOccupied = true };
                    else if ((i == 0 || i == Settings.boardSize + 1) && (j != 0 && j != Settings.boardSize + 1))
                        _cells[i, j] = new Cell() { Type = TypeCell.BorderMidHor, IsOccupied = true };
                    else if (j == 0 || j == Settings.boardSize + 1)
                        _cells[i, j] = new Cell() { Type = TypeCell.BorderMidVert, IsOccupied = true }; 
                }
            }

            //init default cells
            for (int i = 1; i < Size - 1; i++)
            {
                for (int j = 1; j < Size - 1; j++)
                {
                    _cells[i, j] = new Cell() { Type = TypeCell.Empty };
                }
            }
        }

        /// <summary>
        /// Add ships to list Ships
        /// </summary>
        private void AddShips()
        {
            int i;
            for (i = 0; i < Settings.fourDeckersCount; i++)
                Ships.Add(new Ship(4));
            for (i = 0; i < Settings.threeDeckersCount; i++)
                Ships.Add(new Ship(3));
            for (i = 0; i < Settings.twoDeckersCount; i++)
                Ships.Add(new Ship(2));
            for (i = 0; i < Settings.oneDeckersCount; i++)
                Ships.Add(new Ship(1));
        }

        /// <summary>
        /// Sets ships in random order on the board
        /// </summary>
        public void ArrangeShips()
        {
            AddShips();
            Array mOrientations = Enum.GetValues(typeof(Orientation));

            foreach (Ship s in Ships)
            { 
                Orientation orient;
                int r, c;
                do
                {
                    orient = (Orientation)_rand.Next(mOrientations.Length);
                    r = _rand.Next(1, Size - 1);
                    c = _rand.Next(1, Size - 1);
                } while (!CanArrangeShip(s, r, c, orient));

                for (int partShip = 0; partShip < s.Length; partShip++)
                {
                    switch (orient)
                    {
                        case Orientation.HORIZONTAL:
                            _cells[r, c + partShip].AddShip(s, partShip, r, c + partShip);
                            break;
                        case Orientation.VERTICAL:
                            _cells[r + partShip, c].AddShip(s, partShip, r + partShip, c);
                            break; 
                    }
                    SetOccupied(r, c, s, orient);
                }
            } 
        }
         
        private void SetOccupied(int r, int c, Ship s, Orientation o)
        {
            switch (o)
            {
                case Orientation.HORIZONTAL:
                    for (int i = r - 1; i <= r + 1; i++)
                    {
                        if (i < 1 || i >= Size - 1)
                            continue;
                        for (int j = c - 1; j <= c + s.Length; j++)
                        {
                            if (j < 1 || j >= Size - 1)
                                continue;
                            _cells[i, j].IsOccupied = true;
                        }
                    }
                    break;

                case Orientation.VERTICAL:
                    for (int i = r - 1; i <= r + s.Length; i++)
                    {
                        if (i < 1 || i >= Size - 1)
                            continue;
                        for (int j = c - 1; j <= c + 1; j++)
                        {
                            if (j < 1 || j >= Size + 1)
                                continue;
                            _cells[i, j].IsOccupied = true;
                        }
                    }
                    break; 
            }
        }

        private bool CanArrangeShip(Ship s, int r, int c, Orientation o)
        {
            switch (o)
            {
                case Orientation.HORIZONTAL:
                    if (c + s.Length > Size)
                        return false;

                    for (int i = 0; i < s.Length; i++)
                    {
                        if (_cells[r, c + i].IsOccupied)
                            return false;
                    } 
                    break;

                case Orientation.VERTICAL:
                    if (r + s.Length > Size)
                        return false;

                    for (int i = 0; i < s.Length; i++)
                    {
                        if (_cells[r + i, c].IsOccupied)
                            return false;
                    }
                    break; 
            }
            return true;
        }

        /// <summary>
        /// Shoots at specify cell on the board
        /// </summary>
        /// <param name="row">Row number for cell</param>
        /// <param name="col">Col number for cell</param>
        /// <returns>Is exist a hidden ship in given cell</returns>
        public bool Shot(int row, int col)
        {
            if (_cells[row, col].Ship != null)
            {
                Ship ship = _cells[row, col].Ship;
                if (_cells[row, col].Type == TypeCell.HiddenShip)
                { 
                    _cells[row, col].Type = TypeCell.DamagetShip; 
                    if (CheckOnKilled(ship))
                    {
                        ship.IsKilled = true;

                        SetKilledShipAndCellsAround(ship);
                    } 
                    return true;
                } 
            }
            else if (_cells[row, col].Type == TypeCell.Empty)
            {
                _cells[row, col].Type = TypeCell.MuffShot;
            } 
            return false;
        } 

        private bool CheckOnKilled(Ship ship)
        {
            int count = 0;
            for (int i = 0; i < ship.Length; i++)
                if (_cells[ship[i].Row, ship[i].Col].Type == TypeCell.DamagetShip)
                    count++;

            return count == ship.Length;
        }

        /// <summary>
        /// Changing type of cells around the ship that was killed and cells in which the ship located
        /// </summary>
        /// <param name="ship">Killed ship</param>
        private void SetKilledShipAndCellsAround(Ship ship)
        {
            int minRow = ship[0].Row > 1 ? ship[0].Row - 1 : ship[0].Row, 
                minCol = ship[0].Col > 1 ? ship[0].Col - 1 : ship[0].Col;

            int maxRow = ship[ship.Length - 1].Row < Settings.boardSize ? ship[ship.Length - 1].Row + 1 : ship[ship.Length - 1].Row,
                maxCol = ship[ship.Length - 1].Col < Settings.boardSize ? ship[ship.Length - 1].Col + 1 : ship[ship.Length - 1].Col;

            for (int r = minRow; r <= maxRow; r++) 
                for (int c = minCol; c <= maxCol; c++)
                {
                    if (_cells[r, c].Type != TypeCell.MuffShot)
                    { 
                        _cells[r, c].Type = TypeCell.AreaAroundShip;
                    }
                }
             
            for (int i = 0; i < ship.Length; i++)
                _cells[ship[i].Row, ship[i].Col].Type = TypeCell.KilledShip;

            Ships.Find(s => s[0].Equals(ship[0])).IsKilled = true; 
        }
         
        public Cell this[int row, int col]
        {
            get
            {
                return _cells[row, col];
            }
            set
            {
                _cells[row, col] = value;
            }
        }

        #region Override Equals&GetHashCode for UnitTesting serialize/deserialize
          
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;

            Board board = (Board)obj;

            if (this.Ships.Count != board.Ships.Count)
            {
                return false;
            }

            for (int i = 0; i < Ships.Count; i++)
            {
                if (!this.Ships[i].Equals(board.Ships[i]))
                {
                    return false;
                }
            }

            for (int x = 0; x < _cells.GetLength(0); x++)
            {
                for (int y = 0; y < _cells.GetLength(1); y++)
                {
                    if (!this._cells[x,y].Equals(board._cells[x,y]))
                    {
                        return false;
                    }
                }
            } 
              
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            for (int i = 0; i < Ships.Count; i++)
            {
                hash ^= Ships[i].GetHashCode();
            }

            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    hash ^= _cells[i, j].GetHashCode();
                }
            } 
            return hash;
        }

        #endregion
    }
}

/*
  Console.WriteLine("╔═╗");
  Console.WriteLine("║ ║");
  Console.WriteLine("╚═╝"); 
  */
