using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADV;
using ActionGame;
using ChaCustom;
using Illusion;
using Illusion.Extensions;
using Illusion.Game;
using Illusion.Game.Elements.EasyLoader;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Wedding
{
	public class WeddingScene : BaseLoader
	{
		[Serializable]
		private class UnionData
		{
			[SerializeField]
			private WeddingSelectBaseView _view;

			[SerializeField]
			private Vector3 _position = Vector3.zero;

			[SerializeField]
			private Vector3 _rotation = Vector3.zero;

			[SerializeField]
			private string exp;

			[SerializeField]
			private string state;

			[SerializeField]
			private string asset;

			[SerializeField]
			private string motionBundle;

			[SerializeField]
			private string ikBundle;

			public WeddingSelectBaseView view
			{
				get
				{
					return _view;
				}
			}

			public Vector3 position
			{
				get
				{
					return _position;
				}
			}

			public Quaternion rotation
			{
				get
				{
					return Quaternion.Euler(_rotation);
				}
			}

			public SaveData.CharaData charaData { get; set; }

			private int personality
			{
				get
				{
					return (chaCtrl.sex != 0) ? chaCtrl.fileParam.personality : (-100);
				}
			}

			public ChaControl chaCtrl { get; private set; }

			public Illusion.Game.Elements.EasyLoader.Motion motion { get; private set; }

			public IKMotion ikmotion { get; private set; }

			public void Initialize(SaveData.CharaData charaData, ChaControl chaCtrl)
			{
				this.charaData = charaData;
				this.chaCtrl = chaCtrl;
				charaData.SetRoot(chaCtrl.gameObject);
				LoadDefaultCoordinate(chaCtrl, false);
				chaCtrl.Load();
				motion = new Illusion.Game.Elements.EasyLoader.Motion(motionBundle, asset, state);
				ikmotion = new IKMotion(ikBundle, asset);
				chaCtrl.transform.SetPositionAndRotation(position, rotation);
				Play();
			}

			public void Play()
			{
				ChangeExp();
				ChangeMotion();
			}

			public void ChangeMotion()
			{
				if (motion.Setting(chaCtrl.animBody, motionBundle, asset, state, false))
				{
					motion.Play(chaCtrl.animBody);
				}
				CalcIK();
			}

			public void ChangeExp()
			{
				Game.Expression expression = Singleton<Game>.Instance.GetExpression(personality, exp);
				if (expression != null)
				{
					expression.Change(chaCtrl);
					chaCtrl.ChangeLookEyesPtn(0);
					chaCtrl.ChangeLookNeckPtn(3);
				}
			}

			public void CalcIK()
			{
				ikmotion.Setting(chaCtrl, ikBundle, asset, motion.state, false);
			}
		}

		private class ADVParam : SceneParameter
		{
			public bool isInVisibleChara = true;

			public ADVParam(MonoBehaviour scene)
				: base(scene)
			{
			}

			public override void Init(Data data)
			{
				ADVScene aDVScene = SceneParameter.advScene;
				TextScenario scenario = aDVScene.Scenario;
				WeddingScene weddingScene = base.mono as WeddingScene;
				aDVScene.Map = weddingScene.map;
				scenario.BackCamera.fieldOfView = Camera.main.fieldOfView;
				scenario.LoadBundleName = data.bundleName;
				scenario.LoadAssetName = data.assetName;
				aDVScene.Stand.SetPositionAndRotation(data.position, data.rotation);
				scenario.heroineList = data.heroineList;
				scenario.transferList = data.transferList;
				WeddingScene weddingScene2 = data.scene as WeddingScene;
				UnionData[] unionData = weddingScene2.unionData;
				foreach (UnionData unionData2 in unionData)
				{
					scenario.commandController.AddChara((unionData2.charaData is SaveData.Player) ? (-1) : 0, new CharaData(unionData2.charaData, unionData2.chaCtrl, scenario, new CharaData.MotionReserver
					{
						ikMotion = unionData2.ikmotion
					}, true, data.isParentChara));
				}
				scenario.ChangeCurrentChara(0);
				if (data.camera != null)
				{
					scenario.BackCamera.transform.SetPositionAndRotation(data.camera.position, data.camera.rotation);
				}
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

		private VoiceInfo.Param[] _personalityes;

		private TMP_Dropdown _dropdownPersonality;

		[SerializeField]
		private BGM bgm = BGM.Memories;

		[SerializeField]
		private WeddingView view;

		[SerializeField]
		private Transform initCameraTarget;

		[SerializeField]
		[Header("男設定")]
		private UnionData maleData;

		[SerializeField]
		[Header("女設定")]
		private UnionData femaleData;

		private ActionMap map;

		private CameraEffector cameraEffector;

		private UnionData[] unionData;

        private readonly BaseCameraControl_Ver2.NoCtrlFunc cameraNoCtrlCondition = () => Illusion.Utils.uGUI.isMouseHit;

        public VoiceInfo.Param[] personalityes
		{
			get
			{
				return this.GetCache(ref _personalityes, () => Singleton<Manager.Voice>.Instance.voiceInfoList.Where((VoiceInfo.Param p) => p.No >= 0).ToArray());
			}
		}

		private TMP_Dropdown dropdownPersonality
		{
			get
			{
				return this.GetCacheObject(ref _dropdownPersonality, () => (femaleData.view as WeddingSelectFemaleView).dropdownPersonality);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			ParameterList.Add(new ADVParam(this));
			unionData = new UnionData[2] { maleData, femaleData };
		}

		private void OnDestroy()
		{
			ParameterList.Remove(this);
		}

		private IEnumerator Start()
		{
			base.enabled = false;
			Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM(bgm));
			ChaControl chaControl = Singleton<Character>.Instance.CreateFemale(base.gameObject, 0);
			unionData[chaControl.sex].Initialize(new SaveData.Heroine(chaControl.chaFile, true), chaControl);
			ChaControl chaControl2 = Singleton<Character>.Instance.CreateMale(base.gameObject, 0);
			SaveData.Player player = Singleton<Game>.Instance.Player;
			player.SetCharFile(chaControl2.chaFile);
			unionData[chaControl2.sex].Initialize(player, chaControl2);
			map = this.GetOrAddComponent<ActionMap>();
			yield return new WaitUntil(() => map.infoDic != null);
			map.Change("教会", Scene.Data.FadeType.None);
			cameraEffector = view.camCtrl.GetComponent<CameraEffector>();
			cameraEffector.SetDOFTarget(initCameraTarget);
			view.camCtrl.targetObj = initCameraTarget;
			view.camCtrl.NoCtrlCondition = cameraNoCtrlCondition;
			BindToUI();
			yield return Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out);
			base.enabled = true;
		}

		private void ChangePersonality(int value)
		{
			unionData[1].charaData.parameter.personality = personalityes[value].No;
		}

		private void BindToUI()
		{
			view.clothCtrl.kindSet = clothesFileControl.SettingKind.load;
			view.clothCtrl.OnDisableAsObservable().DelayFrame(1).Subscribe(delegate
			{
				if (view != null && view.camCtrl != null)
				{
					view.camCtrl.NoCtrlCondition = cameraNoCtrlCondition;
				}
			});
			dropdownPersonality.options = personalityes.Select((VoiceInfo.Param p) => new TMP_Dropdown.OptionData(p.Personality)).ToList();
			var anon = personalityes.Select((VoiceInfo.Param p, int index) => new
			{
				no = p.No,
				index = index
			}).FirstOrDefault(p => Singleton<Game>.Instance.weddingData.personality.Contains(p.no));
			dropdownPersonality.value = anon.index;
			dropdownPersonality.onValueChanged.AsObservable().Subscribe(delegate(int value)
			{
				ChangePersonality(value);
				this.unionData[1].ChangeExp();
			});
			ChangePersonality(dropdownPersonality.value);
			UnionData[] array = this.unionData;
			foreach (UnionData unionData in array)
			{
				BindToChaCtrl(unionData.view, unionData.chaCtrl);
			}
			view.btEnter.OnClickAsObservable().Subscribe(delegate
			{
				Next();
			});
			view.btReturn.OnClickAsObservable().Subscribe(delegate
			{
				Illusion.Game.Utils.Sound.Play(SystemSE.cancel);
				Singleton<Scene>.Instance.UnLoad();
			});
		}

		private void BindToChaCtrl(WeddingSelectBaseView selectView, ChaControl chaCtrl)
		{
			selectView.Initialize();
			selectView.btCharaFile.OnClickAsObservable().Subscribe(delegate
			{
				string levelName = "WeddingCharaSelect";
				AddScene("scene/20/wedding/charaselect.unity3d", levelName, delegate
				{
					WeddingCharaSelect rootComponent = Scene.GetRootComponent<WeddingCharaSelect>(levelName);
					if (!(rootComponent == null))
					{
						rootComponent.sex = chaCtrl.sex;
						CameraControl_Ver2 cameraControl_Ver = (rootComponent.camCtrl = view.camCtrl);
						rootComponent.NoCtrlCondition = cameraNoCtrlCondition;
						if (cameraControl_Ver != null)
						{
							cameraControl_Ver.NoCtrlCondition = () => true;
						}
						rootComponent.chaFileSubject.Subscribe(delegate(ChaFileControl chaFile)
						{
							cameraEffector.crossFade.FadeStart();
							ChaFileControl chaFile2 = chaCtrl.chaFile;
							chaFile2.CopyCustom(chaFile.custom);
							chaFile2.CopyCoordinate(chaFile.coordinate);
							chaFile2.CopyParameter(chaFile.parameter);
							chaCtrl.ChangeCoordinateType();
							chaCtrl.Reload();
							byte sex = chaCtrl.sex;
							if (sex != 0)
							{
								ChangePersonality(dropdownPersonality.value);
							}
							unionData[sex].CalcIK();
						}).AddTo(rootComponent);
					}
				});
			});
			selectView.btCharaDefault.OnClickAsObservable().Subscribe(delegate
			{
				byte sex2 = chaCtrl.sex;
				chaCtrl.LoadPreset(sex2, string.Empty);
				LoadDefaultCoordinate(chaCtrl, false);
				chaCtrl.Reload();
				cameraEffector.crossFade.FadeStart();
				if (sex2 != 0)
				{
					ChangePersonality(dropdownPersonality.value);
				}
				unionData[sex2].CalcIK();
			});
			selectView.btSchool.OnClickAsObservable().Subscribe(delegate
			{
				chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.School01);
			});
			selectView.btFromSchool.OnClickAsObservable().Subscribe(delegate
			{
				chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.School02);
			});
			selectView.btGymsuit.OnClickAsObservable().Subscribe(delegate
			{
				chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.Gym);
			});
			selectView.btSwimsuit.OnClickAsObservable().Subscribe(delegate
			{
				chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.Swim);
			});
			selectView.btClub.OnClickAsObservable().Subscribe(delegate
			{
				chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.Club);
			});
			selectView.btPlain.OnClickAsObservable().Subscribe(delegate
			{
				chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.Plain);
			});
			selectView.btPajamas.OnClickAsObservable().Subscribe(delegate
			{
				chaCtrl.ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType.Pajamas);
			});
			selectView.btClothFile.OnClickAsObservable().Subscribe(delegate
			{
				view.clothCtrl.chaCtrl = chaCtrl;
				view.clothCtrl.gameObject.SetActive(true);
			});
			selectView.btDefCloth.OnClickAsObservable().Subscribe(delegate
			{
				LoadDefaultCoordinate(chaCtrl, true);
			});
			selectView.btInner.OnClickAsObservable().Subscribe(delegate
			{
				chaCtrl.fileStatus.shoesType = 0;
			});
			selectView.btOuter.OnClickAsObservable().Subscribe(delegate
			{
				chaCtrl.fileStatus.shoesType = 1;
			});
		}

		private static void LoadDefaultCoordinate(ChaControl chaCtrl, bool reload)
		{
			byte sex = chaCtrl.sex;
			string assetBundleName = (new string[2] { "custom/cos_wed_m_20.unity3d", "custom/cos_wed_f_20.unity3d" })[sex];
			string assetName = (new string[2] { "cos_tuxedo", "cos_wedding" })[sex];
			TextAsset ta = CommonLib.LoadAsset<TextAsset>(assetBundleName, assetName, false, string.Empty);
			AssetBundleManager.UnloadAssetBundle(assetBundleName, true);
			chaCtrl.nowCoordinate.LoadFile(ta);
			chaCtrl.fileStatus.shoesType = 1;
			if (reload)
			{
				chaCtrl.Reload(false, true, true, true);
			}
		}

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

		private void Next()
		{
			Illusion.Game.Utils.Sound.Play(SystemSE.ok_s);
			Observable.FromCoroutine((CancellationToken __) => Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In)).Subscribe(delegate
			{
				view.camCtrl.gameObject.SetActiveIfDifferent(false);
				EventSystem.current.SetSelectedGameObject(null);
				List<Program.Transfer> list = Program.Transfer.NewList();
				UnionData[] array = this.unionData;
				foreach (UnionData unionData in array)
				{
					Program.SetParam(unionData.charaData, list);
				}
				list.Add(Program.Transfer.Create(true, Command.CameraLock, bool.TrueString));
				list.Add(Program.Transfer.Create(true, Command.CharaMotionDefault, "0"));
				list.Add(Program.Transfer.Create(true, Command.CharaMotionDefault, "-1"));
				list.Add(Program.Transfer.Create(true, Command.CharaActive, "0", bool.TrueString, "Right"));
				list.Add(Program.Transfer.Create(true, Command.CharaActive, "-1", bool.TrueString, "Left"));
				list.Add(Program.Transfer.Create(true, Command.CameraSetFov, "23"));
				list.Add(Program.Transfer.Create(true, Command.CharaLookEyesTarget, "-1", "0"));
				list.Add(Program.Transfer.Create(true, Command.CharaLookNeckTarget, "-1", "0"));
				list.Add(Program.Transfer.Create(true, Command.CharaLookEyesTarget, "0", "0"));
				list.Add(Program.Transfer.Create(true, Command.CharaLookNeckTarget, "0", "0"));
				int advNo = 1100;
				SaveData.Heroine heroine = this.unionData[1].charaData as SaveData.Heroine;
				list.Add(Program.Transfer.Open(Program.FindADVBundleFilePath(advNo, heroine), advNo.ToString()));
				StartCoroutine(Program.Open(new Data
				{
					position = Vector3.zero,
					rotation = Quaternion.identity,
					scene = this,
					transferList = list,
					heroineList = new List<SaveData.Heroine> { heroine },
					isParentChara = true
				}, new Program.OpenDataProc
				{
					onLoad = delegate
					{
						StartCoroutine(Wait());
					}
				}));
			});
		}

		private  IEnumerator Wait()
		{
			view.canvas.enabled = false;
			yield return null;
            yield return new WaitWhile(() => Program.isADVScene);
            if (!Scene.isReturnTitle && !Scene.isGameEnd)
			{
				Singleton<Manager.Voice>.Instance.StopAll();
				UnionData[] array = this.unionData;
				foreach (UnionData unionData in array)
				{
					unionData.Play();
				}
				view.camCtrl.gameObject.SetActiveIfDifferent(true);
				view.camCtrl.Reset(0);
				view.camCtrl.SetCameraData(view.camCtrl.GetCameraData());
				yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
				view.canvas.enabled = true;
				Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM(bgm));
			}
		}
	}
}
