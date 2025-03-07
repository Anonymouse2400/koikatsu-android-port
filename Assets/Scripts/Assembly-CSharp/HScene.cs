using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADV;
using ActionGame;
using ActionGame.H;
using FreeH;
using Illusion.Game;
using Illusion.Game.Elements.EasyLoader;
using IllusionUtility.GetUtility;
using Manager;
using UnityEngine;

public class HScene : BaseLoader
{
	public class AddParameter
	{
		public float[] aibus = new float[5];

		public float houshi;

		public float[] sonyus = new float[2];
	}

	public class InfoADVFile
	{
		public string assetbudle;

		public string adv;
	}

	public class ADVParam : SceneParameter
	{
		public bool isInVisibleChara = true;

		public float distanceCamera = 1f;

		public Vector3 correctionCamera = Vector3.zero;

		public ADVParam(MonoBehaviour scene)
			: base(scene)
		{
		}

		public override void Init(Data data)
		{
			ADVScene aDVScene = SceneParameter.advScene;
			TextScenario scenario = aDVScene.Scenario;
			scenario.BackCamera.fieldOfView = Camera.main.fieldOfView;
			scenario.LoadBundleName = data.bundleName;
			scenario.LoadAssetName = data.assetName;
			aDVScene.Stand.SetPositionAndRotation(data.position, data.rotation);
			scenario.heroineList = data.heroineList;
			scenario.transferList = data.transferList;
			if (!data.heroineList.IsNullOrEmpty())
			{
				for (int i = 0; i < data.heroineList.Count; i++)
				{
					SaveData.Heroine heroine = data.heroineList[i];
					MotionIK motionIK;
					Singleton<Game>.Instance.advAnimePack.SetDefalut(heroine.chaCtrl, out motionIK);
					IKMotion iKMotion = new IKMotion();
					iKMotion.Create(heroine.chaCtrl, motionIK);
					scenario.commandController.AddChara(i, new CharaData(heroine, heroine.chaCtrl, scenario, new CharaData.MotionReserver
					{
						ikMotion = iKMotion
					}, true, data.isParentChara));
				}
				scenario.ChangeCurrentChara(0);
			}
			if (!data.isParentChara)
			{
				var array = (from p in data.heroineList
					select p.chaCtrl into chaCtrl
					where chaCtrl != null
					select new
					{
						chaCtrl = chaCtrl,
						height = chaCtrl.GetShapeBodyValue(0),
						bust = chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.BUSTUP_TARGET).transform
					}).ToArray();
				var anon = array[0];
				Vector3 position = anon.chaCtrl.transform.position;
				array = array.OrderBy(p => p.height).ToArray();
				var array2 = array;
				foreach (var anon2 in array2)
				{
					anon2.chaCtrl.transform.position = position;
					anon2.chaCtrl.animBody.Update(0f);
				}
				anon = array[0];
				Transform bust = anon.bust;
				Vector3 position2 = bust.position;
				var anon3 = array.Last();
				position2.y += anon3.bust.position.y - bust.position.y;
				position2.y += Mathf.Lerp(0f, 0.05f, anon3.chaCtrl.GetShapeBodyValue(1));
				position2.y -= MotionIK.GetShapeLerpPositionValue(array.Average(p => p.height), new Vector3(0f, -0.09f, 0f), new Vector3(0f, 0.11f, 0f)).y;
				Vector3 targetPos = position2;
				position2 += bust.forward * distanceCamera;
				position2 += bust.TransformDirection(correctionCamera);
				Quaternion rotation = OpenData.CameraData.ToTargetRotation(position2, targetPos);
				Transform transform = anon.chaCtrl.transform;
				scenario.Characters.SetPositionAndRotation(transform.position, transform.rotation);
				scenario.BackCamera.transform.SetPositionAndRotation(position2, rotation);
			}
			scenario.Vars["Orgasm"] = new ValData(ValData.Convert((base.mono as HScene).orgasm, typeof(int)));
			float fadeInTime = data.fadeInTime;
			if (fadeInTime > 0f)
			{
				aDVScene.fadeTime = fadeInTime;
			}
			else
			{
				isInVisibleChara = false;
			}
		}

		public override void Release()
		{
			ADVScene aDVScene = SceneParameter.advScene;
			aDVScene.gameObject.SetActive(false);
		}

