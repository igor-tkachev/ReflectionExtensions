#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.Aspects
{
	using TypeBuilder.Builders;

	[AttributeUsage(
		AttributeTargets.Class |
		AttributeTargets.Interface |
		AttributeTargets.Property |
		AttributeTargets.Method,
		AllowMultiple=true)]
	public class LogAttribute : InterceptorAttribute
	{
		public LogAttribute()
			: this(typeof(LoggingAspect), null)
		{
		}

		public LogAttribute(string parameters)
			: this(typeof(LoggingAspect), parameters)
		{
		}

		protected LogAttribute(Type interceptorType, string? parameters)
			: base(
				interceptorType,
				InterceptType.OnCatch | InterceptType.OnFinally,
				parameters,
				TypeBuilderConsts.Priority.LoggingAspect)
		{
		}

		private bool _hasFileName;
		private string? _fileName;
		public  string?  FileName
		{
			get => _fileName;
			set { _fileName = value; _hasFileName = true; }
		}

		private bool _hasMinCallTime;
		private int     _minCallTime;
		public  int      MinCallTime
		{
			get => _minCallTime;
			set { _minCallTime = value; _hasMinCallTime = true; }
		}

		private bool _hasLogExceptions;
		private bool    _logExceptions;
		public  bool     LogExceptions
		{
			get => _logExceptions;
			set { _logExceptions = value; _hasLogExceptions = true;}
		}

		private bool _hasLogParameters;
		private bool    _logParameters;
		public  bool     LogParameters
		{
			get => _logParameters;
			set { _logParameters = value; _hasLogParameters = true;}
		}

		public override string? ConfigString
		{
			get
			{
				var s = base.ConfigString;

				if (_hasFileName)      s += ";FileName="      + FileName;
				if (_hasMinCallTime)   s += ";MinCallTime="   + MinCallTime;
				if (_hasLogExceptions) s += ";LogExceptions=" + LogExceptions;
				if (_hasLogParameters) s += ";LogParameters=" + LogParameters;

				if (!string.IsNullOrEmpty(s) && s[0] == ';')
					s = s.Substring(1);

				return s;
			}
		}
	}
}

#endif
