using System.Text;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TestTestTest : BaseLoader
{
	[SerializeField]
	private Button btnRand;

	[SerializeField]
	private Text textRand;

	private bool get;

	private void Start()
	{
		btnRand.OnClickAsObservable().Subscribe(delegate
		{
			Singleton<Character>.Instance.nextNetworkScene = "RandomNetChara";
			Singleton<Character>.Instance.netRandCharaNum = 10;
			Singleton<Character>.Instance.netRandChara = null;
			get = true;
			Scene.Data data = new Scene.Data
			{
				levelName = "NetworkCheckScene",
				isAdd = true,
				isFade = false,
				isAsync = true
			};
			Singleton<Scene>.Instance.LoadReserve(data, true);
		});
		Observable.EveryUpdate().Subscribe(delegate
		{
			if (get && Singleton<Character>.Instance.netRandChara != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				ChaFileControl[] netRandChara = Singleton<Character>.Instance.netRandChara;
				foreach (ChaFileControl chaFileControl in netRandChara)
				{
					stringBuilder.Append(chaFileControl.parameter.fullname).Append("\r\n");
				}
				textRand.text = stringBuilder.ToString();
				get = false;
			}
		}).AddTo(this);
	}
}
