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
        private int playedKnights = 0;

        public Player(Agent agent, int id)
        {
            this.agent = agent;
            agent.Reset(id);
            this.ID = id;
        }

        public int PlayedKnights
        {
            get
            {
                return playedKnights;
            }
            set
            {
                playedKnights = value;
            }
        }

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
