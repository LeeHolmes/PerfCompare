using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;

using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace PerfCompare
{
	/// <summary>
	/// Support code for the Performance Comparison tool
	/// </summary>
	public class PerfCompareSupport
	{
        private int executionCount = -1;
        private string hotSpotCode = "";
        private string compilerParams = "";

        private string templateCode = @"/// Guidelines for changing this code:
/// - The namespace must be called PerfCompare
/// - The class must be called PerformanceRunner
/// - PerformanceRunner must implement IPerformanceRunner,
///   meaning that it must have a 'public TimeSpan TestPerformance()' 
///   method
/// - The tag, EXECUTION_COUNT (in brackets), will be replaced with the 
///   same value from the UI
/// - The tag, HOTSPOT_CODE (in brackets), will be replaced with an empty 
///   string
/// - The ILDasm UI Tab will disassemble (and show) all code in the 
///   ExecuteHotSpot() method

using System;

namespace PerfCompare
{
    public class PerformanceRunner : IPerformanceRunner
    {
        public TimeSpan TestPerformance()
        {
            DateTime start = DateTime.Now;
            for(int performanceTestCounter = 0; 
                performanceTestCounter < [EXECUTION_COUNT]; 
                    performanceTestCounter++)
            {
                ExecuteHotSpot();
            }
            return DateTime.Now.Subtract(start);
        }

        public static void ExecuteHotSpot()
        {
            [HOTSPOT_CODE]
        }
    }
}
";

        public PerfCompareSupport()
		{
		}

        public int ExecutionCount { get { return executionCount; } set { executionCount = value; } }
        public string HotSpotCode { get { return hotSpotCode; } set { hotSpotCode = value; } }
        public string CompilerParams { get { return compilerParams; } set { compilerParams = value; } }
        public string TemplateCode { get { return templateCode; } set { templateCode = value; } }

        public TimeSpan TestPerformance()
        {
            if(executionCount < 0)
                throw new InvalidExecutionCountException("Execution count not specified.");

            return GenerateAndExecute(executionCount, hotSpotCode);
        }

        public TimeSpan Calibrate()
        {
            int calibrationCount = 10000000;
            string calibrationCode = "";

            return GenerateAndExecute(calibrationCount, calibrationCode);
        }

        public string GetIlDasm(string pathToIlDasm)
        {
            string generatedCode = "";
            generatedCode = templateCode.Replace("[EXECUTION_COUNT]", executionCount.ToString());
            generatedCode = generatedCode.Replace("[HOTSPOT_CODE]", hotSpotCode);

            string filename = Path.GetTempFileName();
            GenerateDll(generatedCode, filename);
            string ilDasmOutput = GetIlDasmOutput(pathToIlDasm, filename);
            File.Delete(filename);

            return CleanOutput(ilDasmOutput);
        }

        private string CleanOutput(string ilDasmOutput)
        {
            string[] lines = ilDasmOutput.Split('\n');
            string returnString = "";
            bool inMethod = false;

            foreach(string line in lines)
            {
                if(line.IndexOf(".method public") >= 0)
                    inMethod = true;

                if(inMethod)
                    returnString += line + "\n";

                if(line.IndexOf("end of method") >= 0)
                    break;
            }
            
            if(returnString.Length == 0)
                throw new InvalidCodeException("Error: ILDasm could not find method PerfCompare.PerformanceRunner::ExecuteHotSpot");

            return returnString;
        }

        private string GetIlDasmOutput(string pathToIlDasm, string filename)
        {
            if(! File.Exists(filename))
                throw new FileNotFoundException(filename);

            string outputPath = Path.GetTempFileName();

            Process p = new Process();
            
            p.StartInfo.FileName = pathToIlDasm;
            p.StartInfo.Arguments = String.Format("/out:{0} /NOBAR /item=PerfCompare.PerformanceRunner::ExecuteHotSpot {1}", outputPath, filename);
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();

            TextReader fileReader = File.OpenText(outputPath);
            string result = fileReader.ReadToEnd();

            fileReader.Close();
            File.Delete(outputPath);

            return result;
        }

        private TimeSpan GenerateAndExecute(int executionCount, string hotSpotCode)
        {
            string generatedCode = "";
            
            generatedCode = templateCode.Replace("[EXECUTION_COUNT]", executionCount.ToString());
            generatedCode = generatedCode.Replace("[HOTSPOT_CODE]", hotSpotCode);

            Assembly generationResult = CompileCode(generatedCode, null);

            IPerformanceRunner performanceRunner = 
                (IPerformanceRunner) generationResult.CreateInstance("PerfCompare.PerformanceRunner");
            TimeSpan result = performanceRunner.TestPerformance();

            return result;
        }

        private Assembly CompileCode(string codeToCompile, string filename)
        {
            CodeSnippetCompileUnit csu = new CodeSnippetCompileUnit(codeToCompile);            
        
            // Obtains an ICodeCompiler from a CodeDomProvider class.
            CSharpCodeProvider provider = new CSharpCodeProvider();
            ICodeCompiler compiler = provider.CreateCompiler();

            // Configure the compiler parameters
            string[] assemblies = 
            { 
                @"System.dll", 
                Assembly.GetExecutingAssembly().Location,
                Process.GetCurrentProcess().MainModule.FileName
            };
            CompilerParameters cp = new CompilerParameters(assemblies);
            cp.CompilerOptions = compilerParams;

            // Either a DLL or assembly should be generated.
            if((filename != null) && (filename.Length > 0))
            {
                cp.GenerateExecutable = false;
                cp.OutputAssembly = filename;
            }
            else
                cp.GenerateInMemory = true;

            // Invokes compilation. 
            CompilerResults cr = compiler.CompileAssemblyFromDom(cp, csu);
            
            if(cr.Errors.Count > 0)
            {
                string errorLines = "Error: Could not build DLL:\n";

                foreach(CompilerError error in cr.Errors)
                    errorLines += "\n\t" + error.Line + ":\t" + error.ErrorText;
                throw new InvalidCodeException(errorLines);
            }

            if(filename == null)
                return cr.CompiledAssembly;
            else
                return null;
        }

        private void GenerateDll(string codeToCompile, string filename)
        {
            CompileCode(codeToCompile, filename);
        }
	}
}
