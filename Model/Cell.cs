using System;
using System.Runtime.Serialization;

namespace SeaBattle
{
    [DataContract]
    [Serializable]
    public class Cell //change to public for XmlSerializ
    { 
        public Cell()
        {
            Type = TypeCell.Empty;
            Ship = null;
        }

        [DataMember]
        public TypeCell Type { get; set; }
        [DataMember]
        public Ship Ship { get; set; }
         
        public bool IsOccupied { get; set; }
         
        /// <summary>
        /// Adds part of ship into the cell 
        /// </summary>
        /// <param name="s">ship</param>
        /// <param name="indxDeckShip">index of some part of ship</param>
        /// <param name="r">row on board in which deck located</param>
        /// <param name="c">col on board in which deck located</param>
        public void AddShip(Ship s, int indxDeckShip, int r, int c)
        {
            Ship = s;

            Ship[indxDeckShip] = new Point(r,c); 

            Type = TypeCell.HiddenShip;
        }

        #region Override Equals&GetHashCode for UnitTesting serialize/deserialize
         
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;

            Cell cell = (Cell)obj;

            if (this.Type != cell.Type)
            {
                return false;
            }

            if (this.Ship == null && cell.Ship != null)
            {
                return false;
            }

            if (this.Ship == null && cell.Ship == null)
            {
                return true;
            }

            return this.Ship.Equals(cell.Ship);
        }

        public override int GetHashCode()
        {
            return Ship.GetHashCode() ^ Type.GetHashCode();
        }

        #endregion
    }
}
