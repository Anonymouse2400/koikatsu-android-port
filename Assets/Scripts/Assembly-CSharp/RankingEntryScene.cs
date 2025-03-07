using Illusion.Extensions;
using Illusion.Game;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class RankingEntryScene : MonoBehaviour
{
	public string backSceneName = "Title";

	[SerializeField]
	private Canvas cvsChangeScene;

	[SerializeField]
	private InputField inpHandleName;

	[SerializeField]
	private GameObject objMsg01;

	[SerializeField]
	private GameObject objMsg02;

	[SerializeField]
	private Button btnYes;

	[SerializeField]
	private Button btnNo;

	private string handleName = string.Empty;

	private bool modeFirst = true;

	private void Start()
	{
		Singleton<Scene>.Instance.sceneFade.SortingOrder();
		if ("RankingScene" == backSceneName)
		{
			modeFirst = false;
			objMsg01.SetActiveIfDifferent(false);
			objMsg02.SetActiveIfDifferent(true);
		}
		else
		{
			modeFirst = true;
			objMsg01.SetActiveIfDifferent(true);
			objMsg02.SetActiveIfDifferent(false);
		}
		handleName = Singleton<Game>.Instance.rankSaveData.userName;
		inpHandleName.text = handleName;
		inpHandleName.ActivateInputField();
		inpHandleName.OnValueChangedAsObservable().Subscribe(delegate
		{
			handleName = inpHandleName.text;
		});
		Observable.EveryUpdate().Subscribe(delegate
		{
			btnYes.interactable = !handleName.IsNullOrEmpty();
		}).AddTo(this);
		btnYes.OnClickAsObservable().Subscribe(delegate
		{
			Singleton<Game>.Instance.rankSaveData.userName = handleName;
			Singleton<Game>.Instance.rankSaveData.Save();
			Utils.Sound.Play(SystemSE.ok_s);
			cvsChangeScene.gameObject.SetActive(true);
			if ("RankingScene" == backSceneName)
			{
				Singleton<Scene>.Instance.UnLoad();
			}
			else
			{
				Scene.Data data = new Scene.Data
				{
					levelName = "NetworkCheckScene",
					isAdd = false,
					isFade = true,
					isAsync = true,
					onLoad = delegate
					{
						NetworkCheckScene rootComponent = Scene.GetRootComponent<NetworkCheckScene>("NetworkCheckScene");
						if (!(rootComponent == null))
						{
							rootComponent.nextSceneName = "RankingScene";
						}
					}
				};
				Singleton<Scene>.Instance.LoadReserve(data, true);
			}
		});
		btnNo.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.cancel);
			if ("RankingScene" == backSceneName)
			{
				Singleton<Scene>.Instance.UnLoad();
			}
			else
			{
				Singleton<Scene>.Instance.LoadReserve(new Scene.Data
				{
					levelName = "Title",
					isFade = true
				}, false);
			}
		});
	}
}
