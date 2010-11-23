using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;

namespace PlugIn.Gamers
{
    class StrongerCardGamer : GamerBase
    {
        #region IPlayerGamer Members

        public override Card RequestPlay()
        {
            Suit? strongSuit = this.CurrentRoundStatus.Trump;
            Suit? turnSuit = null;
            if (this.CurrentRoundStatus.LeadingPlayer > 0)
                turnSuit = this.CurrentRoundStatus.CurrentPlay[this.CurrentRoundStatus.LeadingPlayer].Value.Suit;

            Card chosen = default(Card);
            if (turnSuit.HasValue)
            {
                chosen = (from c in Cards
                          where c.Suit == turnSuit.Value
                          orderby c.Value descending
                          select c).FirstOrDefault();
            }
            if (chosen.IsEmpty())
            {
                chosen = (from c in Cards
                          where c.Suit == strongSuit
                          orderby c.Value descending
                          select c).FirstOrDefault();
            }
            if (chosen.IsEmpty())
            {
                chosen = (from c in Cards
                          orderby c.Value descending
                          select c).First();
            }
            return chosen;
        }

        #endregion
    }
}
