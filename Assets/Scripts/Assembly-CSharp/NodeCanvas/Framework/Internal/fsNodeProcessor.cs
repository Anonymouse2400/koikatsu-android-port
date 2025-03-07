using System;
using System.Collections.Generic;
using System.Linq;
using ParadoxNotion;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;

namespace NodeCanvas.Framework.Internal
{
	public class fsNodeProcessor : fsObjectProcessor
	{
		public override bool CanProcess(Type type)
		{
			return typeof(Node).RTIsAssignableFrom(type);
		}

		public override void OnBeforeSerialize(Type storageType, object instance)
		{
		}

		public override void OnAfterSerialize(Type storageType, object instance, ref fsData data)
		{
		}

		public override void OnBeforeDeserialize(Type storageType, ref fsData data)
		{
			if (data.IsNull)
			{
				return;
			}
			Dictionary<string, fsData> json = data.AsDictionary;
			fsData value;
			if (!json.TryGetValue("$type", out value))
			{
				return;
			}
			Type type = ReflectionTools.GetType(value.AsString);
			if (type == null || type == typeof(MissingNode))
			{
				string targetFullTypeName = ((type != null) ? json["missingType"].AsString : value.AsString);
				Type[] allTypes = ReflectionTools.GetAllTypes();
				foreach (Type type2 in allTypes)
				{
					DeserializeFromAttribute deserializeFromAttribute = type2.RTGetAttribute<DeserializeFromAttribute>(false);
					if (deserializeFromAttribute != null && deserializeFromAttribute.previousTypeNames.Any((string n) => n == targetFullTypeName))
					{
						json["$type"] = new fsData(type2.FullName);
						return;
					}
				}
				string text = targetFullTypeName.Split('.').LastOrDefault();
				Type[] allTypes2 = ReflectionTools.GetAllTypes();
				foreach (Type type3 in allTypes2)
				{
					if (type3.Name == text && type3.IsSubclassOf(typeof(Node)))
					{
						json["$type"] = new fsData(type3.FullName);
						return;
					}
				}
			}
			if (type == null)
			{
				json["recoveryState"] = new fsData(data.ToString());
				json["missingType"] = new fsData(value.AsString);
				json["$type"] = new fsData(typeof(MissingNode).FullName);
			}
			if (type != typeof(MissingNode))
			{
				return;
			}
			Type type4 = ReflectionTools.GetType(json["missingType"].AsString);
			if (type4 != null)
			{
				string asString = json["recoveryState"].AsString;
				Dictionary<string, fsData> asDictionary = fsJsonParser.Parse(asString).AsDictionary;
				json = json.Concat(asDictionary.Where((KeyValuePair<string, fsData> kvp) => !json.ContainsKey(kvp.Key))).ToDictionary((KeyValuePair<string, fsData> c) => c.Key, (KeyValuePair<string, fsData> c) => c.Value);
				json["$type"] = new fsData(type4.FullName);
				data = new fsData(json);
			}
		}

		public override void OnAfterDeserialize(Type storageType, object instance)
		{
		}
	}
}
