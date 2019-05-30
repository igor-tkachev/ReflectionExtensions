#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4
#define NETSTANDARDLESS1_4
#endif
#if NETSTANDARDLESS1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
#define NETSTANDARDLESS1_6
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
using ReflectionExtensions.TypeBuilder;
#if !NET20 && !NET30
using System.Linq.Expressions;
#endif

namespace ReflectionExtensions.Reflection
{

	/// <summary>
	/// Provides fast access to type and its members.
	/// </summary>
	/// <typeparam name="T">Type to access.</typeparam>
	public class TypeAccessor<T> : TypeAccessor
	{
		internal TypeAccessor()
		{
			var type = InstanceType = typeof(T);

			if (!type.IsValueTypeEx())
			{

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

				type = InstanceType = IsClassBuilderNeeded(type) ? TypeFactory.GetType(type) : type;
#endif

				var ctor  = type.IsAbstractEx() ? null : type.GetDefaultConstructor();
				var ctor2 = type.IsAbstractEx() ? null : type.GetConstructorEx(typeof(InitContext));

#if !NET20 && !NET30
				var p = Expression.Parameter(typeof(InitContext), "p");
#endif

				switch (ctor, ctor2, type.IsAbstractEx())
				{
					case (null, null, true) :
						_createInstance  =        ThrowAbstractException;
						_createInstance2 = ctx => ThrowAbstractException();
						break;

					case (null, null, false) :
						_createInstance  =        ThrowException;
						_createInstance2 = ctx => ThrowException();
						break;

					case (var c1, null, _) :
#if NET20 || NET30
						_createInstance  = ()  => (T)Activator.CreateInstance(InstanceType);
						_createInstance2 = ctx => (T)Activator.CreateInstance(InstanceType);
#else
						_createInstance  = Expression.Lambda<Func<T>>(Expression.New(c1)).Compile();
						_createInstance2 = ctx => _createInstance();
#endif
						break;

					case (null, var c2, _) :
#if NET20 || NET30
						_createInstance  = ()  => (T)Activator.CreateInstance(InstanceType, new object?[] { (InitContext?)null });
						_createInstance2 = ctx => (T)Activator.CreateInstance(InstanceType, new object[] { ctx });
#else
						_createInstance2 = Expression.Lambda<Func<InitContext, T>>(Expression.New(c2, p), p).Compile();
						_createInstance  = () => _createInstance2(null);
#endif
						break;

					case var (c1, c2, _):
#if NET20 || NET30
						_createInstance  = ()  => (T)Activator.CreateInstance(InstanceType);
						_createInstance2 = ctx => (T)Activator.CreateInstance(InstanceType, new object[] { ctx });
#else
						_createInstance = Expression.Lambda<Func<T>>(Expression.New(c1)).Compile();
						_createInstance2 = Expression.Lambda<Func<InitContext, T>>(Expression.New(c2, p), p).Compile();
#endif
						break;

				}
			}

			var members = new List<MemberInfo>();

#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6

			foreach (var memberInfo in type.GetMembersEx())
			{
				switch (memberInfo)
				{
					case FieldInfo f when !f.IsStatic && f.IsPublic :
						members.Add(memberInfo);
						break;

					case PropertyInfo p when !p.GetMethod.IsStatic && p.GetMethod.IsPublic && p.GetIndexParameters().Length == 0 :
						members.Add(memberInfo);
						break;
				}
			}

#else

			foreach (var memberInfo in type.GetMembersEx(BindingFlags.Instance | BindingFlags.Public))
			{
				if (memberInfo.MemberType == MemberTypes.Field ||
					memberInfo.MemberType == MemberTypes.Property && ((PropertyInfo)memberInfo).GetIndexParameters().Length == 0)
				{
					members.Add(memberInfo);
				}
			}

#endif

			// Add explicit interface implementation properties support
			// Or maybe we should support all private fields/properties?
			//
			if (!type.IsInterfaceEx() && !type.IsArrayEx())
			{
				var interfaceMethods = new List<MethodInfo>();

				foreach (var ti in type.GetInterfacesEx())
					foreach (var tm in type.GetInterfaceMapEx(ti).TargetMethods)
						interfaceMethods.Add(tm);

				if (interfaceMethods.Count > 0)
				{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6

					foreach (var pi in type.GetPropertiesEx())
					{
						if (!pi.GetMethod.IsStatic && !pi.GetMethod.IsPublic && pi.GetIndexParameters().Length == 0)
						{
							var getMethod = pi.GetMethod;
							var setMethod = pi.SetMethod;

							if ((getMethod == null || interfaceMethods.Contains(getMethod)) &&
								(setMethod == null || interfaceMethods.Contains(setMethod)))
							{
								members.Add(pi);
							}
						}
					}

#else

					foreach (var pi in type.GetPropertiesEx(BindingFlags.NonPublic | BindingFlags.Instance))
					{
						if (pi.GetIndexParameters().Length == 0)
						{
							var getMethod = pi.GetGetMethod(true);
							var setMethod = pi.GetSetMethod(true);

							if ((getMethod == null || interfaceMethods.Contains(getMethod)) &&
								(setMethod == null || interfaceMethods.Contains(setMethod)))
							{
								members.Add(pi);
							}
						}
					}

#endif
				}
			}

			foreach (var member in members)
				AddMember(new MemberAccessor(this, member));

			_instance = this;
		}

