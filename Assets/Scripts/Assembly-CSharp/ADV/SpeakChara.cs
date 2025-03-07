using System;
using System.Collections.Generic;
using ActionGame.Chara;
using Manager;
using UniRx;
using UnityEngine;

namespace ADV
{
	public class SpeakChara : MonoBehaviour
	{
		public string bundleName = string.Empty;

		public string assetName = string.Empty;

		private Base _chara;

		[SerializeField]
		private TextScenario scenario;

		private GameObject _scenarioObject;

		private IDisposable disposable;

		public bool isOpen
		{
			get
			{
				return scenarioObject.activeSelf;
			}
		}

		private Base chara
		{
			get
			{
				return this.GetComponentCache(ref _chara);
			}
		}

		private GameObject scenarioObject
		{
			get
			{
				return this.GetCacheObject(ref _scenarioObject, () => scenario.gameObject);
			}
		}

		private void Initialize()
		{
			SaveData.Heroine heroine = chara.heroine;
			SaveData.Player player = Singleton<Game>.Instance.Player;
			List<Program.Transfer> list = new List<Program.Transfer>();
			Program.SetParam(player, heroine, list);
			Program.SetADVCharacter(0, heroine, scenario);
			if (player.chaCtrl != null)
			{
				Program.SetADVCharacter(1, player, scenario);
			}
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			if (actScene != null)
			{
				foreach (KeyValuePair<string, Func<string>> aDVVariable in actScene.Cycle.ADVVariables)
				{
					scenario.Vars[aDVVariable.Key] = new ValData(ValData.Convert(aDVVariable.Value(), typeof(string)));
				}
			}
			scenario.regulate.AddRegulate(Regulate.Control.ClickNext);
			scenario.regulate.AddRegulate(Regulate.Control.Skip);
			scenario.regulate.AddRegulate(Regulate.Control.AutoForce);
			list.Add(Program.Transfer.Open(bundleName, assetName, bool.FalseString));
			scenario.LoadBundleName = string.Empty;
			scenario.LoadAssetName = string.Empty;
			scenario.currentChara = new CharaData(heroine, chara.chaCtrl, scenario, null);
			scenario.heroineList = new List<SaveData.Heroine>(new SaveData.Heroine[1] { heroine });
			scenario.transferList = list;
		}

		public void Open()
		{
			scenarioObject.SetActive(true);
			Singleton<Voice>.Instance.Stop(chara.Head);
			disposable = scenario.OnInitializedAsync.Subscribe(delegate
			{
				Initialize();
			});
		}

		public void Close()
		{
			scenarioObject.SetActive(false);
			if (disposable != null)
			{
				disposable.Dispose();
			}
			disposable = null;
		}
	}
}
