using System.Runtime.Serialization;

namespace SeaBattle
{
    [DataContract]
    public enum TypeCell //change to public for XmlSerializ
    {  
        //borders
        [EnumMember]
        BorderLeftUp,
        [EnumMember]
        BorderRigthUp,
        [EnumMember]
        BorderMidVert,
        [EnumMember]
        BorderMidHor,
        [EnumMember]
        BorderLeftDown,
        [EnumMember]
        BorderRigthDown,

        //cell state
        [EnumMember]
        Empty,
        [EnumMember]
        MuffShot,
        [EnumMember]
        AreaAroundShip,
        [EnumMember]
        HiddenShip,
        [EnumMember]
        DamagetShip,
        [EnumMember]
        KilledShip
    }
}
