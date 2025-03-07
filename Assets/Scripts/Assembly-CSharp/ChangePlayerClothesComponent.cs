using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Illusion.Game;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ChangePlayerClothesComponent : MonoBehaviour
{
	[SerializeField]
	private Toggle[] tglItem;

	private bool initialized;

	private IntReactiveProperty cloth;

	public bool isOpen
	{
		get
		{
			return base.gameObject.activeSelf;
		}
	}

	public void Open()
	{
		cloth.Value = Singleton<Game>.Instance.Player.changeClothesType;
		for (int i = 0; i < tglItem.Length; i++)
		{
			tglItem[i].isOn = i == cloth.Value + 1;
		}
		base.gameObject.SetActiveIfDifferent(true);
	}

	public void Close()
	{
		base.gameObject.SetActiveIfDifferent(false);
	}

	public void Initialize()
	{
		if (initialized)
		{
			return;
		}
		initialized = true;
		cloth = new IntReactiveProperty(Singleton<Game>.Instance.Player.changeClothesType);
		cloth.Skip(1).Subscribe(delegate(int i)
		{
			Singleton<Game>.Instance.Player.changeClothesType = i;
			Utils.Sound.Play(SystemSE.sel);
		});
		(from i in cloth.Skip(1)
			where i >= 0
			select i).Subscribe(delegate(int i)
		{
			ChaControl chaCtrl = Singleton<Game>.Instance.actScene.Player.chaCtrl;
			if (chaCtrl.fileStatus.coordinateType != i)
			{
				chaCtrl.ChangeCoordinateTypeAndReload((ChaFileDefine.CoordinateType)i);
			}
		});
		(from list in tglItem.Select(UnityUIComponentExtensions.OnValueChangedAsObservable).CombineLatest()
			select list.IndexOf(true) into i
			where i >= 0
			select i).Subscribe(delegate(int i)
		{
			cloth.Value = i - 1;
		});
	}

	[ContextMenu("Setup")]
	private void Setup()
	{
		tglItem = GetComponentsInChildren<Toggle>();
	}
}
