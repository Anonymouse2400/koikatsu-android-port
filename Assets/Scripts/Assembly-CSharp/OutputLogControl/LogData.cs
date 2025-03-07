using MessagePack;

namespace OutputLogControl
{
	[MessagePackObject(true)]
	public class LogData
	{
		public int type { get; set; }

		public string time { get; set; }

		public string msg { get; set; }
	}
}
