using System;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
	public class fsForwardConverter : fsConverter
	{
		private string _memberName;

		public fsForwardConverter(fsForwardAttribute attribute)
		{
			_memberName = attribute.MemberName;
		}

		public override bool CanProcess(Type type)
		{
			throw new NotSupportedException("Please use the [fsForward(...)] attribute.");
		}

		private fsResult GetProperty(object instance, out fsMetaProperty property)
		{
			fsMetaProperty[] properties = fsMetaType.Get(Serializer.Config, instance.GetType()).Properties;
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i].MemberName == _memberName)
				{
					property = properties[i];
					return fsResult.Success;
				}
			}
			property = null;
			return fsResult.Fail("No property named \"" + _memberName + "\" on " + instance.GetType().CSharpName());
		}

		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			serialized = fsData.Null;
            FullSerializer.fsResult success = FullSerializer.fsResult.Success;
            fsMetaProperty property;
			fsResult fsResult = (success += GetProperty(instance, out property));
			if (fsResult.Failed)
			{
				return success;
			}
			object instance2 = property.Read(instance);
			return Serializer.TrySerialize(property.StorageType, instance2, out serialized);
		}

		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
            FullSerializer.fsResult success = FullSerializer.fsResult.Success;
            fsMetaProperty property;
			fsResult fsResult = (success += GetProperty(instance, out property));
			if (fsResult.Failed)
			{
				return success;
			}
			object result = null;
			if ((success += Serializer.TryDeserialize(data, property.StorageType, ref result)).Failed)
			{
				return success;
			}
			property.Write(instance, result);
			return success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return fsMetaType.Get(Serializer.Config, storageType).CreateInstance();
		}
	}
}
