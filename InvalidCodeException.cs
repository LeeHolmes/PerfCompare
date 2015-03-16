using System;

namespace PerfCompare
{
    /// <summary>
    /// Thown when the test code is invalid.
    /// </summary>
    public class InvalidCodeException : InvalidOperationException
    {
        public InvalidCodeException(string message) : base(message) {}
    }
}
