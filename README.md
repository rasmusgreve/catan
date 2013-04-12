catan
==========================================================

The Settlers of Catan AI Framework

----------------------------------------------------------
This project is intended as an experiment at making a
framework for competing at the game of Settlers of Catan
by Mayfair Games using AI's.

The project is made in C# using Visual Studio 2010 and
XNA 4.0. It can be found here:

http://www.microsoft.com/en-us/download/details.aspx?id=23714

http://www.microsoft.com/visualstudio/eng/downloads#d-2010-express

----------------------------------------------------------
                      RUNNING A MATCH
----------------------------------------------------------

????

----------------------------------------------------------
                          AGENTS
----------------------------------------------------------
To create an AI you must implement the Agent interface.

Each time it is your AI's turn, it will recieve an object
containing everything visible about the board and the
players, this includes:

* The board and everything on it.
* The players and what you can see without revealing
  secrets (hand count, development card count, number of
  played knights, etc.)
* Your own hand and unused development cards.
* Game log.
  
The given object is also what you'll use to perform chosen
actions in the game, this includes:

* Playing development card.
* Rolling the dice.
* Proposing a trade.
* Trading with the bank.
* Building roads, settlements and cities.
* Buying development cards.

----------------------------------------------------------
                         TRADING
----------------------------------------------------------

The trickies thing about this game compared to other board
games is that you have trades that can go back and forth.
This dynamic is solved by having the following flow in a
trade where the other AI's will get involved aswell:

1.  Current player proposes a trade (can include 
    wildcards)

2.  The other players will see the trade and can accept
   (3a) or propose a counter-offer (without wildcards)
   (3b).
   
3a. The current player can choose between all players
    who accepted the offer, or cancel the trade.
	
3b. The current player can then accept a counter-offer
    or cancel the trade.
