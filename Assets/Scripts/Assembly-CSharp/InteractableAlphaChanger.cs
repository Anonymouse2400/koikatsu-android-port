using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

internal class InteractableAlphaChanger : MonoBehaviour
{
	[SerializeField]
	[Header("Interactable参照用トグル")]
	private Toggle flagToggle;

	[SerializeField]
	[Header("カラー変更対象")]
	private TextMeshProUGUI targetTextMesh;

	[SerializeField]
	[Header("無効時の色")]
	[Range(0f, 1f)]
	private float offAlpha = 0.3f;

	private void Awake()
	{
		if (flagToggle == null)
		{
			Object.Destroy(this);
			return;
		}
		if (targetTextMesh == null)
		{
			targetTextMesh = GetComponent<TextMeshProUGUI>();
		}
		if (targetTextMesh == null)
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		Color baseColor = targetTextMesh.color;
		BoolReactiveProperty isInteract = new BoolReactiveProperty(flagToggle.interactable);
		isInteract.Subscribe(delegate(bool isOn)
		{
			Color color = baseColor;
			if (!isOn)
			{
				color = targetTextMesh.color;
				color.a = offAlpha;
			}
			targetTextMesh.color = color;
		});
		this.OnEnableAsObservable().Subscribe(delegate
		{
			isInteract.Value = flagToggle.interactable;
		});
		(from _ in this.UpdateAsObservable()
			select flagToggle.interactable).DistinctUntilChanged().Subscribe(delegate(bool interactable)
		{
			isInteract.Value = interactable;
		});
	}
}
