using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Manager;
using UnityEngine;

public class LogoScene : BaseLoader
{
	[SerializeField]
	private float waitTime = 2f;

	private IEnumerator Start()
	{
		base.enabled = false;
		List<AudioClip> clipList = new List<AudioClip>();
		AssetBundleData.GetAssetBundleNameListFromPath("sound/data/systemse/brandcall/", true).ForEach(delegate(string file)
		{
			clipList.AddRange(AssetBundleManager.LoadAllAsset(file, typeof(AudioClip)).GetAllAssets<AudioClip>());
			AssetBundleManager.UnloadAssetBundle(file, true);
		});
		yield return new WaitWhile(() => Singleton<Scene>.Instance.sceneFade.IsFadeNow);
		clipList.RemoveAll((AudioClip p) => p == null);
		AudioClip clip = clipList.Shuffle().FirstOrDefault();
		AudioSource source = null;
		if (clip != null)
		{
			source = Singleton<Manager.Sound>.Instance.Play(Manager.Sound.Type.SystemSE, clip);
		}
		yield return new WaitForSecondsRealtime(waitTime);
		if (source != null)
		{
			yield return new WaitWhile(() => source.isPlaying);
			Object.Destroy(source.gameObject);
		}
		clipList.ForEach(Resources.UnloadAsset);
		clipList.Clear();
		clip = null;
		Singleton<Scene>.Instance.LoadReserve(new Scene.Data
		{
			levelName = "Title",
			isFade = true
		}, false);
		base.enabled = true;
	}
}
