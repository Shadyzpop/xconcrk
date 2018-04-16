using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace xConCrk
{
    public class ModsLoader
    {
        public CodeCompiler Compile = new CodeCompiler("xCrack");

        private List<Modular> Mods = new List<Modular>();
        private List<CodeFuncProps> ModFuncs = new List<ModsLoader.CodeFuncProps>();

        public string modName = string.Empty;
        public int preloaded = 0;

        public string starterCode = String.Empty;
        public string starterCodeClass = String.Empty;

        public async Task<bool> LoadMods(int preloadMod, bool list = false)
        {
            string[] mfiles = null;

            string currentBaseDir = AppDomain.CurrentDomain.BaseDirectory;
            string currentModsDir = currentBaseDir + "mods";

            if (!Directory.Exists(currentModsDir))
            {
                Directory.CreateDirectory(currentModsDir);
            }

            mfiles = Directory.GetFiles(currentModsDir, "*.cs");

            if (mfiles == null || mfiles.Length == 0)
            {
                // Logger.wlfromClass("ModsLoader", "No Mods found, Thus the program wont work properly");
                return false;
            }
            
            for (int x = 0; x < mfiles.Length; x++)
            {
                string mfile = mfiles[x];

                StreamReader reader = new StreamReader(mfile);
                var code = reader.ReadToEnd();

                var codeclass = Path.GetFileNameWithoutExtension(mfile);
                try
                {
                    var ModName = codeclass;
                    Mods.Add(new Modular { Name = codeclass, Code = code, CodeClass = codeclass, index = x });
                }

                catch (Exception e)
                {
                    Logger.err("Couldnt Load mod -> {0} - Exception: {1}]\n", codeclass, e.Message);
                }
                reader.Close();
            }

            int selected = new int();

            if(preloadMod >= 0)
            {
                selected = preloadMod;
            }

            if (list)
            {
                for (int i = 0; i < Mods.Count; i++)
                {
                    Logger.w(
                        new String[] { i.ToString(), ") ", Mods[i].Name + "\n" },
                        new ConsoleColor[] { ConsoleColor.Yellow, ConsoleColor.White, ConsoleColor.White });
                }
                return true;
            }

            try
            {
                // Logger.wlfromClass("ModsLoader", "Selected " + Mods[selected].Name);
                this.starterCode = Mods[selected].Code;
                this.starterCodeClass = Mods[selected].CodeClass;

                return LoadSettings();
            }
            catch
            {
                Logger.err("Your Pre-selected Mod arg is unknown..\n Trying to pick again..\n\n");
                return false;
            }
            
        }

        private bool LoadSettings()
        {
            try
            {
                var uAgent = Compile.ExecuteCode(starterCode, starterCodeClass, "UserAgent", false, null);
                ModFuncs.Add(new CodeFuncProps { Name = "UserAgent", FuncObj = uAgent });

                var FollowRedirect = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "FollowRedirect", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "FollowRedirect", FuncObj = FollowRedirect });

                var UseSSL = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "UseSSL", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "UseSSL", FuncObj = UseSSL });

                var KeepCookies = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "KeepCookies", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "KeepCookies", FuncObj = KeepCookies });

                var IPE = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "IgnoreProtocolErrors", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "IPE", FuncObj = IPE });

                var DefaultReturnFlag = Compile.ExecuteCode(starterCode, starterCodeClass, "DefaultReturnFlag", false, null);
                ModFuncs.Add(new CodeFuncProps { Name = "DefaultReturnFlag", FuncObj = DefaultReturnFlag });

                var UseCaptureRequest = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "UseCaptureRequest", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "UseCaptureRequest", FuncObj = UseCaptureRequest });

                var FirstRequestMethod = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "FirstRequestMethod", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "FirstRequestMethod", FuncObj = FirstRequestMethod });

                var FirstRequestData = Compile.ExecuteCode(starterCode, starterCodeClass, "FirstRequestData", false, null);
                ModFuncs.Add(new CodeFuncProps { Name = "FirstRequestData", FuncObj = FirstRequestData });

                var Phase1FollowRedirect = Compile.ExecuteCode(starterCode, starterCodeClass, "Phase1FollowRedirect", false, null);
                ModFuncs.Add(new CodeFuncProps { Name = "Phase1FollowRedirect", FuncObj = Phase1FollowRedirect });


                var UsePhase2 = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "UsePhase2", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "UsePhase2", FuncObj = UsePhase2 });


                var CrackURI = Compile.ExecuteCode(starterCode, starterCodeClass, "CrackURI", false, null);
                ModFuncs.Add(new CodeFuncProps { Name = "CrackURI", FuncObj = CrackURI });

                var FirstRequestURI = Compile.ExecuteCode(starterCode, starterCodeClass, "FirstRequestURI", false, null);
                ModFuncs.Add(new CodeFuncProps { Name = "FirstRequestURI", FuncObj = FirstRequestURI });

                var GetOrPostCracking = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "GetOrPostCracking", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "GetOrPostCracking", FuncObj = GetOrPostCracking });

                var CrackingData = Compile.ExecuteCode(starterCode, starterCodeClass, "CrackingData", false, null);
                ModFuncs.Add(new CodeFuncProps { Name = "CrackingData", FuncObj = CrackingData });

                var CaptureURI = UseCaptureRequest ? Compile.ExecuteCode(starterCode, starterCodeClass, "CaptureURI", false, null) : null;
                ModFuncs.Add(new CodeFuncProps { Name = "CaptureURI", FuncObj = CaptureURI });

                var CaptureRequestData = Compile.ExecuteCode(starterCode, starterCodeClass, "CaptureRequestData", false, null);
                ModFuncs.Add(new CodeFuncProps { Name = "CaptureRequestData", FuncObj = CaptureRequestData });

                var MethodCaptureURI = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "MethodCaptureURI", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "MethodCaptureURI", FuncObj = MethodCaptureURI });


                var FollowRedirectCapture = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "FollowRedirectCapture", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "FollowRedirectCapture", FuncObj = FollowRedirectCapture });

                var Phase1DataType = Compile.ExecuteCode(starterCode, starterCodeClass, "Phase1DataType", false, null).ToString();
                var Phase2DataType = Compile.ExecuteCode(starterCode, starterCodeClass, "Phase2DataType", false, null).ToString();
                var Phase3DataType = Compile.ExecuteCode(starterCode, starterCodeClass, "Phase3DataType", false, null).ToString();

                ModFuncs.Add(new CodeFuncProps { Name = "Phase1DataType", FuncObj = Phase1DataType });
                ModFuncs.Add(new CodeFuncProps { Name = "Phase2DataType", FuncObj = Phase2DataType });
                ModFuncs.Add(new CodeFuncProps { Name = "Phase3DataType", FuncObj = Phase2DataType });


                var UseCustomHeaders = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "UseCustomHeaders", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "UseCustomHeaders", FuncObj = UseCustomHeaders });
                if (UseCustomHeaders)
                {
                    var BeforeCrackingHeaders = Compile.ExecuteCode(starterCode, starterCodeClass, "BeforeCrackingHeaders", false, null) as List<Tuple<string, string>>;
                    ModFuncs.Add(new CodeFuncProps { Name = "BeforeCrackingHeaders", FuncObj = BeforeCrackingHeaders });
                    var InCrackingHeaders = Compile.ExecuteCode(starterCode, starterCodeClass, "InCrackingHeaders", false, null) as List<Tuple<string, string>>;
                    ModFuncs.Add(new CodeFuncProps { Name = "InCrackingHeaders", FuncObj = InCrackingHeaders });
                }

                var UseCustomCookies = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "UseCustomCookies", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "UseCustomCookies", FuncObj = UseCustomCookies });

                var Debug = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "Debug", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "Debug", FuncObj = Debug });

                var RemoveBadProxies = bool.Parse(Compile.ExecuteCode(starterCode, starterCodeClass, "RemoveBadProxies", false, null).ToString());
                ModFuncs.Add(new CodeFuncProps { Name = "RemoveBadProxies", FuncObj = RemoveBadProxies });
                if (UseCustomCookies)
                {
                    var BeforeCrackingCookies = Compile.ExecuteCode(starterCode, starterCodeClass, "BeforeCrackingCookies", false, null) as List<Tuple<string, string>>;
                    var InCrackingCookies = Compile.ExecuteCode(starterCode, starterCodeClass, "InCrackingCookies", false, null) as List<Tuple<string, string>>;
                    var CaptureCookies = Compile.ExecuteCode(starterCode, starterCodeClass, "CaptureCookies", false, null) as List<Tuple<string, string>>;

                    ModFuncs.Add(new CodeFuncProps { Name = "BeforeCrackingCookies", FuncObj = BeforeCrackingCookies });
                    ModFuncs.Add(new CodeFuncProps { Name = "InCrackingCookies", FuncObj = InCrackingCookies });
                    ModFuncs.Add(new CodeFuncProps { Name = "CaptureCookies", FuncObj = CaptureCookies });
                }

                var MNm = Compile.ExecuteCode(starterCode, starterCodeClass, "Name", false).ToString();
                modName = MNm;
                var MNv = Compile.ExecuteCode(starterCode, starterCodeClass, "Version", false).ToString();
                var MNa = Compile.ExecuteCode(starterCode, starterCodeClass, "Author", false).ToString();
                Logger.wfromClass(MNm,
                    new String[] { "Loaded Mod " + MNm + " Version: ", MNv, " By ", MNa + "\n" },
                    new ConsoleColor[] { ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.White, ConsoleColor.White }
                    );
                return true;
            }
            catch (Exception e)
            {
                Logger.err("Error with the Mod -> " + e.Message);
                return false;
            }

        }

        public T GetFun<T>(string funcName)
        {
            if (ModFuncs.Count < 0)
                return (T) Convert.ChangeType(null, typeof(T));
            foreach (var func in ModFuncs)
            {
                if (func.Name == funcName)
                    return (T) Convert.ChangeType(func.FuncObj, typeof(T));
            }
            return (T) Convert.ChangeType(null, typeof(T));
        }

        private struct Modular
        {
            public String Name { get; set; }
            public String Code { get; set; }
            public String CodeClass { get; set; }
            public int index { get; set; }
        }
        
        private struct CodeFuncProps
        {
            public String Name;
            public object FuncObj;
        }
    }
}
