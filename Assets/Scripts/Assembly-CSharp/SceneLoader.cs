using System;
using System.Collections;
using Manager;
using UnityEngine;

public class SceneLoader : BaseLoader
{
	public string assetBundleName;

	public string levelName;

	public bool isLoad = true;

	public bool isAsync = true;

	public bool isFade = true;

	public bool isOverlap;

	public bool isLoadingImageDraw = true;

	public string manifestFileName;

	public Action onLoad;

	[SerializeField]
	protected bool isStartAfterErase = true;

	public Func<IEnumerator> onFadeIn { get; set; }

	public Func<IEnumerator> onFadeOut { get; set; }

	public Scene.Data.FadeType fadeType { get; set; }

	protected virtual void Start()
	{
		Scene.Data data = new Scene.Data();
		data.assetBundleName = assetBundleName;
		data.levelName = levelName;
		data.isAdd = !isLoad;
		data.isAsync = isAsync;
		data.isOverlap = isOverlap;
		data.manifestFileName = manifestFileName;
		data.onLoad = onLoad;
		data.onFadeIn = onFadeIn;
		data.onFadeOut = onFadeOut;
		Scene.Data data2 = data;
		if (isFade)
		{
			data2.isFade = isFade;
		}
		else
		{
			data2.fadeType = fadeType;
		}
		Singleton<Scene>.Instance.LoadReserve(data2, isLoadingImageDraw);
		if (isStartAfterErase)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
