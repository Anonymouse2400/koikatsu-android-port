using System;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
	public class fsTypeConverter : fsConverter
	{
		public override bool CanProcess(Type type)
		{
			return typeof(Type).IsAssignableFrom(type);
		}

		public override bool RequestCycleSupport(Type type)
		{
			return false;
		}

		public override bool RequestInheritanceSupport(Type type)
		{
			return false;
		}

		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			Type type = (Type)instance;
			serialized = new fsData(type.FullName);
			return fsResult.Success;
		}

		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			if (!data.IsString)
			{
				return fsResult.Fail("Type converter requires a string");
			}
			instance = fsTypeCache.GetType(data.AsString);
			if (instance == null)
			{
				return fsResult.Fail("Unable to find type " + data.AsString);
			}
			return fsResult.Success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return storageType;
		}
	}
}
