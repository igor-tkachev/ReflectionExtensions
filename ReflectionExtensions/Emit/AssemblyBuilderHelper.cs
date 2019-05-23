#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Threading;

using JetBrains.Annotations;

namespace ReflectionExtensions.Emit
{
	/// <summary>
	/// A wrapper around the <see cref="AssemblyBuilder"/> and <see cref="ModuleBuilder"/> classes.
	/// </summary>
	/// <include file="Examples.CS.xml" path='examples/emit[@name="Emit"]/*' />
	/// <include file="Examples.VB.xml" path='examples/emit[@name="Emit"]/*' />
	/// <seealso cref="System.Reflection.Emit.AssemblyBuilder">AssemblyBuilder Class</seealso>
	/// <seealso cref="System.Reflection.Emit.ModuleBuilder">ModuleBuilder Class</seealso>
	[PublicAPI]
	public class AssemblyBuilderHelper
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyBuilderHelper"/> class
		/// with the specified parameters.
		/// </summary>
		/// <param name="path">The path where the assembly will be saved.</param>
		public AssemblyBuilderHelper(string path)
			: this(path, null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyBuilderHelper"/> class
		/// with the specified parameters.
		/// </summary>
		/// <param name="path">The path where the assembly will be saved.</param>
		/// <param name="version">The assembly version.</param>
		/// <param name="keyFile">The key pair file to sign the assembly.</param>
		public AssemblyBuilderHelper(string path, Version? version, string? keyFile)
		{
			var idx = path.IndexOf(',');

			if (idx > 0)
			{
				path = path.Substring(0, idx);

				if (path.Length >= 200)
				{
					idx = path.IndexOf('`');

					if (idx > 0)
					{
						var idx2 = path.LastIndexOf('.');

						if (idx2 > 0 && idx2 > idx)
							path = path.Substring(0, idx + 1) + path.Substring(idx2 + 1);
					}
				}
			}

			path = path.Replace("+", ".").Replace("<", "_").Replace(">", "_");

			if (path.Length >= 260)
			{
				path = path.Substring(0, 248);

				for (var i = 0; i < int.MaxValue; i++)
				{
					var newPath = $"{path}_{i:0000}.dll";

					if (!System.IO.File.Exists(newPath))
					{
						path = newPath;
						break;
					}
				}
			}

			var assemblyName = System.IO.Path.GetFileNameWithoutExtension(path);
			var assemblyDir  = System.IO.Path.GetDirectoryName(path);

			Path               = path;
			AssemblyName.Name = assemblyName;

			if (version != null)
				AssemblyName.Version = version;

			if (!string.IsNullOrEmpty(keyFile))
			{
				AssemblyName.Flags        |= AssemblyNameFlags.PublicKey;
				AssemblyName.KeyPair       = new StrongNameKeyPair(System.IO.File.OpenRead(keyFile));
				AssemblyName.HashAlgorithm = System.Configuration.Assemblies.AssemblyHashAlgorithm.SHA1;
			}

#if DEBUG
			AssemblyName.Flags |= AssemblyNameFlags.EnableJITcompileTracking;
#else
			AssemblyName.Flags |= AssemblyNameFlags.EnableJITcompileOptimizer;
#endif

			_createAssemblyBuilder = _ =>
			{
#if NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2
				_assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run);
#else
				_assemblyBuilder =
					string.IsNullOrEmpty(assemblyDir)?
					Thread.GetDomain().DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.RunAndSave):
					Thread.GetDomain().DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.RunAndSave, assemblyDir);
#endif

				_assemblyBuilder.SetCustomAttribute(BLToolkitAttribute);

