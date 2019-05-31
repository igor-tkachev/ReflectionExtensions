using System;
using System.Collections.Generic;


namespace ReflectionExtensions.Reflection
{
	public class InitContext
	{
		public object[]?      MemberParameters { get; set; }
		public object[]?      Parameters       { get; set; }
		public bool           IsInternal       { get; set; }
		public bool           IsLazyInstance   { get; set; }
		public object?        Parent           { get; set; }
//		public object         SourceObject     { get; set; }
//		public ObjectMapper   ObjectMapper     { get; set; }
//		public MappingSchema  MappingSchema    { get; set; }
		public bool           IsSource         { get; set; }
//		public bool           StopMapping      { get; set; }
//		public IMapDataSource DataSource       { get; set; }
//		public bool           IsLinqSource     { get; set; }

		private Dictionary<object,object>? _items;
		public  Dictionary<object,object>   Items
		{
			get { return _items ??= new Dictionary<object, object>(); }
		}

		public bool IsDestination
		{
			get => !IsSource;
			set => IsSource = !value;
		}
	}
}
