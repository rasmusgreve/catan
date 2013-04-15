using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public interface Agent
    {
        void Reset(int assignedID); // the agent should reset for a new game, the given id is the
                                    // players position in the game (0 is first and 3 is last) //TODO: Nej?

        void PlaceStart(GameState state, StartActions actions); // Only (one) build settlement and build road (legally) will
                                          // be accepted.
        void BeforeDiceRoll(GameState state, GameActions actions); // you have the chance to play a development card before the roll. (Page 16, #4)

        int MoveRobber(GameState state); // you must move the robber to a new tile (CAN BE THE DESERT! Page 16, note 34)

        int ChooseOpponentToDrawFrom(int[] validOpponents); //You must choose an opponent to draw a card from (called after you move the robber)

        Resource[] DiscardCards(GameState state, int toDiscard); // you must discard the toDiscard amount. The
                                                            // returned array must contain the types of cards to discard

        /// <summary>
        /// Use the GameState to perform the turn. Only legal actions will be accepted. Return to finish turn.
        /// Must finish trading before building
        /// If you have played a development card under "BeforeDiceRoll" you can't do it now!
        /// </summary>
        void PerformTurn(GameState state, GameActions actions);
        void HandleTrade(Trade offer); // Change the status of the Trade and return. In case of counter-
                                       // offer remove extra Resource from the trade Lists.
    }
}
