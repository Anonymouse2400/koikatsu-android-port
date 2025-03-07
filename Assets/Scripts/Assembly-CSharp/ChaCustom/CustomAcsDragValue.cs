using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomAcsDragValue : MonoBehaviour
	{
		[SerializeField]
		private CustomAcsMoveWindow customAcsMoveWin;

		[SerializeField]
		private int type;

		[SerializeField]
		private int xyz;

		private Image imgCol;

		private void Awake()
		{
			imgCol = GetComponent<Image>();
		}

		private void Start()
		{
			float backMousePos = 0f;
			ObservableEventTrigger observableEventTrigger = base.gameObject.AddComponent<ObservableEventTrigger>();
			(from _ in this.UpdateAsObservable().SkipUntil(observableEventTrigger.OnPointerDownAsObservable().Do(delegate
				{
					backMousePos = Input.mousePosition.x;
					if ((bool)imgCol)
					{
						imgCol.color = new Color(imgCol.color.r, imgCol.color.g, imgCol.color.b, 1f);
					}
				})).TakeUntil(observableEventTrigger.OnPointerUpAsObservable().Do(delegate
				{
					customAcsMoveWin.UpdateHistory();
					if ((bool)imgCol)
					{
						imgCol.color = new Color(imgCol.color.r, imgCol.color.g, imgCol.color.b, 0f);
					}
				}))
					.RepeatUntilDestroy(this)
				select Input.mousePosition.x - backMousePos).Subscribe(delegate(float move)
			{
				backMousePos = Input.mousePosition.x;
				if (type == 0 && xyz == 0)
				{
					move *= -1f;
				}
				customAcsMoveWin.UpdateDragValue(type, xyz, move);
			}).AddTo(this);
		}
	}
}
