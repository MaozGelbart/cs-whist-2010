using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;
using PlugIn.Bidders;

namespace PlugIn
{
    class Brainiac : LegoPlayer
    {
        public Brainiac()
            : base(new SmartBidder(), new PlugIn.CardExchangers.MaximizeTricksBidCardExchanger(), new PlugIn.Gamers.SmartGamer())
        {
        }

        public override string Name
        {
            get { return "Brainiac"; }
        }
    }
}
