using System;

namespace PerfCompare
{
	/// <summary>
	/// Interface for loading PerformanceRunner objects.
	/// </summary>
	public interface IPerformanceRunner
	{
        TimeSpan TestPerformance();
	}
}
