using System;
using NUnit.Framework;

namespace PerfCompare
{
	/// <summary>
	/// Test the PerfCompareSupport class
	/// </summary>
	[TestFixture]
	public class TestPerfCompare
	{
        [Test]
        [ExpectedException(typeof(InvalidExecutionCountException))]
        public void IterationsError()
        {
            PerfCompareSupport supportClass = new PerfCompareSupport();
            TimeSpan result = supportClass.TestPerformance();
        }

        [Test]
        public void CalibrationRun()
        {
            PerfCompareSupport supportClass = new PerfCompareSupport();

            TimeSpan result = supportClass.Calibrate();
            Assertion.Assert(result.Ticks > 0);
        }

        [Test]
        [ExpectedException(typeof(InvalidCodeException))]
        public void ThrowsOnBadCode()
        {
            PerfCompareSupport supportClass = new PerfCompareSupport();
            supportClass.ExecutionCount = 1;
            supportClass.HotSpotCode = "System.Threading.Thread.Sleeeeeeep(1);";

            TimeSpan result = supportClass.TestPerformance();
        }

        [Test]
        public void GeneratesAppropriateResults()
        {
            PerfCompareSupport supportClass = new PerfCompareSupport();
            supportClass.ExecutionCount = 2;
            supportClass.HotSpotCode = @"System.Threading.Thread.Sleep(200);";

            TimeSpan result = supportClass.TestPerformance();

            Assertion.Assert(result.TotalMilliseconds > 300);
            Assertion.Assert(result.TotalMilliseconds < 500);
        }

