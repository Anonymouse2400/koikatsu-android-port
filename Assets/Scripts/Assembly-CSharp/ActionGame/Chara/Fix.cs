using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ADV;
using Illusion.Extensions;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame.Chara
{
	public sealed class Fix : Base
	{
		private BoolReactiveProperty _isPlayerAction = new BoolReactiveProperty();

		[SerializeField]
		private SpritesAnim _spritesAnim;

		[SerializeField]
		private bl_MiniMapItem miniMapIcon;

		[SerializeField]
		private SpeakChara _speak;

		[SerializeField]
		private NavMeshObstacle _obstacle;

		private MotionVoice motionVoice = new MotionVoice();

		[SerializeField]
		private Transform _advCamPos;

		public override bool isAction
		{
			get
			{
				return true;
			}
		}

		public FixEventScheduler.Result eventSchedulerResult { get; private set; }

		public bool isPlayerAction
		{
			get
			{
				return _isPlayerAction.Value;
			}
			set
			{
				_isPlayerAction.Value = value;
			}
		}

		public SpeakChara speak
		{
			get
			{
				return _speak;
			}
		}

		public NavMeshObstacle obstacle
		{
			get
			{
				return _obstacle;
			}
		}

		public Transform advCamPos
		{
			get
			{
				return _advCamPos;
			}
		}

		public static Fix Create(Transform parent, SaveData.Heroine heroine)
		{
			GameObject asset = AssetBundleManager.LoadAsset("action/chara.unity3d", "Fix", typeof(GameObject)).GetAsset<GameObject>();
			Fix component = UnityEngine.Object.Instantiate(asset, parent, false).GetComponent<Fix>();
			component.charaData = heroine;
			component.name = GetObjectName(heroine);
			component.gameObject.SetActive(true);
			return component;
		}

		private static string GetObjectName(SaveData.Heroine heroine)
		{
			return string.Format("Fix({0})_{1}_{2}", heroine.FixCharaIDOrPersonality, Cycle.GetClassRoomName(heroine.schoolClass), heroine.schoolClassIndex);
		}

		public override void PlayAnimation()
		{
			if (!(base.chaCtrl == null))
			{
				ItemClear();
				ResetKind();
				if (base.isActive && base.isArrival)
				{
					PlayAnimationItemKindSet(true);
				}
				else
				{
					base.motion.state = "Idle";
				}
				SetParameterHeightAnimation();
				base.motion.Play(base.animator);
			}
		}

		public override void LoadAnimator()
		{
			CharaInfo.Param param = base.actScene.charaInfoDic[base.heroine.fixCharaID];
			base.motion.bundle = param.MotionBundle;
			base.motion.asset = param.MotionAsset;
			base.motion.LoadAnimator(base.animator);
		}

		public override void ChangeNowCoordinate()
		{
			if (!(base.chaCtrl == null))
			{
				ChaFileDefine.CoordinateType type = ChaFileDefine.CoordinateType.School01;
				if (!base.heroine.isTeacher && eventSchedulerResult.param != null && !eventSchedulerResult.param.Coordinate.IsNullOrEmpty())
				{
					string coordinate = eventSchedulerResult.param.Coordinate;
					int num = coordinate.Check(true, Enum.GetNames(typeof(ChaFileDefine.CoordinateType)));
					type = (ChaFileDefine.CoordinateType)num;
				}
				base.chaCtrl.ChangeCoordinateTypeAndReload(type);
			}
		}

		public override void Replace(SaveData.CharaData charaData)
		{
			base.Replace(charaData);
			base.name = GetObjectName(base.heroine);
			miniMapIcon.iconItem.text = charaData.Name;
			motionVoice.Init(base.heroine);
		}

		public void SetEventSchedulerResult(FixEventScheduler.Result result)
		{
			eventSchedulerResult = result;
		}

		public void Speak(string bundleName, string assetName)
		{
			if (speak.isOpen)
			{
				speak.Close();
			}
			speak.bundleName = bundleName;
			speak.assetName = assetName;
			speak.Open();
		}

		protected override void Awake()
		{
			base.Awake();
		}

		protected override IEnumerator Start()
		{
			yield return new WaitWhile(() => base.actScene.Player.chaCtrl == null);
			yield return new WaitUntil(() => base.actScene.Player.chaCtrl.loadEnd);
			yield return StartCoroutine(_003CStart_003E__BaseCallProxy0());
			if (_isCharaLoad)
			{
			}
			_isPlayerAction.Subscribe(delegate(bool value)
			{
				if (!(base.actScene == null))
				{
					if (!value)
					{
						base.actScene.actionChangeUI.Remove(ActionChangeUI.ActionType.CharaEvent);
					}
					else if (!base.heroine.isTeacher)
					{
						base.actScene.actionChangeUI.Set(ActionChangeUI.ActionType.CharaEvent);
					}
				}
			}).AddTo(disposables);
			GameObject ag = _actionIcon.gameObject;
			Transform at = _actionIcon.transform;
			(from active in base.OnActiveChangeObservable.TakeUntilDestroy(base.actScene)
				where !active
				select active).Subscribe(delegate(bool active)
			{
				if (base.actScene.Player.actionTarget == this)
				{
					base.actScene.Player.actionTarget = null;
				}
				ag.SetActive(active);
			}).AddTo(disposables);
			ag.OnEnableAsObservable().Subscribe(delegate
			{
				if (base.heroine.isTeacher)
				{
					_actionIcon.sprite = null;
					_spritesAnim.isEnable = true;
					_spritesAnim.Play(0);
				}
				else
				{
					_spritesAnim.isEnable = false;
					_actionIcon.sprite = _sprites[1];
				}
			});
			if (base.isCharaLoad)
			{
				_isPlayerAction.Subscribe(delegate(bool isOn)
				{
					ag.SetActiveIfDifferent(isOn);
				}).AddTo(disposables);
				(from _ in this.UpdateAsObservable()
					where ag.activeSelf
					select _).Subscribe(delegate
				{
					Vector3 headPos = base.HeadPos;
					headPos.y += 0.5f;
					at.position = headPos;
				});
			}
			else
			{
				base.OnActiveChangeObservable.Subscribe(delegate(bool active)
				{
					ag.SetActiveIfDifferent(active);
				}).AddTo(disposables);
			}
			(from _ in this.UpdateAsObservable().TakeUntilDestroy(base.actScene)
				where base.isActive
				select _).Subscribe(delegate
			{
				LookForTarget(_isPlayerAction.Value ? base.actScene.Player : null);
			});
			base.OnActiveChangeObservable.Subscribe(delegate(bool active)
			{
				_obstacle.enabled = active && _isCharaLoad;
			}).AddTo(disposables);
			yield return new WaitWhile(() => miniMapIcon.iconItem == null);
			miniMapIcon.iconItem.text = base.charaData.Name;
			Sprites minimapIconSprites = GetComponents<Sprites>()[1];
			(from _ in this.UpdateAsObservable()
				where base.heroine != null
				select (!base.heroine.isTeacher) ? 1 : 0).DistinctUntilChanged().Subscribe(delegate(int index)
			{
				miniMapIcon.iconItem.SetIcon(minimapIconSprites[index]);
			});
			base.OnActiveChangeObservable.Subscribe(delegate(bool active)
			{
				miniMapIcon.SetVisible(active);
			}).AddTo(disposables);
			base.OnActiveChangeObservable.Where((bool active) => !active).Subscribe(delegate
			{
				speak.Close();
			}).AddTo(disposables);
			if (base.chaCtrl != null)
			{
				base.actScene.VisibleList.Add(base.chaCtrl);
			}
			motionVoice.Init(base.heroine);
			(from active in base.OnActiveChangeObservable
				where !active
				select active into _
				where Singleton<Voice>.IsInstance()
				select _).Subscribe(delegate
			{
				Singleton<Voice>.Instance.Stop(base.Head);
			}).AddTo(disposables);
			(from _ in this.UpdateAsObservable()
				where base.isActive
				where !Singleton<Game>.Instance.IsRegulate(true)
				where !Program.isADVProcessing
				select _).Subscribe(delegate
			{
				motionVoice.Proc();
			});
			base.initialized = true;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (base.actScene != null && base.actScene.VisibleList != null && base.chaCtrl != null)
			{
				base.actScene.VisibleList.Remove(base.chaCtrl);
			}
		}

		[DebuggerHidden]
		[CompilerGenerated]
		private IEnumerator _003CStart_003E__BaseCallProxy0()
		{
			return base.Start();
		}
	}
}
