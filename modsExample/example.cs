using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
namespace xCrack
{
    public class example
    {

        public static string Betweenstring(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                try
                {
                    Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                    End = strSource.IndexOf(strEnd, Start);
                    if (strSource.Substring(Start, End - Start).Length <= 0)
                        return "";
                    else
                        return strSource.Substring(Start, End - Start);
                }
                catch
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public string SimpleJson(string str, string id)
        {
            return Betweenstring(str, string.Format("\"{0}\":\"", id), "\"");
        }

        public static void log(int thread, string format, params object[] pars)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(Name() + " thread#" + thread.ToString());
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.Write(string.Format(format, pars) + "\r\n");
        }

        public static void save(string[] creds)
        {
            var saveDir = Directory.GetCurrentDirectory() + @"\xConCrkLogs";
            var currentData = DateTime.Today.ToString("dd-MM-yyyy");
            var saveFile = saveDir + string.Format(@"\{0} {1}.txt", Name(), currentData);

            Directory.CreateDirectory(saveDir);
            if (!File.Exists(saveFile))
            {
                using (StreamWriter sw = File.CreateText(saveFile))
                {
                    sw.WriteLine(string.Format("{0}:{1}", creds));
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(saveFile))
                {
                    sw.WriteLine(string.Format("{0}:{1}", creds));
                }
            }
        }

        public static string Name()
        {
            return "Example";
        }

        public static string Version()
        {
            return "1.0";
        }

        public static string Author()
        {
            return "Shadyzpop";
        }

        public bool Debug()
        {
            return false;
        }

        public bool RemoveBadProxies()
        {
            return true; // false = keep circulating proxies
        }

        #region Phase1

        public bool Phase1FollowRedirect()
        {
            return false;
        }

        public bool FirstRequestMethod()
        {
            // true = post, false = get
            return false;
        }

        public string FirstRequestData()
        {
            return "";
        }

        public string Phase1DataType()
        {
            return "";
        }

        public string FirstRequestURI()
        {
            return "https://example.com/";
        }

        public string[] FirstRequestToken(string ResponseContent, string ResponseHeaders, string ResponseCookies, string[] creds, int threadindex)
        {
            string[] Tokens = new string[] { "Token#1", "Token#2" };
            return Tokens;
        }

        #endregion

        #region Phase2

        public bool UsePhase2()
        {
            return true;
        }

        public bool GetOrPostCracking()
        {
            // true = post, false = get
            return true;
        }

        public string CrackURI()
        {
            return "https://example.com/dosomething";
        }

        public string CrackingData()
        {
            return "token1=$TOKEN[0]&token2=$TOKEN[1]"; // $TOKEN is FirstRequestToken return data
        }

        public string Phase2DataType()
        {
            return "multipart/form-data";
        }

        public string[] CrackingURIResponse(string ResponseContent, string ResponseHeaders, string ResponseCookies, string[] xTokens, string[] creds, int threadindex)
        {
            string[] res = new string[] { "" };
            if (ResponseContent.Contains("success"))
            {
                log(threadindex, "data -> {0}:{1}", creds);
                save(creds);
                res[0] = "$true";
                return res;
            }

            res[0] = "$false";
            return res;
        }

        #endregion

        #region Phase3

        public bool UseCaptureRequest()
        {
            return false;
        }

        public bool MethodCaptureURI()
        {
            // true = post, false = get
            return false;
        }

        public bool FollowRedirectCapture()
        {
            return false;
        }

        public string CaptureRequestData()
        {
            return "";
        }

        public string Phase3DataType()
        {
            return "";
        }

        public string CaptureURI()
        {
            return "";
        }

        public string[] CaptureURIResponse(string ResponseContent, string ResponseHeader, string ResponseCookies, string[] creds, int threadindex)
        {
            return new string[] { "" };
        }

        #endregion

        public bool IgnoreProtocolErrors()
        {
            return true;
        }

        public string HttpException(string ExceptionMessage, int HttpStatusCode)
        {
            // TODO
            return "";
        }

        // TODO
        public bool AutoSave()
        {
            return false;
        }

        public bool KeepCookies()
        {
            return true;
        }

        public bool UseSSL()
        {
            return true;
        }

        public bool FollowRedirect()
        {
            return false;
        }

        public string UserAgent()
        {
            return "$Chrome";
        }

        public bool UseCustomHeaders()
        {
            return true;
        }

        public string DefaultReturnFlag()
        {
            return "false";
        }

        public List<Tuple<string, string>> BeforeCrackingHeaders()
        {
            List<Tuple<string, string>> data = new List<Tuple<string, string>>();

            data.Add(new Tuple<string, string>("Accept", "*"));

            data.Add(new Tuple<string, string>("ContentEncoding", "gzip, deflate"));

            data.Add(new Tuple<string, string>("token1", "$TOKEN[0]"));

            return data;
        }

        public List<Tuple<string, string>> InCrackingHeaders()
        {
            return null;
        }

        public List<Tuple<string, string>> CaptureHeaders()
        {
            return null;
        }

        // Cookies
        public bool UseCustomCookies()
        {
            return false;
        }

        public List<Tuple<string, string>> BeforeCrackingCookies()
        {
            return null;
        }

        public List<Tuple<string, string>> InCrackingCookies()
        {
            return null;
        }

        public List<Tuple<string, string>> CaptureCookies()
        {
            return null;
        }
    }
}