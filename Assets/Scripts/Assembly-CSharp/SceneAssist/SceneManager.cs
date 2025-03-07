using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SceneAssist
{
	public class SceneManager : MonoBehaviour
	{
		[SerializeField]
		private Image imageNowLoading;

		[SerializeField]
		private Slider sliderProgress;

		[SerializeField]
		private Image imageLoadingAnime;

		public Image ImageNowLoading
		{
			get
			{
				return imageNowLoading;
			}
		}

		public Slider SliderProgress
		{
			get
			{
				return sliderProgress;
			}
		}

		public Image ImageLoadingAnime
		{
			get
			{
				return imageLoadingAnime;
			}
		}

		public bool NowLoadingActive
		{
			set
			{
				if ((bool)imageNowLoading && imageNowLoading.gameObject.activeSelf != value)
				{
					imageNowLoading.gameObject.SetActive(value);
				}
			}
		}

		public bool ProgressActive
		{
			get
			{
				return sliderProgress != null && sliderProgress.IsActive();
			}
			set
			{
				if ((bool)sliderProgress && sliderProgress.gameObject.activeSelf != value)
				{
					sliderProgress.gameObject.SetActive(value);
				}
			}
		}

		public bool LoadingAnimeActive
		{
			set
			{
				if ((bool)imageLoadingAnime && imageLoadingAnime.enabled != value)
				{
					imageLoadingAnime.enabled = value;
				}
			}
		}

		public bool Active
		{
			set
			{
				if ((bool)imageNowLoading && imageNowLoading.gameObject.activeSelf != value)
				{
					imageNowLoading.gameObject.SetActive(value);
				}
				if ((bool)sliderProgress && sliderProgress.gameObject.activeSelf != value)
				{
					sliderProgress.gameObject.SetActive(value);
				}
				if ((bool)imageLoadingAnime && imageLoadingAnime.enabled != value)
				{
					imageLoadingAnime.enabled = value;
				}
			}
		}

		public float ProgressValue
		{
			get
			{
				return (!(sliderProgress != null)) ? 1f : sliderProgress.value;
			}
			set
			{
				if ((bool)sliderProgress)
				{
					sliderProgress.value = value;
				}
			}
		}

		public float NowLoadingAlpha
		{
			set
			{
				if ((bool)imageNowLoading)
				{
					Color color = imageNowLoading.color;
					color.a = value;
					imageNowLoading.color = color;
				}
			}
		}

		public float LoadingAnimeAlpha
		{
			set
			{
				if ((bool)imageLoadingAnime)
				{
					Color color = imageLoadingAnime.color;
					color.a = value;
					imageLoadingAnime.color = color;
				}
			}
		}

		public void SetAlpha(float _a)
		{
			NowLoadingAlpha = _a;
			LoadingAnimeAlpha = _a;
		}

		private void Reset()
		{
			imageNowLoading = GetComponentsInChildren<Image>().FirstOrDefault((Image p) => p.name == "NowLoading");
			sliderProgress = GetComponentsInChildren<Slider>().FirstOrDefault((Slider p) => p.name == "Progress");
			imageLoadingAnime = GetComponentsInChildren<Image>().FirstOrDefault((Image p) => p.name == "LoadingAnime");
		}
	}
}
