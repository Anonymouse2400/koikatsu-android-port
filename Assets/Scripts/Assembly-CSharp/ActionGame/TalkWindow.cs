using Illusion.Game;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace ActionGame
{
	public class TalkWindow : MonoBehaviour
	{
		public static Transform LookTransform;

		[SerializeField]
		private Text text;

		private float timer;

		public Text Text
		{
			get
			{
				return text;
			}
		}

		public void Set(SaveData.CharaData chara, string text, AssetBundleData voice)
		{
			base.gameObject.SetActive(true);
			timer = 0f;
			Text.text = text;
			if (voice != null)
			{
				Utils.Voice.Setting setting = new Utils.Voice.Setting();
				setting.assetBundleName = voice.bundle;
				setting.assetName = voice.asset;
				SaveData.Heroine heroine = chara as SaveData.Heroine;
				if (heroine != null)
				{
					setting.no = heroine.FixCharaIDOrPersonality;
				}
				else
				{
					setting.no = chara.personality;
				}
				Utils.Voice.Play(setting);
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (LookTransform != null)
			{
				base.transform.LookAt(LookTransform);
				base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y + 180f, 0f);
			}
			timer = Mathf.Min(timer + Time.deltaTime, Manager.Config.TextData.AutoWaitTime);
			if (timer >= Manager.Config.TextData.AutoWaitTime)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
