using System;
using System.Collections.Generic;

namespace xConCrk
{
    class MainT
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                if (args[0] == "--help" || args[0] == "-h")
                {
                    Logger.wlfromClass("Main", "Usage: xconcrk <Threads> <ModIndex> <AccountsPath> <ProxyPathOrIp: Optional> <ProxyTypeIndex: Optional> <TestData: Optional>", ConsoleColor.White);
                    return;
                }

                if (args[0] == "list" || args[0] == "ls")
                {
                    Selector.Mods(string.Empty, true).GetAwaiter().GetResult();
                    return;
                }

                try
                {
                    List<ThreadController> Controllers = new List<ThreadController>();

                    var loadEngines = Selector.Mods(args[1]).GetAwaiter().GetResult();
                    var loadAccounts = Selector.Accounts(args[2]);

                    List<string> loadProxies = new List<string>();

                    if (args.Length > 3)
                    {
                        loadProxies = Selector.Proxies(args[3]);
                        Selector.ProxyType((ProxyTools.GetType)Convert.ToInt32(args[4]));
                    }
                    // for each engine, feed accounts/proxies, trigger a thread, keep in a list..
                    foreach(RawEngine engine in loadEngines)
                    {
                        engine.Accounts = loadAccounts;
                        if(loadProxies.Count > 0)
                        {
                            engine.Proxies = loadProxies;
                        }
                        ThreadController engineController = new ThreadController(engine);

                        engineController.TriggerThreads(Convert.ToInt32(args[0]));
                        Controllers.Add(engineController);
                    }
                    Logger.wlfromClass("Main", "All engines have been started, {0} engines in total", ConsoleColor.White, Controllers.Count);

                }
                catch (Exception ex)
                {
                    Logger.err("Invalid Arguments provided.\n Usage: xconcrk <Threads> <ModIndex/s> <AccountsPath> <ProxyPathOrIp: Optional> <ProxyTypeIndex: Optional> <TestData: Optional>");
                    Logger.err(ex.Message);
                }
            }
            else
                Logger.err("Invalid Arguments provided.\n Usage: xconcrk <Threads> <ModIndex/s> <AccountsPath> <ProxyPathOrIp: Optional> <ProxyTypeIndex: Optional> <TestData: Optional>");
        }
    }
}
