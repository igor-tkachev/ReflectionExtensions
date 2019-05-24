using System;

namespace ReflectionExtensions.Patterns
{
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property)]
	public sealed class MustImplementAttribute : Attribute
	{
		public MustImplementAttribute(bool implement, bool throwException, string? exceptionMessage)
		{
			Implement        = implement;
			ThrowException   = throwException;
			ExceptionMessage = exceptionMessage;
		}

		public MustImplementAttribute(bool implement, bool throwException)
			: this(implement, throwException, null)
		{
		}

		public MustImplementAttribute(bool implement, string exceptionMessage)
			: this(implement, true, exceptionMessage)
		{
		}

		public MustImplementAttribute(bool implement)
			: this(implement, true, null)
		{
		}

		public MustImplementAttribute()
			: this(true, true, null)
		{
		}

		public bool    Implement        { get; }
		public bool    ThrowException   { get; set; }
		public string? ExceptionMessage { get; set; }

		/// <summary>
		/// All methods are optional and throws <see cref="NotImplementedException"/> at run tune.
		/// </summary>
		public static readonly MustImplementAttribute Default   = new MustImplementAttribute(false, true, null);

		/// <summary>
		/// All methods are optional and does nothing at run tune.
		/// Return value and all output parameters will be set to appropriate default values.
		/// </summary>
		public static readonly MustImplementAttribute Aggregate = new MustImplementAttribute(false, false, null);
	}
}
