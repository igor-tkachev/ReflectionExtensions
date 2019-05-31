using System;
using System.Diagnostics;
using System.Reflection;

#if !NET20 && !NET30
using System.Linq.Expressions;
#endif

using JetBrains.Annotations;

namespace ReflectionExtensions.Reflection
{
	/// <summary>
	/// Provides fast access to a type member.
	/// </summary>
	[DebuggerDisplay("Name = {Name}, Type = {Type}")]
	[PublicAPI]
	public class MemberAccessor
	{
		// ReSharper disable once NotNullMemberIsNotInitialized
		internal MemberAccessor([NotNull] TypeAccessor typeAccessor, [NotNull] string memberName)
			: this(typeAccessor,
#if NET20 || NET30
			typeAccessor.Type.GetMemberEx(memberName)[0]
#else
			Expression.PropertyOrField(Expression.Constant(null, typeAccessor.Type), memberName).Member
#endif
				)
		{
		}

		static MemberInfo GetMemberInfo(TypeAccessor typeAccessor, string memberName)
		{
#if NET20 || NET30
			var memberInfo = typeAccessor.Type.GetMemberEx(memberName)[0];
#else
			var memberInfo = Expression.PropertyOrField(Expression.Constant(null, typeAccessor.Type), memberName).Member;
#endif

			return memberInfo;
		}

		internal MemberAccessor([NotNull] TypeAccessor typeAccessor, [NotNull] MemberInfo memberInfo)
		{
			TypeAccessor = typeAccessor;
			MemberInfo   = memberInfo;

			var propertyInfo = MemberInfo as PropertyInfo;

			Type = propertyInfo?.PropertyType ?? ((FieldInfo)MemberInfo).FieldType;

			propertyInfo = MemberInfo as PropertyInfo;

			if (propertyInfo != null)
			{
				HasGetter = propertyInfo.GetGetMethodEx(true) != null;
				HasSetter = propertyInfo.GetSetMethodEx(true) != null;
			}
			else
			{
				HasGetter = true;
				HasSetter = !((FieldInfo)MemberInfo).IsInitOnly;
			}

			if (HasGetter)
			{
#if NET20 || NET30

				if (propertyInfo != null)
				{
					_getter = o => propertyInfo.GetValue(o, null);
				}
				else
				{
					var fieldInfo = (FieldInfo)MemberInfo;
					_getter = o => fieldInfo.GetValue(o);
				}

#else
				var objParam = Expression.Parameter(typeof(object), "obj");
				var getter   = Expression.Lambda<Func<object,object?>>(
					Expression.Convert(
						Expression.MakeMemberAccess(
							Expression.Convert(objParam, TypeAccessor.InstanceType),
							memberInfo),
						typeof(object)),
					objParam);

				_getter = getter.Compile();
#endif
			}
			else
			{
				_getter = o => null;

				if (propertyInfo != null && typeAccessor.Type != typeAccessor.InstanceType)
				{
					var getMethod = typeAccessor.InstanceType.GetMethodEx("get_" + propertyInfo.Name);

					if (getMethod != null)
					{
						HasGetter = true;

#if NET20 || NET30

						_getter = o => getMethod.Invoke(o, new object[0]);

#else

						var objParam = Expression.Parameter(typeof(object), "obj");
						var getter   = Expression.Lambda<Func<object,object?>>(
							Expression.Convert(
								Expression.Call(
									Expression.Convert(objParam, TypeAccessor.InstanceType),
									getMethod),
								typeof(object)),
							objParam);

						_getter = getter.Compile();

#endif
					}
				}
			}

			if (HasSetter)
			{
#if NET20 || NET30 || NET35

				if (propertyInfo != null)
				{
					_setter = (o,v) => propertyInfo.SetValue(o, v, null);
				}
				else
				{
					var fieldInfo = (FieldInfo)MemberInfo;
					_setter = (o,v) => fieldInfo.SetValue(o,v);
				}

#else

				var objParam   = Expression.Parameter(typeof(object), "obj");
				var valueParam = Expression.Parameter(typeof(object?), "valueParam");
				var setter     = Expression.Lambda<Action<object,object?>>(
					Expression.Assign(
						Expression.MakeMemberAccess(
							Expression.Convert(objParam, TypeAccessor.InstanceType),
							memberInfo),
						Expression.Convert(valueParam, Type)),
					objParam,
					valueParam);

				_setter = setter.Compile();

#endif
			}
			else
			{
				_setter = (o,v) => { };

				if (propertyInfo != null && typeAccessor.Type != typeAccessor.InstanceType)
				{
					var setMethod = typeAccessor.InstanceType.GetMethodEx("set_" + propertyInfo.Name);

					if (setMethod != null)
					{
						HasSetter = true;

#if NET20 || NET30 || NET35

						_setter = (o,v) => setMethod.Invoke(o, new object?[] { v });

#else

						var objParam   = Expression.Parameter(typeof(object), "obj");
						var valueParam = Expression.Parameter(typeof(object?), "valueParam");
						var setter     = Expression.Lambda<Action<object,object?>>(
							Expression.Call(
								Expression.Convert(objParam, TypeAccessor.InstanceType),
								setMethod,
								Expression.Convert(valueParam, Type)),
							objParam,
							valueParam);

						_setter = setter.Compile();

#endif
					}
				}
			}
		}

		Func  <object,object?> _getter;
		Action<object,object?> _setter;

		#region Public Properties

		/// <summary>
		/// Member <see cref="MemberInfo"/>.
		/// </summary>
		public MemberInfo MemberInfo { get; }

		/// <summary>
		/// Parent <see cref="TypeAccessor"/>.
		/// </summary>
		public TypeAccessor TypeAccessor { get; }

		/// <summary>
		/// True, if the member has getter.
		/// </summary>
		public bool HasGetter { get; }

		/// <summary>
		/// True, if the member has setter.
		/// </summary>
		public bool HasSetter { get; }

		/// <summary>
		/// Member <see cref="Type"/>.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Member name.
		/// </summary>
		public string Name => MemberInfo.Name;

		/// <summary>
		/// Gets member value for provided object.
		/// </summary>
		/// <param name="o">Object to access.</param>
		/// <returns>Member value.</returns>
		public object? GetValue(object o) => _getter(o);

		/// <summary>
		/// Sets member value for provided object.
		/// </summary>
		/// <param name="o">Object to access.</param>
		/// <param name="value">Value to set.</param>
		public void SetValue(object o, object? value) => _setter(o, value);

		#endregion
	}
}
