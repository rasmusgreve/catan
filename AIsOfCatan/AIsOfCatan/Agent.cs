using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public interface Agent
    {
        /// <summary>
        /// Reset the agent, getting it ready for a new game
        /// The agent should store the assigned ID for later in the game
        /// </summary>
        /// <param name="assignedID">The ID assigned to this player</param>
        void Reset(int assignedID);

        /// <summary>
        /// This method is called twice at the beginning of the game and is where
        /// the agent is to decide where to place the initial settlements and roads before the game begins
        /// Players receive resources from the last settlement placed in the beginning of the game
        /// </summary>
        /// <param name="state">The state of the game when the agent is to place its pieces</param>
        /// <param name="actions">An action object where methods for placing a settlement and a road is</param>
        void PlaceStart(GameState state, StartActions actions);

        /// <summary>
        /// In your agents turn, before the dice roll you have a chance of playing a development card. (rules p. 16 - #4)
        /// A maximum of one development card may be played in each turn, thus if a card is played now, it can't be
        /// done after the dice roll
        /// Reasonably the only kind of development cards it makes sense to play before the dice roll is a knight
        /// </summary>
        /// <param name="state">The state of the game when the agent is to decide whether or not to play a development card</param>
        /// <param name="actions">An action obejct where methods for playing development cards are. There are also methods for building which may not be called at this time</param>
        void BeforeDiceRoll(GameState state, GameActions actions);

        /// <summary>
        /// If the dice roll comes out as 7 or you play a knight you must move the robber to a new location
        /// After relocation of the robber, a call to ChooseOpponentToDrawFrom is made allowing your agent to choose which
        /// opponent, of the ones who have a building on the tile where you place the robber, you want to draw a random card from
        /// The robber must be moved to a new position (rules p. 10 - top)
        /// The robber can be moved back to the desert (riles p. 16 - note 34)
        /// </summary>
        /// <param name="state">The state of the game when the agent is to decide where to place the robber</param>
        /// <returns>The index of the hex where the robber will move to</returns>
        int MoveRobber(GameState state);
        
        /// <summary>
        /// After moving the robber you must choose which player you want to draw a card from
        /// The array validOptions contains IDs of players with buildings on hex where the robber was placed
        /// If an invalid player ID is returned you will receive no resources
        /// </summary>
        /// <param name="validOpponents">IDs of players with buildings on hex where the robber was placed</param>
        /// <returns>The chosen ID from the list</returns>
        int ChoosePlayerToDrawFrom(int[] validOpponents); //You must choose an opponent to draw a card from (called after you move the robber)

        /// <summary>
        /// If a 7 is rolled on any turn, players with more than 7 cards must discard half of their cards rounded down
        /// In this method your agent must choose which resources to discard.
        /// The agents resources can be found in the supplied GameState object and the number toDiscard says how many must be discarded
        /// Notice! If your agent returns a wrong amount of resources, the resources to discard will be chosen at random.
        /// </summary>
        /// <param name="state">The state of the game when cards must be discarded</param>
        /// <param name="toDiscard">The amount of cards to discard</param>
        /// <returns>An array of resource types telling which cards to discard (duplicates allowed)</returns>
        Resource[] DiscardCards(GameState state, int toDiscard);

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
