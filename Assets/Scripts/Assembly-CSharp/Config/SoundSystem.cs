using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using Manager;

namespace Config
{
	public class SoundSystem : BaseSystem
	{
		public SoundData Master = new SoundData();

		public SoundData BGM = new SoundData();

		public SoundData ENV = new SoundData();

		public SoundData SystemSE = new SoundData();

		public SoundData GameSE = new SoundData();

		public SoundData AutoBGM = new SoundData();

		public SoundData[] Sounds
		{
			get
			{
				return new SoundData[6] { Master, BGM, ENV, SystemSE, GameSE, AutoBGM };
			}
		}

		public SoundSystem(string elementName)
			: base(elementName)
		{
			Master.ChangeEvent += delegate(SoundData sd)
			{
				MixerVolume.Set(Manager.Sound.Mixer, MixerVolume.Names.MasterVolume, sd.GetVolume());
			};
			BGM.ChangeEvent += delegate(SoundData sd)
			{
				MixerVolume.Set(Manager.Sound.Mixer, MixerVolume.Names.BGMVolume, sd.GetVolume());
			};
			ENV.ChangeEvent += delegate(SoundData sd)
			{
				MixerVolume.Set(Manager.Sound.Mixer, MixerVolume.Names.ENVVolume, sd.GetVolume());
			};
			SystemSE.ChangeEvent += delegate(SoundData sd)
			{
				MixerVolume.Set(Manager.Sound.Mixer, MixerVolume.Names.SystemSEVolume, sd.GetVolume());
			};
			GameSE.ChangeEvent += delegate(SoundData sd)
			{
				MixerVolume.Set(Manager.Sound.Mixer, MixerVolume.Names.GameSEVolume, sd.GetVolume());
			};
		}

		public override void Init()
		{
			SoundData[] sounds = Sounds;
			foreach (SoundData soundData in sounds)
			{
				soundData.Switch = true;
			}
			Master.Volume = 100;
			BGM.Volume = 40;
			ENV.Volume = 80;
			SystemSE.Volume = 50;
			GameSE.Volume = 70;
			AutoBGM.Volume = 40;
		}

		public override void Read(string rootName, XmlDocument xml)
		{
			try
			{
				XmlNodeList xmlNodeList = null;
				FieldInfo[] fieldInfos = base.FieldInfos;
				for (int i = 0; i < fieldInfos.Length; i++)
				{
					string xpath = rootName + "/" + elementName + "/" + fieldInfos[i].Name;
					xmlNodeList = xml.SelectNodes(xpath);
					if (xmlNodeList != null)
					{
						XmlElement xmlElement = xmlNodeList.Item(0) as XmlElement;
						if (xmlElement != null)
						{
							Sounds[i].Parse(xmlElement.InnerText);
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public override void Write(XmlWriter writer)
		{
			FieldInfo[] fieldInfos = base.FieldInfos;
			writer.WriteStartElement(elementName);
			for (int i = 0; i < fieldInfos.Length; i++)
			{
				writer.WriteStartElement(fieldInfos[i].Name);
				writer.WriteValue(Sounds[i]);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		[Conditional("OUTPUT_LOG")]
		private void Log(string str)
		{
		}
	}
}
