#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.TypeBuilder
{
	using ReflectionExtensions.Reflection;
	using ReflectionExtensions.TypeBuilder;

	[TestFixture]
	public class NoInstanceAttributeTest
	{
		public abstract class PersonCitizenship
		{
		}

		public abstract class Person
		{
			[NoInstance]
			public abstract PersonCitizenship Citizenship { get; set; }
		}

		[Test]
		public void Text()
		{
			Person person = (Person)TypeAccessor.CreateInstance(typeof(Person));

			Assert.IsNull(person.Citizenship);

			person.Citizenship = (PersonCitizenship)TypeAccessor.CreateInstance(typeof(PersonCitizenship));

			Assert.IsNotNull(person.Citizenship);
		}
	}
}

#endif
