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
        
        private Player[] players;

        private Board board;
        private List<DevelopmentCard> developmentCardStack = new List<DevelopmentCard>();
        private int[] resourceBank;
        private int turn;

        public void StartGame(Agent[] agents, int boardSeed, int diceSeed)
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
            turn = diceRandom.Next(agents.Length);
            PlaceStarts();
            GameLoop();
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

        private void GameLoop()
        {
            while (!GameFinished())
            {
                TakeTurn(players[turn].Agent);
                NextTurn();
            }
        }

        private Boolean GameFinished()
        {
            //TODO: Implement this
            throw new NotImplementedException();
        }

        private void TakeTurn(Agent player)
        {
            //player.BeforeDiceRoll(gameState); //TODO
            int roll = RollDice();
            if (roll == 7)
            {
                //player.MoveRobber(gameState); //TODO
            }
            else
            {
                //TODO: Hand out resources
                //player.PerformTurn(gameState); //TODO
            }
        }

        private void PlaceStarts()
        {
            for (int i = 0; i < players.Length; i++)
            {
               // players[turn].Agent.PlaceStart(new GameState(board, developmentCardStack, resourceBank, players, turn));//TODO
                NextTurn();
            }
            for (int i = 0; i < players.Length; i++)
            {
                PrevTurn();
                //players[turn].Agent.PlaceStart(gameState); //TODO
                //TODO: Hand out resources
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

            player.Agent.MoveRobber(new GameState(board, developmentCardStack, resourceBank, players, turn));
        }

        public bool PlayRoadBuilding(Player player, int firstTile1, int secondTile1, int firstTile2, int secondTile2)
        {
            if (!player.DevelopmentCards.Contains(DevelopmentCard.RoadBuilding)) throw new InsufficientResourcesException("No Road building found in hand");

            player.DevelopmentCards.Remove(DevelopmentCard.RoadBuilding);
            throw new NotImplementedException();
        }

        public void PlayYearOfPlenty(Player player, Resource resource1, Resource resource2)
        {
            if (resourceBank[(int)resource1] == 0) throw new NoMoreCardsException("Resource bank is out of " + resource1.ToString());
            if (resourceBank[(int)resource2] == 0) throw new NoMoreCardsException("Resource bank is out of " + resource2.ToString());
            if (!player.DevelopmentCards.Contains(DevelopmentCard.YearOfPlenty)) throw new InsufficientResourcesException("No Year of Plenty found in hand");

            player.DevelopmentCards.Remove(DevelopmentCard.YearOfPlenty);
            GetResource(player, resource1);
            GetResource(player, resource2);
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
            throw new NotImplementedException();
        }

        public bool BuildCity(Player player, int firstTile, int secondTile, int thirdTile)
        {
            throw new NotImplementedException();
        }

        public bool BuildRoad(Player player, int firstTile, int secondTile)
        {
            throw new NotImplementedException();
        }

    }
}
