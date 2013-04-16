using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    /// <summary>
    /// Data object containing information about a player in the game
    /// </summary>
    public class Player
    {
        public Player(Agent agent, int id)
        {
            agent.Reset(id);

            RoadsLeft = 15; //You start with 15 roads
            CitiesLeft = 4; //          - and 4 cities
            SettlementsLeft = 5; //     - and 5 settlements, according to (rules p. 2 - Game Contents)
            PlayedKnights = 0;

            Agent = agent;
            Id = id;
            DevelopmentCards = new List<DevelopmentCard>();
            Resources = new List<Resource>();
        }

        /// <summary>
        /// How many knights has this player played in this game so far
        /// </summary>
        public int PlayedKnights { get; set; }

        /// <summary>
        /// How many road pieces does this player have left to place
        /// </summary>
        public int RoadsLeft { get; set; }
        
        /// <summary>
        /// How many city pieces does this player have left to place
        /// </summary>
        public int CitiesLeft { get; set; }

        /// <summary>
        /// How many settlement pieces does this player have left to place
        /// </summary>
        public int SettlementsLeft { get; set; }
        
        /// <summary>
        /// The agent deciding actions for this player
        /// </summary>
        public Agent Agent { get; private set; }

        /// <summary>
        /// The Id of this player
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The development cards that this player has in his hand
        /// </summary>
        public List<DevelopmentCard> DevelopmentCards { get; private set; }

        /// <summary>
        /// The resource cards that this player has in his hand
        /// </summary>
        public List<Resource> Resources { get; private set; }

    }
}
