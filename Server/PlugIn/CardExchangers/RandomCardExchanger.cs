using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;

namespace PlugIn.CardExchangers
{
    internal class RandomCardExchanger : CardExchangerBase
    {
        #region ICardExchanger Members

        public override Card[] RequestExhangeCards()
        {
            return Cards.GetRange(0, 3).ToArray();
        }

        #endregion
    }
}
