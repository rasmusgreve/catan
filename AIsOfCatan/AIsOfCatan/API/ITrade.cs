using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIsOfCatan
{
    public interface ITrade
    {
        /// <summary>
        /// The proposed list of resources that you should give in this trade
        /// The format is a list of lists of resources. This is to be interpreted
        /// as the opponent wanting you to give either all of Give[0] or all of Give[1] ... for what is in Take
        /// </summary>
        List<List<Resource>> Give { get; }
        
        /// <summary>
        /// The proposed list of resources that you should take in this trade
        /// The format is a list of lists of resources. This is to be interpreted
        /// as the opponent wanting you to take either all of Take[0] or all of Take[1] ... for what is in Give
        /// </summary>
        List<List<Resource>> Take { get; }

        /// <summary>
        /// Get the status of the trade
        /// </summary>
        TradeStatus Status { get; }

        /// <summary>
        /// To accept the offer, choose a sublist from Give and Take and put in this method
        /// To counter the offer, build new lists of resources (or expanding on the existing) and put in this method
        /// </summary>
        /// <param name="give">A list of resources you want to trade away</param>
        /// <param name="take">A list of resources you want to receive in the trade</param>
        /// <returns>The accepted/countered offer (should be the returned value of your implementation of the HandleTrade method)</returns>
        ITrade Respond(List<Resource> give, List<Resource> take);
        
        /// <summary>
        /// Decline the trade without making a counteroffer (the opponent might still initiate another trade proposal)
        /// </summary>
        /// <returns>The declined offer (should be the returned value of your implementation of the HandleTrade method)</returns>
        ITrade Decline();
    }
}
