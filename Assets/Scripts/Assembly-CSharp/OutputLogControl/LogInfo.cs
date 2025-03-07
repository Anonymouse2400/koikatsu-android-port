using System;
using System.Collections.Generic;
using MessagePack;

namespace OutputLogControl
{
	[MessagePackObject(true)]
	public class LogInfo
	{
		public const string LogTag = "OutputLog";

		private const string LogVersion = "1.0.0";

		public string tag { get; set; }

		public Version version { get; set; }

		public Dictionary<string, List<LogData>> dictLog { get; set; }

		public LogInfo()
		{
			tag = "OutputLog";
			version = new Version("1.0.0");
			dictLog = new Dictionary<string, List<LogData>>();
		}
	}
}
