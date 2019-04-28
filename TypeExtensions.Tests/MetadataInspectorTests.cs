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

		[Test]
		public void PrintTupleTest()
		{
			Console.WriteLine(("", 2, 3, null as string, 5, 6, 7));
			Console.WriteLine(("", 2, 3, null as string, 5, 6, 7, 8));
			Console.WriteLine(("", 2, 3, null as string, 5, 6, 7, 8, 9));

			var t = ValueTuple.Create(1, 2, 3, 4, 5, 6, 7, 8);
		}
	}
}
