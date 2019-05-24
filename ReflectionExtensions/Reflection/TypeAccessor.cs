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
	[DebuggerDisplay("Type = {" + nameof(Type) + "}")]
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
		[DebuggerStepThrough]
		public virtual object CreateInstance() =>
			throw new InvalidOperationException($"The '{Type.Name}' type must have public default or init constructor.");

		#endregion

		#region Public Members

		/// <summary>
		/// Type to access.
		/// </summary>
		public abstract Type Type { get; }

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

				var accessorType = typeof(TypeAccessor<>).MakeGenericType(type);

				accessor = (TypeAccessor)Activator.CreateInstance(accessorType, true);

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
			lock (_accessors)
			{
				if (_accessors.TryGetValue(typeof(T), out var accessor))
					return (TypeAccessor<T>)accessor;

				return (TypeAccessor<T>)(_accessors[typeof(T)] = new TypeAccessor<T>());
			}
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

		private static bool IsAssociatedType(Type type)
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

		public static TypeAccessor AssociateType(Type parent, Type child)
		{
			if (!parent.IsSameOrParentOf(child))
				throw new ArgumentException($"'{parent}' must be a base type of '{child}'", nameof(child));

			var accessor = GetAccessor(child);

			accessor = (TypeAccessor)Activator.CreateInstance(accessor.GetType());

			lock (_accessors)
				_accessors.Add(parent, accessor);

			return accessor;
		}

		public delegate Type GetAssociatedType(Type parent);
		public static event GetAssociatedType AssociatedTypeHandler;

		#endregion
	}
}
