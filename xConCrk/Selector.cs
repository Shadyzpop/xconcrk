using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shadynet;

namespace xConCrk
{
    public static class Selector
    {
        public static async Task<List<RawEngine>> Mods(string preloads, bool list = false)
        {
            if (list)
            {
                var temp = new RawEngine();
                await temp.startModsLoader(0, list);
                return new List<RawEngine>();
            }
            List<RawEngine> res = new List<RawEngine>();
            if (preloads.Contains('[') && preloads.Contains(','))
            {
                string rawModsArg = Helper.Betweenstring(preloads, "[", "]");
                string[] allModsArg = rawModsArg.Split(',');
                foreach(string premod in allModsArg)
                {
                    try
                    {
                        var engine = new RawEngine();
                        await engine.startModsLoader(Convert.ToInt32(premod));
                        res.Add(engine);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                }
            }
            else
            {
                try
                {
                    var eng = new RawEngine();
                    await eng.startModsLoader(Convert.ToInt32(preloads));
                    res.Add(eng);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
            }

            return res;
        }

        public static List<string> Accounts(string path)
        {
            try
            {
                return IOHandler.loadAccounts(path);
            }
            catch (Exception e) { Logger.err("Exception: {0}", e.Message); return new List<string>(); }
        }

        public static List<string> Proxies(string path)
        {
            try
            {
                return IOHandler.loadProxies(path);
            }
            catch (Exception e) { Logger.err("Exception: {0}", e.Message); return new List<string>(); }
        }

        public static void ProxyType(ProxyTools.GetType PreType = ProxyTools.GetType.NONE)
        {
            if(PreType != ProxyTools.GetType.NONE)
            {
                ProxyTools.GT = PreType;
                return;
            }
            //0http 1socks
            Logger.wfromClass("Selector", new String[] { "Select Proxy type:" }, new ConsoleColor[] { ConsoleColor.White });
            int i = 0;
            foreach (var type in Enum.GetValues(typeof(ProxyTools.GetType)))
            {
                Logger.w(new string[] { i.ToString(), ") ", type.ToString() + "\n" }, new ConsoleColor[] { ConsoleColor.Yellow, ConsoleColor.White, ConsoleColor.White });
            }
            Console.Write("Your Pick: ");

            try
            {
                int xpick = Convert.ToInt32(Console.ReadLine());
                ProxyTools.GT = (ProxyTools.GetType)xpick;
                Logger.wlfromClass("Selector", "Selected " + (xpick == 0 ? "HTTP" : "SOCKS") + " Proxy type");
            }
            catch(Exception e)
            {
                Logger.err("Exception: {0}", e.Message);
            }
        }
    }
}
