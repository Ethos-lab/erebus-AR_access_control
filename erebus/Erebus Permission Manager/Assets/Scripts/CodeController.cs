using erebus.Core.Transpile;
using RoslynCSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Erebus
{
    namespace LanguageManager
    {
        public class CodeController : MonoBehaviour
        {
            public static CodeController Instance => instance;
            private static CodeController instance = null;

            [SerializeField] private AssemblyReferenceAsset[] referenceAssets;
            private ScriptDomain domain = null;
            private byte[] baseAssembly = null;
            private List<string> baseAssemblyMethodList = null;

            private void Awake()
            {
                if (instance != null && instance != this)
                {
                    Destroy(this.gameObject);
                    return;
                }
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            private void Start()
            {
                domain = ScriptDomain.CreateDomain("Main domain", true);
                foreach (var referenceAsset in referenceAssets)
                    domain.RoslynCompilerService.ReferenceAssemblies.Add(referenceAsset);

                var baseAssemblyWrapperCode = BetterStreamingAssets.ReadAllText("BaseAssemblyEntryPoint.cs");
                var baseAssemblyWrapperScriptAssembly = CompileAndLoad(baseAssemblyWrapperCode);
                domain.RoslynCompilerService.ReferenceAssemblies.Add(baseAssemblyWrapperScriptAssembly);

                baseAssembly = domain.CompiledAssemblies[0].AssemblyImage;
                InitBaseAssemblyMethodList(baseAssemblyWrapperScriptAssembly);
            }
            public string ConvertToCSharpCode(string codeText)
            {
                //Convert erebus lang -> C#
                string res = null;
                try
                {
                    var erebusLangRawCode = codeText;
                    var transpiler = new erebusTranspiler();
                    var result = transpiler.RunTranspiler(erebusLangRawCode);
                    res = result.CodeOutput;
                    Debug.Log($"[CodeController] C# Conversion - TimeElapsed: {result.TimeElapsed}");
                }
                catch (Exception e)
                {
                    Debug.Log($"[ConvertToCSharpCode] {e.Message}");
                }
                return res;
            }
            public byte[] ConvertToAssemblyBytes(string codeText)
            {
                //Compile C# to assembly bytes
                var erebusLangAssembly = CompileAndLoad(codeText);
                var assemblyBytes = erebusLangAssembly.AssemblyImage;
                return assemblyBytes;
            }
            private ScriptAssembly CompileAndLoad(string inputCode)
            {
                var type = domain.CompileAndLoadMainSource(
                inputCode, ScriptSecurityMode.UseSettings);
                return type.Assembly;
            }
            public byte[] GetBaseAssembly()
            {
                return baseAssembly;
            }
            private void InitBaseAssemblyMethodList(ScriptAssembly baseEntryCodeScriptAssembly)
            {
                var baseEntryCodeAssembly = baseEntryCodeScriptAssembly.CompileResult.OutputAssembly;
                var program = baseEntryCodeAssembly.GetType("Erebus.AccessControl.BaseAssemblyEntryPoint");
                var methods = program.GetMethods();

                baseAssemblyMethodList = new List<string>();
                var trimPattern = new char[] { 'G', 'e', 't' };
                var programNamespaceType = program;
                foreach (var method in methods)
                {
                    var methodName = method.Name;
                    var methodNamespaceType = method.DeclaringType;
                    var methodReturnType = method.ReturnType;
                    var methodParamCount = method.GetParameters().Length;
                    //var methodParamCount = method.GetParameters()[0].Name;
                    var isGenericType = methodReturnType.IsGenericType;
                    // && methodReturnType?.GetGenericTypeDefinition() == typeof(List<>))
                    if (!isGenericType && methodParamCount == 0)
                    {
                        if (methodNamespaceType == programNamespaceType)
                        {
                            var processedMethodName = methodName.TrimStart(trimPattern);
                            if (methodName != processedMethodName)
                            {
                                //var finalMethodName = MatchMethodNameWithAttrName(processedMethodName);
                                baseAssemblyMethodList.Add(processedMethodName);
                            }
                        }
                    }
                }
            }
            private string MatchMethodNameWithAttrName(string methodName)
            {
                string res = "";
                switch (methodName)
                {
                    case "AppName":
                        res = "Application Name";
                        break;
                    case "CompanyName":
                        res = "Company Name";
                        break;
                    case "CurrentTime":
                        res = "Time";
                        break;
                    case "CurrentLocation":
                        res = "Location";
                        break;
                    case "CurrentFaceId":
                        res = "Face ID";
                        break;
                }
                return res;
            }
            public List<string> GetIncludedAttrNames(string targetCSharpCode)
            {
                print(targetCSharpCode);
                List<string> includedMethodList = new List<string>();
                foreach (var baseAssemblyMethodName in baseAssemblyMethodList)
                {
                    //Unique set of method names
                    if (!includedMethodList.Contains(baseAssemblyMethodName) && targetCSharpCode.Contains(baseAssemblyMethodName))
                    {
                        var matchedBaseAssemblyMethodName = MatchMethodNameWithAttrName(baseAssemblyMethodName);
                        if (matchedBaseAssemblyMethodName != "")
                            includedMethodList.Add(matchedBaseAssemblyMethodName);
                    }
                }
                return includedMethodList;
            }
        }
    }
}
