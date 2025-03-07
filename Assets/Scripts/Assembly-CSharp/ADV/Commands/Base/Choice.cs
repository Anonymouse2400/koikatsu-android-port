using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Illusion.Game;
using Localize.Translate;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace ADV.Commands.Base
{
	public class Choice : CommandBase
	{
		public class ChoiceData
		{
			public readonly Transform transform;

			public readonly string text;

			public readonly string jump;

			public ChoiceData(Transform transform, string text, string jump)
			{
				this.transform = transform;
				this.text = text;
				this.jump = jump;
			}
		}

		private List<ChoiceData> choices = new List<ChoiceData>();

		private int id = -1;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Visible", "Case" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					bool.TrueString,
					"text,tag"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			base.scenario.ChoicesInit();
			int count = 0;
			if (!bool.Parse(args[count++]))
			{
				base.scenario.textController.Clear();
			}
			else if (!base.scenario.textController.MessageWindow.text.IsNullOrEmpty())
			{
				(base.scenario as MainScenario).SafeProc(delegate(MainScenario main)
				{
					main.BackLog.Logs.SafeProc(main.BackLog.Logs.Count - 1, delegate(BackLog.Data log)
					{
						if (!log.voiceList.IsNullOrEmpty())
						{
							base.scenario.currentCharaData.CreateVoiceList();
							base.scenario.currentCharaData.voiceList.AddRange(log.voiceList);
						}
					});
				});
			}
			RectTransform rectTransform = base.scenario.Choices.GetChild(0) as RectTransform;
			foreach (var item in args.Skip(count).Select((string s, int index) => new { s, index }))
			{
				if (!item.s.IsNullOrEmpty())
				{
					string[] array = item.s.Split(',');
					if (array.Length > 1)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(rectTransform.gameObject, base.scenario.Choices);
						gameObject.SetActive(true);
						gameObject.name = "Choice" + (item.index + 1);
						choices.Add(new ChoiceData(gameObject.transform, base.scenario.ReplaceText(array[0], true), base.scenario.ReplaceVars(array[1])));
					}
				}
			}
			base.scenario.isChoice = true;
			base.scenario.Choices.gameObject.SetActive(true);
			choices.ForEach(delegate(ChoiceData p)
			{
				p.transform.GetComponentInChildren<UnityEngine.UI.Text>().text = p.text;
				Button bt = p.transform.GetComponent<Button>();
				bt.onClick.RemoveAllListeners();
				bt.onClick.AddListener(delegate
				{
					id = ButtonProc(bt.name);
				});
				p.transform.gameObject.SetActive(true);
			});
			base.scenario.isSkip = base.scenario.isSkip && Manager.Config.TextData.ChoicesSkip;
			base.scenario.isAuto = base.scenario.isAuto && Manager.Config.TextData.ChoicesAuto;
		}

		public override bool Process()
		{
			base.Process();
			return id >= 0;
		}

		public override void Result(bool processEnd)
		{
			base.Result(processEnd);
			if (!processEnd || id < 0)
			{
				return;
			}
			string jump = choices[id].jump;
			string selectName = ((!Localize.Translate.Manager.isTranslate) ? "選択肢" : string.Empty);
			(base.scenario as MainScenario).SafeProc(delegate(MainScenario main)
			{
				main.BackLog.Add(new BackLog.Data(base.localLine, new Text.Data(selectName, choices[id].text), null));
			});
			foreach (Transform choice in base.scenario.Choices)
			{
				if (choice.gameObject.activeSelf)
				{
					UnityEngine.Object.Destroy(choice.gameObject);
				}
			}
			base.scenario.ChoicesInit();
			base.scenario.SearchTagJumpOrOpenFile(jump, base.localLine);
		}

		private static int ButtonProc(string btName)
		{
			Utils.Sound.Play(SystemSE.sel);
			int result = 0;
			try
			{
				int.TryParse(Regex.Replace(btName, "[^0-9]", string.Empty), out result);
			}
			catch (Exception)
			{
			}
			return result - 1;
		}
	}
}
