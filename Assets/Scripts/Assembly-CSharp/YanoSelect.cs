using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class YanoSelect : BaseLoader
{
	[SerializeField]
	private Button btnCustomNewM;

	[SerializeField]
	private Button btnCustomNewF;

	private void Start()
	{
		if ((bool)btnCustomNewM)
		{
			btnCustomNewM.OnClickAsObservable().Subscribe(delegate
			{
				GoCustomScene(true, 0);
			});
		}
		if ((bool)btnCustomNewF)
		{
			btnCustomNewF.OnClickAsObservable().Subscribe(delegate
			{
				GoCustomScene(true, 1);
			});
		}
	}

	private void GoCustomScene(bool modeNew, byte modeSex, ChaFileControl chaFileCtrl = null)
	{
		Scene.Data data = new Scene.Data();
		data.levelName = "CustomScene";
		data.isAdd = false;
		data.isFade = true;
		data.isAsync = true;
		data.onLoad = delegate
		{
			CustomScene rootComponent = Scene.GetRootComponent<CustomScene>("CustomScene");
			if (!(rootComponent == null))
			{
				rootComponent.modeNew = modeNew;
				rootComponent.modeSex = modeSex;
				rootComponent.chaFileCtrl = chaFileCtrl;
			}
		};
		Scene.Data data2 = data;
		Singleton<Scene>.Instance.LoadReserve(data2, true);
	}
}