        [Test]
        public void GeneratesGoodComparison()
        {
            string lengthString = @"bool returnVal = (""Test"".Length > 0);";
            string equalsString = @"bool returnVal = (""Test"".Equals(""""));";

            PerfCompareSupport supportClass = new PerfCompareSupport();
            supportClass.ExecutionCount = 50000000;
            supportClass.HotSpotCode = lengthString;

            TimeSpan lengthResults = supportClass.TestPerformance();
            
            supportClass.HotSpotCode = equalsString;
            TimeSpan equalsResults = supportClass.TestPerformance();

            Assertion.Assert(lengthResults.Ticks < equalsResults.Ticks);
        }

		[Test]
		public void GetsLogLine()
		{
			string inputCode = @"LogLine(""Test"");";
			string expected = "Test\r\n";

			PerfCompareSupport supportClass = new PerfCompareSupport();
			supportClass.ExecutionCount = 1;
			supportClass.HotSpotCode = inputCode;

			TimeSpan lengthResults = supportClass.TestPerformance();
			string logResult = supportClass.Output;
            
			Assertion.AssertEquals(expected, logResult);
		}


        [Test]
        public void GetsIlDasm()
        {
            string pathToIlDasm = "C:\\Program Files\\Microsoft Visual Studio .NET 2003\\SDK\\v1.1\\bin\\ildasm.exe";
            string expectedResults = @"
    .method public hidebysig instance void 
            ExecuteHotSpot() cil managed
    {
      // Code size       14 (0xe)
      .maxstack  2
      .locals init (string V_0)
      IL_0000:  ldstr      ""Hi There""
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_000c:  pop
      IL_000d:  ret
    } // end of method PerformanceRunner::ExecuteHotSpot
";
            expectedResults = expectedResults.TrimStart('\n', '\r');

            string testCode = @"
            string myString = ""Hi There"";
            int result = myString.Length;
";
            PerfCompareSupport supportClass = new PerfCompareSupport();
            supportClass.ExecutionCount = 50000000;
            supportClass.HotSpotCode = testCode;
            string ilDasmResults = supportClass.GetIlDasm(pathToIlDasm);

            Assertion.AssertEquals(expectedResults, ilDasmResults);
        }

        [Test]
        public void PutsDllInRightPlace()
        {
            string pathToIlDasm = "C:\\Program Files\\Microsoft Visual Studio .NET 2003\\SDK\\v1.1\\bin\\ildasm.exe";
            string testCode = @"int myValue = 0;";

            System.Environment.CurrentDirectory = "C:\\";
            PerfCompareSupport supportClass = new PerfCompareSupport();
            supportClass.ExecutionCount = 1;
            supportClass.HotSpotCode = testCode;
            string ilDasmResults = supportClass.GetIlDasm(pathToIlDasm);

            Assertion.AssertNotNull(ilDasmResults);
        }

        [Test]
        public void AllowsUnsafe()
        {
            string pathToIlDasm = "C:\\Program Files\\Microsoft Visual Studio .NET 2003\\SDK\\v1.1\\bin\\ildasm.exe";
            string testCode = @"unsafe {}";

            PerfCompareSupport supportClass = new PerfCompareSupport();
            supportClass.ExecutionCount = 1;
            supportClass.HotSpotCode = testCode;
            supportClass.CompilerParams = "/unsafe";
            string ilDasmResults = supportClass.GetIlDasm(pathToIlDasm);

            Assertion.AssertNotNull(ilDasmResults);
        }

        [Test]
        public void SupportsFreeFormPerf()
        {
            string testCode = @"
using System;

namespace PerfCompare
{
    public class PerformanceRunner : PerformanceRunnerBase
    {
        public override TimeSpan TestPerformance()
        {
            return new TimeSpan(12345);
        }
    }
}
";

            PerfCompareSupport supportClass = new PerfCompareSupport();
            supportClass.ExecutionCount = 1;
            supportClass.TemplateCode = testCode;
            TimeSpan results = supportClass.TestPerformance();

            Assertion.AssertEquals(12345, results.Ticks);
        }

        [Test]
        public void SupportsFreeFormILDasm()
        {
            string pathToIlDasm = "C:\\Program Files\\Microsoft Visual Studio .NET 2003\\SDK\\v1.1\\bin\\ildasm.exe";

            string testCode = @"
using System;

namespace PerfCompare
{
    public class PerformanceRunner : PerformanceRunnerBase
    {
        public override TimeSpan TestPerformance()
        {
            return new TimeSpan(12345);
        }

        public void ExecuteHotSpot()
        {
            string myString = ""Hi There"";
            int result = myString.Length;
        }
    }
}
";

            string expectedResults = @"
    .method public hidebysig instance void 
            ExecuteHotSpot() cil managed
    {
      // Code size       14 (0xe)
      .maxstack  2
      .locals init (string V_0)
      IL_0000:  ldstr      ""Hi There""
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_000c:  pop
      IL_000d:  ret
    } // end of method PerformanceRunner::ExecuteHotSpot
";
            expectedResults = expectedResults.TrimStart('\n', '\r');

            PerfCompareSupport supportClass = new PerfCompareSupport();
            supportClass.ExecutionCount = 50000000;
            supportClass.TemplateCode = testCode;
            string ilDasmResults = supportClass.GetIlDasm(pathToIlDasm);

            Assertion.AssertEquals(expectedResults, ilDasmResults);
        }

        [Test]
        [ExpectedException(typeof(InvalidCodeException))]
        public void ILDasmNoHotspot()
        {
            string pathToIlDasm = "C:\\Program Files\\Microsoft Visual Studio .NET 2003\\SDK\\v1.1\\bin\\ildasm.exe";

            string testCode = @"
using System;

namespace PerfCompare
{
    public class PerformanceRunner : IPerformanceRunner
    {
        public TimeSpan TestPerformance()
        {
            return new TimeSpan(12345);
        }
    }
}
";
            PerfCompareSupport supportClass = new PerfCompareSupport();
            supportClass.ExecutionCount = 50000000;
            supportClass.TemplateCode = testCode;
            string ilDasmResults = supportClass.GetIlDasm(pathToIlDasm);
        }

        [Test]
        public void RunHistoryToString()
        {
            RunHistory testHistory = 
                new RunHistory(1, "Code", DateTime.MinValue, 2, 3, "Comment", "/switches", true);
            string expectedResults = 
                "1/1/0001 12:00:00 AM\tComment\t1\t2\t3\t\"Code\"\t/switches\tTrue";

            Assertion.AssertEquals(expectedResults, testHistory.ToString());
        }

		[Test]
		public void RunHistoryToStringWithNewLines()
		{
			RunHistory testHistory = 
				new RunHistory(1, "Code\r\nCode2", DateTime.MinValue, 2, 3, "Comment", "/switches", true);
			string expectedResults = 
				"1/1/0001 12:00:00 AM\tComment\t1\t2\t3\t\"Code\r\nCode2\"\t/switches\tTrue";

			Assertion.AssertEquals(expectedResults, testHistory.ToString());
		}

		[Test]
		public void RunHistoryToStringWithNewQuotes()
		{
			RunHistory testHistory = 
				new RunHistory(1, "Code\r\n\"Code2", DateTime.MinValue, 2, 3, "Comment", "/switches", true);
			string expectedResults = 
				"1/1/0001 12:00:00 AM\tComment\t1\t2\t3\t\"Code\r\n\"\"Code2\"\t/switches\tTrue";

			Assertion.AssertEquals(expectedResults, testHistory.ToString());
		}

		[Test]
		public void RunHistoryToStringWithTab()
		{
			RunHistory testHistory = 
				new RunHistory(1, "Code\r\n\tCode2", DateTime.MinValue, 2, 3, "Comment", "/switches", true);
			string expectedResults = 
				"1/1/0001 12:00:00 AM\tComment\t1\t2\t3\t\"Code\r\n\tCode2\"\t/switches\tTrue";

			Assertion.AssertEquals(expectedResults, testHistory.ToString());
		}

		[Test]
		public void RunHistoryHeader()
		{
			string expectedResults = 
				"Run Time\tComment\tIterations\tTicks\tMilliseconds\tCode\tCompiler Switches\tAdvanced Edit";

			Assertion.AssertEquals(expectedResults, RunHistory.Header);
		}
	}
}
