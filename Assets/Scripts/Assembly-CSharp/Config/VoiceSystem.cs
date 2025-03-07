using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Manager;
using UnityEngine;

namespace Config
{
	public class VoiceSystem : BaseSystem
	{
		public class Voice
		{
			public string file { get; private set; }

			public SoundData sound { get; private set; }

			public Voice(string file, SoundData sound)
			{
				this.file = file;
				this.sound = sound;
			}
		}

		public SoundData PCM = new SoundData();

		public Dictionary<int, Voice> chara;

		public VoiceSystem(string elementName, Dictionary<int, string> dic)
			: base(elementName)
		{
			PCM.ChangeEvent += delegate(SoundData sd)
			{
				float volume = sd.GetVolume();
				MixerVolume.Set(Manager.Voice.Mixer, MixerVolume.Names.PCMVolume, volume);
			};
			chara = dic.ToDictionary((KeyValuePair<int, string> v) => v.Key, (KeyValuePair<int, string> v) => new Voice(v.Value, new SoundData()));
			chara.Select((KeyValuePair<int, Voice> p) => new
			{
				sd = new
				{
					p.Key,
					p.Value.sound
				}
			}).ToList().ForEach(p =>
			{
				p.sd.sound.ChangeEvent += delegate(SoundData sd)
				{
					float volume2 = sd.GetVolume();
					int key = p.sd.Key;
					Singleton<Manager.Voice>.Instance.GetPlayingList(key).ForEach(delegate(AudioSource playing)
					{
						playing.volume = volume2;
					});
				};
			});
		}

		public override void Init()
		{
			PCM.Switch = true;
			PCM.Volume = 100;
			foreach (KeyValuePair<int, Voice> item in chara)
			{
				SoundData sound = item.Value.sound;
				sound.Switch = true;
				sound.Volume = 100;
			}
		}

		public override void Read(string rootName, XmlDocument xml)
		{
			try
			{
				XmlNodeList xmlNodeList = null;
				string text = rootName + "/" + elementName + "/";
				xmlNodeList = xml.SelectNodes(text + "PCM");
				if (xmlNodeList != null)
				{
					XmlElement xmlElement = xmlNodeList.Item(0) as XmlElement;
					if (xmlElement != null)
					{
						PCM.Parse(xmlElement.InnerText);
					}
				}
				foreach (KeyValuePair<int, Voice> item in chara)
				{
					xmlNodeList = xml.SelectNodes(text + item.Value.file);
					XmlElement xmlElement2 = xmlNodeList.Item(0) as XmlElement;
					if (xmlElement2 != null)
					{
						item.Value.sound.Parse(xmlElement2.InnerText);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public override void Write(XmlWriter writer)
		{
			writer.WriteStartElement(elementName);
			writer.WriteStartElement("PCM");
			writer.WriteValue(PCM);
			writer.WriteEndElement();
			foreach (KeyValuePair<int, Voice> item in chara)
			{
				Voice value = item.Value;
				writer.WriteStartElement(value.file);
				writer.WriteValue(value.sound);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
