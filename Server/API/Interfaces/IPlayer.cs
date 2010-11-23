using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    /// <summary>
    /// The main interface must be implemented to create a humen or AI player.
    /// MUST HAVE AN ARGUMENT-LESS CONSTRUCTOR if to be created from PlayerFactory
    /// </summary>
    public interface IPlayer : IPlayerBase, ICardExchanger, IBidder, IGamer
    {
    }
}
