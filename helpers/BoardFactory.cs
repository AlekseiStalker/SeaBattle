 
namespace SeaBattle
{
    public class BoardFactory
    { 
        private static Board _newBoard;

        public static int GetCells() => Settings.boardSize + Settings.bordersSize;
        public static int GetShips() => Settings.oneDeckersCount + Settings.twoDeckersCount + Settings.threeDeckersCount + Settings.fourDeckersCount;

        public static Board MakeBoard()
        {
            _newBoard = new Board(GetCells(), GetShips());
            return _newBoard;
        }
    }
} 
