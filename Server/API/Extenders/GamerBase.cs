using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    /// <summary>
    /// Inherit this class to implement gamer part
    /// </summary>
    public abstract class GamerBase : PlayerPartExtender, IGamer
    {
        public abstract Card RequestPlay();
    }
}
