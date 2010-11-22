using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;

namespace PlugIn.CardExchangers
{
    internal class RandomCardExchanger : PlayerPartExtender<ICardExchanger>, ICardExchanger
    {
        #region ICardExchanger Members

        public void RequestExhangeCards()
        {
            List<Card> given = Cards.GetRange(0, 3);
            if( OnGetExchangedCardsCompleted != null)
                OnGetExchangedCardsCompleted(this, new RecieveCardsEventArgs(given.ToArray()));
        }

        public event EventHandler<RecieveCardsEventArgs> OnGetExchangedCardsCompleted;

        #endregion
    }
}
