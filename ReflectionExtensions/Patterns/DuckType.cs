using System;

namespace ReflectionExtensions.Patterns
{
	/// <summary>
	/// Reserved to internal BLToolkit use.
	/// </summary>
	public abstract class DuckType
	{
		// This column must be protected. Do not change.
		protected object[]? _objects;
		public    object[]   Objects
		{
			get         => _objects ?? throw new InvalidOperationException();
			private set => _objects = value;
		}

		internal void SetObjects(params object[] objects)
		{
			Objects = objects;
		}
	}
}
