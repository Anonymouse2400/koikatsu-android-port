using System;
using Illusion.Game;
using Localize.Translate;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace FreeH
{
	public class FreeHPreviewCharaList : BaseLoader
	{
		[SerializeField]
		private CustomFileListSelecter charFile;

		public event Action<ChaFileControl> onEnter = delegate
		{
		};

		public event Action onCancel = delegate
		{
		};

		private void Start()
		{
			Canvas canvas = GetComponent<Canvas>();
			CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
			this.ObserveEveryValueChanged((FreeHPreviewCharaList _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
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
				where Singleton<Scene>.Instance.NowSceneNames[0] == "FreeHCharaSelectFemale" || Singleton<Scene>.Instance.NowSceneNames[0] == "FreeHCharaSelectMale" || Singleton<Scene>.Instance.NowSceneNames[0] == "LiveCharaSelectFemale"
				where Input.GetMouseButtonDown(1)
				select _).Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.cancel);
				this.onCancel();
			});
		}
	}
}
