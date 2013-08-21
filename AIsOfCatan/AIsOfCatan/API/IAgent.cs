
namespace AIsOfCatan
{
    /// <summary>
    /// To make an artificial intelligence capable of playing catan you need to implement this interface.
    /// The methods are called in the following order:
    ///     * Reset
    ///     * PlaceStart
    ///     * PlaceStart
    ///     Repeated until the end of the game:
    ///         * BeforeDiceRoll
    ///         * PerformTurn
    /// 
    /// At any point the methods
    ///     1 MoveRobber
    ///     2 ChoosePlayerToDrawFrom
    ///     3 DiscardCards
    ///     4 HandleTrade
    /// can be called. 
    ///     #1, #2 and #3 if you roll 7. 
    ///     #3 if someone else rolls 7. 
    ///     #1 and #2 if you play a knight.
    ///     #4 if another player proposes a trade
    /// 
    /// The players of the game are referred to by an integer id.
    /// The id of your agent will be given when the method Reset is called. This value should be stored.
    /// Additional game rules and conditions can be found in the rest of the API interfaces:
    ///     * IGameState    - Contains the board, count of remaining resources and development cards + count of other players resource cards
    ///     * IBoard        - Tile types, numbers and roads, settlements and cities can be found here.
    ///     * IStartAction  - Methods to place your first settlements and roads before the game begins
    ///     * IGameActions  - Methods for taking actions during your turn. (Place road/settlement/city, play dev. card, propose trade, etc.)
    ///     * ITrade        - Information regarding a trade proposal and methods for reversing or counterproposing
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Reset the agent, getting it ready for a new game
        /// The agent should store the assigned Id for later in the game
        /// </summary>
        /// <param name="assignedId">The Id assigned to this player</param>
        void Reset(int assignedId);

        /// <summary>
        /// Return a human readable name for this agent
        /// This name should never change and should be implemented by just returning a string constant
        /// For information regarding strategy or behaviour use the getDescription() method
        /// </summary>
        /// <returns>A human readable name for this agent</returns>
        string GetName();

        /// <summary>
        /// Return a description of this agent
        /// The description can be anything you want it to be
        /// Intended usages include debugging of agents and reporting current strategy
        /// </summary>
        /// <returns>A human readable description for this agent</returns>
        string GetDescription();

        /// <summary>
        /// This method is called twice at the beginning of the game and is where
        /// the agent is to decide where to place the initial settlements and roads before the game begins
        /// Players receive resources from the last settlement placed in the beginning of the game
        /// </summary>
        /// <param name="state">The state of the game when the agent is to place its pieces</param>
        /// <param name="actions">An action object where methods for placing a settlement and a road is</param>
        void PlaceStart(IGameState state, IStartActions actions);

        /// <summary>
        /// In your agents turn, before the dice roll you have a chance of playing a development card. (rules p. 16 - #4)
        /// A maximum of one development card may be played in each turn, thus if a card is played now, it can't be
        /// done after the dice roll
        /// Reasonably the only kind of development cards it makes sense to play before the dice roll is a knight
        /// </summary>
        /// <param name="state">The state of the game when the agent is to decide whether or not to play a development card</param>
        /// <param name="actions">An action obejct where methods for playing development cards are. There are also methods for building which may not be called at this time</param>
        void BeforeDiceRoll(IGameState state, IGameActions actions);

        /// <summary>
        /// If the dice roll comes out as 7 or you play a knight you must move the robber to a new location
        /// After relocation of the robber, a call to ChooseOpponentToDrawFrom is made allowing your agent to choose which
        /// opponent, of the ones who have a building on the tile where you place the robber, you want to draw a random card from
        /// The robber must be moved to a new position (rules p. 10 - top)
        /// The robber can be moved back to the desert (riles p. 16 - note 34)
        /// </summary>
        /// <param name="state">The state of the game when the agent is to decide where to place the robber</param>
        /// <returns>The index of the hex where the robber will move to</returns>
        int MoveRobber(IGameState state);
        
        /// <summary>
        /// After moving the robber you must choose which player you want to draw a card from
        /// The array validOptions contains IDs of players with buildings on hex where the robber was placed
        /// If an invalid player Id is returned you will receive no resources
        /// </summary>
        /// <param name="validOpponents">IDs of players with buildings on hex where the robber was placed</param>
        /// <returns>The chosen Id from the list</returns>
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
        Resource[] DiscardCards(IGameState state, int toDiscard);

        /// <summary>
        /// After the dice roll and resources have been handed out or the robber moved you can perform any number of actions
        /// This method is supplied a GameState which is where the current state of the game is located
        /// The supplied IGameActions object contains methods for doing various things that are possible during the turn
        /// Note that you may play at most 1 development card in each turn. This also includes in the BeforeDiceRoll method.
        /// When you are done with your turn simply return from the function and the next player will have his turn.
        /// </summary>
        /// <param name="state">The state of the game as the main part of your turn begins</param>
        /// <param name="actions">The actions you can perform during your turn. Note that performing actions doesn't update the GameState but most methods return a new updated version</param>
        void PerformTurn(IGameState state, IGameActions actions);

        /// <summary>
        /// During a players turn it is possible to propose a trade with the other players
        /// In this method you must handle this case by choosing what to do when a trade is proposed
        /// The input trade is the one that the current player has proposed to all players
        /// You may choose to either decline the offer or accept/counter the offer with other resources
        /// Note that even though you have acceptet the trade, it is up to the proposing player 
        /// to choose which player he want to trade with, and if he wants to trade at all
        /// </summary>
        /// <param name="offer">The proposing players trade offer</param>
        /// <param name="proposingPlayerId">The id of the player who proposed the trade</param>
        /// <returns>Either offer.Decline() or offer.Respond(give, take) (see ITrade)</returns>
        ITrade HandleTrade(ITrade offer, int proposingPlayerId);
    }
}
