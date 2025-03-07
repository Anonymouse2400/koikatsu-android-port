using System;
using Illusion.Game;
using Localize.Translate;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Wedding
{
	public class WeddingPreviewCharaList : BaseLoader
	{
		[SerializeField]
		private Canvas _canvas;

		[SerializeField]
		private CustomFileListSelecter charFile;

		public Canvas canvas
		{
			get
			{
				return _canvas;
			}
		}

		public CustomFileListSelecter CharFile
		{
			get
			{
				return charFile;
			}
		}

		public event Action<ChaFileControl> onEnter = delegate
		{
		};

		public event Action onCancel = delegate
		{
		};

		private void Start()
		{
			CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
			this.ObserveEveryValueChanged((WeddingPreviewCharaList _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
			{
				canvasGroup.interactable = isOn;
			});
			charFile.onEnter += delegate(ChaFileControl file)
			{
				this.onEnter(file);
			};
			charFile.onCancel += delegate
			{
				this.onCancel();
			};
			(from _ in this.UpdateAsObservable()
				where _canvas.enabled
				where Singleton<Scene>.Instance.NowSceneNames[0] == "WeddingCharaSelect"
				where Input.GetMouseButtonDown(1)
				select _).Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.cancel);
				this.onCancel();
			});
		}
	}
}
