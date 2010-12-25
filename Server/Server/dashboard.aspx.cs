using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Brain;

namespace Server
{
    public partial class dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var games = (from g in GameFactory.Games
                         select new
                         {
                             Name = g.Name,
                             Player1Name = g.GetGameStatus().PlayerNames[0],
                             Player2Name = g.GetGameStatus().PlayerNames[1],
                             Player3Name = g.GetGameStatus().PlayerNames[2],
                             Player4Name = g.GetGameStatus().PlayerNames[3],
                             CurrentRound = g.GetGameStatus().RoundNumber + 1,
                             StartedAt = g.TimeStarted
                         }).ToArray();
            gv_games.DataSource = games;
            gv_games.DataBind();
        }
    }
}