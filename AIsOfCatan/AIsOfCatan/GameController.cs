using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    class GameController
    {
        private int turn;
        private Random dieRandom;
        private GameState gameState;
        private Agent[] players;
        public void StartGame(Agent[] players, int boardSeed, int dieSeed)
        {
            this.players = players;
            dieRandom = new Random(dieSeed);
            gameState = new GameState(new Board(boardSeed),null,null,null,0);
            turn = dieRandom.Next(players.Length);

            PlaceStarts();
            GameLoop();
        }

        private void GameLoop()
        {
            while (!GameFinished())
            {
                TakeTurn(players[turn]);
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
            int roll = Roll();
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
                players[turn].PlaceStart(gameState);
                NextTurn();
            }
            for (int i = 0; i < players.Length; i++)
            {
                PrevTurn();
                players[turn].PlaceStart(gameState);
                //TODO: Hand out resources
            }
        }

        private int Roll()
        {
            return dieRandom.Next(6) + dieRandom.Next(6) + 2;
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
