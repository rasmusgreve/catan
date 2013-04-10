using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    interface Agent
    {
        void PlaceStart(GameState state); // Only (one) build settlement and build road (legally) will
                                          // be accepted.

        void PerformTurn(GameState state); // Use the GameState to perform the turn. Only legal actions
                                           // will be accepted. Return to finish turn.
        void HandleTrade(Trade offer); // Change the status of the Trade and return. In case of counter-
                                       // offer remove extra Resources from the trade Lists.
    }
}
