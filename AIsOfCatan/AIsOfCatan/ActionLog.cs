using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    interface ILogEntry { int Turn { get; }}
    class BasicLogEntry : ILogEntry
    {
        public BasicLogEntry(int turn)
        {
            Turn = turn;
        }
        public int Turn { get; private set; }   
    }

    interface IDiceRollLogEntry : ILogEntry { int Roll { get; } }
    class DiceRollLogEntry : BasicLogEntry, IDiceRollLogEntry
    {
        public DiceRollLogEntry(int turn, int roll) : base(turn)
        {
            Roll = roll;
        }
        public int Roll { get; private set; }
        public override string ToString()
        {
            return "Player " + Turn + " rolled " + Roll;
        }
    }

    interface IPlayKnightLogEntry : ILogEntry { int RobberPosition { get; } }
    class PlayKnightLogEntry : BasicLogEntry, IPlayKnightLogEntry
    {
        public PlayKnightLogEntry(int turn, int position)
            : base(turn)
        {
            RobberPosition = position;
        }
        public int RobberPosition { get; private set; }
    }

    class ActionLog
    {

    }
}
