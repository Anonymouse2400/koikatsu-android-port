using ActionGame.Chara;
using ActionGame.Chara.Mover;
using RootMotion.FinalIK;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ActionGame
{
	public class HandIK : MonoBehaviour
	{
		private const int LeftLayerHand = 1;

		private const int RightLayerHand = 2;

		[SerializeField]
		private HoldingHands _holdingHands;

		private ReactiveProperty<int> state = new ReactiveProperty<int>();

		private ActionGame.Chara.Base chachedChara;

		private CompositeDisposable disposables = new CompositeDisposable();

		public bool isHanding
		{
			get
			{
				return holdingHandUnion != null;
			}
		}

		public HoldingHands holdingHandUnion { get; set; }

		public HandIK target
		{
			get
			{
				return _holdingHands.targetHandIK;
			}
		}

		public HoldingHands holdingHands
		{
			get
			{
				return _holdingHands;
			}
		}

		public FullBodyBipedIK ik { get; set; }

		public IKEffector HandRight
		{
			get
			{
				return ik.solver.rightHandEffector;
			}
		}

		public IKEffector HandLeft
		{
			get
			{
				return ik.solver.leftHandEffector;
			}
		}

		public ActionGame.Chara.Base chara
		{
			get
			{
				return chachedChara;
			}
		}

		public ActionGame.Chara.Mover.Base move
		{
			get
			{
				return chara.move;
			}
		}

		private Animator animator
		{
			get
			{
				return chara.animator;
			}
		}

		public void Set(HandIK target)
		{
			Release();
			_holdingHands.Set(target);
		}

		public void Release()
		{
			if (!(holdingHandUnion == null))
			{
				holdingHandUnion.Release();
				holdingHandUnion = null;
			}
		}

		public void PositionWeight(bool isRight, float weight, bool isUseRotation)
		{
			if (isRight)
			{
				HandRight.positionWeight = weight;
				HandRight.rotationWeight = ((!isUseRotation) ? 0f : weight);
				HandLeft.positionWeight = 0f;
				HandLeft.rotationWeight = 0f;
				SetLayerWeight(1, 0f);
				SetLayerWeight(2, 1f);
			}
			else
			{
				HandRight.positionWeight = 0f;
				HandRight.rotationWeight = 0f;
				HandLeft.positionWeight = weight;
				HandLeft.rotationWeight = ((!isUseRotation) ? 0f : weight);
				SetLayerWeight(1, 1f);
				SetLayerWeight(2, 0f);
			}
		}

		private void SetLayerWeight(int index, float weight)
		{
			if (animator.isActiveAndEnabled && index < animator.layerCount)
			{
				animator.SetLayerWeight(index, weight);
			}
		}

		private void Awake()
		{
			chachedChara = GetComponent<ActionGame.Chara.Base>();
		}

		private void Start()
		{
			(from _ in state
				where base.enabled
				where ik != null
				select _).Subscribe(delegate
			{
				ik.solver.Initiate(ik.solver.GetRoot());
			}).AddTo(disposables);
			(from _ in this.LateUpdateAsObservable()
				where base.enabled
				where animator != null
				where animator.isActiveAndEnabled
				select _).Subscribe(delegate
			{
				state.Value = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
			}).AddTo(disposables);
		}

		private void OnEnable()
		{
			if (ik != null)
			{
				ik.enabled = true;
				WeightZeroCalc(ik.solver);
			}
		}

		private void OnDisable()
		{
			if (ik != null)
			{
				ik.enabled = false;
			}
			if (animator != null)
			{
				SetLayerWeight(1, 0f);
				SetLayerWeight(2, 0f);
			}
		}

		private void OnDestroy()
		{
			disposables.Dispose();
		}

		private static void SyncCalc(IKSolverFullBodyBiped solver)
		{
			Sync(solver.leftFootEffector);
			Sync(solver.leftThighEffector);
			Sync(solver.leftShoulderEffector);
			Sync(solver.rightFootEffector);
			Sync(solver.rightThighEffector);
			Sync(solver.rightShoulderEffector);
		}

		private static void WeightZeroCalc(IKSolverFullBodyBiped solver)
		{
			WeightZero(solver.leftFootEffector);
			WeightZero(solver.leftThighEffector);
			WeightZero(solver.leftShoulderEffector);
			WeightZero(solver.rightFootEffector);
			WeightZero(solver.rightThighEffector);
			WeightZero(solver.rightShoulderEffector);
		}

		private static void Sync(IKEffector eff)
		{
			eff.position = eff.bone.position;
			eff.rotation = eff.bone.rotation;
		}

		private static void WeightZero(IKEffector eff)
		{
			eff.positionWeight = 0f;
			eff.rotationWeight = 0f;
		}
	}
}
