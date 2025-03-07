using Illusion.Game;
using UnityEngine;

internal class BGMLoader : BaseLoader
{
	[SerializeField]
	private BGM bgm;

	[SerializeField]
	private bool isAssetEqualPlay;

	[SerializeField]
	private float fadeTime = 0.8f;

	private void Start()
	{
		Utils.Sound.SettingBGM settingBGM = new Utils.Sound.SettingBGM(bgm);
		settingBGM.fadeTime = fadeTime;
		settingBGM.isAssetEqualPlay = isAssetEqualPlay;
		Utils.Sound.Play(settingBGM);
		Object.Destroy(base.gameObject);
	}
}
