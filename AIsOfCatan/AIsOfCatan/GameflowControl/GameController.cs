using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIsOfCatan.Log;

namespace AIsOfCatan
{
    public class GameController
    {
        private Random diceRandom;
        private Random shuffleRandom;
        private Player[] players;

        private List<LogEvent> log = new List<LogEvent>(); // Here you go Rasmus

        private Board board;
        private List<DevelopmentCard> developmentCardStack = new List<DevelopmentCard>();
        private int[] resourceBank;
        private int turn;
        private int largestArmySize = 2; //One less than the minimum for getting the largest army card
        private int longestRoadLength = 0;
        private int largestArmyId = -1;
        private int longestRoadId = -1;

        private const int LargestArmyMinimum = 3;
        private const int LongestRoadMinimum = 5;

        /// <summary>
        /// Start the game. This method will run for the length of the game and returns the id of the winner
        /// </summary>
        /// <param name="agents">The competing agents (The order in which they are submitted is irrelevant)</param>
        /// <param name="boardSeed">The seed for the board generator, used to shuffle development cards, and for drawing a random card after moving the robber</param>
        /// <param name="diceSeed">The seed for the dice</param>
        /// <returns>The id of the winner of the game (-1 in case of error)</returns>
        public int StartGame(IAgent[] agents, int boardSeed, int diceSeed)
        {
            //Initialize random number generators
            diceRandom = new Random(diceSeed);
            shuffleRandom = new Random(boardSeed); //The card deck is based on the seed of the board
               
            //Build player list
            players = new Player[agents.Length];
            for (int i = 0; i < agents.Length; i++)
            {
                players[i] = new Player(agents[i], i);
            }

            //Set up board
            board = new Board(boardSeed);
            PopulateDevelopmentCardStack();
            resourceBank = new int[] { 19, 19, 19, 19, 19 };

            //Start the game!
            turn = 0; //TODO: Shuffle agents array? !IMPORTANT! DO IT BEFORE THE IDs ARE ASSIGNED!
            PlaceStarts();
            return GameLoop();
        }

        /// <summary>
        /// Populate and shuffle the development card stack according to the rules (rules p. 2 - Game Contents)
        /// </summary>
        private void PopulateDevelopmentCardStack()
        {
            developmentCardStack.Clear();

            for (int i = 0; i < 14; i++) developmentCardStack.Add(DevelopmentCard.Knight);
            for (int i = 0; i <  5; i++) developmentCardStack.Add(DevelopmentCard.VictoryPoint);
            for (int i = 0; i <  2; i++)
            {
                developmentCardStack.Add(DevelopmentCard.RoadBuilding);
                developmentCardStack.Add(DevelopmentCard.YearOfPlenty);
                developmentCardStack.Add(DevelopmentCard.Monopoly);
            }

            //Shuffle!
            for (int n = developmentCardStack.Count-1; n > 1; n--)
            {
                int k = shuffleRandom.Next(n);
                var aux = developmentCardStack[k];
                developmentCardStack[k] = developmentCardStack[n];
                developmentCardStack[n] = aux;
            }
        }

        /// <summary>
        /// Executes the game, each player turn by turn until a player wins
        /// </summary>
        /// <returns>The Id of the winning player</returns>
        private int GameLoop()
        {
            while (true)
            {
                try
                {
                    TakeTurn(players[turn]);
                }
                catch (AgentActionException e)
                {
                    Console.WriteLine("Player " + players[turn].Id + ", caused an exception: " + e.GetType().Name);
                    Console.WriteLine("       -> Message: " + e.Message);
                    if (e.StopGame)
                    {
                        Console.WriteLine("This is game breaking, and the game ends now.");
                        return -1;
                    }
                }
                if (HasWon(players[turn])) return players[turn].Id;

                NextTurn();
            }
        }

        /// <summary>
        /// Find out if a given player has won (if his number of victory points is over or equal to 10)
        /// Points are counted as:
        ///     Settlements give 1 point each
        ///     Cities give 2 points each
        ///     Victory point development cards give 1 point each
        ///     Having the largest army or the longest road give 2 points each.
        /// </summary>
        /// <param name="player">The player to test</param>
        /// <returns>True if the player has 10 or more victory points</returns>
        private Boolean HasWon(Player player)
        {
            int points = 0;
            points += (5 - player.SettlementsLeft) * 1;
            points += (4 - player.CitiesLeft) * 2;
            
            points += player.DevelopmentCards.Count(c => c == DevelopmentCard.VictoryPoint) * 1;
            
            if (player.Id == largestArmyId) points += 2;
            if (player.Id == longestRoadId) points += 2;
            
            return (points >= 10);
        }

