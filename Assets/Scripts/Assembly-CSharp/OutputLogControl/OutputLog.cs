using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MessagePack;
using UnityEngine;

namespace OutputLogControl
{
	public sealed class OutputLog
	{
		public static readonly string outputDir = Application.dataPath + "/";

		[Conditional("OUTPUT_LOG")]
		public static void Log(string msg, bool unityLog = false, string filename = "Log")
		{
			AddMessage(filename, msg, 0, unityLog);
		}

		[Conditional("OUTPUT_LOG")]
		public static void Warning(string msg, bool unityLog = false, string filename = "Log")
		{
			AddMessage(filename, msg, 1, unityLog);
		}

		[Conditional("OUTPUT_LOG")]
		public static void Error(string msg, bool unityLog = false, string filename = "Log")
		{
			AddMessage(filename, msg, 2, unityLog);
		}

		private static void AddMessage(string filename, string msg, byte type, bool unityLog = false)
		{
			if (unityLog)
			{
				switch (type)
				{
				case 0:
					UnityEngine.Debug.Log(msg);
					break;
				case 1:
					UnityEngine.Debug.LogWarning(msg);
					break;
				case 2:
					UnityEngine.Debug.LogError(msg);
					break;
				}
			}
			if (!Directory.Exists(outputDir))
			{
				Directory.CreateDirectory(outputDir);
			}
			string key = DateTime.Now.ToString("yyyy年MM月dd日");
			string time = DateTime.Now.ToString("HH:mm:ss");
			string path = outputDir + filename + ".mpt";
			LogInfo logInfo = new LogInfo();
			try
			{
				if (File.Exists(path))
				{
					byte[] array = File.ReadAllBytes(path);
					if (array != null)
					{
						logInfo = MessagePackSerializer.Deserialize<LogInfo>(array);
					}
				}
			}
			catch (Exception)
			{
				UnityEngine.Debug.LogWarning(string.Format("{0}:ファイルが読み込めない", filename));
			}
			List<LogData> value;
			if (!logInfo.dictLog.TryGetValue(key, out value))
			{
				value = new List<LogData>();
				logInfo.dictLog[key] = value;
			}
			LogData logData = new LogData();
			logData.time = time;
			logData.type = type;
			logData.msg = msg;
			value.Add(logData);
			byte[] bytes = MessagePackSerializer.Serialize(logInfo);
			File.WriteAllBytes(path, bytes);
		}
	}
}
