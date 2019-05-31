using System;
using System.Collections.Generic;
using System.Diagnostics;

using JetBrains.Annotations;

namespace ReflectionExtensions.Reflection
{
	using TypeBuilder;

	/// <summary>
	/// Provides fast access to type and its members.
	/// </summary>
	[DebuggerDisplay("Type = {Type}, InstanceType = {InstanceType}")]
	[PublicAPI]
	public abstract class TypeAccessor
	{
		#region Protected Emit Helpers

		/// <summary>
		/// Adds <see cref="MemberAccessor"/>.
		/// </summary>
		/// <param name="member">Instance of <see cref="MemberAccessor"/>.</param>
		protected void AddMember(MemberAccessor member)
		{
			Members.Add(member);

			_membersByName[member.MemberInfo.Name] = member;
		}

		#endregion

		#region CreateInstance

		/// <summary>
		/// Creates an instance of the accessed type.
		/// </summary>
		/// <returns>An instance of the accessed type.</returns>
		[Pure]
		public virtual object CreateInstance() =>
			throw new InvalidOperationException($"The '{Type.Name}' type must have public default or init constructor.");

		[Pure]
		public virtual object CreateInstance(InitContext? context)
		{
			return CreateInstance();
		}

		[Pure]
		public object CreateInstanceEx()
		{
			return ObjectFactory != null
				? ObjectFactory.CreateInstance(this, null)
				: CreateInstance((InitContext?)null);
		}

		[Pure]
		public object CreateInstanceEx(InitContext context)
		{
			return ObjectFactory != null ? ObjectFactory.CreateInstance(this, context) : CreateInstance(context);
		}

		#endregion

		#region ObjectFactory

		public IObjectFactory? ObjectFactory { get; set; }

		#endregion

		#region Public Members

		/// <summary>
		/// Type to access.
		/// </summary>
		public abstract Type Type         { get; }

		public abstract Type InstanceType { get; }


		/// <summary>
		/// Type members.
		/// </summary>
		[NotNull, ItemNotNull]
		public List<MemberAccessor> Members { get; } = new List<MemberAccessor>();

		#endregion

		#region Items

		readonly Dictionary<string,MemberAccessor> _membersByName = new Dictionary<string,MemberAccessor>();

		/// <summary>
		/// Returns <see cref="MemberAccessor"/> by its name.
		/// </summary>
		/// <param name="memberName">Member name.</param>
		/// <returns>Instance of <see cref="MemberAccessor"/>.</returns>
		public MemberAccessor this[[NotNull] string memberName] => _membersByName[memberName];

		public bool HasMember([NotNull] string memberName) => _membersByName.ContainsKey(memberName);

		/// <summary>
		/// Returns <see cref="MemberAccessor"/> by index.
		/// </summary>
		/// <param name="index">Member index.</param>
		/// <returns>Instance of <see cref="MemberAccessor"/>.</returns>
		public MemberAccessor this[int index] => Members[index];

		#endregion

		#region Static Members

		static readonly Dictionary<Type,TypeAccessor> _accessors = new Dictionary<Type,TypeAccessor>();

		/// <summary>
		/// Creates an instance of <see cref="TypeAccessor"/>.
		/// </summary>
		/// <param name="type">Type to access.</param>
		/// <returns>Instance of <see cref="TypeAccessor"/>.</returns>
		[Pure]
		public static TypeAccessor GetAccessor(Type type)
		{
			lock (_accessors)
			{
				if (_accessors.TryGetValue(type, out var accessor))
					return accessor;

				if (IsAssociatedType(type))
					return _accessors[type];

				var accessorType = typeof(TypeAccessor<>).MakeGenericType(type);

				accessor = (TypeAccessor)Activator.CreateInstance(accessorType, true);

				lock (_accessors)
					_accessors[type] = accessor;

				return accessor;
			}
		}

		/// <summary>
		/// Creates an instance of <see cref="TypeAccessor"/>.
		/// </summary>
		/// <typeparam name="T">Type to access.</typeparam>
		/// <returns>Instance of <see cref="TypeAccessor"/>.</returns>
		[NotNull, Pure]
		public static TypeAccessor<T> GetAccessor<T>()
		{
			return (TypeAccessor<T>)GetAccessor(typeof(T));
		}

		internal static bool IsInstanceBuildable(Type type)
		{
			if (!type.IsInterfaceEx())
				return true;

			lock (_accessors)
			{
				if (_accessors.ContainsKey(type))
					return true;

				if (IsAssociatedType(type))
					return true;
			}

			var attrs = type.GetCustomAttributesEx<AutoImplementInterfaceAttribute>(true);

			return attrs != null && attrs.Length > 0;
		}

		static bool IsAssociatedType(Type type)
		{
			if (AssociatedTypeHandler != null)
			{
				var child = AssociatedTypeHandler(type);

				if (child != null)
				{
					AssociateType(type, child);
					return true;
				}
			}

			return false;
		}

		public interface ITypeAccessorCreator
		{
			TypeAccessor CreateTypeAccessor(TypeAccessor associatedAccessor);
		}

		public class TypeAccessorCreator<T> : ITypeAccessorCreator
		{
			TypeAccessor ITypeAccessorCreator.CreateTypeAccessor(TypeAccessor associatedAccessor)
			{
				return new TypeAccessor<T>(associatedAccessor);
			}
		}

		public static TypeAccessor AssociateType(Type parent, Type child)
		{
			if (!parent.IsSameOrParentOf(child))
				throw new ArgumentException($"'{parent}' must be a base type of '{child}'", nameof(child));

			var accessor            = GetAccessor(child);
			var accessorCreatorType = typeof(TypeAccessorCreator<>).MakeGenericType(parent);
			var accessorCreator     = (ITypeAccessorCreator)Activator.CreateInstance(accessorCreatorType);
			var parentAccessor      = accessorCreator.CreateTypeAccessor(accessor);

			lock (_accessors)
				_accessors[parent] = parentAccessor;

			return accessor;
		}

		public delegate Type? GetAssociatedType(Type parent);
		public static event GetAssociatedType AssociatedTypeHandler;

		public static object CreateInstance(Type type)
		{
			return GetAccessor(type).CreateInstance();
		}

		public static object CreateInstance(Type type, InitContext? context)
		{
			return GetAccessor(type).CreateInstance(context);
		}

		public static object CreateInstanceEx(Type type)
		{
			return GetAccessor(type).CreateInstanceEx();
		}

		public static object CreateInstanceEx(Type type, InitContext context)
		{
			return GetAccessor(type).CreateInstanceEx(context);
		}

		public static T CreateInstance<T>()
		{
			return (T)CreateInstance(typeof(T));
		}

		public static T CreateInstance<T>(InitContext context)
		{
			return (T)CreateInstance(typeof(T), context);
		}

		public static T CreateInstanceEx<T>()
		{
			return (T)CreateInstanceEx(typeof(T));
		}

		public static T CreateInstanceEx<T>(InitContext context)
		{
			return (T)CreateInstanceEx(typeof(T), context);
		}

		#endregion
	}
}