        /// <summary>
        /// Executes all parts of a players turn
        ///     1. Allow the play of a development card before the dice are rolled
        ///     2. Roll the dice, hand out resources to all players, and move the robber if roll is 7
        ///     3. Allow all actions according to the rules
        /// </summary>
        /// <param name="player">The player whose turn it is</param>
        private void TakeTurn(Player player)
        {
            var actions = new MainActions(player, this);
            player.Agent.BeforeDiceRoll(new GameState(board, developmentCardStack, resourceBank, players, turn, log), actions);
            
            int roll = RollDice();
            actions.DieRoll();
            log.Add(new RollLogEvent(player.Id,roll));
            
            if (roll == 7)
            {
                //Discard if over 7 cards
                foreach (var p in players)
                {
                    if (p.Resources.Count > 7)
                    {
                        var cards = p.Agent.DiscardCards(new GameState(board, developmentCardStack, resourceBank, players, p.Id, log), p.Resources.Count / 2);
                        if (cards.Length != p.Resources.Count / 2)
                        {
                            //Clone, shuffle, take, convert
                            cards = p.Resources.ToList().OrderBy(e => Guid.NewGuid()).Take(p.Resources.Count / 2).ToArray();
                        }
                        foreach (var c in cards)
                        {
                            PayResource(p, c);
                        }
                        log.Add(new DiscardCardsLogEvent(p.Id,cards.ToList()));
                    }
                }
                MoveRobber(player, new GameState(board, developmentCardStack, resourceBank, players, turn, log));
            }
            else
            {
                HandOutResources(roll);
            }
            var afterResourcesState = new GameState(board, developmentCardStack, resourceBank, players, turn, log);
            player.Agent.PerformTurn(afterResourcesState, actions);

            player.NewDevelopmentCards.Clear(); //Reset new development cards
        }

        /// <summary>
        /// Let a player move the robber to a new location and draw a random card from a player with a building on the tile
        /// If the agent moves the robber to a water tile or the tile that it was already on, nothing will happen
        /// If the agent tries to draw a card from a unaffected player, no cards will be drawn
        /// </summary>
        /// <param name="player">The player that must move the robber</param>
        /// <param name="gameState">The gamestate to send to the player</param>
        private void MoveRobber(Player player, GameState gameState)
        {
            int robberPosition = player.Agent.MoveRobber(gameState);
            if (board.GetTile(robberPosition).Terrain == Terrain.Water || robberPosition == board.GetRobberLocation())
            {
                Console.WriteLine("IAgent " + player.Agent.GetType().Name + " moved robber illegally");
                throw new AgentActionException("Agent " + player.Agent.GetType().Name + " moved robber illegally", true);
            }
            board = board.MoveRobber(robberPosition);

            log.Add(new MoveRobberLogEvent(player.Id, robberPosition));
            //Draw a card from an opponent
            var opponents = new List<int>();
            foreach (var piece in board.GetPieces(robberPosition))
            {
                if (piece.Player == player.Id) continue;
                if (opponents.Contains(piece.Player)) continue;
                opponents.Add(piece.Player);
            }
            if (opponents.Count == 0) return; //No opponents to draw from
            int choice = player.Agent.ChoosePlayerToDrawFrom(opponents.ToArray());

            log.Add(new StealCardLogEvent(player.Id, choice));

            if (!opponents.Contains(choice)) //If the agent gives a bad answer, we ask again
            {
                Console.WriteLine("IAgent " + player.Agent.GetType().Name + " chose an illegal player to draw from");
                return;
            }

            if (players[choice].Resources.Count == 0) return; //Nothing to take
            

            //Move a card from one player to another
            var position = shuffleRandom.Next(players[choice].Resources.Count);
            var toMove = players[choice].Resources[position];
            players[choice].Resources.RemoveAt(position);
            player.Resources.Add(toMove);
        }

        /// <summary>
        /// Hand out resources to players according to a roll
        /// Gives resources to players with buildings on tiles with a number corresponding to the roll
        /// and only if the tile doesn't have the robber.
        /// If there is not enough resources of a kind in the bank so that all player who shall receive,
        /// can get the amount they are allowed to, none of that kind are dealt (see rules p. 8 top)
        /// </summary>
        /// <param name="roll">The value of the roll</param>
        private void HandOutResources(int roll)
        {
            //Map from PlayerID to dictionary that maps resource to amount
            var handouts = new Dictionary<int, Dictionary<Resource, int>>();
            var handoutSums = new Dictionary<Resource, int>();
            for (int i = 0; i < players.Length; i++)
            {
                handouts[i] = new Dictionary<Resource, int>();
                foreach (Resource r in Enum.GetValues(typeof(Resource)))
                    handouts[i][r] = 0;
            }
            foreach (Resource r in Enum.GetValues(typeof(Resource)))
                handoutSums[r] = 0;
            //Count how many resources to be dealt
            for (int i = 0; i <= 44; i++)
            {
                var tile = board.GetTile(i);
                if (tile.Value != roll || board.GetRobberLocation() == i) continue;
                foreach (var piece in board.GetPieces(i))
                {
                    int incr = (piece.Token == Token.Settlement) ? 1 : 2;
                    handouts[piece.Player][(Resource)tile.Terrain] += incr;
                    handoutSums[(Resource)tile.Terrain] += incr;
                }
            }

            //Check if there are enough resources in the bank
            foreach (var resource in handoutSums.Keys)
            {
                if (resourceBank[(int)resource] < handoutSums[resource])
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        handouts[i][resource] = 0;
                    }
                }
            }

