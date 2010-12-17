using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    /// <summary>
    /// Inherit this class to implement card exchanger
    /// </summary>
    public abstract class CardExchangerBase : PlayerPartExtender, ICardExchanger
    {
        public abstract Card[] RequestExhangeCards();
    }
}
