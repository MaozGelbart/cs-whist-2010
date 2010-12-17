using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;
using PlugIn.Bidders;

namespace PlugIn
{
    class DeepHotMagenta: LegoPlayer
    {
        public DeepHotMagenta()
            :base(new SmartBidder(), new PlugIn.CardExchangers.MaximizeTricksBidCardExchanger(), new PlugIn.Gamers.StrongerCardGamer())
        {
        }

        public override string Name
        {
            get { return "DeepHotMagenta"; }
        }
    }
}
