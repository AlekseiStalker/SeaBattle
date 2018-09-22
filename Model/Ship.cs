using System;
using System.Runtime.Serialization; 

namespace SeaBattle
{
    [DataContract]
    [KnownType(typeof(Point[]))]
    [Serializable]
    public class Ship //changed to public for XmlSerializ
    {    
        public Ship(int size)
        { 
            _partShip = new Point[size];
            for (int i = 0; i < _partShip.Length; i++)
            {
                _partShip[i] = new Point();
            }
        }

        [DataMember]
        public Point[] _partShip; //changed to public for XmlSerializ
        [DataMember]
        public bool IsKilled { get; set; }

        public int Length => _partShip.Length;


        /// <summary>
        /// Getter&Setter by index deck
        /// </summary>
        /// <param name="numDeck">index deck</param>
        /// <returns>Deck of ship</returns>
        public Point this[int numDeck]
        {
            get
            { 
                return _partShip[numDeck];
            }
            set
            {
                _partShip[numDeck] = value;
            }
        }

        /// <summary>
        /// Getter&Setter by coordinates (row&col)
        /// </summary>
        /// <param name="r">row on board in which deck located</param>
        /// <param name="c">col on board in which deck located</param>
        /// <returns>Deck of ship</returns>
        public Point this[int r, int c]
        {
            get
            {
                for (int indx = 0; indx < _partShip.Length; indx++)
                {
                    if (_partShip[indx].Row == r && _partShip[indx].Col == c)
                    {
                        return _partShip[indx];
                    } 
                } 
                throw new SeaBattleException("Ship on them index not found.");
            }
            set
            {
                for (int indx = 0; indx < _partShip.Length; indx++)
                {
                    if (_partShip[indx].Row == r && _partShip[indx].Col == c)
                    {
                        _partShip[indx] = value; 
                    }
                } 
            }
        }

        #region Override Equals&GetHashCode for UnitTesting serialize/deserialize
         
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;

            Ship ship = (Ship)obj;

            if (this.IsKilled != ship.IsKilled ||
                this._partShip.Length != ship._partShip.Length)
            {
                return false;
            }

            for (int i = 0; i < _partShip.Length; i++)
            {
                if (!this._partShip[i].Equals(ship._partShip[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            for (int i = 0; i < _partShip.Length; i++)
            {
                hash ^= _partShip[i].GetHashCode();
            }
            return hash ^ IsKilled.GetHashCode();
        }

        #endregion
    }
}
