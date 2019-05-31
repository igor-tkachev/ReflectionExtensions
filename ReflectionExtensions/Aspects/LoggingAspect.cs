#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Text;

namespace ReflectionExtensions.Aspects
{
	public delegate void LogOperation(InterceptCallInfo interceptCallInfo, LoggingAspect.Parameters parameters);
	public delegate void LogOutput   (string logText, string? fileName);

	public class LoggingAspect : Interceptor
	{
		public class Parameters
		{
			public string? FileName;
			public int     MinCallTime;
			public bool    LogExceptions;
			public bool    LogParameters;
		}

		string?             _instanceFileName;
		int?                _instanceMinCallTime;
		bool?               _instanceLogExceptions;
		bool?               _instanceLogParameters;
		readonly Parameters _parameters = new Parameters();

		public override void Init(CallMethodInfo info, string configString)
		{
			base.Init(info, configString);

			string[] ps = configString.Split(';');

			foreach (string p in ps)
			{
				string[] vs = p.Split('=');

				if (vs.Length == 2)
				{
					switch (vs[0].ToLower().Trim())
					{
						case "filename":      _instanceFileName      =            vs[1].Trim();  break;
						case "mincalltime":   _instanceMinCallTime   = int. Parse(vs[1].Trim()); break;
						case "logexceptions": _instanceLogExceptions = bool.Parse(vs[1].Trim()); break;
						case "logparameters": _instanceLogParameters = bool.Parse(vs[1].Trim()); break;
					}
				}
			}
		}

		protected override void OnFinally(InterceptCallInfo info)
		{
			if (IsEnabled)
			{
				_parameters.FileName      = _instanceFileName      ?? FileName;
				_parameters.MinCallTime   = _instanceMinCallTime   ?? MinCallTime;
				_parameters.LogExceptions = _instanceLogExceptions ?? LogExceptions;
				_parameters.LogParameters = _instanceLogParameters ?? LogParameters;

				LogOperation(info, _parameters);
			}
		}

		#region Parameters

		public static bool   LogParameters { get; set; } = true;
		public static bool   LogExceptions { get; set; } = true;
		public static int    MinCallTime   { get; set; }
		public static string FileName      { get; set; }
		public static bool   IsEnabled     { get; set; } = true;

		#endregion

		#region LogOperation

		public static LogOperation LogOperation { get; set; } = LogOperationInternal;

		static void LogOperationInternal(InterceptCallInfo info, Parameters parameters)
		{
			var end  = DateTime.Now;
			var time = (int)((end - info.BeginCallTime).TotalMilliseconds);

			if (info.Exception != null && parameters.LogExceptions ||
				info.Exception == null && time >= parameters.MinCallTime)
			{
				Debug.Assert(info.ParameterValues != null, "info.ParameterValues != null");

				string? callParameters = null;
				var     plen           = info.ParameterValues.Length;

				if (parameters.LogParameters && plen > 0)
				{
					var sb     = new StringBuilder();
					var values = info.ParameterValues;

					FormatParameter(values[0], sb);

					for (var i = 1; i < plen; i++)
					{
						FormatParameter(values[i], sb.Append(", "));
					}

					callParameters = sb.ToString();
				}

				string? exText = null;

				if (info.Exception != null)
					exText = $" with exception '{info.Exception.GetType().FullName}' - \"{info.Exception.Message}\"";

				LogOutput(
					string.Format("{0}: {1}.{2}({3}) - {4} ms{5}{6}.",
						end,
						info.CallMethodInfo?.MethodInfo.DeclaringType?.FullName,
						info.CallMethodInfo?.MethodInfo.Name,
						callParameters,
						time,
						info.Cached? " from cache": null,
						exText),
					parameters.FileName);
			}
		}

		static void FormatParameter(object parameter, StringBuilder sb)
		{
			if (parameter == null)
				sb.Append("<null>");
			else if (parameter is string)
				sb.Append('"').Append((string)parameter).Append('"');
			else if (parameter is char)
				sb.Append('\'').Append((char)parameter).Append('\'');
			else if (parameter is IEnumerable)
			{
				sb.Append('[');
				bool first = true;
				foreach (object item in (IEnumerable)parameter)
				{
					FormatParameter(item, first? sb: sb.Append(','));
					first = false;
				}
				sb.Append(']');
			}
			else if (parameter is IFormattable)
			{
				IFormattable formattable = (IFormattable)parameter;
				sb.Append(formattable.ToString(null, CultureInfo.InvariantCulture));
			}
			else
				sb.Append(parameter.ToString());
		}

		#endregion

		#region LogOuput

		public static LogOutput LogOutput { get; set; } = LogOutputInternal;

		private static void LogOutputInternal(string logText, string? fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				Debug.WriteLine(logText);
			else
				using (StreamWriter sw = new StreamWriter(fileName, true))
					sw.WriteLine(logText);
		}

		#endregion
	}
}

#endif
