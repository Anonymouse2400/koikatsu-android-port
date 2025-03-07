using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

namespace Config
{
	public class AdditionalFunctionsSystem : BaseSystem
	{
		public class CheatPropertyInt
		{
			public bool isON;

			public int property;

			public static implicit operator string(CheatPropertyInt _data)
			{
				return string.Format("isON[{0}] : property[{1}]", _data.isON, _data.property);
			}

			public void Parse(string _str)
			{
				Match match = Regex.Match(_str, "isON\\[([a-zA-Z]*)\\] : property\\[([0-9]*)\\]");
				if (match.Success)
				{
					isON = bool.Parse(match.Groups[1].Value);
					property = int.Parse(match.Groups[2].Value);
				}
			}
		}

		public CheatPropertyInt expH = new CheatPropertyInt
		{
			isON = false,
			property = 1
		};

		public bool firstHEasy;

		public bool immediatelyFinish = true;

		public bool ADVEventNotOmission;

		public bool TalkTimeNoneWalkStop = true;

		public bool AINotPlayerTarget;

		public bool AINotPlayerTargetCommunication;

		public bool mobVisible = true;

		public Color mobColor = new Color(0f, 0f, 1f, 0.5f);

		public bool OtherClassRegisterMax;

		public bool DateMapSelectNoneEvent;

		public int AIActionCorrectionH = 100;

		public int AIActionCorrectionTalk = 100;

		public int CommunicationCorrectionHeroineAction = 100;

		public AdditionalFunctionsSystem(string elementName)
			: base(elementName)
		{
		}

		public override void Init()
		{
			expH = new CheatPropertyInt
			{
				isON = false,
				property = 1
			};
			firstHEasy = false;
			immediatelyFinish = true;
			ADVEventNotOmission = false;
			TalkTimeNoneWalkStop = true;
			AINotPlayerTarget = false;
			AINotPlayerTargetCommunication = false;
			mobVisible = true;
			mobColor = new Color(0f, 0f, 1f, 0.5f);
			OtherClassRegisterMax = false;
			DateMapSelectNoneEvent = false;
			AIActionCorrectionH = 100;
			AIActionCorrectionTalk = 100;
			CommunicationCorrectionHeroineAction = 100;
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
					if (xmlNodeList == null)
					{
						continue;
					}
					XmlElement xmlElement = xmlNodeList.Item(0) as XmlElement;
					if (xmlElement != null)
					{
						if (i == 0)
						{
							expH.Parse(xmlElement.InnerText);
						}
						else
						{
							fieldInfos[i].SetValue(this, BaseSystem.Cast(xmlElement.InnerText, fieldInfos[i].FieldType));
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
				if (i == 0)
				{
					writer.WriteValue(expH);
				}
				else
				{
					writer.WriteValue(BaseSystem.ConvertString(fieldInfos[i].GetValue(this)));
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
