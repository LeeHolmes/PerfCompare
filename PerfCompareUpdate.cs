using System;
using System.Xml;
using System.Net;
using System.IO;

namespace PerfCompare
{
	/// <summary>
	/// Holds logic for PerfCompare updates.
	/// </summary>
	public class PerfCompareUpdate
	{
		private string host;
		private string currentVersion;
		private bool checkedUpdate = false;
		private bool updateAvailable = false;

		public PerfCompareUpdate(string host, string currentVersion)
		{
			this.host = host;
			this.currentVersion = currentVersion;
		}

		public bool UpdateAvailable
		{
			get 
			{ 
				if(! checkedUpdate)
				{
					string versionUrl = host + "/PerfCompareVersion.xml?currentVersion=" + currentVersion;

					XmlDocument xmlDoc = new XmlDocument();
					xmlDoc.Load(versionUrl);

					string availableVersion = xmlDoc.SelectSingleNode("//PerfCompare/CurrentVersion").InnerText;
					if(currentVersion != availableVersion)
						updateAvailable = true;
				}

				return updateAvailable;
			}
		}
		
		public string UpdateNotice
		{
			get
			{
				string noticeUrl = host + "/PerfCompareNotice.rtf";

				WebRequest request = WebRequest.Create(noticeUrl);
				WebResponse webResponse = request.GetResponse();
				StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());

				return responseReader.ReadToEnd();
			}
		}
	}
}
