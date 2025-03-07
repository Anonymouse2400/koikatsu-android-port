using System;
using System.Text;
using System.Xml;
using Illusion.Elements.Xml;
using UnityEngine;

namespace Manager
{
	public class KeyInput : Singleton<KeyInput>
	{
		public class Pad : Data
		{
			private enum SaveNames
			{
				Action = 0,
				Jump = 1,
				Crouch = 2,
				Battle = 3,
				Dush = 4,
				Icon = 5,
				Menu = 6,
				Map = 7
			}

			public class Element
			{
				public class Button
				{
					public string name = string.Empty;

					public string gameName = string.Empty;

					public KeyCode code;

					public Button(string name, string gameName, KeyCode code)
					{
						this.name = name;
						this.gameName = gameName;
						this.code = code;
					}
				}

				public Button[] elements = new Button[Enum.GetValues(typeof(SaveNames)).Length];

				public Button Action
				{
					get
					{
						return elements[0];
					}
					set
					{
						elements[0] = value;
					}
				}

				public Button Jump
				{
					get
					{
						return elements[1];
					}
					set
					{
						elements[1] = value;
					}
				}

				public Button Crouch
				{
					get
					{
						return elements[2];
					}
					set
					{
						elements[2] = value;
					}
				}

				public Button Battle
				{
					get
					{
						return elements[3];
					}
					set
					{
						elements[3] = value;
					}
				}

				public Button Dush
				{
					get
					{
						return elements[4];
					}
					set
					{
						elements[4] = value;
					}
				}

				public Button Icon
				{
					get
					{
						return elements[5];
					}
					set
					{
						elements[5] = value;
					}
				}

				public Button Menu
				{
					get
					{
						return elements[6];
					}
					set
					{
						elements[6] = value;
					}
				}

				public Button Map
				{
					get
					{
						return elements[7];
					}
					set
					{
						elements[7] = value;
					}
				}

				public void SetKey(int _key, KeyCode _code)
				{
					int num = -1;
					for (int i = 0; i < elements.Length; i++)
					{
						if (i != _key && elements[i].code == _code)
						{
							num = i;
							break;
						}
					}
					if (num == -1)
					{
						elements[_key].code = _code;
						return;
					}
					elements[num].code = elements[_key].code;
					elements[_key].code = _code;
				}
			}

			private const string KeyElementName = "Key";

			public Element data { get; private set; }

			public Pad(string elementName)
				: base(elementName)
			{
				data = new Element();
			}

			public override void Init()
			{
				data.Action = new Element.Button("まる", "決定・アクション・攻撃", KeyCode.JoystickButton1);
				data.Jump = new Element.Button("ばつ", "Cancel・ジャンプ", KeyCode.JoystickButton2);
				data.Crouch = new Element.Button("しかく", "しゃがむ", KeyCode.JoystickButton3);
				data.Icon = new Element.Button("さんかく", "選択対象切り替え", KeyCode.JoystickButton0);
				data.Battle = new Element.Button("L1", "通常、戦闘切り替え", KeyCode.JoystickButton6);
				data.Dush = new Element.Button("R1", "走り/ガード", KeyCode.JoystickButton7);
				data.Menu = new Element.Button("START", "メニュー", KeyCode.JoystickButton8);
				data.Map = new Element.Button("SELECT", "マップ", KeyCode.JoystickButton9);
			}

			public override void Read(string rootName, XmlDocument xml)
			{
				try
				{
					for (int i = 0; i < Enum.GetValues(typeof(SaveNames)).Length; i++)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(rootName);
						stringBuilder.Append("/");
						stringBuilder.Append(elementName);
						stringBuilder.Append("/");
						SaveNames saveNames = (SaveNames)i;
						stringBuilder.Append(saveNames.ToString());
						XmlNodeList xmlNodeList = xml.SelectNodes(stringBuilder.ToString());
						if (xmlNodeList == null || xmlNodeList[0] == null || !(data.elements[i].name == xmlNodeList[0].Attributes[0].LocalName))
						{
							continue;
						}
						data.elements[i].gameName = xmlNodeList[0].Attributes[0].Value;
						string innerText = xmlNodeList[0].SelectSingleNode("Key").InnerText;
						foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
						{
							if (value.ToString() == innerText)
							{
								data.elements[i].code = value;
								break;
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
				writer.WriteStartElement(elementName);
				for (int i = 0; i < Enum.GetValues(typeof(SaveNames)).Length; i++)
				{
					SaveNames saveNames = (SaveNames)i;
					writer.WriteStartElement(saveNames.ToString());
					writer.WriteAttributeString(data.elements[i].name, data.elements[i].gameName);
					writer.WriteElementString("Key", data.elements[i].code.ToString());
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
		}

		private const string UserPath = "Config";

		private const string FileName = "Key.xml";

		private const string RootName = "KeyConfig";

		private const string ElementName = "KeyButton";

		private Control xmlCtrl;

		public Pad _Pad { get; private set; }

		public void Reset()
		{
			xmlCtrl.Init();
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
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				_Pad = new Pad("KeyButton");
				xmlCtrl = new Control("Config", "Key.xml", "KeyConfig", _Pad);
				Load();
			}
		}
	}
}
