using System;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace xConCrk
{
    public class CodeCompiler
    {
        private string Namespace = string.Empty;
        private string netVersion = "4.0";
        private string language = "CSharp";
        List<string> Refs = new List<string>();

        private static string currentBaseDir = AppDomain.CurrentDomain.BaseDirectory;
        private static string currentConfDir = currentBaseDir + "CompilerConfig.ini";

        public CodeCompiler(string np)
        {
            this.Namespace = np;
            LoadConfig();
        }

        public void LoadConfig()
        {
            if (!File.Exists(currentConfDir))
            {
                var Cdata = "Language=CSharp\r\nVersion=4.0\r\nReferences=mscorlib.dll,System.dll,System.Core.dll";
                File.WriteAllText(currentConfDir, Cdata);
            }
            var ConfigData = Split(File.ReadAllText(currentConfDir), "\r\n");
            foreach (var data in ConfigData)
            {
                if (!string.IsNullOrEmpty(data) && data.Length > 5)
                {
                    var Tio = Split(data, "=");
                    if (Tio[0].ToLower().Contains("ver"))
                    {
                        netVersion = Tio[1];
                    }
                    if (Tio[0].ToLower().Contains("refer"))
                    {
                        if (Tio[1].Contains(","))
                            Refs.AddRange(Split(Tio[1], ","));
                        else
                            Refs.Add(Tio[1]);
                    }
                    if (Tio[0].ToLower().Contains("lang"))
                    {
                        language = Tio[1];
                    }
                }
            }
            if (Refs.Count < 1)
            {
                Refs.AddRange(new string[] { "mscorlib.dll", "System.dll", "System.Core.dll" });
            }
        }

        public object ExecuteCode(string code, string classname, string functionname, bool isstatic, params object[] args)
        {
            object returnval = null;
            Assembly asm = BuildAssembly(code);
            object instance = null;
            Type type = null;
            if (isstatic)
            {
                type = asm.GetType(Namespace + "." + classname);
            }
            else
            {
                instance = asm.CreateInstance(Namespace + "." + classname);
                type = instance.GetType();
            }

            try
            {
                MethodInfo method = type.GetMethod(functionname);
                returnval = method.Invoke(instance, args);
                return returnval;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<object> ExecuteCodeAsync(string code, string classname, string functionname, bool isstatic, params object[] args)
        {
            object returnval = null;
            Assembly asm = await BuildAssemblyAsync(code);
            object instance = null;
            Type type = null;
            if (isstatic)
            {
                type = asm.GetType(Namespace + "." + classname);
            }
            else
            {
                instance = asm.CreateInstance(Namespace + "." + classname);
                type = instance.GetType();
            }

            try
            {
                MethodInfo method = type.GetMethod(functionname);
                returnval = method.Invoke(instance, args);
                return returnval;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private Task<Assembly> BuildAssemblyAsync(string code)
        {
            return Task.Run(() => { return BuildAssembly(code); });
        }

        private Assembly BuildAssembly(string code)
        {
            CodeDomProvider provider;
            if (CodeDomProvider.IsDefinedLanguage(language))
            {
                provider = CodeDomProvider.CreateProvider(language);
                var options = new Dictionary<string, string> { { "CompilerVersion", "v" + netVersion } };
                CSharpCodeProvider codeProvider = new CSharpCodeProvider(options);
                CompilerParameters compilerparams = new CompilerParameters();
                compilerparams.GenerateExecutable = false;
                compilerparams.GenerateInMemory = true;

                foreach (var Ref in Refs)
                {
                    compilerparams.ReferencedAssemblies.Add(Ref);
                }

                CompilerResults results = provider.CompileAssemblyFromSource(compilerparams, code);
                if (results.Errors.HasErrors)
                {
                    StringBuilder errors = new StringBuilder("xCracker Compiler Errors :\r\n");
                    foreach (CompilerError error in results.Errors)
                    {
                        errors.AppendFormat("Line {0},{1}\t: {2}\n", error.Line, error.Column, error.ErrorText);
                    }
                    throw new Exception(errors.ToString());
                }
                else
                {
                    return results.CompiledAssembly;
                }
            }
            else
            {
                throw new Exception("Something went wrong with the compiler (Unknown Language Provided?)");
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
    }
}
