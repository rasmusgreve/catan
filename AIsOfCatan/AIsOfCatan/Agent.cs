using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    interface Agent
    {
        void Reset(int assignedID); // the agent should reset for a new game, the given id is the
                                    // players position in the game (0 is first and 3 is last)

        void PlaceStart(GameState state); // Only (one) build settlement and build road (legally) will
                                          // be accepted.
        void BeforeDieRoll(GameState state); // you have the chance to play a knight before the die roll

        int MoveRobber(GameState state); // you must move the robber to a tile not containing the desert

        Resource[] DiscardCards(GameState state, int toDiscard); // you must discard the toDiscard amount. The
                                                            // returned array must contain the indices of
                                                            // the cards to discard.

        void PerformTurn(GameState state); // Use the GameState to perform the turn. Only legal actions
                                           // will be accepted. Return to finish turn.
        void HandleTrade(Trade offer); // Change the status of the Trade and return. In case of counter-
                                       // offer remove extra Resource from the trade Lists.
    }
}
