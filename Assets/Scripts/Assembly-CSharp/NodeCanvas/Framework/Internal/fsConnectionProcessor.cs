using System;
using System.Collections.Generic;
using System.Linq;
using ParadoxNotion;
using ParadoxNotion.Serialization.FullSerializer;

namespace NodeCanvas.Framework.Internal
{
	public class fsConnectionProcessor : fsObjectProcessor
	{
		public override bool CanProcess(Type type)
		{
			return typeof(Connection).RTIsAssignableFrom(type);
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
			if (type == null || type == typeof(MissingConnection))
			{
				string text = ((type != null) ? json["missingType"].AsString : value.AsString);
				string text2 = text.Split('.').LastOrDefault();
				Type[] allTypes = ReflectionTools.GetAllTypes();
				foreach (Type type2 in allTypes)
				{
					if (type2.Name == text2 && type2.IsSubclassOf(typeof(Connection)))
					{
						json["$type"] = new fsData(type2.FullName);
						return;
					}
				}
			}
			if (type == null)
			{
				json["recoveryState"] = new fsData(data.ToString());
				json["missingType"] = new fsData(value.AsString);
				json["$type"] = new fsData(typeof(MissingConnection).FullName);
			}
			if (type != typeof(MissingConnection))
			{
				return;
			}
			Type type3 = ReflectionTools.GetType(json["missingType"].AsString);
			if (type3 != null)
			{
				string asString = json["recoveryState"].AsString;
				Dictionary<string, fsData> asDictionary = fsJsonParser.Parse(asString).AsDictionary;
				json = json.Concat(asDictionary.Where((KeyValuePair<string, fsData> kvp) => !json.ContainsKey(kvp.Key))).ToDictionary((KeyValuePair<string, fsData> c) => c.Key, (KeyValuePair<string, fsData> c) => c.Value);
				json["$type"] = new fsData(type3.FullName);
				data = new fsData(json);
			}
		}

		public override void OnAfterDeserialize(Type storageType, object instance)
		{
		}
	}
}
