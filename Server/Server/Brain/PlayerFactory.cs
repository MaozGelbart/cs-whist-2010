using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.Clients;
using System.Reflection;
using System.IO;
using Server.API;

namespace Brain
{
    public class PlayerFactory
    {
        private static Dictionary<string, PlugInInfo> playerPlugIns = new Dictionary<string, PlugInInfo>();
        private static Dictionary<string, string> playerPlugInsDescription = new Dictionary<string, string>();
        private static bool isInit = false;
        public static void RegisterAllPlugIns()
        {
            isInit = true;
            AddPlugIn("dumb", typeof(DumbPlayer));
            AddPlugIn("higherBidderDumb", typeof(HighBidder));
            string[] files = Directory.GetFiles("C:\\projects\\Whist01\\playerPlugIns", "*.dll");
            foreach(string file in files)
            {
                Assembly assm = System.Reflection.Assembly.LoadFile(file);
                var types = (from t in assm.GetTypes()
                            where t.GetInterface("Server.API.IPlayer") != null && t.IsClass && !t.IsAbstract
                            select t).ToArray();
                foreach (Type t in types)
                {
                    AddPlugIn(t.ToString(), t);
                }
            }
        }

        private static void AddPlugIn(string id, Type t)
        {
            try
            {
                ConstructorInfo cons = t.GetConstructor(new Type[0]);
                IPlayer ply = (IPlayer)cons.Invoke(new object[0]);
                PlugInInfo info = new PlugInInfo
                {
                    ID = id,
                    Name = ply.Name,
                    Constr = cons
                };
                playerPlugIns.Add(id, info);
            }
            catch { }
        }

        public static WCFServer.PlayerPlugin[] GetPlayerPlugIns()
        {
            if (!isInit)
                RegisterAllPlugIns();
            return (from p in playerPlugIns.Values
                    select new WCFServer.PlayerPlugin { ID = p.ID, Name = p.Name }).ToArray();
        }

        public static Server.API.IPlayer CreatePlayer(string player_AI_code)
        {
            if (!isInit)
                RegisterAllPlugIns();
            PlugInInfo info = playerPlugIns[player_AI_code];
            if (info != null)
            {
                return (Server.API.IPlayer)info.Constr.Invoke(new object[0]);
            }
            else
                return null;
        }
    }

    class PlugInInfo
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public ConstructorInfo Constr { get; set; }

    }
}