using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Facebook;

namespace Server.facebook
{
    public partial class game2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string token = this.Request["accessToken"];
            string uid = this.Request["uid"];
            try
            {

                FacebookAPI fb = new FacebookAPI(token);
                var json = fb.Get("/me");
                initParams_SL.Attributes["value"] = "viewMode=facebook,uid=" + json.Dictionary["id"].String + ",firstName=" +  json.Dictionary["first_name"].String;
                string photo = fb.GetImageLocation("/me/picture", null);
                initParams_SL.Attributes["value"] += ",photo=" + photo;
            }
            catch (Exception ex)
            {
            }

        }
    }
}