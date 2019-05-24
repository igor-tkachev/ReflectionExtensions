#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4
#define NETSTANDARDLESS1_4
#endif
#if NETSTANDARDLESS1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
#define NETSTANDARDLESS1_6
#endif

using System;
using System.Collections.Generic;
using System.Reflection;

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
		static TypeAccessor()
		{
			var type = typeof(T);

			if (type.IsValueTypeEx())
			{
				_createInstance = () => default;
			}
			else
			{
				var ctor = type.IsAbstractEx() ? null : type.GetDefaultConstructor();

#if NET20 || NET30

				if (ctor == null)
				{
					if (type.IsAbstractEx()) _createInstance = ThrowAbstractException;
					else                     _createInstance = ThrowException;
				}
				else
				{
					_createInstance = () => (T)Activator.CreateInstance(type);
				}

#else
				Expression<Func<T>> createInstanceExpression;

				if (ctor == null)
				{
					Expression<Func<T>> mi;

					if (type.IsAbstractEx()) mi = () => ThrowAbstractException();
					else                     mi = () => ThrowException();

					var body = Expression.Call(null, ((MethodCallExpression)mi.Body).Method);

					createInstanceExpression = Expression.Lambda<Func<T>>(body);
				}
				else
				{
					createInstanceExpression = Expression.Lambda<Func<T>>(Expression.New(ctor));
				}

				_createInstance = createInstanceExpression.Compile();
#endif
			}

#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6

			foreach (var memberInfo in type.GetMembersEx())
			{
				switch (memberInfo)
				{
					case FieldInfo f when !f.IsStatic && f.IsPublic :
						_members.Add(memberInfo);
						break;

					case PropertyInfo p when !p.GetMethod.IsStatic && p.GetMethod.IsPublic && p.GetIndexParameters().Length == 0 :
						_members.Add(memberInfo);
						break;
				}
			}

#else

			foreach (var memberInfo in type.GetMembersEx(BindingFlags.Instance | BindingFlags.Public))
			{
				if (memberInfo.MemberType == MemberTypes.Field ||
					memberInfo.MemberType == MemberTypes.Property && ((PropertyInfo)memberInfo).GetIndexParameters().Length == 0)
				{
					_members.Add(memberInfo);
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
								_members.Add(pi);
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
								_members.Add(pi);
							}
						}
					}

#endif
				}
			}
		}

		static readonly List<MemberInfo> _members = new List<MemberInfo>();

		internal TypeAccessor()
		{
			foreach (var member in _members)
				AddMember(new MemberAccessor(this, member));
		}

		static T ThrowException() =>
			throw new InvalidOperationException($"The '{typeof(T).FullName}' type must have default or init constructor.");

		static T ThrowAbstractException() =>
			throw new InvalidOperationException($"Cant create an instance of abstract class '{typeof(T).FullName}'.");

		static Func<T> _createInstance;

		/// <summary>
		/// Creates an instance of <see cref="TypeAccessor"/>.
		/// </summary>
		/// <returns>Instance of <see cref="TypeAccessor"/>.</returns>
		public override object CreateInstance() => _createInstance();

		/// <summary>
		/// Creates an instance of <see cref="TypeAccessor"/>.
		/// </summary>
		/// <returns>Instance of <see cref="TypeAccessor"/>.</returns>
		public T Create() => _createInstance();

		/// <summary>
		/// Type to access.
		/// </summary>
		public override Type Type => typeof(T);

		/// <summary>
		/// Creates an instance of <see cref="TypeAccessor{T}"/>.
		/// </summary>
		/// <returns>Instance of <see cref="TypeAccessor{T}"/>.</returns>
		public static TypeAccessor<T> GetAccessor() => GetAccessor<T>();
	}
}
