using ChaCustom;
using Illusion.Component;
using Manager;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class CustomScene : BaseLoader
{
	public bool modeNew = true;

	public byte modeSex = 1;

	public ChaFileControl chaFileCtrl;

	[SerializeField]
	private CustomControl customCtrl;

	[SerializeField]
	private CanvasGroup cgScene;

	public GameObject objDemo;

	public Image demo_fade;

	private void Start()
	{
		Singleton<Scene>.Instance.sceneFade.SortingOrder();
		ShortcutKey shortcutKey = base.gameObject.AddComponent<ShortcutKey>();
		ShortcutKey.Proc proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.F1;
		proc.enabled = false;
		ShortcutKey.Proc proc2 = proc;
		proc2.call.AddListener(shortcutKey._OpenConfig);
		shortcutKey.procList.Add(proc2);
		Singleton<CustomBase>.Instance.shortcutConfig = proc2;
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.F2;
		proc.enabled = false;
		proc2 = proc;
		shortcutKey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.F5;
		proc.enabled = false;
		proc2 = proc;
		shortcutKey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.F11;
		proc.enabled = false;
		proc2 = proc;
		shortcutKey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.Escape;
		proc.enabled = false;
		proc2 = proc;
		shortcutKey.procList.Add(proc2);
		Initialize();
		Singleton<Manager.Sound>.Instance.Stop(Manager.Sound.Type.SystemSE);
		cgScene.blocksRaycasts = true;
	}

	public new void Initialize()
	{
		Singleton<Character>.Instance.enableCorrectArmSize = true;
		Singleton<Character>.Instance.enableCorrectHandSize = true;
		customCtrl.Initialize(modeSex, modeNew);
		if (!modeNew)
		{
			customCtrl.backChaFileCtrl = chaFileCtrl;
		}
		ChaControl chaControl = LoadChara();
		customCtrl.Entry(chaControl, modeNew);
		Singleton<CustomBase>.Instance.motionIK = new MotionIK(chaControl);
		Singleton<CustomBase>.Instance.motionIK.SetPartners(Singleton<CustomBase>.Instance.motionIK);
		if (Singleton<CustomBase>.Instance.lstPose != null)
		{
			Singleton<CustomBase>.Instance.animeAssetBundleName = Singleton<CustomBase>.Instance.lstPose[0].list[1];
			Singleton<CustomBase>.Instance.animeAssetName = Singleton<CustomBase>.Instance.lstPose[0].list[2];
			Singleton<CustomBase>.Instance.LoadAnimation(Singleton<CustomBase>.Instance.animeAssetBundleName, Singleton<CustomBase>.Instance.animeAssetName);
			Singleton<CustomBase>.Instance.PlayAnimation(Singleton<CustomBase>.Instance.lstPose[0].list[4], -1f);
		}
	}

	private ChaControl LoadChara()
	{
		ChaFileControl chaFileControl = new ChaFileControl();
		ChaControl chaControl = null;
		Singleton<Character>.Instance.DeleteCharaAll();
		Singleton<Character>.Instance.BeginLoadAssetBundle();
		if (modeNew)
		{
			Singleton<Character>.Instance.editChara = null;
			chaControl = Singleton<Character>.Instance.CreateChara(modeSex, null, 0);
			chaControl.chaFile.pngData = null;
			chaControl.chaFile.facePngData = null;
		}
		else
		{
			modeSex = chaFileCtrl.parameter.sex;
			ChaFile.CopyChaFile(chaFileControl, chaFileCtrl);
			chaFileControl.pngData = chaFileCtrl.pngData;
			chaFileControl.facePngData = chaFileCtrl.facePngData;
			chaControl = Singleton<Character>.Instance.CreateChara(modeSex, null, 0, chaFileControl);
		}
		chaControl.releaseCustomInputTexture = false;
		chaControl.Load();
		chaControl.hideMoz = true;
		chaControl.fileStatus.visibleSon = false;
		return chaControl;
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		if (Singleton<CustomBase>.IsInstance())
		{
			Singleton<CustomBase>.Instance.customSettingSave.Save();
		}
		if (Singleton<Character>.IsInstance())
		{
			Singleton<Character>.Instance.chaListCtrl.SaveItemID();
			Singleton<Character>.Instance.DeleteCharaAll();
			Singleton<Character>.Instance.EndLoadAssetBundle();
			Singleton<Character>.Instance.enableCorrectArmSize = false;
			Singleton<Character>.Instance.enableCorrectHandSize = true;
		}
	}
}
