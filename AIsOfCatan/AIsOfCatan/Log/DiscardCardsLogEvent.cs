using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class DiscardCardsLogEvent : LogEvent
    {
        public int Player { get; private set; }
        private List<Resource> cards;
        public List<Resource> Cards { get { return cards.ToList(); } }

        public DiscardCardsLogEvent(int player, List<Resource> cards)
        {
            Player = player;
            this.cards = cards;
        }

        public override string ToString()
        {
            return "Player " + Player + " discards " + cards.ToDeepString();
        }
    }
}
