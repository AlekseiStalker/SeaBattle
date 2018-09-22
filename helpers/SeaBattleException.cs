using System;

namespace SeaBattle
{
    class SeaBattleException : Exception
    {
        public SeaBattleException(string message)
       : base(message)
        { }
    }
}
