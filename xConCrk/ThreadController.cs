using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xConCrk
{
    public class ThreadController
    {
        private RawEngine TargetEngine = new RawEngine();
        public ThreadController(RawEngine engine)
        {
            TargetEngine = engine;
        }

        public object softLock = new object();

        public int progress = 0;

        public bool Flag2 = true;

        public List<Thread> ThreadsList = new List<Thread>();

        public void HandleAlert()
        {
            lock(softLock)
            {
                if (Flag2)
                {
                    Flag2 = false;
                    Logger.wlfromClass("ThreadController #" + TargetEngine.Loader.modName, "All accounts has been checked, Stopping Engine..");
                }
            }
        }

        public void ThreadHandler(int threadIndex)
        {
            progress = TargetEngine.Accounts.Count;
            while (true && TargetEngine.Accounts.Count > 0)
            {
                switch (TargetEngine.flag)
                {
                    case ThreadFlags.Stop:
                        Logger.wlfromClass("ThreadController #" + TargetEngine.Loader.modName, "Stopping Engine (exception hit / ip banned)..");
                        AblivateThread(Thread.CurrentThread);
                        return;

                    case ThreadFlags.Running:
                        try
                        {
                            string accountemp = string.Empty;
                            if (TargetEngine.Accounts.Count <= 0)
                            {
                                AblivateThread(Thread.CurrentThread);
                                TargetEngine.flag = ThreadFlags.Stop;
                                return;
                            }
                            else
                            {
                                lock (TargetEngine.Accounts)
                                {
                                    int progressionTag = TargetEngine.Accounts.Count % 2 == 0 ? 4 : 5;
                                    if(progress - TargetEngine.Accounts.Count == progressionTag)
                                    {
                                        Logger.wfromClass("Progression #" + TargetEngine.Loader.modName, new string[] { string.Format("{0} Accounts left, {1} Proxies\n", TargetEngine.Accounts.Count, TargetEngine.Proxies.Count) }, new ConsoleColor[] { ConsoleColor.White });
                                        progress = TargetEngine.Accounts.Count;
                                    }
                                    accountemp = TargetEngine.Accounts[0];
                                    TargetEngine.Accounts.Remove(accountemp);
                                }

                                if (string.IsNullOrEmpty(accountemp))
                                {
                                    Thread.Sleep(200);
                                    break;
                                }


                                string proxytemp = string.Empty;
                                lock (softLock)
                                {
                                    if (ProxyTools.GT != ProxyTools.GetType.NONE && TargetEngine.Proxies.Count > 0)
                                    {
                                        proxytemp = TargetEngine.Proxies[ProxyTools.GetProxy(TargetEngine.Proxies.Count)];
                                        if (ProxyTools.GM != ProxyTools.GetMethod.Loop)
                                            TargetEngine.Proxies.Remove(proxytemp);
                                    }
                                }
                                string RequestResponse = TargetEngine.Engine(accountemp, proxytemp, threadIndex);
                                RawEngine.ResponseTypes ParsedResponse = TargetEngine.ResponseParser(RequestResponse);
                                TargetEngine.ResponseHandler(ParsedResponse, accountemp, proxytemp);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.wlfromClass("Thread #" + threadIndex.ToString() + " - #" + TargetEngine.Loader.modName, "Exception: " + ex.Message + " stack -> " + ex.StackTrace);
                            AblivateThread(Thread.CurrentThread);
                            return;
                        }
                } 
            }

            HandleAlert();
        }

        public void TriggerThreads(int threads)
        {
            lock (softLock)
            {
                Logger.wlfromClass("ThreadController #" + TargetEngine.Loader.modName, "Starting {0} Threads on {1} Accounts with {2} Proxies of type {3}", ConsoleColor.White,
                threads, TargetEngine.Accounts.Count, TargetEngine.Proxies.Count, ProxyTools.GT);

                if (threads > TargetEngine.Accounts.Count)
                {
                    threads = TargetEngine.Accounts.Count;
                }
                for (int i = 0; i < threads; i++)
                {
                    //Logger.wlfromClass("ThreadController", "Creating Thread #{0}..", ConsoleColor.White, i);
                    Thread thread = new Thread(() => { ThreadHandler(i); });
                    Thread.Sleep(500);
                    thread.Start();
                    ThreadsList.Add(thread);
                }
                Logger.wlfromClass("ThreadController #" + TargetEngine.Loader.modName, string.Format("{0} Threads Has been started.", threads));
            }
        }

        public void AblivateThread(Thread thread)
        {
            lock (ThreadsList)
            {
                ThreadsList.Remove(thread);
            }
        }

        // not in use
        public void StopThreads()
        {
            TargetEngine.flag = ThreadFlags.Stop;
            while (ThreadsList.Count > 0)
            {
                Console.WriteLine("[StopThreads] {0} threads left..", ThreadsList.Count);
                Thread.Sleep(1000);
            }
            Console.WriteLine("[StopThreads] All Threads Stopped.");
        }

        public enum ThreadFlags
        {
            Start,
            Stop,
            Running,
            Error
        }
    }
}
