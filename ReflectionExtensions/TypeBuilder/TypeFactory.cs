#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ReflectionExtensions.TypeBuilder
{
	using Builders;
	using Reflection.Emit;

	public static class TypeFactory
	{
		#region Create Assembly

		public static bool SaveTypes { get; set; }
		public static bool SealTypes { get; set; } = true;

		static AssemblyBuilderHelper GetAssemblyBuilder(Type type, string suffix)
		{
			var assemblyDir = AppDomain.CurrentDomain.BaseDirectory;

			// Dynamic modules are locationless, so ignore them.
			// _ModuleBuilder is the base type for both
			// ModuleBuilder and InternalModuleBuilder classes.
			//
			if (
#if !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETCOREAPP2_2
				!(type.Module is _ModuleBuilder) &&
#endif
				type.Module.FullyQualifiedName.IndexOf('<') < 0)
				assemblyDir = Path.GetDirectoryName(type.Module.FullyQualifiedName);

			var fullName = type.FullName;

			if (type.IsGenericTypeEx())
				fullName = AbstractClassBuilder.GetTypeFullName(type);

			Debug.Assert(fullName    != null, $"{nameof(fullName)} != null");
			Debug.Assert(assemblyDir != null, $"{nameof(assemblyDir)} != null");

			fullName = fullName.Replace('<', '_').Replace('>', '_');

			return new AssemblyBuilderHelper(Path.Combine(assemblyDir, $"{fullName}.{suffix}.dll"));
		}

#if !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETCOREAPP2_2

		static void SaveAssembly(AssemblyBuilderHelper assemblyBuilder, Type type)
		{
			if (!SaveTypes)
				return;
			try
			{
				assemblyBuilder.Save();
				WriteDebug($"The '{type.FullName}' type saved in '{assemblyBuilder.Path}'.");
			}
			catch (Exception ex)
			{
				WriteDebug($"Can't save the '{assemblyBuilder.Path}' assembly for the '{type.FullName}' type: {ex.Message}.");
			}
		}

#endif

		#endregion

		#region GetType

		static readonly Dictionary<Type,IDictionary<object,Type>> _builtTypes = new Dictionary<Type,IDictionary<object,Type>>(10);

		public static Type? GetType(object hashKey, Type sourceType, ITypeBuilder typeBuilder)
		{
			if (hashKey     == null) throw new ArgumentNullException(nameof(hashKey));
			if (sourceType  == null) throw new ArgumentNullException(nameof(sourceType));
			if (typeBuilder == null) throw new ArgumentNullException(nameof(typeBuilder));

			try
			{
				lock (_builtTypes)
				{
					Type? type;

					if (_builtTypes.TryGetValue(typeBuilder.GetType(), out var builderTable))
					{
						if (builderTable.TryGetValue(hashKey, out type))
							return type;
					}
					else
					{
						_builtTypes.Add(typeBuilder.GetType(), builderTable = new Dictionary<object,Type>());
					}

					var assemblyBuilder = GetAssemblyBuilder(sourceType, typeBuilder.AssemblyNameSuffix);

					type = typeBuilder.Build(assemblyBuilder);

					if (type != null)
					{
						builderTable.Add(hashKey, type);
#if !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETCOREAPP2_2
						SaveAssembly(assemblyBuilder, type);
#endif
					}

					return type;
				}
			}
			catch (TypeBuilderException)
			{
				throw;
			}
			catch (Exception ex)
			{
				// Convert an Exception to TypeBuilderException.
				//
				throw new TypeBuilderException($"Could not build the '{sourceType.FullName}' type.", ex);
			}
		}

		public static Type GetType(Type sourceType)
		{
			var t =  GetType(sourceType, true);

			Debug.Assert(t != null, nameof(t) + " != null");

			return t;
		}

		public static Type? GetType(Type sourceType, bool throwException)
		{
			if (sourceType.IsSealedEx())
				return sourceType;

			if (sourceType.IsValueTypeEx())
				return sourceType;

			if (!sourceType.IsAbstract && sourceType.IsDefinedEx(typeof(BLToolkitGeneratedAttribute), true))
				return sourceType;

			var t = GetType(sourceType, sourceType, new AbstractClassBuilder(sourceType));

			if (throwException && t == null)
					throw new TypeBuilderException($"Type '{sourceType}' cannot be created.");

			return t;
		}

#if NET20 || NET30

		public static T CreateInstance<T>()
			where T: class
		{
			return Activator.CreateInstance<T>();
		}

#else

		static class InstanceCreator<T>
		{
			public static readonly Func<T> Create =
				System.Linq.Expressions.Expression.Lambda<Func<T>>(
					System.Linq.Expressions.Expression.New(TypeFactory.GetType(typeof(T)))).Compile();
		}

		public static T CreateInstance<T>()
			where T: class
		{
			return InstanceCreator<T>.Create();
		}

#endif

		#endregion

		#region Private Helpers

		[Conditional("DEBUG")]
		static void WriteDebug(string format, params object[] parameters)
		{
			Debug.WriteLine(string.Format(format, parameters));
		}

		#endregion
	}
}

#endif
