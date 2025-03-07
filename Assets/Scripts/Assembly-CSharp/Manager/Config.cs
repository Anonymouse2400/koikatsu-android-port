using System.Diagnostics;
using Config;
using Illusion.Elements.Xml;
using UnityEngine;

namespace Manager
{
	public sealed class Config : Singleton<Config>
	{
		private const string UserPath = "config";

		private const string FileName = "system.xml";

		private const string RootName = "System";

		private Control xmlCtrl;

		public static bool initialized { get; private set; }

		public static SoundSystem SoundData { get; private set; }

		public static TextSystem TextData { get; private set; }

		public static ActionSystem ActData { get; private set; }

		public static EtceteraSystem EtcData { get; private set; }

		public static AdditionalFunctionsSystem AddData { get; private set; }

		public static DebugSystem DebugStatus { get; private set; }

		public void Reset()
		{
			if (xmlCtrl != null)
			{
				xmlCtrl.Init();
			}
		}

		public void Load()
		{
			xmlCtrl.Read();
		}

		public void Save()
		{
			xmlCtrl.Write();
		}

		protected override void Awake()
		{
			if (CheckInstance())
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		private void Start()
		{
			SoundData = new SoundSystem("Sound");
			TextData = new TextSystem("Text");
			ActData = new ActionSystem("Act");
			EtcData = new EtceteraSystem("Etc");
			AddData = new AdditionalFunctionsSystem("Add");
			DebugStatus = new DebugSystem("Debug");
			xmlCtrl = new Control("config", "system.xml", "System", SoundData, TextData, ActData, EtcData, AddData, DebugStatus);
			Load();
			initialized = true;
		}

		[Conditional("__DEBUG_PROC__")]
		private void DBLog(object o)
		{
		}
	}
}