		public override void WaitEndProc()
		{
		}
	}

	private SaveData.Player player;

	private List<SaveData.Heroine> heroineList;

	private OpenHData.Data dataH;

	private int finishADV;

	private int orgasm;

	private bool isFreeH;

	private AddParameter addParameter = new AddParameter();

	private HFlag.EMode mode = HFlag.EMode.none;

	private Dictionary<int, InfoADVFile> dicADVInfo = new Dictionary<int, InfoADVFile>();

	protected override void Awake()
	{
		base.Awake();
		ADVParam aDVParam = new ADVParam(this);
		aDVParam.distanceCamera = 2.2f;
		aDVParam.correctionCamera = new Vector3(0f, -0.5f, 0f);
		ParameterList.Add(aDVParam);
	}

	private void OnDestroy()
	{
		ParameterList.Remove(this);
	}

	private IEnumerator Start()
	{
		if (Singleton<Scene>.IsInstance())
		{
			GameObject commonSpace = Singleton<Scene>.Instance.commonSpace;
			if (commonSpace != null)
			{
				OpenHData component = commonSpace.GetComponent<OpenHData>();
				if (component != null && component.data != null)
				{
					dataH = component.data as OpenHData.Data;
				}
				Object.Destroy(component);
			}
		}
		dataH.SafeProc(delegate(OpenHData.Data data)
		{
			isFreeH = data.isFreeH;
		});
		ActionScene actscene = Singleton<Game>.Instance.actScene;
		if ((bool)actscene)
		{
			actscene.isPenetration = false;
			dataH.newHeroione = actscene.GetHSceneOtherHeroine(dataH.lstFemale[0]);
		}
		dataH.isFirstPlayMasturbation = dataH.peepCategory.Any((int c) => MathfEx.IsRange(1100, c, 1199, true));
		string levelName = string.Empty;
		if (dataH != null && !dataH.peepCategory.Where((int c) => MathfEx.IsRange(2000, c, 2999, true) || MathfEx.IsRange(1010, c, 1099, true) || MathfEx.IsRange(1100, c, 1199, true)).Any() && !Singleton<Game>.Instance.glSaveData.tutorialHash.Contains(2))
		{
			Utils.Scene.OpenTutorial(2);
			yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
			yield return new WaitWhile(Utils.Scene.IsTutorial);
			if (Scene.isReturnTitle || Scene.isGameEnd)
			{
				yield break;
			}
			yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
		}
		int HScenePlayCount = 0;
		while (true)
		{
			levelName = "HProc";
			HSceneProc hSceneProc = null;
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = levelName,
				isAdd = true,
				isAsync = true,
				onLoad = delegate
				{
					hSceneProc = Scene.GetRootComponent<HSceneProc>(levelName);
					hSceneProc.dataH = dataH;
				}
			}, false);
			yield return new WaitWhile(() => hSceneProc == null);
			yield return StartCoroutine(HProcEndWait(hSceneProc));
			bool isEnd = (dataH != null && dataH.peepCategory.Where((int c) => MathfEx.IsRange(2000, c, 2002, true)).Any()) || mode == HFlag.EMode.peeping || mode == HFlag.EMode.masturbation || mode == HFlag.EMode.lesbian;
			if (isFreeH || (isEnd && (actscene == null || !actscene.isPenetration)))
			{
				if (mode == HFlag.EMode.masturbation || mode == HFlag.EMode.lesbian)
				{
					foreach (SaveData.Heroine item in dataH.lstFemale)
					{
						item.lewdness /= 2;
					}
				}
				Singleton<Scene>.Instance.UnLoad();
				if (isFreeH)
				{
					GameObject commonSpace2 = Singleton<Scene>.Instance.commonSpace;
					if (commonSpace2 != null)
					{
						FreeHBackData freeHBackData = commonSpace2.AddComponent<FreeHBackData>();
						freeHBackData.heroine = dataH.lstFemale[0];
						freeHBackData.partner = ((dataH.lstFemale.Count <= 1) ? null : dataH.lstFemale[1]);
						freeHBackData.player = dataH.player;
						freeHBackData.map = dataH.mapNoFreeH;
						freeHBackData.timeZone = dataH.timezoneFreeH;
						freeHBackData.statusH = dataH.statusFreeH;
						freeHBackData.stageH1 = dataH.stageFreeH1;
						freeHBackData.stageH2 = dataH.stageFreeH2;
						freeHBackData.discovery = dataH.isFound;
						freeHBackData.categorys = dataH.peepCategory;
					}
				}
				yield break;
			}
			yield return StartCoroutine(ResultTalk());
			if ((bool)actscene && !actscene.isPenetration)
			{
				break;
			}
			if (HScenePlayCount > 0)
			{
				if (mode != HFlag.EMode.houshi3P && mode != HFlag.EMode.sonyu3P)
				{
					break;
				}
				if ((bool)actscene)
				{
					foreach (SaveData.Heroine item2 in dataH.lstFemale)
					{
						actscene.actCtrl.SetDesire(5, item2, actscene.actCtrl.GetDesire(5, item2) / 2);
						item2.lewdness /= 2;
					}
				}
				Singleton<Scene>.Instance.UnLoad();
				yield break;
			}
			ActionMap map = null;
			if ((bool)actscene)
			{
				map = actscene.GetComponent<ActionMap>();
			}
			if (mode == HFlag.EMode.masturbation)
			{
				int pose = 0;
				bool isMasturbation = dataH.peepCategory.Any((int c) => c == 1010 || c == 1011);
				bool isPrivateRoomMasturbation = dataH.peepCategory.Any((int c) => MathfEx.IsRange(2000, c, 2999, true));
				if (isMasturbation || isPrivateRoomMasturbation)
				{
					pose = 1;
				}
				dataH.peepCategory = new List<int> { 0 };
				dataH.newHeroione = null;
				dataH.isLoadPeepLoom = false;
				if (isPrivateRoomMasturbation && (bool)map)
				{
					int changeMap = 45;
					if (map.no == 51)
					{
						changeMap = map.prevNo;
					}
					map.Change(changeMap, Scene.Data.FadeType.None);
					yield return new WaitUntil(() => !map.isMapLoading);
				}
				if (pose == 1)
				{
					GlobalMethod.HPointAppointNullDatail hPointAppointNullDatail = new GlobalMethod.HPointAppointNullDatail();
					hPointAppointNullDatail.mode = HFlag.EMode.aibu;
					hPointAppointNullDatail.idMap = (map ? map.no : 0);
					hPointAppointNullDatail.kindGet = 2;
					hPointAppointNullDatail.lstCategory = new List<int> { 0 };
					hPointAppointNullDatail.pos = dataH.position;
					GlobalMethod.HPointAppointNullDatail detail = hPointAppointNullDatail;
					GlobalMethod.HPointAppointNullGetData hPointAppointNull = GlobalMethod.GetHPointAppointNull(detail);
					dataH.position = hPointAppointNull.pos;
					dataH.rotation = hPointAppointNull.rot;
				}
			}
			else if (mode == HFlag.EMode.lesbian)
			{
				dataH.peepCategory = new List<int> { 3000 };
				dataH.newHeroione = null;
				dataH.kind = null;
				GlobalMethod.HPointAppointNullDatail hPointAppointNullDatail = new GlobalMethod.HPointAppointNullDatail();
				hPointAppointNullDatail.mode = HFlag.EMode.houshi3P;
				hPointAppointNullDatail.idMap = (map ? map.no : 0);
				hPointAppointNullDatail.kindGet = 2;
				hPointAppointNullDatail.lstCategory = new List<int> { 3000 };
				hPointAppointNullDatail.pos = dataH.position;
				GlobalMethod.HPointAppointNullDatail detail2 = hPointAppointNullDatail;
				GlobalMethod.HPointAppointNullGetData hPointAppointNull2 = GlobalMethod.GetHPointAppointNull(detail2);
				dataH.position = hPointAppointNull2.pos;
				dataH.rotation = hPointAppointNull2.rot;
			}
			else
			{
				dataH.peepCategory = new List<int> { 3000 };
				dataH.lstFemaleCoordinateType.Add(dataH.newHeroione.chaCtrl.fileStatus.coordinateType);
				dataH.newHeroione = null;
				dataH.kind = null;
				GlobalMethod.HPointAppointNullDatail hPointAppointNullDatail = new GlobalMethod.HPointAppointNullDatail();
				hPointAppointNullDatail.mode = HFlag.EMode.houshi3P;
				hPointAppointNullDatail.idMap = (map ? map.no : 0);
				hPointAppointNullDatail.kindGet = 2;
				hPointAppointNullDatail.lstCategory = new List<int> { 3000 };
				hPointAppointNullDatail.pos = dataH.position;
				GlobalMethod.HPointAppointNullDatail detail3 = hPointAppointNullDatail;
				GlobalMethod.HPointAppointNullGetData hPointAppointNull3 = GlobalMethod.GetHPointAppointNull(detail3);
				dataH.position = hPointAppointNull3.pos;
				dataH.rotation = hPointAppointNull3.rot;
			}
			dataH.clothStates.Clear();
			dataH.accessoryStates.Clear();
			dataH.lstFemaleCoordinateType.Clear();
			for (int i = 0; i < dataH.lstFemale.Count; i++)
			{
				dataH.clothStates.Add(new List<int>());
				for (int j = 0; j < 9; j++)
				{
					dataH.clothStates[i].Add(dataH.lstFemale[i].chaCtrl.fileStatus.clothesState[j]);
				}
				dataH.accessoryStates.Add(new List<bool>());
				for (int k = 0; k < dataH.lstFemale[i].chaCtrl.fileStatus.showAccessory.Length; k++)
				{
					dataH.accessoryStates[i].Add(dataH.lstFemale[i].chaCtrl.fileStatus.showAccessory[k]);
				}
				dataH.lstFemaleCoordinateType.Add(dataH.lstFemale[i].chaCtrl.fileStatus.coordinateType);
			}
			if (dataH.lstFemale[0].chaCtrl.hiPoly)
			{
				Singleton<Character>.Instance.DeleteChara(dataH.lstFemale[0].chaCtrl);
			}
			if (dataH.player.chaCtrl.hiPoly)
			{
				Singleton<Character>.Instance.DeleteChara(dataH.player.chaCtrl);
			}
			if (dataH.lstFemale.Count > 1 && dataH.lstFemale[1].chaCtrl.hiPoly)
			{
				Singleton<Character>.Instance.DeleteChara(dataH.lstFemale[1].chaCtrl);
			}
			Camera mainCamera = Camera.main;
			if ((bool)mainCamera)
			{
				Object.Destroy(mainCamera.transform.parent.gameObject);
			}
			HScenePlayCount++;
			dataH.hScenePlayCount = HScenePlayCount;
		}
		if (dataH.lstFemale.Count != 0 && (bool)dataH.lstFemale[0].chaCtrl)
		{
			dataH.lstFemale[0].chaCtrl.SetClothesStateAll(0);
			dataH.lstFemale[0].chaCtrl.SetAccessoryStateAll(true);
			dataH.lstFemale[0].chaCtrl.mouthCtrl.SafeProc(delegate(FBSCtrlMouth m)
			{
				m.OpenMin = 0f;
			});
			dataH.lstFemale[0].chaCtrl.ChangeLookNeckTarget(0);
			dataH.lstFemale[0].chaCtrl.ChangeLookEyesTarget(0);
		}
		levelName = "HSceneResult";
		HSceneResult hSceneResult = null;
		Singleton<Scene>.Instance.LoadReserve(new Scene.Data
		{
			levelName = levelName,
			isAdd = true,
			isAsync = true,
			onLoad = delegate
			{
				hSceneResult = Scene.GetRootComponent<HSceneResult>(levelName);
				hSceneResult.finishADV = finishADV;
				hSceneResult.lstHeroine = heroineList;
				hSceneResult.player = player;
				addParameter.aibus.CopyTo(hSceneResult.addParameter.aibus, 0);
				hSceneResult.addParameter.houshi = addParameter.houshi;
				addParameter.sonyus.CopyTo(hSceneResult.addParameter.sonyus, 0);
			}
		}, false);
		yield return new WaitWhile(() => hSceneResult == null);
		if (!Scene.isReturnTitle && !Scene.isGameEnd)
		{
			yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
			yield return StartCoroutine(HResult(hSceneResult));
			Singleton<Scene>.Instance.UnLoad();
		}
	}

	private IEnumerator HProcEndWait(HSceneProc hSceneProc)
	{
		yield return new WaitUntil(() => hSceneProc.isEnd);
		finishADV = hSceneProc.finishADV;
		orgasm = hSceneProc.countOrg;
		mode = hSceneProc.flags.mode;
		hSceneProc.addParameter.aibus.CopyTo(addParameter.aibus, 0);
		addParameter.houshi = hSceneProc.addParameter.houshi;
		hSceneProc.addParameter.sonyus.CopyTo(addParameter.sonyus, 0);
		yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
		yield return StartCoroutine(hSceneProc.ResetScene());
		yield return null;
		if ((bool)hSceneProc.flags.lstHeroine[0].chaCtrl)
		{
			hSceneProc.flags.lstHeroine[0].chaCtrl.ReSetupDynamicBoneBust();
		}
		heroineList = hSceneProc.flags.lstHeroine;
		player = hSceneProc.flags.player;
		ActionScene actscene = Singleton<Game>.Instance.actScene;
		if ((bool)actscene && actscene.isPenetration && (mode == HFlag.EMode.aibu || mode == HFlag.EMode.houshi || mode == HFlag.EMode.sonyu) && dataH.newHeroione != null)
		{
			yield return StartCoroutine(hSceneProc.CreateFemaleCharacterAppoint(dataH.newHeroione, dataH.newHeroione.chaCtrl.fileStatus.coordinateType));
			heroineList.Add(dataH.newHeroione);
		}
		hSceneProc.enabled = false;
		Singleton<Scene>.Instance.UnLoad();
	}

	private IEnumerator ResultTalk()
	{
		LoadADVInfo();
		List<Program.Transfer> listTransfer = new List<Program.Transfer> { Program.Transfer.Create(false, Command.SceneFadeRegulate, bool.FalseString) };
		SaveData.Heroine heroine = heroineList[0];
		Program.SetParam(player, heroine, listTransfer);
		InfoADVFile info;
		if (!dicADVInfo.TryGetValue(finishADV, out info))
		{
			GlobalMethod.DebugLog("Hシーン終了ADVの番号取得エラー [取得番号:" + finishADV + "]", 1);
			finishADV = 6;
			info = dicADVInfo[finishADV];
		}
		if (MathfEx.IsRange(100, finishADV, 199, true))
		{
			listTransfer.Add(Program.Transfer.Open(info.assetbudle, info.adv, bool.TrueString));
		}
		else
		{
			listTransfer.Add(Program.Transfer.Open(Program.FindADVBundleFilePath(info.adv, heroine), info.adv, bool.TrueString));
		}
		bool isParentChara = false;
		if (MathfEx.IsRange(200, finishADV, 299, true))
		{
			isParentChara = true;
		}
		Transform camT = Camera.main.transform;
		Object.Destroy(camT.GetComponent<CameraControl_Ver2>());
		yield return StartCoroutine(Program.Open(new Data
		{
			fadeInTime = 0f,
			position = Vector3.zero,
			rotation = Quaternion.identity,
			camera = new OpenData.CameraData
			{
				position = camT.position,
				rotation = camT.rotation
			},
			scene = this,
			transferList = listTransfer,
			heroineList = heroineList,
			isParentChara = isParentChara
		}));
		Camera mainCamera = Camera.main;
		if ((bool)mainCamera)
		{
			mainCamera.clearFlags = CameraClearFlags.Depth;
			mainCamera.cullingMask = 1 << LayerMask.NameToLayer("Chara");
			GameObject gameObject = mainCamera.transform.FindLoop("CameraMap");
			if ((bool)gameObject)
			{
				Camera component = gameObject.GetComponent<Camera>();
				if ((bool)component)
				{
					component.enabled = true;
				}
			}
			CameraEffector component2 = mainCamera.GetComponent<CameraEffector>();
			if ((bool)component2)
			{
				component2.config.SetFog(false);
			}
		}
		yield return Program.Wait("H");
	}

	private IEnumerator HResult(HSceneResult _hSceneResult)
	{
		yield return new WaitUntil(() => _hSceneResult.isEnd);
		yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
		Singleton<Scene>.Instance.UnLoad();
	}

	private bool LoadADVInfo()
	{
		dicADVInfo.Clear();
		string text = GlobalMethod.LoadAllListText("h/list/", "gotoADV");
		if (text.IsNullOrEmpty())
		{
			return false;
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			int result = 0;
			int.TryParse(data[i, num++], out result);
			InfoADVFile value;
			if (!dicADVInfo.TryGetValue(result, out value))
			{
				value = (dicADVInfo[result] = new InfoADVFile());
			}
			value.assetbudle = data[i, num++];
			value.adv = data[i, num++];
		}
		return true;
	}
}
