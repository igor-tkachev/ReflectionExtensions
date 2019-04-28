using System;

namespace TypeExtensions.Metadata
{
	public class InstalledFrameworkInfo
	{
		public string  Name        { get; }
		public string? SubName     { get; }
		public string  Version     { get; }
		public string? SP          { get; }
		public string? InstallPath { get; }

		public InstalledFrameworkInfo(string name, string? subName, string version, string? sp, string? installPath)
		{
			Name        = name;
			SubName     = subName;
			Version     = version;
			SP          = sp;
			InstallPath = installPath;
		}

		public void Deconstruct(out string name, out string? subName, out string version, out string? sp, out string? installPath)
		{
			name        = Name;
			subName     = SubName;
			version     = Version;
			sp          = SP;
			installPath = InstallPath;
		}

		public override string ToString()
		{
			switch (this)
			{
				case (var name, null,   var ver, null,   null)     : return $"{name}, {ver}";
				case (var name, null,   var ver, var sp, null)     : return $"{name}, {ver}, SP{sp}";
				case (var name, var sn, var ver, null,   null)     : return $"{name} - {sn}, {ver}";
				case (var name, var sn, var ver, var sp, null)     : return $"{name} - {sn}, {ver}, SP{sp}";
				case (var name, var sn, var ver, null,   var path) : return $"{name} - {sn}, {ver}, {path}";
				case (var name, var sn, var ver, var sp, var path) : return $"{name} - {sn}, {ver}, SP{sp}, {path}";
			}

			return "unknown";
		}
	}
}
