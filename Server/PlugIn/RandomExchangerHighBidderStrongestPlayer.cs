using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;

namespace PlugIn
{
    public class RandomExchangerHighBidderStrongestPlayer : LegoPlayer
    {
        public RandomExchangerHighBidderStrongestPlayer()
            :base(new HighBidder(), new PlugIn.CardExchangers.RandomCardExchanger(), new PlugIn.Gamers.StrongerCardGamer())
        {
        }

        public override string Name
        {
            get { return "The phantom"; }
        }
    }
}
