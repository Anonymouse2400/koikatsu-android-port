using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Illusion.Game;
using Illusion.Game.Extensions;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class CustomTitleTrial : MonoBehaviour
{
	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private GraphicRaycaster graphicRaycaster;

	private CanvasGroup canvasGroup;

	[SerializeField]
	private Button btnStart;

	[SerializeField]
	private Button btnUploader;

	[SerializeField]
	private Button btnDownloader;

	[SerializeField]
	private Button btnExit;

	public void OnCustom()
	{
		Enter("CustomScene");
	}

	public void OnUploader()
	{
		Enter("Uploader");
	}

	public void OnDownloader()
	{
		Enter("Downloader");
	}

	public void OnEnd()
	{
		Enter("Exit");
	}

	private void Enter(string next)
	{
		if (!graphicRaycaster.enabled)
		{
			return;
		}
		if (next != null)
		{
			if (!(next == "CustomScene"))
			{
				if (!(next == "Uploader"))
				{
					if (!(next == "Downloader"))
					{
						if (next == "Exit")
						{
							Singleton<Scene>.Instance.GameEnd();
						}
					}
					else
					{
						graphicRaycaster.enabled = false;
						Scene.Data data = new Scene.Data();
						data.levelName = "NetworkCheckScene";
						data.isAdd = false;
						data.isFade = true;
						data.isAsync = true;
						data.onLoad = delegate
						{
							NetworkCheckScene rootComponent = Scene.GetRootComponent<NetworkCheckScene>("NetworkCheckScene");
							if (!(rootComponent == null))
							{
								rootComponent.nextSceneName = "Downloader";
							}
						};
						Scene.Data data2 = data;
						Singleton<Scene>.Instance.LoadReserve(data2, true);
					}
				}
				else
				{
					graphicRaycaster.enabled = false;
					Scene.Data data = new Scene.Data();
					data.levelName = "NetworkCheckScene";
					data.isAdd = false;
					data.isFade = true;
					data.isAsync = true;
					data.onLoad = delegate
					{
						NetworkCheckScene rootComponent2 = Scene.GetRootComponent<NetworkCheckScene>("NetworkCheckScene");
						if (!(rootComponent2 == null))
						{
							rootComponent2.nextSceneName = "Uploader";
						}
					};
					Scene.Data data3 = data;
					Singleton<Scene>.Instance.LoadReserve(data3, true);
				}
			}
			else
			{
				graphicRaycaster.enabled = false;
				Scene.Data data = new Scene.Data();
				data.levelName = next;
				data.isAdd = false;
				data.isFade = true;
				data.isAsync = true;
				data.onLoad = delegate
				{
					CustomScene rootComponent3 = Scene.GetRootComponent<CustomScene>(next);
					if (!(rootComponent3 == null))
					{
						rootComponent3.modeNew = true;
						rootComponent3.modeSex = 1;
						rootComponent3.chaFileCtrl = null;
					}
				};
				Scene.Data data4 = data;
				Singleton<Scene>.Instance.LoadReserve(data4, true);
			}
		}
		Utils.Sound.Play(SystemSE.ok_s);
	}

	private void Start()
	{
		Scene.isReturnTitle = false;
		canvasGroup = canvas.GetComponent<CanvasGroup>();
		if (canvasGroup != null)
		{
			this.ObserveEveryValueChanged((CustomTitleTrial _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
			{
				canvasGroup.interactable = isOn;
			});
		}
		Singleton<Game>.Instance.NewGame();
		Singleton<Scene>.Instance.UnloadBaseScene();
		Singleton<Scene>.Instance.UnloadAddScene();
		if (Singleton<Character>.IsInstance())
		{
			Singleton<Character>.Instance.EndLoadAssetBundle();
		}
		btnStart.SelectSE();
		btnUploader.SelectSE();
		btnDownloader.SelectSE();
		btnExit.SelectSE();
		List<AudioClip> clipList = new List<AudioClip>();
		AssetBundleData.GetAssetBundleNameListFromPath("sound/data/systemse/titlecall/", true).ForEach(delegate(string file)
		{
			clipList.AddRange(AssetBundleManager.LoadAllAsset(file, typeof(AudioClip)).GetAllAssets<AudioClip>());
			AssetBundleManager.UnloadAssetBundle(file, true);
		});
		clipList.RemoveAll((AudioClip p) => p == null);
		AudioClip clip = clipList.Shuffle().FirstOrDefault();
		(from _ in (from _ in this.UpdateAsObservable()
				where !Singleton<Scene>.Instance.IsFadeNow
				select _).Take(1)
			where clip != null
			select _).Subscribe(delegate
		{
			Utils.Sound.Play(Manager.Sound.Type.SystemSE, clip).OnDestroyAsObservable().Subscribe(delegate
			{
				clipList.ForEach(Resources.UnloadAsset);
				clipList.Clear();
				clip = null;
			});
		});
	}
}
