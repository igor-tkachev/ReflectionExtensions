#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Reflection;

namespace ReflectionExtensions.Aspects
{
	public class ClearCacheAspect
	{
		public static MethodInfo GetMethodInfo(
			object caller, Type declaringType, string methodName, Type[] parameterTypes)
		{
			if (declaringType == null)
				declaringType = caller.GetType();

			return CacheAspect.GetMethodInfo(declaringType, methodName, parameterTypes);
		}

		public static Type GetType(object caller, Type declaringType)
		{
			if (declaringType == null)
				declaringType = caller.GetType();

			if (declaringType.IsAbstract)
				declaringType = TypeBuilder.TypeFactory.GetType(declaringType);

			return declaringType;
		}
	}
}

#endif
