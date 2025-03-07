using Illusion.CustomAttributes;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class spritestage : MonoBehaviour
{
	[Label("ステージメイン")]
	[SerializeField]
	private StageTest stageTest;

	public AudioSource audioSource;

	public Text text;

	public Button btnEnd;

	private void Start()
	{
		btnEnd.OnClickAsObservable().Subscribe(delegate
		{
			stageTest.PlayStop();
		});
	}

	private void Update()
	{
		if ((bool)audioSource && (bool)text)
		{
			text.text = audioSource.time.ToString("00.00");
		}
	}
}
