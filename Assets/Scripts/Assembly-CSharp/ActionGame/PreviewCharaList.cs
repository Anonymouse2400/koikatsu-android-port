using System;
using Localize.Translate;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ActionGame
{
	public class PreviewCharaList : BaseLoader
	{
		[SerializeField]
		private CustomFileListSelecter charFile;

		private BoolReactiveProperty canvasEnabled = new BoolReactiveProperty(false);

		public CustomFileListSelecter CharFile
		{
			get
			{
				return charFile;
			}
		}

		public bool isVisible
		{
			get
			{
				return canvasEnabled.Value;
			}
			set
			{
				canvasEnabled.Value = value;
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
			Canvas canvas = GetComponent<Canvas>();
			canvasEnabled.TakeUntilDestroy(this).Subscribe(delegate(bool isOn)
			{
				canvas.enabled = isOn;
			});
			CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
			this.ObserveEveryValueChanged((PreviewCharaList _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
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
				where canvas.enabled
				where Singleton<Scene>.Instance.NowSceneNames[0] == "ClassRoomSelect"
				where Input.GetMouseButtonDown(1)
				select _).Subscribe(delegate
			{
				this.onCancel();
			});
		}
	}
}
