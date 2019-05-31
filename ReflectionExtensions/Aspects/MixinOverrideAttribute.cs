using System;

namespace ReflectionExtensions.Aspects
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=true)]
	public class MixinOverrideAttribute : Attribute
	{
		public MixinOverrideAttribute(Type? targetInterface, string? methodName)
		{
			TargetInterface = targetInterface;
			MethodName      = methodName;
		}

		public MixinOverrideAttribute(Type targetInterface)
			: this(targetInterface, null)
		{
		}

		public MixinOverrideAttribute(string methodName)
			: this(null, methodName)
		{
		}

		public MixinOverrideAttribute()
		{
		}

		public Type?   TargetInterface { get; set; }
		public string? MethodName      { get; set; }
	}
}