            //Hand out resources
            foreach (var player in players)
            {
                foreach (var resource in handouts[player.Id].Keys)
                {
                    GetResource(player, resource, handouts[player.Id][resource]);
                }
            }
        }

        /// <summary>
        /// Let the players place their starting settlements and roads and receive resources 
        /// from all neighboring tiles from the last placed settlement (rules p. 7 top)
        /// </summary>
        private void PlaceStarts()
        {
            foreach (Player p in players)
            {
                var state = new GameState(board, developmentCardStack, resourceBank, players, turn, log);
                var actions = new StartActions(players[turn], this);
                players[turn].Agent.PlaceStart(state, actions);
                if (!actions.IsComplete())
                {
                    throw new AgentActionException("Agent " + p.Agent.GetType().Name + " did not place the correct amount of start pieces (1/2)", true);
                }
                NextTurn();
            }
            foreach (Player p in players)
            {
                PrevTurn();
                var state = new GameState(board, developmentCardStack, resourceBank, players, turn, log);
                var actions = new StartActions(players[turn], this);
                players[turn].Agent.PlaceStart(state, actions);
                if (!actions.IsComplete())
                {
                    throw new AgentActionException("Agent " + p.Agent.GetType().Name + " did not place the correct amount of start piece (2/2)", true);
                }

                //Hand out resources
                foreach (var pos in actions.GetSettlementPosition())
                {
                    GetResource(players[turn], (Resource)board.GetTile(pos).Terrain);
                }
            }
        }

        /// <summary>
        /// Roll two d6 using the seeded random number generator and get the sum
        /// </summary>
        /// <returns>The sum of the roll</returns>
        private int RollDice()
        {
            int d1 = diceRandom.Next(1,7);
            int d2 = diceRandom.Next(1,7);
            Console.WriteLine("Rolled " + d1 + ", " + d2 + " = " + (d1+d2));
            return d1+d2;
        }

        /// <summary>
        /// Increment the turn variable modulo the number of players
        /// </summary>
        private void NextTurn()
        {
            turn = (turn + 1) % players.Length;
        }

        /// <summary>
        /// Decrement the turn variable, if it goes below 0, wrap around
        /// </summary>
        private void PrevTurn()
        {
            turn = turn - 1;
            if (turn < 0) turn += players.Length;
        }

        /// <summary>
        /// Let a given player pay an amount of a resource to the bank
        /// </summary>
        /// <param name="player">The player that must pay</param>
        /// <param name="resource">The type of resource to pay</param>
        /// <param name="quantity">The quantity of the resource to pay (default 1)</param>
        private void PayResource(Player player, Resource resource, int quantity = 1)
        {
            for (int i = 0; i < quantity; i++)
            {
                player.Resources.Remove(resource);
                resourceBank[(int)resource]++;
            }
        }

        /// <summary>
        /// Give a player an amount of some resource
        /// If there are no more cards in the pile an NoMoreCardsException is thrown
        /// </summary>
        /// <param name="player">The player to give resources to</param>
        /// <param name="resource">The type of resource he receives</param>
        /// <param name="quantity">The amount of the resource he receives (default 1)</param>
        private void GetResource(Player player, Resource resource, int quantity = 1)
        {
            for (int i = 0; i < quantity; i++)
            {
                if (resourceBank[(int)resource] == 0) throw new NoMoreCardsException("Resource bank is out of " + resource.ToString());
                player.Resources.Add(resource);
                resourceBank[(int)resource]--;
            }
        }

        private GameState CurrentGamestate()
        {
            return new GameState(board, developmentCardStack, resourceBank, players, turn, log);
        }

        /// <summary>
        /// Figure out if a given edge on the board is connected to a given players road in some end
        /// </summary>
        /// <param name="firstTile">The index of the first tile on the edge</param>
        /// <param name="secondTile">The index of the second tile on the edge</param>
        /// <param name="playerId">The id of the player to test</param>
        /// <returns>True if the edge is connected to a road</returns>
        private bool RoadConnected(Board board, int firstTile, int secondTile, int playerId)
        {
            return board.GetAdjacentIntersections(firstTile, secondTile)
                     .SelectMany(inter => board.GetAdjacentEdges(inter.Item1, inter.Item2, inter.Item3))
                     .Any(edge => board.GetRoad(edge.Item1, edge.Item2) == playerId);
        }

        /// <summary>
        /// Update the player who has the longest road to be able to determine who has how many points
        /// </summary>
        private void UpdateLongestRoad()
        {
            var playersLongest = board.GetLongestRoad();
            var newLength = playersLongest.OrderByDescending(p => p.Value).First().Value;
            if (newLength < LongestRoadMinimum) //Don't hand out the card if road is too short
            {
                longestRoadId = -1;
                return;
            }
            var ids = playersLongest.Where(p => p.Value == newLength).Select(p => p.Key);
            var newId = (ids.Count()) > 1 ? -1 : ids.First();
            /*
            if (newLength > longestRoadLength) //Road is longer than previously (can only happen with a valid player ID)
            {
                longestRoadLength = newLength;
                longestRoadId = newId;
            }
            else if (newLength < longestRoadLength && newId != -1) //Previously longest must have been divided but someone has a longest road
            {
                longestRoadLength = newLength;
                longestRoadId = newId;
            }
            else if (newLength < longestRoadLength && newId == -1) //Previously longest must have been divided and others are tied
            {
                longestRoadLength = newLength;
                longestRoadId = -1;
            }
            else if (newLength == longestRoadLength)
            {
                //Do nothing
            }
             * */
            //Reduced to:
            if (newLength == longestRoadLength) return;
            longestRoadLength = newLength;
            longestRoadId = newId;
        }

        /// <summary>
        /// Get a list of playable development cards for a given player
        /// Newly bought development cards cannot be played in the same round
        /// </summary>
        /// <param name="player">The player for whom to get the list of playable development cards</param>
        /// <returns>The list of playable development cards</returns>
        private List<DevelopmentCard> GetPlayableDevelopmentCards(Player player)
        {
            var playable = new List<DevelopmentCard>();
            foreach (var card in player.DevelopmentCards)
            {
                if (player.NewDevelopmentCards.Contains(card))
                {
                    player.NewDevelopmentCards.Remove(card);
                }
                else
                {
                    playable.Add(card);
                }
            }
            return playable;
        }

        /*
         * ACTIONS
         */

        /// <summary>
        /// Let a player draw a development card
        /// If the player doesn't have enough resources a InsufficientResourcesException is thrown
        /// If the development card stack is empty a NoMoreCardsException is thrown
        /// Resources to pay for the card are removed from the player hand and returned to the resource bank
        /// </summary>
        /// <param name="player">The player drawing a development card</param>
        /// <returns>The drawn development card</returns>
        public GameState DrawDevelopmentCard(Player player)
        {
            var r = player.Resources;
            if (!(r.Contains(Resource.Grain) && r.Contains(Resource.Wool) && r.Contains(Resource.Ore)))
                throw new InsufficientResourcesException("Not enough resources to buy a development card");

            if (developmentCardStack.Count == 0)
                throw new NoMoreCardsException("Development card stack is empty");

            PayResource(player, Resource.Grain);
            PayResource(player, Resource.Wool);
            PayResource(player, Resource.Ore);

            var last = developmentCardStack.Last();
            developmentCardStack.RemoveAt(developmentCardStack.Count-1);

            log.Add(new BuyDevLogEvent(player.Id));

            player.DevelopmentCards.Add(last);
            player.NewDevelopmentCards.Add(last);
            return CurrentGamestate();
        }

        private readonly Dictionary<int, Dictionary<int, Trade>> proposedTrades = new Dictionary<int, Dictionary<int, Trade>>(); //TODO: Move this

        public Dictionary<int, Trade> ProposeTrade(Player player, Trade trade)
        {
            var dict = new Dictionary<int, Trade>();

            log.Add(new ProposeTradeLogEvent(player.Id, trade.Give, trade.Take));

            foreach (var other in players)
            {
                if (other.Id == player.Id) continue; //No need to propose a trade with yourself
                dict[other.Id] = (Trade)other.Agent.HandleTrade(trade.Reverse(), player.Id);
                if (dict[other.Id].Status == TradeStatus.Countered)
                {
                    //Note, take and give are swapped since dict[other.Id] is as seen from the opponent
                    var give = dict[other.Id].Take.Where(c => c.Count > 0).Select(r => r[0]).ToList();
                    var take = dict[other.Id].Give.Where(c => c.Count > 0).Select(r => r[0]).ToList();
                    log.Add(new CounterTradeLogEvent(other.Id, give, take));
                }
            }
            proposedTrades[player.Id] = dict;

            return dict;
        }

        public GameState CompleteTrade(Player player, int playerid)
        {
            if (!proposedTrades.ContainsKey(player.Id))
                throw new IllegalActionException("Tried to complete a trade, but no trade proposed");
            if (!proposedTrades[player.Id].ContainsKey(playerid) || playerid < 0 || playerid >= players.Length)
                throw new IllegalActionException("Tried to complete a trade with an illegal player Id");
            
            var trade = proposedTrades[player.Id][playerid]; //remember that the trade is as seen from the opponents pov
            var opponent = players[playerid];
            if (trade.Status == TradeStatus.Declined)
                throw new IllegalActionException("Tried to complete a declined trade");

            //Validate trade
            if (trade.Give.Any(res => res.Count > 1) || trade.Take.Any(res => res.Count > 1))
            {
                throw new IllegalActionException("Tried to complete a trade containing wildcards");
            }

            //Complete trade
            var give = new List<Resource>(); //for logging
            var take = new List<Resource>();
            foreach (var res in trade.Give.Where(res => res.Count != 0))
            {
                opponent.Resources.Remove(res[0]);
                player.Resources.Add(res[0]);
                take.Add(res[0]);
            }
            foreach (var res in trade.Take.Where(res => res.Count != 0))
            {
                player.Resources.Remove(res[0]);
                opponent.Resources.Add(res[0]);
                give.Add(res[0]);
            }

            log.Add(new AcceptTradeLogEvent(player.Id, playerid, give, take));

            return CurrentGamestate();
        }

        /// <summary>
        /// Let a player play a knight development card
        /// If the player doesn't have a knight on his hand a InsufficientResourcesException is thrown
        /// A knight is removed from the players hand
        /// The largest army special card is relocated if playing this knight causes it to be
        /// </summary>
        /// <param name="player">The player playing a knight</param>
        public GameState PlayKnight(Player player)
        {
            var playable = GetPlayableDevelopmentCards(player);
            if (!playable.Contains(DevelopmentCard.Knight))
                throw new InsufficientResourcesException("No knight found in hand");

            player.DevelopmentCards.Remove(DevelopmentCard.Knight);
            player.PlayedKnights++;
            if (player.PlayedKnights > largestArmySize)
            {
                largestArmySize = player.PlayedKnights;
                largestArmyId = player.Id;
            }

            MoveRobber(player, new GameState(board, developmentCardStack, resourceBank, players, turn, log));

            log.Add(new PlayKnightLogEvent(player.Id));

            return CurrentGamestate();
        }

        /// <summary>
        /// Let a player play a road building development card
        /// If the player doesn't have a RoadBuilding card on his hand a InsufficientResourcesException is thrown
        /// If the player doesn't have any road pieces left a IllegalActionException is thrown
        /// If the player tries to place a road at a position where a road is already present a IllegalBuildPositionException is thrown
        /// If the player only has one road piece left, the position to place it must be passed as road1Tile1, road1Tile2 (the others are ignored)
        /// </summary>
        /// <param name="player">The player that plays the RoadBuilding development card</param>
        /// <param name="road1Tile1">The first tile that the first road must be along</param>
        /// <param name="road1Tile2">The second tile that the first road must be along</param>
        /// <param name="road2Tile1">The first tile that the second road must be along</param>
        /// <param name="road2Tile2">The second tile that the second road must be along</param>
        public GameState PlayRoadBuilding(Player player, int road1Tile1, int road1Tile2, int road2Tile1, int road2Tile2)
        {
            var playable = GetPlayableDevelopmentCards(player);
            if (!playable.Contains(DevelopmentCard.RoadBuilding)) throw new InsufficientResourcesException("No Road building found in hand");
            if (player.RoadsLeft == 0) throw new IllegalActionException("No more road pieces left of your color");

            //Must always check road1
            if (!board.CanBuildRoad(road1Tile1, road1Tile2))
                throw new IllegalBuildPositionException("The chosen position does not exist");
            if (board.GetRoad(road1Tile1, road1Tile2) != -1)
                throw new IllegalBuildPositionException("There is already a road on the selected position");

            if (player.RoadsLeft == 1)
            {
                if (!RoadConnected(board, road1Tile1, road1Tile2, player.Id))
                    throw new IllegalBuildPositionException("The chosen position is not connected to any of your pieces");

                player.RoadsLeft--;
                board = board.PlaceRoad(road1Tile1, road1Tile2, player.Id);
                log.Add(new PlayRoadBuildingLogEvent(player.Id, new Tuple<int, int>(road1Tile1, road1Tile2)));
            }
            else
            {
                //Check road 2
                if (!board.CanBuildRoad(road2Tile1, road2Tile2))
                    throw new IllegalBuildPositionException("The chosen position does not exist");
                if (board.GetRoad(road2Tile1, road2Tile2) != -1)
                    throw new IllegalBuildPositionException("There is already a road on the selected position");

                //Can't build the same road twice
                if ((road1Tile1 == road2Tile1 && road1Tile2 == road2Tile2) || (road1Tile1 == road2Tile2 && road1Tile2 == road2Tile1))
                    throw new IllegalBuildPositionException("Can't build the same road twice (roadbuilding dev. card)");

                //Place the connected road first (to be able to check that both are connected in the end
                if (RoadConnected(board, road1Tile1, road1Tile2, player.Id))
                {
                    var temp = board.PlaceRoad(road1Tile1, road1Tile2, player.Id);
                    if (RoadConnected(temp, road2Tile1, road2Tile2, player.Id))
                    {
                        board = temp.PlaceRoad(road2Tile1, road2Tile2, player.Id);
                        player.RoadsLeft -= 2;
                    }
                }
                else if (RoadConnected(board, road2Tile1, road2Tile2, player.Id))
                {
                    var temp = board.PlaceRoad(road2Tile1, road2Tile2, player.Id);
                    if (RoadConnected(temp, road1Tile1, road1Tile2, player.Id))
                    {
                        board = temp.PlaceRoad(road1Tile1, road1Tile2, player.Id);
                        player.RoadsLeft -= 2;
                    }
                }
                else
                {
                    throw new IllegalBuildPositionException("The chosen positions are not connected to any of your buildings or roads");
                }
                log.Add(new PlayRoadBuildingLogEvent(player.Id, new Tuple<int, int>(road1Tile1,road1Tile2), new Tuple<int, int>(road2Tile1,road2Tile2)));
            }

            player.DevelopmentCards.Remove(DevelopmentCard.RoadBuilding);
            UpdateLongestRoad();
            return CurrentGamestate();
        }

        /// <summary>
        /// Let a player play a year of plenty development card
        /// If the player doesn't have a YearOfPlenty card on his hand a InsufficientResourcesException is thrown
        /// If the resource bank doesn't have enough cards to fulfill the request a NoMoreCardsException is thrown
        /// </summary>
        /// <param name="player">The player playing the year of plenty development card</param>
        /// <param name="resource1">The type of resource for the first card</param>
        /// <param name="resource2">The type of resource for the second card</param>
        public GameState PlayYearOfPlenty(Player player, Resource resource1, Resource resource2)
        {
            var playable = GetPlayableDevelopmentCards(player);
            if (resourceBank[(int)resource1] == 0) throw new NoMoreCardsException("Resource bank is out of " + resource1);
            if (resourceBank[(int)resource2] == 0) throw new NoMoreCardsException("Resource bank is out of " + resource2);
            if (!playable.Contains(DevelopmentCard.YearOfPlenty)) throw new InsufficientResourcesException("No Year of Plenty found in hand");

            player.DevelopmentCards.Remove(DevelopmentCard.YearOfPlenty);
            GetResource(player, resource1);
            GetResource(player, resource2);
            
            log.Add(new PlayYearOfPlentyLogEvent(player.Id, resource1, resource2));

            return CurrentGamestate();
        }

        /// <summary>
        /// Let a player play a Monopoly development card
        /// If the player doesn't have a Monopoly card on his hand a InsufficientResourcesException is thrown
        /// All resources of the given type is removed from players hands and all given to the playing player
        /// </summary>
        /// <param name="player">The player playing the monopoly development card</param>
        /// <param name="resource">The resource to get monopoly on</param>
        /// <returns></returns>
        public GameState PlayMonopoly(Player player, Resource resource)
        {
            var playable = GetPlayableDevelopmentCards(player);
            if (!playable.Contains(DevelopmentCard.Monopoly)) throw new InsufficientResourcesException("No Monopoly in hand");
            player.DevelopmentCards.Remove(DevelopmentCard.Monopoly);
            //Take all resources of the given type out of all hands
            int count = players.Sum(t => t.Resources.RemoveAll(c => c == resource));
            //And place them in the playing players hand
            for (int i = 0; i < count; i++)
            {
                player.Resources.Add(resource);
            }

            log.Add(new PlayMonopolyLogEvent(player.Id, resource, count));

            return CurrentGamestate();
        }

        /// <summary>
        /// Let a player build a settlement
        /// If the player doesn't have enough resources to build a settlement a InsufficientResourcesException is thrown
        /// If the player tries to build too close to another building, or not connected to a road a IllegalBuildPosition is thrown
        /// If the player doesn't have any more settlement pieces left to place a IllegalActionException is thrown
        /// The required resources are taken from the player and placed back at the resource bank
        /// If the settlement is placed at a harbor, the harbor can be used immediately (rules p. 7 - footnote 12)
        /// </summary>
        /// <param name="player">The player building a settlement</param>
        /// <param name="firstTile">The index of the first tile in the intersection</param>
        /// <param name="secondTile">The index of the second tile in the intersection</param>
        /// <param name="thirdTile">The index of the third tile in the intersection</param>
        /// <returns></returns>
        public GameState BuildSettlement(Player player, int firstTile, int secondTile, int thirdTile)
        {
            var r = player.Resources;
            if (!(r.Contains(Resource.Grain) && r.Contains(Resource.Wool) && r.Contains(Resource.Brick) && r.Contains(Resource.Lumber)))
                throw new InsufficientResourcesException("Not enough resources to buy a settlement");
            if (player.SettlementsLeft == 0)
                throw new IllegalActionException("No more settlement pieces left of your color");
            if (board.GetPiece(firstTile, secondTile, thirdTile) != null)
                throw new IllegalBuildPositionException("The chosen position is occupied by another building");
            if (!board.HasNoNeighbors(firstTile, secondTile, thirdTile))
                throw new IllegalBuildPositionException("The chosen position violates the distance rule");
            if (board.GetRoad(firstTile, secondTile) != player.Id && board.GetRoad(firstTile, thirdTile) != player.Id && board.GetRoad(secondTile, thirdTile) != player.Id)
                throw new IllegalBuildPositionException("The chosen position has no road leading to it");
            if (!board.CanBuildPiece(firstTile, secondTile, thirdTile))
                throw new IllegalBuildPositionException("The chosen position is not valid");

            PayResource(player, Resource.Grain);
            PayResource(player, Resource.Wool);
            PayResource(player, Resource.Brick);
            PayResource(player, Resource.Lumber);

            log.Add(new BuildPieceLogEvent(player.Id, Token.Settlement, new Tuple<int, int, int>(firstTile,secondTile,thirdTile)));

            player.SettlementsLeft--;
            board = board.PlacePiece(firstTile, secondTile, thirdTile, new Board.Piece(Token.Settlement, player.Id));
            UpdateLongestRoad();
            return CurrentGamestate();
        }

        /// <summary>
        /// Let a player upgrade a settlement to a city
        /// If the player doesn't have enough resources to build a city a InsufficientResourcesException is thrown
        /// If the player tries to build at a position where he doesn't have a settlement a IllegalBuildPosition is thrown
        /// If the player doesn't have any more city pieces left to place a IllegalActionException is thrown
        /// The required resources are taken from the player and placed back at the resource bank
        /// The settlement previously on the location is given back to the player
        /// </summary>
        /// <param name="player">The player upgrading to a city</param>
        /// <param name="firstTile">The index of the first tile in the intersection</param>
        /// <param name="secondTile">The index of the second tile in the intersection</param>
        /// <param name="thirdTile">The index of the third tile in the intersection</param>
        /// <returns></returns>
        public GameState BuildCity(Player player, int firstTile, int secondTile, int thirdTile)
        {
            var r = player.Resources;
            if (!(r.Count(c => c == Resource.Ore) >= 3 && r.Count(c => c == Resource.Grain) >= 2))
                throw new InsufficientResourcesException("Not enough resources to buy a city");
            if (player.CitiesLeft == 0)
                throw new IllegalActionException("No more city pieces left of your color");

            Board.Piece piece = board.GetPiece(firstTile, secondTile, thirdTile);
            if (piece == null || piece.Player != player.Id || piece.Token != Token.Settlement)
                throw new IllegalBuildPositionException("The chosen position does not contain one of your settlements");

            PayResource(player, Resource.Ore, 3);
            PayResource(player, Resource.Grain, 2);

            log.Add(new BuildPieceLogEvent(player.Id, Token.City, new Tuple<int, int, int>(firstTile, secondTile, thirdTile)));

            player.CitiesLeft--;
            player.SettlementsLeft++;

            board = board.PlacePiece(firstTile, secondTile, thirdTile, new Board.Piece(Token.City, player.Id));
            return CurrentGamestate();
        }

        /// <summary>
        /// Let a player build a road
        /// If the player doesn't have enough resources to build a road an InsufficientResourcesException is thrown
        /// If the player tries to build at a position not connected to another road, settlement or city an IllegalBuildPositionException is thrown
        /// If the player doesn't have any more road pieces left to place an IllegalActionException is thrown
        /// </summary>
        /// <param name="player">The player building a road</param>
        /// <param name="firstTile">The first tile that the road will be along</param>
        /// <param name="secondTile">The second tile that the road will be along</param>
        /// <returns></returns>
        public GameState BuildRoad(Player player, int firstTile, int secondTile)
        {
            var r = player.Resources;
            if (!(r.Contains(Resource.Brick) && r.Contains(Resource.Lumber)))
                throw new InsufficientResourcesException("Not enough resources to buy a road");
            if (player.RoadsLeft == 0)
                throw new IllegalActionException("No more road pieces left of your color");
            if (board.GetRoad(firstTile, secondTile) != -1)
                throw new IllegalBuildPositionException("The chosen position is occupied by another road");
            if (!RoadConnected(board, firstTile,secondTile, player.Id))
                throw new IllegalBuildPositionException("The chosen position is not connected to any of your pieces");
            if (!board.CanBuildRoad(firstTile, secondTile))
                throw new IllegalBuildPositionException("The chosen position is not valid");
            
            PayResource(player, Resource.Brick);
            PayResource(player, Resource.Lumber);

            log.Add(new BuildRoadLogEvent(player.Id, new Tuple<int, int>(firstTile,secondTile)));

            player.RoadsLeft--;
            board = board.PlaceRoad(firstTile, secondTile, player.Id);
            UpdateLongestRoad();
            return CurrentGamestate();
        }

        /// <summary>
        /// Let a player build a settlement without a requirement for connectivity and free of charge
        /// The settlement still has to follow the distance rule and cannot be placed on top of another building
        /// </summary>
        /// <param name="player">The player placing the settlement</param>
        /// <param name="firstTile">The index of the first tile in the intersection</param>
        /// <param name="secondTile">The index of the second tile in the intersection</param>
        /// <param name="thirdTile">The index of the third tile in the intersection</param>
        public GameState BuildFirstSettlement(Player player, int firstTile, int secondTile, int thirdTile)
        {
            if (board.GetPiece(firstTile, secondTile, thirdTile) != null)
                throw new IllegalBuildPositionException("The chosen position is occupied by another building");
            if (!board.HasNoNeighbors(firstTile, secondTile, thirdTile))
                throw new IllegalBuildPositionException("The chosen position violates the distance rule");
            if (!board.CanBuildPiece(firstTile, secondTile, thirdTile))
                throw new IllegalBuildPositionException("The chosen position is not valid");

            player.SettlementsLeft--;
            board = board.PlacePiece(firstTile, secondTile, thirdTile, new Board.Piece(Token.Settlement, player.Id));
            return CurrentGamestate();
        }

        /// <summary>
        /// Let a player build a road without a requirement for connectivity and free of charge
        /// The road may not be placed on top of another road
        /// Connectivity with previously placed settlement should be controlled elsewhere (in StartActions.cs)
        /// </summary>
        /// <param name="player">The player placing the road</param>
        /// <param name="firstTile">The index of the first tile that the road will be along</param>
        /// <param name="secondTile">The index of the second tile that the road will be along</param>
        public GameState BuildFirstRoad(Player player, int firstTile, int secondTile)
        {
            if (board.GetRoad(firstTile, secondTile) != -1)
                throw new IllegalBuildPositionException("The chosen position is occupied by another road");
            if (!board.CanBuildRoad(firstTile, secondTile))
                throw new IllegalBuildPositionException("The chosen position is not valid");

            player.RoadsLeft--;
            board = board.PlaceRoad(firstTile, secondTile, player.Id);
            return CurrentGamestate();
        }

        /// <summary>
        /// Trade resources with the bank
        /// If no harbor is owned trading is 4 : 1
        /// If a general harbor is owned trading is 3 : 1
        /// Special harbors trade a specific resource 2 : 1
        /// </summary>
        /// <param name="player">The player wanting to trade</param>
        /// <param name="giving">The resource that the player want to give</param>
        /// <param name="receiving">The resource that the player want to receive</param>
        /// <returns>The updated gamestate after trading</returns>
        public GameState TradeBank(Player player, Resource giving, Resource receiving)
        {
            if (resourceBank[(int)receiving] == 0)
                throw new NoMoreCardsException("Resource bank has no more resources of type " + receiving);

            var harbors = board.GetPlayersHarbors(player.Id);
            var hasSpecific = harbors.Contains((HarborType) giving);
            var hasGeneral = harbors.Contains(HarborType.ThreeForOne);

            var amountToGive = (hasSpecific) ? 2 : ((hasGeneral) ? 3 : 4);

            if (player.Resources.Count(r => r == receiving) < amountToGive)
                throw new InsufficientResourcesException("Player hasn't got enough resources to trade");
            
            PayResource(player,giving,amountToGive);
            GetResource(player,receiving);

            log.Add(new TradeBankLogEvent(player.Id,giving,amountToGive,receiving));

            return CurrentGamestate();
        }
    }
}
