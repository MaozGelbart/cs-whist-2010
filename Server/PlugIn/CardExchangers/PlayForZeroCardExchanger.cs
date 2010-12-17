using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;

namespace PlugIn.CardExchangers
{
    class PlayForZeroCardExchanger : CardExchangerBase
    {
        #region ICardExchanger Members

        public override Card[] RequestExhangeCards()
        {
            List<Card> retCards = new List<Card>();

            //arrange he cards by suits
            List<Card>[] suitsCount = ArrangeCardBySuits();            

            //look at each suit which has 2 cards or less and add those cards to the returned array
            foreach (List<Card> suit in suitsCount)
            {
                if (suit.Count <= 2)
                { 
                    retCards.AddRange(suit);
                }
            }

            //see how many cards are in the returned array
            if (retCards.Count == 3)
            {
                return retCards.ToArray();
            }
            else if (retCards.Count > 3)
            {
                //find lowest card and throw away
                while (retCards.Count > 3)
                {
                    //remove the card with the lowest value - till we have only 3 cards left
                    retCards.Remove(GetLowestCardInCollection(retCards));
                }
            }
            else if (retCards.Count < 3)
            {
                //add highest card in my hand to return array - till I have 3 cards
                while (retCards.Count < 3)
                {
                    retCards.Add(GetHighestCardInCollection(Cards));
                }
            }

            return retCards.ToArray();
        }

        #endregion
    }
}
