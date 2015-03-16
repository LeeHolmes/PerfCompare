using System;

namespace PerfCompare
{
	/// <summary>
	/// Thown when a user does not specify the interation count
	/// for the test.
	/// </summary>
	public class InvalidExecutionCountException : InvalidOperationException
	{
        public InvalidExecutionCountException(string message) : base(message) {}
	}
}
