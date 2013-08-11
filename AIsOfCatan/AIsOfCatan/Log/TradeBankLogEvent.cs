using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan.Log
{
    class TradeBankLogEvent : LogEvent
    {
        public int Player { get; private set; }
        public Resource Payment { get; private set; }
        public int Price { get; private set; }
        public Resource Purchase { get; private set; }

        public TradeBankLogEvent(int player, Resource payment, int price, Resource purchase)
        {
            Player = player;
            Payment = payment;
            Price = price;
            Purchase = purchase;
        }

        public override string ToString()
        {
            return "Player " + Player + " traded with the bank " + Price + " " + Payment + " for a " + Purchase;
        }
    }
}
