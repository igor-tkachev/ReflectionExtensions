using System;

using NUnit.Framework;

namespace TypeExtensions.Tests
{
	using Metadata;

	[TestFixture]
	public class MetadataInspectorTests
	{
#if !NET20 && !NET30 && !NET35 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2

		[Test]
		public void GetInstalledFrameworksTest()
		{
			foreach (var framework in MetadataInspector.GetInstalledFrameworks())
				Console.WriteLine(framework);
		}

#endif

		[Test]
		public void GetInstalledFrameworksTest1()
		{
			Console.WriteLine(MetadataInspector.RuntimeVersion);
		}
	}
}
