using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    class GameController
    {
        private int turn;
        private Random diceRandom;
        private Random shuffleRandom;
        private GameState gameState;
        private Player[] players;

        private List<DevelopmentCard> developmentCardStack = new List<DevelopmentCard>();

        public void StartGame(Agent[] agents, int boardSeed, int diceSeed)
        {
            this.players = new Player[agents.Length];
            for (int i = 0; i < agents.Length; i++)
            {
                this.players[i] = new Player(agents[i], i);
            }
            diceRandom = new Random(diceSeed);
            shuffleRandom = new Random(boardSeed); //the card deck is based on the board
            gameState = new GameState(boardSeed);
            turn = diceRandom.Next(agents.Length);

            PlaceStarts();
            GameLoop();
        }
        //                            14      5             2             2             2
//        public enum DevelopmentCard { Knight, VictoryPoint, RoadBuilding, YearOfPlenty, Monopoly }

        private void PopulateDevelopmentCardStack()
        {
            developmentCardStack.Clear(); //Just to be sure

            for (int i = 0; i < 14; i++) developmentCardStack.Add(DevelopmentCard.Knight);
            for (int i = 0; i < 5; i++) developmentCardStack.Add(DevelopmentCard.VictoryPoint);
            for (int i = 0; i < 2; i++)
            {
                developmentCardStack.Add(DevelopmentCard.RoadBuilding);
                developmentCardStack.Add(DevelopmentCard.YearOfPlenty);
                developmentCardStack.Add(DevelopmentCard.Monopoly);
            }

            //Shuffle!

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
            player.BeforeDieRoll(gameState);
            int roll = RollDice();
            if (roll == 7)
            {
                player.MoveRobber(gameState);
            }
            else
            {
                //TODO: Hand out resources
                player.PerformTurn(gameState);
            }
        }

        private void PlaceStarts()
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[turn].Agent.PlaceStart(gameState);
                NextTurn();
            }
            for (int i = 0; i < players.Length; i++)
            {
                PrevTurn();
                players[turn].Agent.PlaceStart(gameState);
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
    }
}
