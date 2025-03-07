using System;
using System.Collections.Generic;
using System.Linq;
using ParadoxNotion.Serialization.FullSerializer;
using ParadoxNotion.Serialization.FullSerializer.Internal;

namespace ParadoxNotion.Serialization
{
	public class fsRecoveryProcessor<TCanProcess, TMissing> : fsObjectProcessor where TMissing : IMissingRecoverable, TCanProcess
	{
		public override bool CanProcess(Type type)
		{
			return typeof(TCanProcess).RTIsAssignableFrom(type);
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
			Type type = fsTypeCache.GetType(value.AsString, storageType);
			if (type == null)
			{
				json["missingType"] = new fsData(value.AsString);
				json["recoveryState"] = new fsData(data.ToString());
				json["$type"] = new fsData(typeof(TMissing).FullName);
			}
			if (type != typeof(TMissing))
			{
				return;
			}
			Type type2 = fsTypeCache.GetType(json["missingType"].AsString, storageType);
			if (type2 != null)
			{
				string asString = json["recoveryState"].AsString;
				Dictionary<string, fsData> asDictionary = fsJsonParser.Parse(asString).AsDictionary;
				json = json.Concat(asDictionary.Where((KeyValuePair<string, fsData> kvp) => !json.ContainsKey(kvp.Key))).ToDictionary((KeyValuePair<string, fsData> c) => c.Key, (KeyValuePair<string, fsData> c) => c.Value);
				json["$type"] = new fsData(type2.FullName);
				data = new fsData(json);
			}
		}
	}
}
