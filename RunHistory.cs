using System;

namespace PerfCompare
{
    /// <summary>
    /// Summary description for RunHistory.
    /// </summary>
    public class RunHistory
    {
        private int iterations;
        private string test;
        private string runTime;
        private long ticks;
        private double milliseconds;
        private string comment;
        private string compilerSwitches;
        private bool advancedEdit;

        public RunHistory(int iterations, string test, DateTime runTime, 
            long ticks, double milliseconds, string comment, 
            string compilerSwitches, bool advancedEdit)
        {
            this.iterations = iterations;
            this.test = test;
            this.runTime = runTime.ToString();
            this.ticks = ticks;
            this.milliseconds = milliseconds;
            this.comment = comment;
            this.compilerSwitches = compilerSwitches;
            this.advancedEdit = advancedEdit;
        }

        public int Iterations { get { return iterations; } }
        public string Test { get { return test; } }
        public string RunTime { get { return runTime; } }
        public long Ticks { get { return ticks; } }
        public double Milliseconds { get { return milliseconds; } }
        public string Comment { get { return comment; } }
        public string CompilerSwitches { get { return compilerSwitches; } }
        public bool AdvancedEdit { get { return advancedEdit; } }

		public static string Header
		{
			get 
			{
				return "Run Time\tComment\tIterations\tTicks\tMilliseconds\tCode\tCompiler Switches\tAdvanced Edit";
			}
		}

        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
                RunTime, Comment, Iterations, Ticks, Milliseconds, 
				"\"" + Test.Replace("\"", "\"\"") + "\"", CompilerSwitches, AdvancedEdit);
        }
	}
}
