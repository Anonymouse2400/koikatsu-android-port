using System;
using System.Linq;
using ChaCustom;
using Illusion.CustomAttributes;
using Illusion.Game;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class LiveCharaSelectSprite : BaseLoader
{
	[SerializeField]
	[Label("ステージメイン")]
	private StageTest stageTest;

	[Label("ヒロイン")]
	public SaveData.Heroine heroine;

	[Label("マイク")]
	public GameObject objMic;

	[SerializeField]
	[Label("キャンバス")]
	private Canvas canvas;

	[SerializeField]
	[Label("服装切り替えウインドウ")]
	private clothesFileControl clothCtrl;

	[SerializeField]
	[Label("キャラ選択")]
	private Button btnCharaFile;

	[SerializeField]
	[Label("アイドルに戻す")]
	private Button btnIdolBack;

	[Label("決定")]
	[SerializeField]
	private Button btnEnter;

	[SerializeField]
	[Label("戻る")]
	private Button btnReturn;

	[SerializeField]
	[Label("校内")]
	private Button btnSchool;

	[SerializeField]
	[Label("下校")]
	private Button btnFromSchool;

	[SerializeField]
	[Label("体操")]
	private Button btnGymsuit;

	[Label("水着")]
	[SerializeField]
	private Button btnSwimsuit;

	[SerializeField]
	[Label("部活")]
	private Button btnClub;

	[SerializeField]
	[Label("私服")]
	private Button btnPlain;

	[SerializeField]
	[Label("お泊り")]
	private Button btnPajamas;

	[SerializeField]
	[Label("服装切り替え")]
	private Button btnClothFile;

	[SerializeField]
	[Label("アイドル服")]
	private Button btnIdolCloth;

	[Label("上履き")]
	[SerializeField]
	private Button btnInner;

	[SerializeField]
	[Label("下履き")]
	private Button btnOuter;

	[Label("マイク")]
	[SerializeField]
	private Toggle tglMic;

	[Label("カメラ")]
	[SerializeField]
	private CameraControl_Ver2 camCtrl;

	private void AddScene(string assetBundleName, string levelName, Action onLoad)
	{
		Singleton<Scene>.Instance.LoadReserve(new Scene.Data
		{
			assetBundleName = assetBundleName,
			levelName = levelName,
			isAdd = true,
			onLoad = onLoad
		}, false);
	}

	private void Start()
	{
		CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
		if (canvasGroup != null)
		{
			this.ObserveEveryValueChanged((LiveCharaSelectSprite _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
			{
				canvasGroup.interactable = isOn;
			});
		}
		clothCtrl.chaCtrl = heroine.chaCtrl;
		clothCtrl.kindSet = clothesFileControl.SettingKind.load;
		btnCharaFile.OnClickAsObservable().Subscribe(delegate
		{
			string levelName = "LiveCharaSelectFemale";
			AddScene("h/scene/freehcharaselect.unity3d", levelName, delegate
			{
				FreeHCharaSelect rootComponent = Scene.GetRootComponent<FreeHCharaSelect>(levelName);
				if (!(rootComponent == null))
				{
					rootComponent.camCtrl = camCtrl;
					if (camCtrl != null)
					{
						camCtrl.NoCtrlCondition = () => true;
					}
					rootComponent.ObserveEveryValueChanged((FreeHCharaSelect p) => p.heroine).Subscribe(delegate(SaveData.Heroine h)
					{
						if (h != null)
						{
							ChaFileControl chaFile = heroine.chaCtrl.chaFile;
							ChaFileControl charFile = h.charFile;
							chaFile.CopyCustom(charFile.custom);
							chaFile.CopyCoordinate(charFile.coordinate);
							chaFile.CopyParameter(charFile.parameter);
							heroine.chaCtrl.ChangeCoordinateType();
							heroine.chaCtrl.Reload();
						}
					});
				}
			});
		});
		btnIdolBack.OnClickAsObservable().Subscribe(delegate
		{
			heroine.charFile.LoadFromAssetBundle("action/fixchara/00.unity3d", "c-5");
			heroine.fixCharaID = -5;
			heroine.chaCtrl.ChangeCoordinateType();
			heroine.chaCtrl.Reload();
		});
		btnSchool.OnClickAsObservable().Subscribe(delegate
		{
			heroine.chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.School01);
		});
		btnFromSchool.OnClickAsObservable().Subscribe(delegate
		{
			heroine.chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.School02);
		});
		btnGymsuit.OnClickAsObservable().Subscribe(delegate
		{
			heroine.chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.Gym);
		});
		btnSwimsuit.OnClickAsObservable().Subscribe(delegate
		{
			heroine.chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.Swim);
		});
		btnClub.OnClickAsObservable().Subscribe(delegate
		{
			heroine.chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.Club);
		});
		btnPlain.OnClickAsObservable().Subscribe(delegate
		{
			heroine.chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.Plain);
		});
		btnPajamas.OnClickAsObservable().Subscribe(delegate
		{
			heroine.chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.Pajamas);
		});
		btnClothFile.OnClickAsObservable().Subscribe(delegate
		{
			clothCtrl.gameObject.SetActive(true);
		});
		btnIdolCloth.OnClickAsObservable().Subscribe(delegate
		{
			TextAsset ta = CommonLib.LoadAsset<TextAsset>("custom/cos_def_f_00.unity3d", "cos_idol", false, string.Empty);
			AssetBundleManager.UnloadAssetBundle("custom/cos_def_f_00.unity3d", true);
			heroine.chaCtrl.nowCoordinate.LoadFile(ta);
			heroine.chaCtrl.Reload(false, true, true, true);
		});
		btnInner.OnClickAsObservable().Subscribe(delegate
		{
			heroine.chaCtrl.fileStatus.shoesType = 0;
		});
		btnOuter.OnClickAsObservable().Subscribe(delegate
		{
			heroine.chaCtrl.fileStatus.shoesType = 1;
		});
		tglMic.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
		{
			objMic.SetActive(isOn);
		});
		Button[] source = new Button[2] { btnCharaFile, btnClothFile };
		Button[] source2 = new Button[11]
		{
			btnIdolBack, btnSchool, btnFromSchool, btnGymsuit, btnSwimsuit, btnClub, btnPlain, btnPajamas, btnIdolCloth, btnInner,
			btnOuter
		};
		source.ToList().ForEach(delegate(Button bt)
		{
			bt.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.window_o);
			});
		});
		source2.ToList().ForEach(delegate(Button bt)
		{
			bt.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
			});
		});
		btnEnter.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
		});
		tglMic.OnPointerClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.sel);
		});
		btnEnter.OnClickAsObservable().Subscribe(delegate
		{
			stageTest.PlayStart();
		});
		Action returnProc = delegate
		{
			Observable.NextFrame().Subscribe(delegate
			{
				Singleton<Scene>.Instance.UnLoad();
			});
		};
		btnReturn.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.cancel);
			returnProc();
		});
	}
}
