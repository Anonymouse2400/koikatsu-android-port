using System;
using System.Collections;
using System.Collections.Generic;
using ActionGame.Chara.Mover;
using ActionGame.MapObject;
using ActionGame.MapSound;
using Illusion.Game.Elements.EasyLoader;
using Manager;
using UniRx;
using UnityEngine;

namespace ActionGame.Chara
{
	public abstract class Base : MonoBehaviour
	{
		public class WaitPointData
		{
			public WaitPoint wp { get; private set; }

			public int paramIndex { get; private set; }

			public int motionIndex { get; private set; }

			public WaitPoint.Parameter param
			{
				get
				{
					return wp.parameterList[paramIndex];
				}
			}

			public WaitPoint.Parameter.Motion motion
			{
				get
				{
					return param.motionList[motionIndex];
				}
			}

			public WaitPointData(WaitPoint wp, int paramIndex, int motionIndex)
			{
				this.wp = wp;
				this.paramIndex = paramIndex;
				this.motionIndex = motionIndex;
			}
		}

		private bool isLoadStart;

		private bool isLoadAsync;

		protected IntReactiveProperty _mapNo = new IntReactiveProperty();

		private SaveData.CharaData _charaData;

		[SerializeField]
		protected SpriteRenderer _actionIcon;

		[SerializeField]
		protected Sprites _sprites;

		private const int HitGateNoneID = -1;

		private int _hitGateID = -1;

		private ReactiveProperty<bool> onActiveChangeStream;

		private bool? reserveActive;

		protected CompositeDisposable disposables = new CompositeDisposable();

		private int _state;

		private bool _isPopOK;

		private Transform _cachedTransform;

		private Transform _head;

		private Transform _bust;

		protected bool _isCharaLoad = true;

		private List<GameObject> _itemObjList = new List<GameObject>();

		private List<Kind> resetKindList = new List<Kind>();

		protected static int hashHeight = Animator.StringToHash("height");

		private Illusion.Game.Elements.EasyLoader.Motion _motion = new Illusion.Game.Elements.EasyLoader.Motion();

		public int mapNo
		{
			get
			{
				return _mapNo.Value;
			}
			set
			{
				_mapNo.Value = value;
			}
		}

		public SaveData.CharaData charaData
		{
			get
			{
				return _charaData;
			}
			protected set
			{
				_charaData = value;
				heroine = _charaData as SaveData.Heroine;
				player = _charaData as SaveData.Player;
			}
		}

		public SaveData.Heroine heroine { get; private set; }

		public SaveData.Player player { get; private set; }

		public abstract bool isAction { get; }

		public bool isGateHit
		{
			get
			{
				return _hitGateID != -1;
			}
		}

		public int hitGateID
		{
			get
			{
				return _hitGateID;
			}
			set
			{
				_hitGateID = value;
			}
		}

		public IObservable<bool> OnActiveChangeObservable
		{
			get
			{
				return onActiveChangeStream;
			}
		}

		public ActionGame.Chara.Mover.Base move { get; protected set; }

		public ActionScene actScene
		{
			get
			{
				return (!Singleton<Game>.IsInstance()) ? null : Singleton<Game>.Instance.actScene;
			}
		}

		public Animator animator
		{
			get
			{
				return (!(chaCtrl == null)) ? chaCtrl.animBody : null;
			}
		}

		public bool isActive
		{
			get
			{
				return onActiveChangeStream != null && onActiveChangeStream.Value;
			}
		}

		public ChaControl chaCtrl { get; private set; }

		public Collider baseCollider { get; private set; }

		public bool initialized { get; protected set; }

		public Vector3 HeadPos
		{
			get
			{
				return (!(Head != null)) ? Vector3.zero : Head.position;
			}
		}

		public Vector3 BustPos
		{
			get
			{
				return (!(Bust != null)) ? Vector3.zero : Bust.position;
			}
		}

		public Transform Head
		{
			get
			{
				return this.GetCacheObject(ref _head, delegate
				{
					if (chaCtrl == null)
					{
						return (Transform)null;
					}
					GameObject referenceInfo = chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.HeadParent);
					return (referenceInfo == null) ? null : referenceInfo.transform;
				});
			}
		}

