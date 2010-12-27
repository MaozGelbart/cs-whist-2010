using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlugIn.Bidders;
using Server.API;

namespace PlugIn
{
    class JustinBieber : LegoPlayer
    {
        public JustinBieber()
            : base(new JustinBidder(), new PlugIn.CardExchangers.PlayForZeroCardExchanger(), new PlugIn.Gamers.SmartGamer())
        {
        }

        public override string Name
        {
            get { return "Justin Bieber"; }
        }
    }
}
