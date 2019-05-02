using System;
using System.Collections.Generic;

#if !NET20 && !NET30 && !NET35 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2
using Microsoft.Win32;
#endif

namespace ReflectionExtensions.Metadata
{
	public static class MetadataInspector
	{
#if !NET20 && !NET30 && !NET35 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2

		// Stolen from https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed
		//
		public static IEnumerable<InstalledFrameworkInfo> GetInstalledFrameworks()
		{
			// Opens the registry key for the .NET Framework entry.
			//
			using (var ndpKey = RegistryKey
				.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
				.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
			{
				foreach (var versionKeyName in ndpKey.GetSubKeyNames())
				{
					// Skip .NET Framework 4.5 version information.
					//
					if (versionKeyName == "v4")
					{
						continue;
					}

					if (versionKeyName.StartsWith("v"))
					{
						var versionKey = ndpKey.OpenSubKey(versionKeyName);

						// Get the .NET Framework version value.
						//
						var name = (string)versionKey.GetValue("Version", "");

						if (!string.IsNullOrEmpty(name))
						{
							// Get the service pack (SP) number.
							//
							var sp = versionKey.GetValue("SP", "").ToString();

							// Get the installation flag, or an empty string if there is none.
							//
							var install = versionKey.GetValue("Install", "").ToString();

							if (string.IsNullOrEmpty(install)) // No install info; it must be in a child subkey.
							{
								yield return new InstalledFrameworkInfo(versionKeyName, null, name, null, null);
							}
							else if (!(string.IsNullOrEmpty(sp)) && install == "1")
							{
								yield return new InstalledFrameworkInfo(versionKeyName, null, name, sp, null);
							}

							continue;
						}

						foreach (var subKeyName in versionKey.GetSubKeyNames())
						{
							var subKey  = versionKey.OpenSubKey(subKeyName);
							var subName = subKey.GetValue("Version", "").ToString();
							var sp      = subKey.GetValue("SP",      "").ToString();
							var install = subKey.GetValue("Install", "").ToString();

							switch (install, sp)
							{
								case ("",  _ )    : yield return new InstalledFrameworkInfo(versionKeyName, subKeyName, subName, null, null); break;
								case ("1", "")    : yield return new InstalledFrameworkInfo(versionKeyName, subKeyName, subName, null, null); break;
								case ("1", var s) : yield return new InstalledFrameworkInfo(versionKeyName, subKeyName, subName, s,    null); break;
							}
						}
					}
				}
			}

			using (var ndpKey = RegistryKey
				.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
				.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"))
			{
				if (ndpKey?.GetValue("Release") != null)
				{
					// Checking the version using >= enables forward compatibility.
					string CheckFor45PlusVersion(int releaseKey)
					{
						if (releaseKey >= 528040) return "v4.8";
						if (releaseKey >= 461808) return "v4.7.2";
						if (releaseKey >= 461308) return "v4.7.1";
						if (releaseKey >= 460798) return "v4.7";
						if (releaseKey >= 394802) return "v4.6.2";
						if (releaseKey >= 394254) return "v4.6.1";
						if (releaseKey >= 393295) return "v4.6";
						if (releaseKey >= 379893) return "v4.5.2";
						if (releaseKey >= 378675) return "v4.5.1";
						if (releaseKey >= 378389) return "v4.5";

						// This code should never execute. A non-null release key should mean
						// that 4.5 or later is installed.
						return "No 4.5 or later version detected";
					}

					var name    = CheckFor45PlusVersion((int)ndpKey.GetValue("Release"));
					var version = (string)ndpKey.GetValue("Version",     "");
					var path    = (string)ndpKey.GetValue("InstallPath", null);

					yield return new InstalledFrameworkInfo(name, "Full", version, null, path);
				}
			}
		}
#endif

#if !NETSTANDARD1_0

		public static string RuntimeVersion
		{
			get
			{
#if NET20 || NET30 || NET35 || NET40 || NET45
				var v = Environment.Version;
				return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
#else
				return System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
#endif
			}
		}
#endif
	}
}
