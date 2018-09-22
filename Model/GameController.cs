using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Xml; 

namespace SeaBattle
{  
    [DataContract] 
    public class GameController  //make public for UnitTests
    {
        readonly string storePath = @"F:\Work_data\DBBest\DBBest_MyProjects\Sources\ConsoleApps\SeaBattle\bin\Debug";
         
        public GameController()
        { 
            _serializedObjects = new List<Board>();
        }
         
        public GameController(string newPath) //created for UnitTests
        {
            storePath = newPath;
        }

        public Board UserShootingBoard { get; private set; }

        public Board UserShipBoard { get; private set; }

        public Board ComputerShipBoard { get; private set; }

        public ComputerPlayer CompPlayer { get; private set; } //make public for UnitTests

        [DataMember]
        public List<Board> _serializedObjects { get; set; }// for serialization
        //[DataMember]
        //public List<int> _compLuckyShotPos { get; set; }

        /// <summary>
        /// Create new game boards and arrange ships
        /// </summary>
        public void InitNewGame()
        {
            UserShootingBoard = BoardFactory.MakeBoard();

            UserShipBoard = BoardFactory.MakeBoard();
            UserShipBoard.ArrangeShips();

            ComputerShipBoard = BoardFactory.MakeBoard();
            ComputerShipBoard.ArrangeShips();
             
            CompPlayer = new ComputerPlayer();
        }
         
        public void ResetComputerPlayer()
        {
            CompPlayer = new ComputerPlayer(UserShipBoard);
        }

        /// <summary>
        /// Call when user shot, if shot muff - call ComputerShot.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        public void UserShot(int r, int c)
        {
            bool luckyShot = ComputerShipBoard.Shot(r, c);
            RefreshUserShotingBoardState();

            if (!luckyShot)
            {
                ComputerShot();
            }
        } 

        private void ComputerShot()
        {
            CompPlayer.Shot(UserShipBoard);
        }

        private void RefreshUserShotingBoardState()
        {
            for (int x = 1; x < UserShootingBoard.Size - 1; x++)
            {
                for (int y = 1; y < UserShootingBoard.Size - 1; y++)
                {
                    if (ComputerShipBoard[x,y].Type == TypeCell.DamagetShip || 
                        ComputerShipBoard[x,y].Type == TypeCell.KilledShip ||
                        ComputerShipBoard[x,y].Type == TypeCell.MuffShot ||
                        ComputerShipBoard[x, y].Type == TypeCell.AreaAroundShip)
                    {
                        UserShootingBoard[x, y].Type = ComputerShipBoard[x, y].Type;
                    }
                }
            }
        }

        public void SetCountShipsLeft(Board board, out int shipsAlive, out int fourDeck, out int threeDeck, out int twoDeck, out int oneDeck)
        { 
            shipsAlive = board.Ships.Where(c => !c.IsKilled).Count();
            fourDeck = board.Ships.Where(c => c.Length == 4 && !c.IsKilled).Count();
            threeDeck = board.Ships.Where(c => c.Length == 3 && !c.IsKilled).Count();
            twoDeck = board.Ships.Where(c => c.Length == 2 && !c.IsKilled).Count();
            oneDeck = board.Ships.Where(c => c.Length == 1 && !c.IsKilled).Count();
        }

        /// <summary>
        /// Checks on winner the game
        /// </summary>
        /// <returns>Name of winner</returns>
        public string CheckOnWinner()
        {
            string winner = "";

            if (CheckOnWin(UserShipBoard))
            {
                winner = " Computer";
            }

            if (CheckOnWin(ComputerShipBoard)) winner = " User";

            return winner;
        }
         
        private bool CheckOnWin(Board board)
        { 
            int count = 0;
            foreach (Ship s in board.Ships)
                if (!s.IsKilled)
                    count++;

            return count == 0;
        }

        #region Binary Serialize
        public void BinarySerialize()
        {
            AddSerializedObjectsToList();

            using (FileStream fs = new FileStream(storePath + "store.dat", FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, _serializedObjects);
            }
        }

        public void BinaryDeserialize()
        {
            using (FileStream fs = new FileStream(storePath +  "store.dat", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                _serializedObjects = (List<Board>)formatter.Deserialize(fs);
            }

            GetSerializedObjectsFromList();
        }
        #endregion

        #region JSON Serialize
        public void JsonSerialize()
        {
            AddSerializedObjectsToList();

            using (FileStream writer = new FileStream(storePath + "store.json", FileMode.Create))
            {
                try
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<Board>));
                    ser.WriteObject(writer, _serializedObjects);
                }
                catch (Exception ex)
                {
                    string ms = ex.Message;
                    throw;
                }
            }
        }

        public void JsonDeserialize()
        {
            using (FileStream fs = new FileStream(storePath + "store.json", FileMode.Open))
            {
                using (XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(fs, new XmlDictionaryReaderQuotas()))
                {
                    try
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<Board>));
                        var list = ser.ReadObject(reader, true);
                        _serializedObjects = (List<Board>)list;
                    }
                    catch (Exception ex)
                    {
                        string ms = ex.Message;
                        throw;
                    }
                }  
            }

            GetSerializedObjectsFromList();
        }
        #endregion

        #region XML Serialize
        public void XmlSerialize()
        {
            AddSerializedObjectsToList();

            using (FileStream writer = new FileStream(storePath + "store.xml", FileMode.Create))
            {
                try
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(List<Board>));
                    ser.WriteObject(writer, _serializedObjects); 
                }
                catch (Exception ex)
                {
                    string ms = ex.Message;
                    throw;
                } 
            }
        }

        public void XmlDeserialize()
        {
            using (FileStream fs = new FileStream(storePath + "store.xml", FileMode.Open))
            {
                using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
                {
                    try
                    {
                        DataContractSerializer ser = new DataContractSerializer(typeof(List<Board>));
                        var list = ser.ReadObject(reader, true);
                        _serializedObjects = (List<Board>)list;
                    }
                    catch (Exception ex)
                    {
                        string ms = ex.Message;
                        throw;
                    } 
                } 
            }

            GetSerializedObjectsFromList();
        }
        #endregion

        private void AddSerializedObjectsToList()
        {
            _serializedObjects.Add(UserShootingBoard);
            _serializedObjects.Add(UserShipBoard);
            _serializedObjects.Add(ComputerShipBoard);
        }
        
        private void GetSerializedObjectsFromList()
        {
            UserShootingBoard = _serializedObjects[0];
            UserShipBoard = _serializedObjects[1];
            ComputerShipBoard = _serializedObjects[2];
        } 
    }
} 