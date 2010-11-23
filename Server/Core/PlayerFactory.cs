using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;
using Server.API;

namespace Brain
{
    public class PlayerFactory
    {
        const string PLUGINS_DLLS_FOLDER_PATH = "C:\\projects\\Whist01\\playerPlugIns";
        private static Dictionary<string, PlugInInfo> playerPlugIns = new Dictionary<string, PlugInInfo>();
        private static Dictionary<string, string> playerPlugInsDescription = new Dictionary<string, string>();
        private static bool isInit = false;
        public static void RegisterAllPlugIns()
        {
            isInit = true;
            string[] files = Directory.GetFiles(PLUGINS_DLLS_FOLDER_PATH, "*.dll");
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
                if (ply != null)
                {
                    PlugInInfo info = new PlugInInfo
                    {
                        ID = id,
                        Name = ply.Name,
                        Constr = cons
                    };
                    playerPlugIns.Add(id, info);
                }
            }
            catch { }
        }

        public static PlayerPlugin[] GetPlayerPlugIns()
        {
            if (!isInit)
                RegisterAllPlugIns();
            return (from p in playerPlugIns.Values
                    select new PlayerPlugin { ID = p.ID, Name = p.Name }).ToArray();
        }

        public static Server.API.IAsyncPlayer CreatePlayer(string player_AI_code)
        {
            if (!isInit)
                RegisterAllPlugIns();
            PlugInInfo info = playerPlugIns[player_AI_code];
            if (info != null)
            {
                IPlayer op = (Server.API.IPlayer)info.Constr.Invoke(new object[0]);
                return new SyncPlayerAdaptor(op);
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