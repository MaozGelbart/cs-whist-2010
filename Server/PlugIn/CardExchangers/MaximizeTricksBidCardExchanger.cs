using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;

//strategy based on http://www.myspades.com/bidding-strategy.php

namespace PlugIn.CardExchangers
{
    class MaximizeTricksBidCardExchanger : CardExchangerBase
    {
        public override Card[] RequestExhangeCards()
        {
            double highestScore = 0;
            Card[] highestScoreCards = new Card[3];

            //check all sub-groups of 10 cards in cards
            for (int i = 2; i < 13; i++)
            {
                for (int j = 1; j < i; j++)
                {
                    for (int k = 0; k < j; k++)
                    {
                        //all suits
                        for (int s = 1; s <= 4; s++)
                        {
                            //create the collection of cards wanted to check
                            ICollection<Card> cards_cpy = new HashSet<Card>(Cards);
                            cards_cpy.Remove(Cards[i]);
                            cards_cpy.Remove(Cards[j]);
                            cards_cpy.Remove(Cards[k]);

                            //check the score of current collection
                            double score = GetHighestBidForTrump((Suit)s, cards_cpy);

                            //score is better?
                            if (score > highestScore)
                            {
                                //save new items
                                highestScoreCards[0] = Cards[i];
                                highestScoreCards[1] = Cards[j];
                                highestScoreCards[2] = Cards[k];
                                highestScore = score;
                            }
                        }
                    }
                }
            }

            return highestScoreCards;
        }
    }
}