				_assemblyBuilder.SetCustomAttribute(
					new CustomAttributeBuilder(
						typeof(AllowPartiallyTrustedCallersAttribute).GetConstructorEx(Type.EmptyTypes),
						new object[0]));
			};
		}

		/// <summary>
		/// Gets the path where the assembly will be saved.
		/// </summary>
		public string Path { get;  }

		/// <summary>
		/// Gets AssemblyName.
		/// </summary>
		public AssemblyName AssemblyName { get; } = new AssemblyName();

		readonly Action<int> _createAssemblyBuilder;

		AssemblyBuilder? _assemblyBuilder;
		/// <summary>
		/// Gets AssemblyBuilder.
		/// </summary>
		public AssemblyBuilder AssemblyBuilder
		{
			get
			{
				if (_assemblyBuilder == null)
					_createAssemblyBuilder(0);
				return _assemblyBuilder ?? throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Gets the path where the assembly will be saved.
		/// </summary>
		public  string  ModulePath => System.IO.Path.GetFileName(Path);

		private ModuleBuilder? _moduleBuilder;
		/// <summary>
		/// Gets ModuleBuilder.
		/// </summary>
		public  ModuleBuilder  ModuleBuilder
		{
			get
			{
				if (_moduleBuilder == null)
				{
					_moduleBuilder = AssemblyBuilder.DefineDynamicModule(ModulePath);
					_moduleBuilder.SetCustomAttribute(BLToolkitAttribute);
				}

				return _moduleBuilder;
			}
		}

		private CustomAttributeBuilder? _blToolkitAttribute;
		/// <summary>
		/// Retrieves a cached instance of <see cref="ReflectionExtensions.TypeBuilder.BLToolkitGeneratedAttribute"/> builder.
		/// </summary>
		public  CustomAttributeBuilder  BLToolkitAttribute
		{
			get
			{
				if (_blToolkitAttribute == null)
				{
					var at = typeof(TypeBuilder.BLToolkitGeneratedAttribute);
					var ci = at.GetConstructorEx(Type.EmptyTypes);

					_blToolkitAttribute = new CustomAttributeBuilder(ci, new object[0]);
				}

				return _blToolkitAttribute;
			}
		}

		/// <summary>
		/// Converts the supplied <see cref="AssemblyBuilderHelper"/> to a <see cref="AssemblyBuilder"/>.
		/// </summary>
		/// <param name="assemblyBuilder">The <see cref="AssemblyBuilderHelper"/>.</param>
		/// <returns>An <see cref="AssemblyBuilder"/>.</returns>
		public static implicit operator AssemblyBuilder(AssemblyBuilderHelper assemblyBuilder)
		{
			return assemblyBuilder.AssemblyBuilder;
		}

		/// <summary>
		/// Converts the supplied <see cref="AssemblyBuilderHelper"/> to a <see cref="ModuleBuilder"/>.
		/// </summary>
		/// <param name="assemblyBuilder">The <see cref="AssemblyBuilderHelper"/>.</param>
		/// <returns>A <see cref="ModuleBuilder"/>.</returns>
		public static implicit operator ModuleBuilder(AssemblyBuilderHelper assemblyBuilder)
		{
			return assemblyBuilder.ModuleBuilder;
		}

#if !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETCOREAPP2_2

		/// <summary>
		/// Saves this dynamic assembly to disk.
		/// </summary>
		public void Save()
		{
			_assemblyBuilder?.Save(ModulePath);
		}

#endif

		#region DefineType Overrides

		/// <summary>
		/// Constructs a <see cref="TypeBuilderHelper"/> for a type with the specified name.
		/// </summary>
		/// <param name="name">The full path of the type.</param>
		/// <returns>Returns the created <see cref="TypeBuilderHelper"/>.</returns>
		/// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string)">ModuleBuilder.DefineType Method</seealso>
		public TypeBuilderHelper DefineType(string name)
		{
			return new TypeBuilderHelper(this, ModuleBuilder.DefineType(name));
		}

		/// <summary>
		/// Constructs a <see cref="TypeBuilderHelper"/> for a type with the specified name and base type.
		/// </summary>
		/// <param name="name">The full path of the type.</param>
		/// <param name="parent">The Type that the defined type extends.</param>
		/// <returns>Returns the created <see cref="TypeBuilderHelper"/>.</returns>
		/// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type)">ModuleBuilder.DefineType Method</seealso>
		public TypeBuilderHelper DefineType(string name, Type parent)
		{
			return new TypeBuilderHelper(this, ModuleBuilder.DefineType(name, TypeAttributes.Public, parent));
		}

		/// <summary>
		/// Constructs a <see cref="TypeBuilderHelper"/> for a type with the specified name, its attributes, and base type.
		/// </summary>
		/// <param name="name">The full path of the type.</param>
		/// <param name="attrs">The attribute to be associated with the type.</param>
		/// <param name="parent">The Type that the defined type extends.</param>
		/// <returns>Returns the created <see cref="TypeBuilderHelper"/>.</returns>
		/// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type)">ModuleBuilder.DefineType Method</seealso>
		public TypeBuilderHelper DefineType(string name, TypeAttributes attrs, Type parent)
		{
			return new TypeBuilderHelper(this, ModuleBuilder.DefineType(name, attrs, parent));
		}

		/// <summary>
		/// Constructs a <see cref="TypeBuilderHelper"/> for a type with the specified name, base type,
		/// and the interfaces that the defined type implements.
		/// </summary>
		/// <param name="name">The full path of the type.</param>
		/// <param name="parent">The Type that the defined type extends.</param>
		/// <param name="interfaces">The list of interfaces that the type implements.</param>
		/// <returns>Returns the created <see cref="TypeBuilderHelper"/>.</returns>
		/// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type,Type[])">ModuleBuilder.DefineType Method</seealso>
		public TypeBuilderHelper DefineType(string name, Type parent, params Type[] interfaces)
		{
			return new TypeBuilderHelper(
				this,
				ModuleBuilder.DefineType(name, TypeAttributes.Public, parent, interfaces));
		}

		/// <summary>
		/// Constructs a <see cref="TypeBuilderHelper"/> for a type with the specified name, its attributes, base type,
		/// and the interfaces that the defined type implements.
		/// </summary>
		/// <param name="name">The full path of the type.</param>
		/// <param name="attrs">The attribute to be associated with the type.</param>
		/// <param name="parent">The Type that the defined type extends.</param>
		/// <param name="interfaces">The list of interfaces that the type implements.</param>
		/// <returns>Returns the created <see cref="TypeBuilderHelper"/>.</returns>
		/// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type,Type[])">ModuleBuilder.DefineType Method</seealso>
		public TypeBuilderHelper DefineType(string name, TypeAttributes attrs, Type parent, params Type[] interfaces)
		{
			return new TypeBuilderHelper(
				this,
				ModuleBuilder.DefineType(name, attrs, parent, interfaces));
		}

		#endregion
	}
}

#endif