		internal TypeAccessor(TypeAccessor associatedAccessor)
		{
			InstanceType = associatedAccessor.InstanceType;

			_createInstance  = ()  => (T)associatedAccessor.CreateInstance();
			_createInstance2 = ctx => (T)associatedAccessor.CreateInstance(ctx);

			foreach (var member in associatedAccessor.Members)
				AddMember(member);

			_instance = this;
		}

		static T ThrowException() =>
			throw new TypeBuilderException($"The '{typeof(T).FullName}' type must have default or init constructor.");

		static T ThrowAbstractException() =>
			throw new TypeBuilderException($"Cant create an instance of abstract class '{typeof(T).FullName}'.");

#nullable disable
		static Func<T>             _createInstance  = ()  => default;
		static Func<InitContext,T> _createInstance2 = ctx => default;
#nullable restore

		/// <summary>
		/// Creates an instance of <see cref="TypeAccessor"/>.
		/// </summary>
		/// <returns>Instance of <see cref="TypeAccessor"/>.</returns>
		public override object CreateInstance() => _createInstance();

		public override object CreateInstance(InitContext? ctx) => _createInstance2(ctx);

		/// <summary>
		/// Creates an instance of <see cref="TypeAccessor"/>.
		/// </summary>
		/// <returns>Instance of <see cref="TypeAccessor"/>.</returns>
		public T Create() => _createInstance();

		/// <summary>
		/// Type to access.
		/// </summary>
		public override Type Type => typeof(T);

		public override Type InstanceType { get; }

		/// <summary>
		/// Creates an instance of <see cref="TypeAccessor{T}"/>.
		/// </summary>
		/// <returns>Instance of <see cref="TypeAccessor{T}"/>.</returns>
		public static TypeAccessor<T> GetAccessor() => GetAccessor<T>();

		static bool IsClassBuilderNeeded(Type type)
		{
			if (type.IsAbstractEx() && !type.IsSealedEx())
			{
				if (!type.IsInterfaceEx())
				{
					if (type.GetDefaultConstructor() != null)
						return true;

					if (type.GetConstructorEx(typeof(InitContext)) != null)
						return true;
				}
				else
				{
					var attr = type.GetCustomAttributeEx<AutoImplementInterfaceAttribute>();

					if (attr != null)
						return true;
				}
			}

			return false;
		}

		private static TypeAccessor<T> _instance;
		public  static TypeAccessor<T>  Instance => _instance ??= GetAccessor<T>();
	}
}
