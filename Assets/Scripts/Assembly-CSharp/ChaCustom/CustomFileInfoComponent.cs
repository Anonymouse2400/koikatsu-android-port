using FileListUI;
using Illusion.Extensions;
using TMPro;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomFileInfoComponent : ThreadFileInfoComponent
	{
		private CustomFileInfo _info;

		public Image imgAddInfo;

		public TextMeshProUGUI tmpTexClub;

		public TextMeshProUGUI tmpTexPersonality;

		public CustomFileInfo info
		{
			get
			{
				return this.GetCache(ref _info, () => base.fileInfo as CustomFileInfo);
			}
		}

		public void UpdateInfo(bool? isOn)
		{
			UpdateInfo();
			SetClub();
			SetPersonality();
			if (isOn.HasValue)
			{
				DisvisibleAddInfo(isOn.Value);
			}
		}

		public void DisvisibleAddInfo(bool isOn)
		{
			if (!(imgAddInfo == null))
			{
				imgAddInfo.gameObject.SetActiveIfDifferent(isOn);
			}
		}

		public void SetClub()
		{
			SetClub(info.club);
		}

		public void SetClub(string text)
		{
			if (!(tmpTexClub == null))
			{
				tmpTexClub.text = text;
			}
		}

		public void SetPersonality()
		{
			SetPersonality(info.personality);
		}

		public void SetPersonality(string text)
		{
			if (!(tmpTexPersonality == null))
			{
				tmpTexPersonality.text = text;
			}
		}
	}
}
