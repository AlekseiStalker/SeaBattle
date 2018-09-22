
using System;
using System.Runtime.Serialization;

namespace SeaBattle
{
    [DataContract]
    [Serializable]
    public struct Point //change to public for XmlSerializ
    {
        [DataMember] 
        /// <summary>
        /// Property Row&Col userd for store coordinates deck of ship
        /// </summary> 
        public int Row { get; set; } // add public setters  for XmlSerealization
        [DataMember]
        public int Col { get; set; }  
         
        public Point(int r, int c)
        {
            Row = r;
            Col = c;
        }

        #region Override Equals&GetHashCode for UnitTesting serialize/deserialize
         
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;

            Point point = (Point)obj;
             
            return this.Col == point.Col && this.Row == point.Row;
        }

        public override int GetHashCode()
        {
            return Row ^ Col;
        }

        #endregion
    }
}
