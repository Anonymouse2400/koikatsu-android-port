using System.Collections;
using Illusion.Game;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PopupCheck : MonoBehaviour
{
	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private Button btnYes;

	[SerializeField]
	private Button btnNo;

	[SerializeField]
	private TextMeshProUGUI textMsg;

	private bool? answer;

	public IEnumerator CheckAnswerCor(IObserver<bool> observer, string msg)
	{
		if ((bool)textMsg)
		{
			textMsg.text = msg;
		}
		canvas.gameObject.SetActive(true);
		answer = null;
		while (true)
		{
			bool? flag = answer;
			if (flag.HasValue)
			{
				break;
			}
			yield return null;
		}
		bool? flag2 = answer;
		observer.OnNext(flag2.HasValue && flag2.Value);
		observer.OnCompleted();
		canvas.gameObject.SetActive(false);
	}

	private void Start()
	{
		if ((bool)btnYes)
		{
			btnYes.OnClickAsObservable().Subscribe(delegate
			{
				answer = true;
				Utils.Sound.Play(SystemSE.ok_s);
			}).AddTo(this);
		}
		if ((bool)btnNo)
		{
			btnNo.OnClickAsObservable().Subscribe(delegate
			{
				answer = false;
				Utils.Sound.Play(SystemSE.cancel);
			}).AddTo(this);
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(1))
		{
			Utils.Sound.Play(SystemSE.cancel);
			answer = false;
		}
	}
}
