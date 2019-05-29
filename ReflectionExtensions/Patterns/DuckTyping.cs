

using System.Diagnostics;
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0
using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace ReflectionExtensions.Patterns
{
	using Common;
	using Reflection;
	using TypeBuilder;
	using TypeBuilder.Builders;

	/// <summary>
	/// Duck typing implementation.
	/// In computer science, duck typing is a term for dynamic typing typical
	/// of some programming languages, such as Smalltalk, Python or ColdFusion,
	/// where a variable's value itself determines what the variable can do.
	/// Thus an object or set of objects having all the methods described in
	/// an interface can be made to implement that interface dynamically
	/// at runtime, even if the object’s class does not include the interface
	/// in its implements clause.
	/// </summary>
	public static class DuckTyping
	{
		#region Single Duck

		static readonly Dictionary<Type,Dictionary<object,Type?>> _duckTypes = new Dictionary<Type,Dictionary<object,Type?>>();

		/// <summary>
		/// Build a proxy type which implements the requested interface by redirecting all calls to the supplied object type.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="objectType">Any type which expected to have all members of the given interface.</param>
		/// <returns>The duck object type.</returns>
		public static Type? GetDuckType(this Type interfaceType, Type objectType)
		{
			if (interfaceType == null)          throw new ArgumentNullException(nameof(interfaceType));
			if (!interfaceType.IsInterfaceEx()) throw new ArgumentException("'interfaceType' must be an interface.", nameof(interfaceType));
			//if (!interfaceType.IsPublicEx() && !interfaceType.IsNestedPublicEx())
			//	throw new ArgumentException("The interface must be public.", nameof(interfaceType));

			Dictionary<object,Type?> types;

			lock(_duckTypes)
				if (!_duckTypes.TryGetValue(interfaceType, out types))
					_duckTypes.Add(interfaceType, types = new Dictionary<object,Type?>());

			Type? type;

			lock (types) if (!types.TryGetValue(objectType, out type))
			{
				type = TypeFactory.GetType(
					new CompoundValue(interfaceType, objectType),
					interfaceType, //objectType,
					new DuckTypeBuilder(MustImplementAttribute.Default, interfaceType, new[] { objectType }));

				types.Add(objectType, type);
			}

			return type;
		}

		/// <summary>
		/// Implements the requested interface for supplied object.
		/// If the supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="baseObjectType">Any type which has all members of the given interface.
		/// When this parameter is set to null, the object type will be used.</param>
		/// <param name="obj">An object which type expected to have all members of the given interface.</param>
		/// <param name="throwException">If true, throws an exception if object can not be created.</param>
		/// <returns>An object which implements the interface.</returns>
		public static object? Implement(this Type interfaceType, Type? baseObjectType, object? obj, bool throwException)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));

			var objType = obj.GetType();

			if (interfaceType.IsSameOrParentOf(objType))
				return obj;

			if (obj is DuckType duckObject)
			{
				if (duckObject.Objects.Length == 1)
				{
					// Switch to underlying objects when a duck object was passed.
					//
					return Implement(interfaceType, baseObjectType, duckObject.Objects[0]);
				}

				// Re-aggregate underlying objects to expose new interface.
				//
				return Aggregate(interfaceType, duckObject.Objects);
			}

			if (baseObjectType == null)
				baseObjectType = objType;
			else if (!baseObjectType.IsSameOrParentOf(objType))
				throw new ArgumentException($"'{objType.FullName}' is not a subtype of '{baseObjectType.FullName}'.", nameof(obj));

			var duckType = interfaceType.GetDuckType(baseObjectType);

			if (duckType == null)
			{
				if (throwException)
					throw new TypeBuilderException($"Interface '{interfaceType}' cannot be implemented.");
				return null;
			}

			var duck = TypeAccessor.GetAccessor(duckType).CreateInstance();

			((DuckType)duck).SetObjects(obj);

			return duck;
		}

		/// <summary>
		/// Implements the requested interface for supplied object.
		/// If the supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="baseObjectType">Any type which has all members of the given interface.
		/// When this parameter is set to null, the object type will be used.</param>
		/// <param name="obj">An object which type expected to have all members of the given interface.</param>
		/// <returns>An object which implements the interface.</returns>
		public static object Implement(this Type interfaceType, Type? baseObjectType, object? obj)
		{
			var o = interfaceType.Implement(baseObjectType, obj, true);

			Debug.Assert(o != null, nameof(o) + " != null");

			return o;
		}

		/// <summary>
		/// Implements the requested interface.
		/// If the supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="obj">An object which type expected to have all members of the given interface.</param>
		/// <param name="throwException">If true, throws an exception if object can not be created.</param>
		/// <returns>An object which implements the interface.</returns>
		public static object? Implement(this Type interfaceType, object obj, bool throwException)
		{
			return interfaceType.Implement(null, obj, throwException);
		}

		/// <summary>
		/// Implements the requested interface.
		/// If the supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="obj">An object which type expected to have all members of the given interface.</param>
		/// <returns>An object which implements the interface.</returns>
		public static object Implement(this Type interfaceType, object? obj)
		{
			return interfaceType.Implement(null, obj);
		}

		/// <summary>
		/// Implements the requested interface for all supplied objects.
		/// If any of supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="baseObjectType">Any type which has all members of the given interface.
		/// When this parameter is set to null, the object type will be used.</param>
		/// <param name="throwException">If true, throws an exception if object can not be created.</param>
		/// <param name="objects">An object array which types expected to have all members of the given interface.
		/// All objects may have different types.</param>
		/// <returns>An array of object which implements the interface.</returns>
		public static object?[] Implement(this Type interfaceType, Type? baseObjectType, bool throwException, params object[] objects)
		{
			if (objects == null) throw new ArgumentNullException(nameof(objects));

			object?[] result = new object[objects.Length];

			for (var i = 0; i < objects.Length; i++)
				result[i] = interfaceType.Implement(baseObjectType, objects[i], throwException);

			return result;
		}

		/// <summary>
		/// Implements the requested interface for all supplied objects.
		/// If any of supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="baseObjectType">Any type which has all members of the given interface.
		/// When this parameter is set to null, the object type will be used.</param>
		/// <param name="objects">An object array which types expected to have all members of the given interface.
		/// All objects may have different types.</param>
		/// <returns>An array of object which implements the interface.</returns>
		public static object[] Implement(this Type interfaceType, Type? baseObjectType, params object[] objects)
		{
			if (objects == null) throw new ArgumentNullException(nameof(objects));

			var result = new object[objects.Length];

			for (var i = 0; i < objects.Length; i++)
				result[i] = interfaceType.Implement(baseObjectType, objects[i]);

			return result;
		}

		/// <summary>
		/// Implements the requested interface for all supplied objects.
		/// If any of supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="objects">An object array which types expected to have all members of the given interface.
		/// All objects may have different types.</param>
		/// <param name="throwException">If true, throws an exception if object can not be created.</param>
		/// <returns>An array of object which implements the interface.</returns>
		public static object?[] Implement(this Type interfaceType, bool throwException, params object[] objects)
		{
			return interfaceType.Implement(null, throwException, objects);
		}

		/// <summary>
		/// Implements the requested interface for all supplied objects.
		/// If any of supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="objects">An object array which types expected to have all members of the given interface.
		/// All objects may have different types.</param>
		/// <returns>An array of object which implements the interface.</returns>
		public static object[] Implement(this Type interfaceType, params object[] objects)
		{
			return interfaceType.Implement(null, objects);
		}

		/// <summary>
		/// Implements the requested interface for supplied object.
		/// If the supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <typeparam name="TI">An interface type to implement.</typeparam>
		/// <param name="obj">An object which type expected to have all members of the given interface.</param>
		/// <param name="throwException">If true, throws an exception if object can not be created.</param>
		/// <returns>An object which implements the interface.</returns>
		public static TI? Implement<TI>(object obj, bool throwException)
			where TI : class
		{
			return (TI?)typeof(TI).Implement(null, obj, throwException);
		}

		/// <summary>
		/// Implements the requested interface for supplied object.
		/// If the supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <typeparam name="TI">An interface type to implement.</typeparam>
		/// <param name="obj">An object which type expected to have all members of the given interface.</param>
		/// <returns>An object which implements the interface.</returns>
		public static TI Implement<TI>(object? obj)
			where TI : class
		{
			return (TI)Implement(typeof(TI), null, obj);
		}

		/// <summary>
		/// Implements the requested interface for supplied object.
		/// If the supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <typeparam name="TI">An interface type to implement.</typeparam>
		/// <typeparam name="T">Any type which has all members of the given interface.</typeparam>
		/// <param name="throwException">If true, throws an exception if object can not be created.</param>
		/// <param name="objects">An object which type expected to have all members of the given interface.</param>
		/// <returns>An object which implements the interface.</returns>
		public static TI? Implement<TI,T>([NotNull] T objects, bool throwException)
			where TI : class
		{
			if (objects == null) throw new ArgumentNullException(nameof(objects));
			return (TI?)typeof(TI).Implement(typeof(T), objects, throwException);
		}

		/// <summary>
		/// Implements the requested interface for supplied object.
		/// If the supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <typeparam name="TI">An interface type to implement.</typeparam>
		/// <typeparam name="T">Any type which has all members of the given interface.</typeparam>
		/// <param name="objects">An object which type expected to have all members of the given interface.</param>
		/// <returns>An object which implements the interface.</returns>
		public static TI Implement<TI,T>([NotNull] T objects)
			where TI : class
		{
			if (objects == null) throw new ArgumentNullException(nameof(objects));
			return (TI)Implement(typeof(TI), typeof(T), objects);
		}

		/// <summary>
		/// Implements the requested interface for all supplied objects.
		/// If any of supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <typeparam name="TI">An interface type to implement.</typeparam>
		/// <param name="objects">An object array which types expected to have all members of the given interface.
		/// All objects may have different types.</param>
		/// <param name="throwException">If true, throws an exception if object can not be created.</param>
		/// <returns>An array of object which implements the interface.</returns>
		public static TI?[] Implement<TI>(bool throwException, params object[] objects)
			where TI : class
		{
			if (objects == null) throw new ArgumentNullException(nameof(objects));

			TI?[] result = new TI[objects.Length];

			for (var i = 0; i < objects.Length; i++)
				result[i] = Implement<TI>(objects[i], throwException);

			return result;
		}

		/// <summary>
		/// Implements the requested interface for all supplied objects.
		/// If any of supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <typeparam name="TI">An interface type to implement.</typeparam>
		/// <param name="objects">An object array which types expected to have all members of the given interface.
		/// All objects may have different types.</param>
		/// <returns>An array of object which implements the interface.</returns>
		public static TI[] Implement<TI>(params object[] objects)
			where TI : class
		{
			if (objects == null) throw new ArgumentNullException(nameof(objects));

			var result = new TI[objects.Length];

			for (var i = 0; i < objects.Length; i++)
				result[i] = Implement<TI>(objects[i]);

			return result;
		}

		/// <summary>
		/// Implements the requested interface for all supplied objects.
		/// If any of supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <typeparam name="TI">An interface type to implement.</typeparam>
		/// <typeparam name="T">Any type which has all members of the given interface.</typeparam>
		/// <param name="objects">An object array which types expected to have all members of the given interface.
		/// All objects may have different types.</param>
		/// <param name="throwException">If true, throws an exception if object can not be created.</param>
		/// <returns>An array of object which implements the interface.</returns>
		public static TI?[] Implement<TI,T>(bool throwException, params T[] objects)
			where TI : class
		{
			if (objects == null) throw new ArgumentNullException(nameof(objects));

			TI?[] result = new TI[objects.Length];

			for (var i = 0; i < objects.Length; i++)
				result[i] = Implement<TI,T>(objects[i], throwException);

			return result;
		}

		/// <summary>
		/// Implements the requested interface for all supplied objects.
		/// If any of supplied object implements the interface, the object itself will be returned.
		/// Otherwise a convenient duck object will be created.
		/// </summary>
		/// <typeparam name="TI">An interface type to implement.</typeparam>
		/// <typeparam name="T">Any type which has all members of the given interface.</typeparam>
		/// <param name="objects">An object array which types expected to have all members of the given interface.
		/// All objects may have different types.</param>
		/// <returns>An array of object which implements the interface.</returns>
		public static TI[] Implement<TI,T>(params T[] objects)
			where TI : class
		{
			if (objects == null) throw new ArgumentNullException(nameof(objects));

			var result = new TI[objects.Length];

			for (var i = 0; i < objects.Length; i++)
				result[i] = Implement<TI,T>(objects[i]);

			return result;
		}

		public  static bool  AllowStaticMembers { get; set; }

		#endregion

		#region Multiple Duck

		/// <summary>
		/// Build a proxy type which implements the requested interface by redirecting all calls to the supplied object type.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="objectTypes">Array of types which expected to have all members of the given interface.</param>
		/// <returns>The duck object type.</returns>
		public static Type? GetDuckType(this Type interfaceType, Type[] objectTypes)
		{
			if (!interfaceType.IsInterface)
				throw new ArgumentException("'interfaceType' must be an interface.", nameof(interfaceType));
			//if (!interfaceType.IsPublic && !interfaceType.IsNestedPublic)
			//	throw new ArgumentException("The interface must be public.", nameof(interfaceType));

			Dictionary<object,Type?> types;

			lock (_duckTypes)
				if (!_duckTypes.TryGetValue(interfaceType, out types))
					_duckTypes.Add(interfaceType, types = new Dictionary<object,Type?>());

			var objects = new object[objectTypes.Length];

			for (var i = 0; i < objectTypes.Length; i++)
				objects[i] = objectTypes[i];

			Type?  type;
			object key = new CompoundValue(objects);

			lock (types) if (!types.TryGetValue(key, out type))
			{
				type = TypeFactory.GetType(
					new CompoundValue(interfaceType, key),
					interfaceType,
					new DuckTypeBuilder(MustImplementAttribute.Aggregate, interfaceType, objectTypes));

				types.Add(key, type);
			}

			return type;
		}

		/// <summary>
		/// Implements the requested interface from supplied set of objects.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="baseObjectTypes">Array of types which have all members of the given interface.
		/// When this parameter is set to null, the object type will be used.</param>
		/// <param name="objects">Array of objects which types expected to have all members of the given interface.</param>
		/// <param name="throwException">If true, throws an exception if object can not be created.</param>
		/// <returns>An object which implements the interface.</returns>
		public static object? Aggregate(this Type interfaceType, Type[]? baseObjectTypes, bool throwException, params object[] objects)
		{
			if (objects == null) throw new ArgumentNullException(nameof(objects));

			if (baseObjectTypes == null)
			{
				baseObjectTypes = new Type[objects.Length];

				for (var i = 0; i < objects.Length; i++)
					if (objects[i] != null)
						baseObjectTypes[i] = objects[i].GetType();
			}
			else
			{
				if (baseObjectTypes.Length != objects.Length)
					throw new ArgumentException("Invalid number of 'baseObjectTypes' or 'objects'.", nameof(baseObjectTypes));

				for (var i = 0; i < objects.Length; i++)
				{
					var objType = objects[i].GetType();

					if (!baseObjectTypes[i].IsSameOrParentOf(objType))
						throw new ArgumentException($"'{objType.FullName}' is not a subtype of '{baseObjectTypes[i].FullName}'.", nameof(objects));
				}
			}

			var duckType = GetDuckType(interfaceType, baseObjectTypes);

			if (duckType == null)
			{
				if (throwException)
					throw new TypeBuilderException($"Interface '{interfaceType}' cannot be implemented.");
				return null;
			}

			var duck = TypeAccessor.GetAccessor(duckType).CreateInstance();

			((DuckType)duck).SetObjects(objects);

			return duck;
		}

		/// <summary>
		/// Implements the requested interface from supplied set of objects.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="baseObjectTypes">Array of types which have all members of the given interface.
		/// When this parameter is set to null, the object type will be used.</param>
		/// <param name="objects">Array of objects which types expected to have all members of the given interface.</param>
		/// <returns>An object which implements the interface.</returns>
		public static object Aggregate(this Type interfaceType, Type[]? baseObjectTypes, params object[] objects)
		{
			var o = interfaceType.Aggregate(baseObjectTypes, true, objects);

			Debug.Assert(o != null, nameof(o) + " != null");

			return o;
		}

		/// <summary>
		/// Implements the requested interface from supplied set of objects.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="objects">Array of object which types expected to have of the given interface.</param>
		/// <param name="throwException">If true, throws an exception if object can not be created.</param>
		/// <returns>An object which implements the interface.</returns>
		public static object? Aggregate(Type interfaceType, bool throwException, params object[] objects)
		{
			return interfaceType.Aggregate(null, throwException, objects);
		}

		/// <summary>
		/// Implements the requested interface from supplied set of objects.
		/// </summary>
		/// <param name="interfaceType">An interface type to implement.</param>
		/// <param name="objects">Array of object which types expected to have of the given interface.</param>
		/// <returns>An object which implements the interface.</returns>
		public static object Aggregate(Type interfaceType, params object[] objects)
		{
			return interfaceType.Aggregate(null, objects);
		}

		/// <summary>
		/// Implements the requested interface from supplied set of objects.
		/// </summary>
		/// <typeparam name="TI">An interface type to implement.</typeparam>
		/// <param name="objects">Array of object which type expected to have all members of the given interface.</param>
		/// <param name="throwException">If true, throws an exception if object can not be created.</param>
		/// <returns>An object which implements the interface.</returns>
		public static TI? Aggregate<TI>(bool throwException, params object[] objects)
			where TI : class
		{
			return (TI?)Aggregate(typeof(TI), null, throwException, objects);
		}

		/// <summary>
		/// Implements the requested interface from supplied set of objects.
		/// </summary>
		/// <typeparam name="TI">An interface type to implement.</typeparam>
		/// <param name="objects">Array of object which type expected to have all members of the given interface.</param>
		/// <returns>An object which implements the interface.</returns>
		public static TI Aggregate<TI>(params object[] objects)
			where TI : class
		{
			return (TI)Aggregate(typeof(TI), null, objects);
		}

		#endregion
	}
}

#endif