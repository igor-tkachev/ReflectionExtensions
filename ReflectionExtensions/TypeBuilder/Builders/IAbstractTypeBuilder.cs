#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections.Generic;

namespace ReflectionExtensions.TypeBuilder.Builders
{
	public interface IAbstractTypeBuilder
	{
		int     ID            { get; set; }
		object? TargetElement { get; set; }

		Type[]  GetInterfaces();
		bool    IsCompatible (BuildContext context, IAbstractTypeBuilder typeBuilder);

		bool    IsApplied    (BuildContext context, List<IAbstractTypeBuilder> builders);
		int     GetPriority  (BuildContext context);
		void    Build        (BuildContext context);
	}
}


#endif
