using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public class GameController
    {
        private Random diceRandom;
        private Random shuffleRandom;
        //TODO: http://www.random.org/integers/?num=200&min=1&max=6&col=2&base=10&format=plain&rnd=new
        private Player[] players;

        private Board board;
        private List<DevelopmentCard> developmentCardStack = new List<DevelopmentCard>();
        private int[] resourceBank;
        private int turn;
        private int largestArmySize = 2; //One less than the minimum for getting the largest army card
        private int largestArmyID = -1;
        private int longestRoadID = -1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agents"></param>
        /// <param name="boardSeed"></param>
        /// <param name="diceSeed"></param>
        /// <returns>The id of the winner of the game</returns>
        public int StartGame(Agent[] agents, int boardSeed, int diceSeed)
        {
            //Build player list
            this.players = new Player[agents.Length];
            for (int i = 0; i < agents.Length; i++)
            {
                this.players[i] = new Player(agents[i], i);
            }

            //Set up board
            board = new Board(boardSeed);
            PopulateDevelopmentCardStack();
            resourceBank = new int[] { 19, 19, 19, 19, 19 };

            //Initialize random number generators
            diceRandom = new Random(diceSeed);
            shuffleRandom = new Random(boardSeed); //The card deck is based on the seed of the board
            
            //Start the game!
            turn = 0; //TODO: Shuffle agents array? !IMPORTANT! DO IT BEFORE THE ID's ARE ASSIGNED!
            PlaceStarts();
            return GameLoop();
        }

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
            for (int n = developmentCardStack.Count; n > 1; n--)
            {
                int k = shuffleRandom.Next(n + 1);
                var aux = developmentCardStack[k];
                developmentCardStack[k] = developmentCardStack[n];
                developmentCardStack[n] = aux;
            }
        }

        /// <summary>
        /// Executes the game, each player turn by turn until a player wins
        /// </summary>
        /// <returns>The ID of the winning player</returns>
        private int GameLoop()
        {
            while (true)
            {
                TakeTurn(players[turn]);
                
                if (HasWon(players[turn])) return players[turn].ID;

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
            
            if (player.ID == largestArmyID) points += 2;
            if (player.ID == longestRoadID) points += 2;
            
            return (points >= 10);
        }

        /// <summary>
        /// Executes all parts of a players turn
        ///     1. Allow the play of a development card before the dice are rolled
        ///     2. Roll the dice (done automatically), hand out resources to all players, and move the robber if roll is 7
        ///     3. Allow all actions according to the rules
        /// </summary>
        /// <param name="player"></param>
        private void TakeTurn(Player player)
        {
            MainActions actions = new MainActions(player, this);
            player.Agent.BeforeDiceRoll(new GameState(board, developmentCardStack, resourceBank, players, turn), actions);
            
            int roll = RollDice();
            actions.DieRoll();
            
            if (roll == 7)
            {
                MoveRobber(player, new GameState(board, developmentCardStack, resourceBank, players, turn));
                //TODO: Discard if over 7 cards
            }
            else
            {
                HandOutResources(roll);
            }
            GameState afterResourcesState = new GameState(board, developmentCardStack, resourceBank, players, turn);
            player.Agent.PerformTurn(afterResourcesState, actions);
        }

        private void MoveRobber(Player player, GameState gameState)
        {
            int robberPosition = player.Agent.MoveRobber(gameState);
            if (board.GetTile(robberPosition).Terrain == Terrain.Water || robberPosition == board.GetRobberLocation())
            {
                Console.WriteLine("Agent " + player.Agent.GetType().Name + " moved robber illegally");
                return;
            }
            board = board.MoveRobber(robberPosition);

            //Draw a card from an opponent
            var opponents = new List<int>();
            foreach (var piece in board.GetPieces(robberPosition))
            {
                if (piece.Player == player.ID) continue;
                if (opponents.Contains(piece.Player)) continue;
                opponents.Add(piece.Player);
            }
            int choice = player.Agent.ChooseOpponentToDrawFrom(opponents.ToArray());
            if (!opponents.Contains(choice)) //If the agent gives a bad answer, we ask again
            {
                Console.WriteLine("Agent " + player.Agent.GetType().Name + " chose an illegal player to draw from");
                return;
            }

            if (players[choice].Resources.Count == 0) return; //Nothing to take
            
            //Move a card from one player to another
            var position = shuffleRandom.Next(players[choice].Resources.Count);
            var toMove = players[choice].Resources[position];
            players[choice].Resources.RemoveAt(position);
            player.Resources.Add(toMove);
        }

        private void HandOutResources(int roll)
        {
            //Map from PlayerID to dictionary that maps resource to amount
            Dictionary<int, Dictionary<Resource, int>> handouts = new Dictionary<int, Dictionary<Resource, int>>();
            Dictionary<Resource, int> handoutSums = new Dictionary<Resource, int>();
            for (int i = 0; i < players.Length; i++) handouts[i] = new Dictionary<Resource,int>();

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
                foreach (var resource in handouts[player.ID].Keys)
                {
                    GetResource(player, resource, handouts[player.ID][resource]);
                }
            }
        }

        private void PlaceStarts()
        {
            for (int i = 0; i < players.Length; i++)
            {
                var state = new GameState(board, developmentCardStack, resourceBank, players, turn);
                var actions = new StartActions(players[turn], this);
                players[turn].Agent.PlaceStart(state, actions);
                NextTurn();
            }
            for (int i = 0; i < players.Length; i++)
            {
                PrevTurn();
                var state = new GameState(board, developmentCardStack, resourceBank, players, turn);
                var actions = new StartActions(players[turn], this);
                players[turn].Agent.PlaceStart(state, actions);
                //Hand out resources
                foreach (var pos in actions.GetHousePosition())
                {
                    var type = (Resource)board.GetTile(pos).Terrain;
                    GetResource(players[turn], type);
                }
            }
        }

        private int RollDice()
        {
            return diceRandom.Next(6) + diceRandom.Next(6) + 2;
        }

        private void NextTurn()
        {
            turn = (turn + 1) % players.Length;
        }

        private void PrevTurn()
        {
            turn = turn - 1;
            if (turn < 0) turn += players.Length;
        }

        private void PayResource(Player player, Resource resource, int quantity = 1)
        {
            for (int i = 0; i < quantity; i++)
            {
                player.Resources.Remove(resource);
                resourceBank[(int)resource]++;
            }
        }

        private void GetResource(Player player, Resource resource, int quantity = 1)
        {
            for (int i = 0; i < quantity; i++)
            {
                if (resourceBank[(int)resource] == 0) throw new NoMoreCardsException("Resource bank is out of " + resource.ToString());
                player.Resources.Add(resource);
                resourceBank[(int)resource]--;
            }
        }

        /*
         * ACTIONS
         */

        public DevelopmentCard DrawDevelopmentCard(Player player)
        {
            var r = player.Resources;
            if (!(r.Contains(Resource.Grain) && r.Contains(Resource.Wool) && r.Contains(Resource.Ore)))
                throw new InsufficientResourcesException("Not enough resources to buy a development card");

            if (developmentCardStack.Count == 0)
                throw new NoMoreCardsException("DevelopmentCardStack is empty");

            PayResource(player, Resource.Grain);
            PayResource(player, Resource.Wool);
            PayResource(player, Resource.Ore);

            var last = developmentCardStack.Last();
            developmentCardStack.RemoveAt(developmentCardStack.Count);
            return last;
        }

        public Trade[] ProposeTrade(Player player, Trade trade)
        {
            throw new NotImplementedException();
        }

        public void CompleteTrade(Player player, int playerid)
        {
            throw new NotImplementedException();
        }

        public void PlayKnight(Player player)
        {
            if (!player.DevelopmentCards.Contains(DevelopmentCard.Knight))
                throw new InsufficientResourcesException("No knight found in hand");

            player.DevelopmentCards.Remove(DevelopmentCard.Knight);
            player.PlayedKnights++;
            if (player.PlayedKnights > largestArmySize)
            {
                largestArmySize = player.PlayedKnights;
                largestArmyID = player.ID;
            }

            MoveRobber(player, new GameState(board, developmentCardStack, resourceBank, players, turn));
        }

        public void PlayRoadBuilding(Player player, int firstTile1, int secondTile1, int firstTile2, int secondTile2)
        {
            if (!player.DevelopmentCards.Contains(DevelopmentCard.RoadBuilding)) throw new InsufficientResourcesException("No Road building found in hand");

            player.DevelopmentCards.Remove(DevelopmentCard.RoadBuilding);

            if (player.RoadsLeft >= 2)
            {
                player.RoadsLeft--;
                if (board.GetRoad(firstTile2, secondTile2) != -1)
                    throw new IllegalBuildPositionException("There is already a road on the selected position");
                board = board.PlaceRoad(firstTile2, secondTile2, player.ID);
            }
            if (player.RoadsLeft >= 1)
            {
                player.RoadsLeft--;
                if (board.GetRoad(firstTile1, secondTile1) != -1)
                    throw new IllegalBuildPositionException("There is already a road on the selected position");
                board = board.PlaceRoad(firstTile1, secondTile1, player.ID);
            }
            else
            {
                throw new IllegalActionException("No more road pieces left of your color");
            }
        }

        public void PlayYearOfPlenty(Player player, Resource resource1, Resource resource2)
        {
            if (resourceBank[(int)resource1] == 0) throw new NoMoreCardsException("Resource bank is out of " + resource1.ToString());
            if (resourceBank[(int)resource2] == 0) throw new NoMoreCardsException("Resource bank is out of " + resource2.ToString());
            if (!player.DevelopmentCards.Contains(DevelopmentCard.YearOfPlenty)) throw new InsufficientResourcesException("No Year of Plenty found in hand");

            player.DevelopmentCards.Remove(DevelopmentCard.YearOfPlenty);
            GetResource(player, resource1);
            GetResource(player, resource2);
            //TODO: What if no more resources in bank?
        }

        public GameState PlayMonopoly(Player player, Resource resource)
        {
            if (!player.DevelopmentCards.Contains(DevelopmentCard.Monopoly)) throw new InsufficientResourcesException("No Monopoly in hand");
            player.DevelopmentCards.Remove(DevelopmentCard.Monopoly);
            //Take all resources of the given type out of all hands
            int count = 0;
            for (int i = 0; i < players.Length; i++)
            {
                count += players[i].Resources.RemoveAll(c => c == resource);
            }
            //And place them in the playing players hand
            for (int i = 0; i < count; i++)
            {
                player.Resources.Add(resource);
            }
            return new GameState(board, developmentCardStack, resourceBank, players, turn);
        }

        public bool BuildHouse(Player player, int firstTile, int secondTile, int thirdTile)
        {
            var r = player.Resources;
            if (!(r.Contains(Resource.Grain) && r.Contains(Resource.Wool) && r.Contains(Resource.Brick) && r.Contains(Resource.Lumber)))
                throw new InsufficientResourcesException("Not enough resources to buy a settlement");
            if (player.SettlementsLeft == 0)
                throw new IllegalActionException("No more settlement pieces left of your color");

            PayResource(player, Resource.Grain);
            PayResource(player, Resource.Wool);
            PayResource(player, Resource.Brick);
            PayResource(player, Resource.Lumber);

            player.SettlementsLeft--;
            //TODO: Build house
            throw new NotImplementedException();
        }

        public bool BuildCity(Player player, int firstTile, int secondTile, int thirdTile)
        {
            var r = player.Resources;
            if (!(r.Count(c => c == Resource.Ore) >= 3 && r.Count(c => c == Resource.Grain) >= 2))
                throw new InsufficientResourcesException("Not enough resources to buy a city");
            if (player.CitiesLeft == 0)
                throw new IllegalActionException("No more city pieces left of your color");

            PayResource(player, Resource.Ore, 3);
            PayResource(player, Resource.Grain, 2);

            player.CitiesLeft--;
            player.SettlementsLeft++;
            //TODO: Build city
            throw new NotImplementedException();
        }

        public bool BuildRoad(Player player, int firstTile, int secondTile)
        {
            var r = player.Resources;
            if (!(r.Contains(Resource.Brick) && r.Contains(Resource.Lumber)))
                throw new InsufficientResourcesException("Not enough resources to buy a road");
            if (player.RoadsLeft == 0)
                throw new IllegalActionException("No more road pieces left of your color");

            PayResource(player, Resource.Brick);
            PayResource(player, Resource.Lumber);

            player.RoadsLeft--;
            //TODO: Build road
            throw new NotImplementedException();
        }

    }
}
