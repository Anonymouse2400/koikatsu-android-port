using ADV;
using Illusion.Game;
using UnityEngine;

namespace ActionGame
{
	public class BGMArea : MonoBehaviour
	{
		public class PlayInfo
		{
			public int BGMID { get; set; }

			public bool EnableVolumeModification { get; set; }

			public float Volume { get; set; }
		}

		[SerializeField]
		private BGM _bgm;

		public BGM BGM
		{
			get
			{
				return _bgm;
			}
			set
			{
				_bgm = value;
			}
		}

		private void Start()
		{
			if (!Program.isADVProcessing)
			{
				Utils.Sound.SettingBGM settingBGM = new Utils.Sound.SettingBGM(_bgm);
				settingBGM.isAssetEqualPlay = false;
				Utils.Sound.Play(settingBGM);
			}
		}
	}
}
