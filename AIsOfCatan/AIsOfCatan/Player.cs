using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public class Player
    {
        private Agent agent;
        public readonly int ID;
        private List<Resource> resources = new List<Resource>();
        private List<DevelopmentCard> developmentCards = new List<DevelopmentCard>();
        
        public Player(Agent agent, int id)
        {
            this.agent = agent;
            agent.Reset(id);
            this.ID = id;
            PlayedKnights = 0;
            RoadsLeft = 15;
            SettlementsLeft = 5;
            CitiesLeft = 4;
        }

        public int PlayedKnights { get; set; }
        public int RoadsLeft { get; set; }
        public int SettlementsLeft { get; set; }
        public int CitiesLeft { get; set; }

        public List<DevelopmentCard> DevelopmentCards
        {
            get
            {
                return developmentCards;
            }
        }

        public List<Resource> Resources
        {
            get
            {
                return resources;
            }
        }

        public Agent Agent
        {
            get
            {
                return agent;
            }
        }
    }
}