		public Transform Bust
		{
			get
			{
				return this.GetCacheObject(ref _bust, delegate
				{
					if (chaCtrl == null)
					{
						return (Transform)null;
					}
					GameObject referenceInfo = chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.BUSTUP_TARGET);
					return (referenceInfo == null) ? null : referenceInfo.transform;
				});
			}
		}

		public virtual Vector3 position
		{
			get
			{
				return cachedTransform.localPosition;
			}
			set
			{
				cachedTransform.localPosition = value;
			}
		}

		public Vector3 eulerAngles
		{
			get
			{
				return cachedTransform.localEulerAngles;
			}
			set
			{
				cachedTransform.localEulerAngles = new Vector3(0f, value.y, 0f);
			}
		}

		public Quaternion rotation
		{
			get
			{
				return cachedTransform.localRotation;
			}
			set
			{
				cachedTransform.localRotation = value;
			}
		}

		public int state
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
			}
		}

		public bool isPopOK
		{
			get
			{
				return _isPopOK;
			}
			set
			{
				_isPopOK = value;
			}
		}

		public  Transform cachedTransform
		{
			get
			{
                return this.GetCacheObject(ref this._cachedTransform, () => base.transform);

            }
        }

		public bool isCharaLoad
		{
			get
			{
				return _isCharaLoad;
			}
			set
			{
				_isCharaLoad = value;
			}
		}

		public List<GameObject> itemObjList
		{
			get
			{
				return _itemObjList;
			}
		}

		public bool isArrival
		{
			get
			{
				return wpData != null;
			}
		}

		public WaitPointData wpData { get; protected set; }

		public bool Visible
		{
			get
			{
				if (chaCtrl == null)
				{
					return false;
				}
				return chaCtrl.GetActiveTop() && chaCtrl.visibleAll;
			}
			set
			{
				if (!(chaCtrl == null))
				{
					chaCtrl.SetActiveTop(value);
					chaCtrl.visibleAll = value;
				}
			}
		}

		private bool isReplace { get; set; }

		public Illusion.Game.Elements.EasyLoader.Motion motion
		{
			get
			{
				return _motion;
			}
		}

		public float Height
		{
			get
			{
				if (chaCtrl == null || !chaCtrl.loadEnd)
				{
					return (!(this is Player)) ? 0.5f : 0.6f;
				}
				return chaCtrl.GetShapeBodyValue(0);
			}
		}

		public float BustSize
		{
			get
			{
				if (chaCtrl == null || !chaCtrl.loadEnd)
				{
					return (!(this is Player)) ? 0.5f : 0f;
				}
				return chaCtrl.GetShapeBodyValue(4);
			}
		}

		public virtual bool isLookForTarget
		{
			get
			{
				if (actScene == null)
				{
					return false;
				}
				if (animator == null)
				{
					return false;
				}
				if (!animator.gameObject.activeInHierarchy)
				{
					return false;
				}
				TalkLookNeck.Param value;
				if (!actScene.talkLookNeckDic.TryGetValue(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, out value))
				{
					return false;
				}
				return value.isLook;
			}
		}

		public virtual bool isBodyForTarget
		{
			get
			{
				return bodyForTargetState > 0;
			}
		}

		public virtual int bodyForTargetState
		{
			get
			{
				if (actScene == null)
				{
					return 0;
				}
				TalkLookBody.Param value;
				if (!actScene.talkLookBodyDic.TryGetValue(motion.state ?? string.Empty, out value))
				{
					return 0;
				}
				return value.State;
			}
		}

		public void LoadStart(bool isAsync = false)
		{
			isLoadStart = true;
			isLoadAsync = isAsync;
		}

		public void HitGateReset()
		{
			_hitGateID = -1;
		}

		public void ItemClear()
		{
			_itemObjList.ForEach(delegate(GameObject item)
			{
				UnityEngine.Object.Destroy(item);
			});
			_itemObjList.Clear();
		}

		protected void ResetKind()
		{
			resetKindList.RemoveAll((Kind kind) => kind == null);
			resetKindList.ForEach(delegate(Kind kind)
			{
				kind.ResetPosition();
			});
			resetKindList.Clear();
		}

		public virtual void SetWaitPoint(WaitPointData wpData)
		{
			this.wpData = wpData;
			PlayAnimation();
		}

		protected void PlayAnimationItemKindSet(bool isNormalMotion)
		{
			WaitPoint.Parameter.Motion motion = wpData.motion;
			state = motion.state;
			if (isNormalMotion)
			{
				this.motion.state = motion.motion;
				motion.ItemSet(itemObjList, chaCtrl);
			}
			else
			{
				WaitPoint.Parameter.Motion.Else motionElse = motion.motionElse;
				this.motion.state = motionElse.motion;
				motionElse.ItemSet(itemObjList, chaCtrl);
			}
			KindMoveShift();
		}

		protected void KindMoveShift()
		{
			if (actScene.Map.mapObjects.IsNullOrEmpty())
			{
				return;
			}
			wpData.wp.kindList.ForEach(delegate(WaitPoint.KindMover kind)
			{
				Kind[] mapObjects = actScene.Map.mapObjects;
				foreach (Kind kind2 in mapObjects)
				{
					if (kind.name == kind2.name)
					{
						resetKindList.Add(kind2);
						kind.MoveShift(kind2);
					}
				}
			});
		}

		public virtual void Replace(SaveData.CharaData charaData)
		{
			isReplace = true;
			this.charaData.SetRoot(null);
			this.charaData = charaData;
			this.charaData.SetRoot(base.gameObject);
			if (isCharaLoad)
			{
				ChaFile.CopyChaFile(chaCtrl.chaFile, this.charaData.charFile);
			}
			if (isActive)
			{
				SetActive(false);
				SetActive(true);
			}
		}

		protected void SetParameterHeightAnimation()
		{
			if (animator.gameObject.activeInHierarchy)
			{
				animator.SetFloat(hashHeight, Height);
			}
		}

		public abstract void PlayAnimation();

		public abstract void LoadAnimator();

		public abstract void ChangeNowCoordinate();

		public void LookForDefault()
		{
			if (!(chaCtrl == null))
			{
				chaCtrl.ChangeLookEyesPtn(0);
				chaCtrl.ChangeLookNeckPtn(3);
			}
		}

		public void LookForTarget(Base target, bool isForce = false)
		{
			if (!(chaCtrl == null))
			{
				if ((isForce || isLookForTarget) && target != null && target.Head != null)
				{
					chaCtrl.ChangeLookEyesPtn(0);
					chaCtrl.ChangeLookNeckPtn(1);
					chaCtrl.ChangeLookNeckTarget(0, target.Head);
				}
				else
				{
					LookForDefault();
				}
			}
		}

		public void SetActive(bool active)
		{
			if (onActiveChangeStream == null)
			{
				reserveActive = active;
			}
			else
			{
				onActiveChangeStream.Value = active;
			}
		}

		public void MapShoesSetting(int mapNo)
		{
			switch (actScene.Map.infoDic[mapNo].State)
			{
			case 0:
				chaCtrl.SetClothesState(7, 3);
				break;
			case 1:
				chaCtrl.fileStatus.shoesType = 0;
				chaCtrl.SetClothesState(7, 0);
				break;
			case 2:
				chaCtrl.fileStatus.shoesType = 1;
				chaCtrl.SetClothesState(7, 0);
				break;
			}
		}

		public void SetPositionAndRotation(Transform t)
		{
			SetPositionAndRotation(t.position, t.rotation);
		}

		public void SetPositionAndRotation(Vector3 pos, Quaternion rot)
		{
			SetPositionAndRotation(pos, rot.eulerAngles);
		}

		public void SetPositionAndRotation(Vector3 pos, Vector3 ang)
		{
			position = pos;
			eulerAngles = ang;
		}

		public void SetRotation(Transform t)
		{
			eulerAngles = t.rotation.eulerAngles;
		}

		public void SetRotation(Quaternion rot)
		{
			eulerAngles = rot.eulerAngles;
		}

		public void SetRotation(Vector3 ang)
		{
			eulerAngles = ang;
		}

		protected virtual void Awake()
		{
			baseCollider = GetComponent<Collider>();
		}

		protected virtual IEnumerator Start()
		{
			onActiveChangeStream = new ReactiveProperty<bool>(reserveActive.HasValue && reserveActive.Value);
			if (charaData == null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				yield break;
			}
			while (!isLoadStart)
			{
				yield return null;
			}
			if (_isCharaLoad)
			{
				ChaFileControl chaFileControl = new ChaFileControl();
				ChaFile.CopyChaFile(chaFileControl, charaData.charFile);
				chaCtrl = Singleton<Character>.Instance.CreateChara(Game.CharaDataToSex(charaData), base.gameObject, 0, chaFileControl, false);
			}
			charaData.SetRoot(base.gameObject);
			if (_isCharaLoad)
			{
				if (!isLoadAsync)
				{
					Singleton<Character>.Instance.loading.Load(chaCtrl);
				}
				else
				{
					Singleton<Character>.Instance.loading.LoadAsync(chaCtrl);
				}
				while (!Singleton<Character>.Instance.loading.IsEnd(chaCtrl))
				{
					yield return null;
				}
				LoadAnimator();
				_mapNo.Subscribe(delegate(int no)
				{
					MapShoesSetting(no);
				}).AddTo(disposables);
			}
			bool init = false;
			OnActiveChangeObservable.Subscribe(delegate(bool active)
			{
				baseCollider.enabled = active;
			}).AddTo(disposables);
			OnActiveChangeObservable.Where((bool _) => _isCharaLoad).Subscribe(delegate(bool active)
			{
				chaCtrl.SetActiveTop(active);
			}).AddTo(disposables);
			(from _ in OnActiveChangeObservable
				where _isCharaLoad
				select _ into active
				where active
				select active).Subscribe(delegate
			{
				if (!init || isReplace)
				{
					Singleton<Character>.Instance.enableCharaLoadGCClear = false;
					chaCtrl.Reload();
					LoadAnimator();
					ChangeNowCoordinate();
					if (_mapNo.Value != 0)
					{
						MapShoesSetting(_mapNo.Value);
					}
					Singleton<Character>.Instance.enableCharaLoadGCClear = true;
				}
				init = true;
				isReplace = false;
				PlayAnimation();
				LookForDefault();
				InitializeMapSE();
			}).AddTo(disposables);
		}

		protected virtual void OnDestroy()
		{
			SetActive(false);
			AssetBundleManager.UnloadAssetBundle("action/chara.unity3d", false);
			disposables.Dispose();
		}

		protected void InitializeMapSE()
		{
			StateMachineEvent[] behaviours = animator.GetBehaviours<StateMachineEvent>();
			StateMachineEvent[] array = behaviours;
			foreach (StateMachineEvent stateMachineEvent in array)
			{
				stateMachineEvent.CharaID = chaCtrl.loadNo;
				stateMachineEvent.Base = cachedTransform;
			}
		}
	}
}
