using System;
using System.Collections.Generic;
using System.IO;

namespace xConCrk
{
    public static class IOHandler
    {
        public static List<string> loadAccounts(string path)
        {
            List<string> temp = new List<string>();
            bool flag = path.Contains(":\\") || path.Contains(":/");
            try
            {
                foreach (string account in File.ReadAllLines(flag ? path : Directory.GetCurrentDirectory() + "\\" + path))
                {
                    temp.Add(account);
                }
                Logger.wlfromClass("IOHandler", "Successfully loaded " + temp.Count + " Accounts.");
                return temp;
                //if (Directory.Exists(@"E:\MyEnvBin\cracked\"))
                //{
                //    foreach (string account in File.ReadAllLines(flag ? @"E:\MyEnvBin\cracked\" + path : Directory.GetCurrentDirectory() + "\\" + path))
                //    {
                //        temp.Add(account);
                //    }
                //    Logger.wlfromClass("IOHandler", "Successfully loaded " + temp.Count + " Accounts From env.");

                //    return temp;
                //}
                //else
                //{

                //}

            }
            catch(Exception e)
            {
                Logger.err("Exception: {0}\n", e.Message);
                throw new Exception();
            }
        }

        public static List<string> loadProxies(string path)
        {
            List<string> temp = new List<string>();
            bool flag = path.Contains(":\\") || path.Contains(":/");
            try
            {
                if (Directory.Exists(@"E:\MyEnvBin\cracked\"))
                {
                    foreach (string account in File.ReadAllLines(flag ? @"E:\MyEnvBin\cracked\" + path : Directory.GetCurrentDirectory() + "\\" + path))
                    {
                        temp.Add(account);
                    }
                    Logger.wlfromClass("IOHandler", "Successfully loaded " + temp.Count + " Proxies From env.");

                    return temp;
                }
                else
                {
                    foreach (string account in File.ReadAllLines(flag ? path : Directory.GetCurrentDirectory() + "\\" + path))
                    {
                        temp.Add(account);
                    }
                    Logger.wlfromClass("IOHandler", "Successfully loaded " + temp.Count + " Proxies.");
                    return temp;
                }
            }
            catch (Exception e)
            {
                Logger.err("Exception: {0}\n", e.Message);
                throw new Exception();
            }
        }
    }
}
