using System;

namespace MessagePack
{
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
	public class UnionAttribute : Attribute
	{
		public int Key { get; private set; }

		public Type SubType { get; private set; }

		public UnionAttribute(int key, Type subType)
		{
			Key = key;
			SubType = subType;
		}
	}
}
