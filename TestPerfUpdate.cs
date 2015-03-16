using System;
using NUnit.Framework;

namespace PerfCompare
{
	/// <summary>
	/// Test the PerfCompareUpdate class.
	/// </summary>
	[TestFixture]
	public class TestPerfUpdate
	{
		[Test]
		public void UpdateAvailable()
		{
			string urlSource = @"E:\Lee\Visual Studio Projects\PerfCompare\testData";
			string currentVersion = "1.0";

			PerfCompareUpdate updater = new PerfCompareUpdate(urlSource, currentVersion);
			Assertion.Assert(updater.UpdateAvailable);
		}

		[Test]
		public void NoUpdateAvailable()
		{
			string urlSource = @"E:\Lee\Visual Studio Projects\PerfCompare\testData";
			string currentVersion = "1.1.200405201";

			PerfCompareUpdate updater = new PerfCompareUpdate(urlSource, currentVersion);
			Assertion.Assert(! updater.UpdateAvailable);
		}

		[Test]
		public void GetsUpdateText()
		{
			string urlSource = @"E:\Lee\Visual Studio Projects\PerfCompare\testData";
			string currentVersion = "1.1.200405201";

			PerfCompareUpdate updater = new PerfCompareUpdate(urlSource, currentVersion);
			string rtfSource = updater.UpdateNotice;

			Assertion.AssertNotNull(rtfSource);
		}

		[Test]
		public void GetsValidText()
		{
			string urlSource = @"E:\Lee\Visual Studio Projects\PerfCompare\testData";
			string currentVersion = "1.1.200405201";

			PerfCompareUpdate updater = new PerfCompareUpdate(urlSource, currentVersion);
			string rtfSource = updater.UpdateNotice;

			Assertion.Assert(rtfSource.IndexOf(@"green128\blue128;\red192\green") > 0);
		}
	}
}
