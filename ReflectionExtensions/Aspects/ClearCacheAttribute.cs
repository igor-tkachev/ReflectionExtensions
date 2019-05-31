#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.Aspects
{
	using TypeBuilder.Builders;

	/// <summary>
	/// http://www.bltoolkit.net/Doc/Aspects/index.htm
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple=true)]
	public class ClearCacheAttribute : AbstractTypeBuilderAttribute
	{
		#region Constructors

		public ClearCacheAttribute()
		{
		}

		public ClearCacheAttribute(string methodName)
		{
			_methodName = methodName;
		}

		public ClearCacheAttribute(string methodName, params Type[] parameterTypes)
		{
			_methodName     = methodName;
			_parameterTypes = parameterTypes;
		}

		public ClearCacheAttribute(Type declaringType, string methodName, params Type[] parameterTypes)
		{
			_declaringType  = declaringType;
			_methodName     = methodName;
			_parameterTypes = parameterTypes;
		}

		public ClearCacheAttribute(Type declaringType)
		{
			_declaringType = declaringType;
		}

		readonly Type?   _declaringType;
		readonly string? _methodName;
		readonly Type[]? _parameterTypes;

		#endregion

		public override IAbstractTypeBuilder TypeBuilder =>
			new Builders.ClearCacheAspectBuilder(_declaringType, _methodName, _parameterTypes);
	}
}

#endif
