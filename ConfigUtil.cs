using System;
using Microsoft.Win32;

namespace PerfCompare
{
	/// <summary>
	/// Configuration Utility Class
	/// </summary>
	public class ConfigUtil
	{
		public static string GetConfiguration(string property)
		{
			string storageLocation = @"Software\Microsoft\PerfCompare";

			RegistryKey currentUserKey = Registry.CurrentUser;
			RegistryKey storageArea;

			storageArea = currentUserKey.OpenSubKey(storageLocation);
			currentUserKey.Close();

			if(storageArea == null)
				return null;

			string returnValue = (string) storageArea.GetValue(property);
			storageArea.Close();

			return returnValue;
		}

		public static void SetConfiguration(string property, string val)
		{
			string storageLocation = @"Software\Microsoft\PerfCompare";

			RegistryKey currentUserKey = Registry.CurrentUser;
			RegistryKey storageArea;

			storageArea = currentUserKey.OpenSubKey(storageLocation, true);
			if(storageArea == null)
				storageArea = currentUserKey.CreateSubKey(storageLocation);

			currentUserKey.Close();

			storageArea.SetValue(property, val);
			storageArea.Close();
		}
	}
}
