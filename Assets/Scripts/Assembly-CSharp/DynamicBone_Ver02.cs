using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[AddComponentMenu("Dynamic Bone/Dynamic Bone Ver02")]
public class DynamicBone_Ver02 : MonoBehaviour
{
	[Serializable]
	public class BoneParameter
	{
		public string Name = string.Empty;

		[Tooltip("参照骨")]
		public Transform RefTransform;

		[Tooltip("回転させる？")]
		public bool IsRotationCalc;

		[Tooltip("空気抵抗")]
		[Range(0f, 1f)]
		public float Damping;

		[Tooltip("弾力(元の位置に戻ろうとする力)")]
		[Range(0f, 1f)]
		public float Elasticity;

		[Tooltip("硬さ(要は移動のリミット：移動制限)")]
		[Range(0f, 1f)]
		public float Stiffness;

		[Tooltip("惰性(ルートが動いた分を加算するか 加算すると親子付されてる感じになる？)")]
		[Range(0f, 1f)]
		public float Inert;

		[Tooltip("次の骨までの長さの制御(回転に影響する：短いと回りやすい(角度が出やすい)\u3000長いと回りにくい(角度が出にくい))")]
		[Range(0f, 100f)]
		public float NextBoneLength = 1f;

		[Tooltip("コリジョンの大きさ")]
		public float CollisionRadius;

		[Tooltip("移動制限")]
		public bool IsMoveLimit;

		[Tooltip("ローカル移動制限最小")]
		public Vector3 MoveLimitMin = Vector3.zero;

		[Tooltip("ローカル移動制限最大")]
		public Vector3 MoveLimitMax = Vector3.zero;

		[Tooltip("骨の長さを留める制限最小")]
		public float KeepLengthLimitMin;

		[Tooltip("骨の長さを留める制限最大")]
		public float KeepLengthLimitMax;

		[Tooltip("潰れ制御")]
		public bool IsCrush;

		[Tooltip("潰れ範囲最小 この間で設定されたスケール値を足す 判定はローカル位置のZ値")]
		public float CrushMoveAreaMin;

		[Tooltip("潰れ範囲最大 この間で設定されたスケール値を足す 判定はローカル位置のZ値")]
		public float CrushMoveAreaMax;

		[Tooltip("潰れた時に加算するXYスケール")]
		public float CrushAddXYMin;

		[Tooltip("伸びた時に加算するXYスケール")]
		public float CrushAddXYMax;

		public BoneParameter()
		{
			Name = string.Empty;
			IsRotationCalc = false;
			Damping = 0f;
			Elasticity = 0f;
			Stiffness = 0f;
			Inert = 0f;
			NextBoneLength = 1f;
			CollisionRadius = 0f;
			IsMoveLimit = false;
			MoveLimitMin = Vector3.zero;
			MoveLimitMax = Vector3.zero;
			KeepLengthLimitMin = 0f;
			KeepLengthLimitMax = 0f;
			IsCrush = false;
			CrushMoveAreaMin = 0f;
			CrushMoveAreaMax = 0f;
			CrushAddXYMin = 0f;
			CrushAddXYMax = 0f;
		}
	}

	public class ParticlePtn
	{
		public float Damping;

		public float Elasticity;

		public float Stiffness;

		public float Inert;

		public float Radius;

		public bool IsRotationCalc = true;

		public float ScaleNextBoneLength = 1f;

		public bool IsMoveLimit;

		public Vector3 MoveLimitMin = Vector3.zero;

		public Vector3 MoveLimitMax = Vector3.zero;

		public float KeepLengthLimitMin;

		public float KeepLengthLimitMax;

		public bool IsCrush;

		public float CrushMoveAreaMin;

		public float CrushMoveAreaMax;

		public float CrushAddXYMin;

		public float CrushAddXYMax;

		public Vector3 EndOffset = Vector3.zero;

		public Vector3 InitLocalPosition = Vector3.zero;

		public Quaternion InitLocalRotation = Quaternion.identity;

		public Vector3 InitLocalScale = Vector3.one;

		public Transform refTrans;

		public Vector3 LocalPosition = Vector3.zero;

		public ParticlePtn()
		{
			Damping = 0f;
			Elasticity = 0f;
			Stiffness = 0f;
			Inert = 0f;
			Radius = 0f;
			IsRotationCalc = true;
			ScaleNextBoneLength = 1f;
			IsMoveLimit = false;
			MoveLimitMin = Vector3.zero;
			MoveLimitMax = Vector3.zero;
			KeepLengthLimitMin = 0f;
			KeepLengthLimitMax = 0f;
			IsCrush = false;
			CrushMoveAreaMin = 0f;
			CrushMoveAreaMax = 0f;
			CrushAddXYMin = 0f;
			CrushAddXYMax = 0f;
			EndOffset = Vector3.zero;
			InitLocalPosition = Vector3.zero;
			InitLocalRotation = Quaternion.identity;
			InitLocalScale = Vector3.one;
			refTrans = null;
			LocalPosition = Vector3.zero;
		}
	}

	[Serializable]
	public class BonePtn
	{
		public string Name = string.Empty;

		[Tooltip("重力")]
		public Vector3 Gravity = Vector3.zero;

		[Tooltip("最後の骨を回すために必要")]
		public Vector3 EndOffset = Vector3.zero;

		[Tooltip("空気抵抗")]
		[Range(0f, 1f)]
		public float EndOffsetDamping;

		[Tooltip("弾力(元の位置に戻ろうとする力)")]
		[Range(0f, 1f)]
		public float EndOffsetElasticity;

		[Range(0f, 1f)]
		[Tooltip("硬さ(要は移動のリミット：移動制限)")]
		public float EndOffsetStiffness;

		[Tooltip("惰性(ルートが動いた分を加算するか 加算すると親子付されてる感じになる？)")]
		[Range(0f, 1f)]
		public float EndOffsetInert;

		public List<BoneParameter> Params = new List<BoneParameter>();

		public List<ParticlePtn> ParticlePtns = new List<ParticlePtn>();
	}

	public class Particle
	{
		public Transform Transform;

		public int ParentIndex = -1;

		public float Damping;

		public float Elasticity;

		public float Stiffness;

		public float Inert;

		public float Radius;

		public bool IsRotationCalc = true;

		public float ScaleNextBoneLength = 1f;

		public bool IsMoveLimit;

		public Vector3 MoveLimitMin = Vector3.zero;

		public Vector3 MoveLimitMax = Vector3.zero;

		public float KeepLengthLimitMin;

		public float KeepLengthLimitMax;

		public bool IsCrush;

		public float CrushMoveAreaMin;

		public float CrushMoveAreaMax;

		public float CrushAddXYMin;

		public float CrushAddXYMax;

		public Vector3 Position = Vector3.zero;

		public Vector3 PrevPosition = Vector3.zero;

		public Vector3 EndOffset = Vector3.zero;

		public Vector3 InitLocalPosition = Vector3.zero;

		public Quaternion InitLocalRotation = Quaternion.identity;

		public Vector3 InitLocalScale = Vector3.one;

		public Transform refTrans;

		public Vector3 LocalPosition = Vector3.zero;
	}

	public class LoadInfo
	{
		public string Comment;

		public float ReflectSpeed;

		public int HeavyLoopMaxCount;

		public List<DynamicBoneCollider> Colliders = new List<DynamicBoneCollider>();

		public List<Transform> Bones = new List<Transform>();

		public List<BonePtn> Patterns = new List<BonePtn>();
	}

	private class TransformParam
	{
		public Vector3 pos;

		public Quaternion rot;

		public Vector3 scale;
	}

	public string Comment = string.Empty;

	public Transform Root;

	public float UpdateRate = 60f;

	[Tooltip("速度UP")]
	[Range(0f, 100f)]
	public float ReflectSpeed = 1f;

	[Range(0f, 10f)]
	[Tooltip("重い時に何回まで回す？正確になるけどその分重くなる")]
	public int HeavyLoopMaxCount = 3;

	public Vector3 Gravity = Vector3.zero;

	public Vector3 Force = Vector3.zero;

	public List<DynamicBoneCollider> Colliders;

	public List<Transform> Bones;

	public List<BonePtn> Patterns;

	private Vector3 ObjectMove = Vector3.zero;

	private Vector3 ObjectPrevPosition = Vector3.zero;

	private float ObjectScale = 1f;

	private float UpdateTime;

	private float Weight = 1f;

	private List<Particle> Particles = new List<Particle>();

	public int PtnNo;

	public string DragAndDrop = string.Empty;

	private void Awake()
	{
		InitNodeParticle();
		SetupParticles();
		InitLocalPosition();
		if (IsRefTransform())
		{
			setPtn(0, true);
		}
		InitTransforms();
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		if (Weight > 0f)
		{
			InitTransforms();
			UpdateDynamicBones(Time.deltaTime);
		}
	}

	private void OnEnable()
	{
		ResetParticlesPosition();
		if (Root != null)
		{
			ObjectPrevPosition = Root.position;
		}
		else
		{
			ObjectPrevPosition = base.transform.position;
		}
	}

	private void OnDisable()
	{
		InitTransforms();
	}

	private void OnValidate()
	{
		UpdateRate = Mathf.Max(UpdateRate, 0f);
		if (Application.isEditor)
		{
			InitNodeParticle();
			SetupParticles();
			InitLocalPosition();
			if (IsRefTransform())
			{
				setPtn(PtnNo, true);
			}
			InitTransforms();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!base.enabled || Root == null)
		{
			return;
		}
		if (Application.isEditor && !Application.isPlaying && base.transform.hasChanged)
		{
			InitNodeParticle();
			SetupParticles();
			InitLocalPosition();
			if (IsRefTransform())
			{
				setPtn(PtnNo, true);
			}
			InitTransforms();
		}
		Gizmos.color = Color.white;
		foreach (Particle particle2 in Particles)
		{
			if (particle2.ParentIndex >= 0)
			{
				Particle particle = Particles[particle2.ParentIndex];
				Gizmos.DrawLine(particle2.Position, particle.Position);
			}
			if (particle2.Radius > 0f)
			{
				Gizmos.DrawWireSphere(particle2.Position, particle2.Radius * ObjectScale);
			}
		}
	}

	public void SetWeight(float _weight)
	{
		if (Weight == _weight)
		{
			return;
		}
		if (_weight == 0f)
		{
			InitTransforms();
		}
		else if (Weight == 0f)
		{
			ResetParticlesPosition();
			if (Root != null)
			{
				ObjectPrevPosition = Root.position;
			}
			else
			{
				ObjectPrevPosition = base.transform.position;
			}
		}
		Weight = _weight;
	}

	public float GetWeight()
	{
		return Weight;
	}

	public void setRoot(Transform _transRoot)
	{
		Root = _transRoot;
	}

	public Particle getParticle(int _idx)
	{
		if (Particles.Count >= _idx)
		{
			return null;
		}
		return Particles[_idx];
	}

	public int getParticleCount()
	{
		return Particles.Count;
	}

	public bool setPtn(int _ptn, bool _isSameForcePtn = false)
	{
		if (Particles == null || Patterns == null)
		{
			return false;
		}
		if (Particles.Count == 0 || Patterns.Count == 0)
		{
			return false;
		}
		if (Patterns.Count <= _ptn)
		{
			return false;
		}
		if (Particles.Count != Patterns[_ptn].ParticlePtns.Count)
		{
			return false;
		}
		if (PtnNo == _ptn && !_isSameForcePtn)
		{
			return false;
		}
		PtnNo = _ptn;
		Gravity = Patterns[_ptn].Gravity;
		for (int i = 0; i < Particles.Count; i++)
		{
			Particle particle = Particles[i];
			ParticlePtn particlePtn = Patterns[_ptn].ParticlePtns[i];
			particle.IsRotationCalc = particlePtn.IsRotationCalc;
			particle.Damping = particlePtn.Damping;
			particle.Elasticity = particlePtn.Elasticity;
			particle.Stiffness = particlePtn.Stiffness;
			particle.Inert = particlePtn.Inert;
			particle.ScaleNextBoneLength = particlePtn.ScaleNextBoneLength;
			particle.Radius = particlePtn.Radius;
			particle.IsMoveLimit = particlePtn.IsMoveLimit;
			particle.MoveLimitMin = particlePtn.MoveLimitMin;
			particle.MoveLimitMax = particlePtn.MoveLimitMax;
			particle.KeepLengthLimitMin = particlePtn.KeepLengthLimitMin;
			particle.KeepLengthLimitMax = particlePtn.KeepLengthLimitMax;
			particle.IsCrush = particlePtn.IsCrush;
			particle.CrushMoveAreaMin = particlePtn.CrushMoveAreaMin;
			particle.CrushMoveAreaMax = particlePtn.CrushMoveAreaMax;
			particle.CrushAddXYMin = particlePtn.CrushAddXYMin;
			particle.CrushAddXYMax = particlePtn.CrushAddXYMax;
			particle.Damping = Mathf.Clamp01(particle.Damping);
			particle.Elasticity = Mathf.Clamp01(particle.Elasticity);
			particle.Stiffness = Mathf.Clamp01(particle.Stiffness);
			particle.Inert = Mathf.Clamp01(particle.Inert);
			particle.ScaleNextBoneLength = Mathf.Max(particle.ScaleNextBoneLength, 0f);
			particle.Radius = Mathf.Max(particle.Radius, 0f);
			particle.InitLocalPosition = particlePtn.InitLocalPosition;
			particle.InitLocalRotation = particlePtn.InitLocalRotation;
			particle.InitLocalScale = particlePtn.InitLocalScale;
			particle.refTrans = particlePtn.refTrans;
			particle.LocalPosition = particlePtn.LocalPosition;
			particle.EndOffset = particlePtn.EndOffset;
		}
		return true;
	}

	public void ResetParticlesPosition()
	{
		if (Root != null)
		{
			ObjectPrevPosition = Root.position;
		}
		else
		{
			ObjectPrevPosition = base.transform.position;
		}
		foreach (Particle particle in Particles)
		{
			if (particle.Transform != null)
			{
				particle.Position = (particle.PrevPosition = particle.Transform.position);
				continue;
			}
			Transform transform = Particles[particle.ParentIndex].Transform;
			particle.Position = (particle.PrevPosition = transform.TransformPoint(particle.EndOffset));
		}
	}

	public void InitLocalPosition()
	{
		List<TransformParam> list = new List<TransformParam>();
		for (int i = 0; i < Particles.Count; i++)
		{
			Particle particle = Particles[i];
			TransformParam transformParam = new TransformParam();
			if (particle.Transform == null)
			{
				list.Add(transformParam);
				continue;
			}
			transformParam.pos = particle.Transform.localPosition;
			transformParam.rot = particle.Transform.localRotation;
			transformParam.scale = particle.Transform.localScale;
			list.Add(transformParam);
		}
		for (int j = 0; j < Patterns.Count; j++)
		{
			BonePtn bonePtn = Patterns[j];
			for (int k = 0; k < bonePtn.Params.Count; k++)
			{
				bonePtn.ParticlePtns[k].InitLocalPosition = bonePtn.Params[k].RefTransform.localPosition;
				bonePtn.ParticlePtns[k].InitLocalRotation = bonePtn.Params[k].RefTransform.localRotation;
				bonePtn.ParticlePtns[k].InitLocalScale = bonePtn.Params[k].RefTransform.localScale;
				bonePtn.ParticlePtns[k].refTrans = bonePtn.Params[k].RefTransform;
			}
			if (bonePtn.ParticlePtns.Count == Particles.Count)
			{
				for (int l = 0; l < Particles.Count; l++)
				{
					Particle particle2 = Particles[l];
					if (!(particle2.Transform == null))
					{
						particle2.Transform.localPosition = bonePtn.ParticlePtns[l].InitLocalPosition;
						particle2.Transform.localRotation = bonePtn.ParticlePtns[l].InitLocalRotation;
						particle2.Transform.localScale = bonePtn.ParticlePtns[l].InitLocalScale;
					}
				}
			}
			for (int m = 1; m < bonePtn.Params.Count; m++)
			{
				if ((bool)bonePtn.Params[m].RefTransform && (bool)bonePtn.Params[m - 1].RefTransform)
				{
					bonePtn.ParticlePtns[m].LocalPosition = CalcLocalPosition(bonePtn.Params[m].RefTransform.position, bonePtn.Params[m - 1].RefTransform);
				}
			}
		}
		for (int n = 0; n < Particles.Count; n++)
		{
			Particle particle3 = Particles[n];
			if (!(particle3.Transform == null))
			{
				particle3.Transform.localPosition = list[n].pos;
				particle3.Transform.localRotation = list[n].rot;
				particle3.Transform.localScale = list[n].scale;
			}
		}
	}

	public void ResetPosition()
	{
		InitLocalPosition();
		setPtn(PtnNo, true);
		if (base.enabled)
		{
			InitTransforms();
		}
	}

	public bool PtnBlend(int _blendAnswerPtn, int _blendPtn1, int _blendPtn2, float _t)
	{
		if (Patterns == null)
		{
			return false;
		}
		int count = Patterns.Count;
		if (count <= _blendAnswerPtn || count <= _blendPtn1 || count <= _blendPtn2)
		{
			return false;
		}
		if (Patterns[_blendAnswerPtn].ParticlePtns.Count != Patterns[_blendPtn1].ParticlePtns.Count || Patterns[_blendPtn2].ParticlePtns.Count != Patterns[_blendPtn1].ParticlePtns.Count)
		{
			return false;
		}
		Patterns[_blendAnswerPtn].Gravity = Vector3.Lerp(Patterns[_blendPtn1].Gravity, Patterns[_blendPtn2].Gravity, _t);
		for (int i = 0; i < Patterns[_blendAnswerPtn].ParticlePtns.Count; i++)
		{
			ParticlePtn particlePtn = Patterns[_blendAnswerPtn].ParticlePtns[i];
			ParticlePtn particlePtn2 = Patterns[_blendPtn1].ParticlePtns[i];
			ParticlePtn particlePtn3 = Patterns[_blendPtn2].ParticlePtns[i];
			particlePtn.IsRotationCalc = particlePtn3.IsRotationCalc;
			particlePtn.Damping = Mathf.Lerp(particlePtn2.Damping, particlePtn3.Damping, _t);
			particlePtn.Elasticity = Mathf.Lerp(particlePtn2.Elasticity, particlePtn3.Elasticity, _t);
			particlePtn.Stiffness = Mathf.Lerp(particlePtn2.Stiffness, particlePtn3.Stiffness, _t);
			particlePtn.Inert = Mathf.Lerp(particlePtn2.Inert, particlePtn3.Inert, _t);
			particlePtn.ScaleNextBoneLength = Mathf.Lerp(particlePtn2.ScaleNextBoneLength, particlePtn3.ScaleNextBoneLength, _t);
			particlePtn.Radius = Mathf.Lerp(particlePtn2.Radius, particlePtn3.Radius, _t);
			particlePtn.IsMoveLimit = particlePtn3.IsMoveLimit;
			particlePtn.MoveLimitMin = Vector3.Lerp(particlePtn2.MoveLimitMin, particlePtn3.MoveLimitMin, _t);
			particlePtn.MoveLimitMax = Vector3.Lerp(particlePtn2.MoveLimitMax, particlePtn3.MoveLimitMax, _t);
			particlePtn.KeepLengthLimitMin = Mathf.Lerp(particlePtn2.KeepLengthLimitMin, particlePtn3.KeepLengthLimitMin, _t);
			particlePtn.KeepLengthLimitMax = Mathf.Lerp(particlePtn2.KeepLengthLimitMax, particlePtn3.KeepLengthLimitMax, _t);
			particlePtn.IsCrush = particlePtn3.IsCrush;
			particlePtn.CrushMoveAreaMin = Mathf.Lerp(particlePtn2.CrushMoveAreaMin, particlePtn3.CrushMoveAreaMin, _t);
			particlePtn.CrushMoveAreaMax = Mathf.Lerp(particlePtn2.CrushMoveAreaMax, particlePtn3.CrushMoveAreaMax, _t);
			particlePtn.CrushAddXYMin = Mathf.Lerp(particlePtn2.CrushAddXYMin, particlePtn3.CrushAddXYMin, _t);
			particlePtn.CrushAddXYMax = Mathf.Lerp(particlePtn2.CrushAddXYMax, particlePtn3.CrushAddXYMax, _t);
			particlePtn.Damping = Mathf.Clamp01(particlePtn.Damping);
			particlePtn.Elasticity = Mathf.Clamp01(particlePtn.Elasticity);
			particlePtn.Stiffness = Mathf.Clamp01(particlePtn.Stiffness);
			particlePtn.Inert = Mathf.Clamp01(particlePtn.Inert);
			particlePtn.ScaleNextBoneLength = Mathf.Max(particlePtn.ScaleNextBoneLength, 0f);
			particlePtn.Radius = Mathf.Max(particlePtn.Radius, 0f);
			particlePtn.InitLocalPosition = Vector3.Lerp(particlePtn2.InitLocalPosition, particlePtn3.InitLocalPosition, _t);
			particlePtn.InitLocalRotation = Quaternion.Lerp(particlePtn2.InitLocalRotation, particlePtn3.InitLocalRotation, _t);
			particlePtn.InitLocalScale = Vector3.Lerp(particlePtn2.InitLocalScale, particlePtn3.InitLocalScale, _t);
			particlePtn.refTrans = particlePtn3.refTrans;
			particlePtn.LocalPosition = Vector3.Lerp(particlePtn2.LocalPosition, particlePtn3.LocalPosition, _t);
			particlePtn.EndOffset = Vector3.Lerp(particlePtn2.EndOffset, particlePtn3.EndOffset, _t);
		}
		return true;
	}

	public bool setGravity(int _ptn, Vector3 _gravity, bool _isNowGravity = true)
	{
		if (Particles == null || Patterns == null)
		{
			return false;
		}
		if (Particles.Count == 0 || Patterns.Count == 0)
		{
			return false;
		}
		if (Patterns.Count <= _ptn)
		{
			return false;
		}
		if (_isNowGravity)
		{
			Gravity = _gravity;
		}
		if (_ptn < 0)
		{
			for (int i = 0; i < Patterns.Count; i++)
			{
				Patterns[i].Gravity = _gravity;
			}
		}
		else
		{
			if (Patterns.Count <= _ptn)
			{
				return false;
			}
			Patterns[_ptn].Gravity = _gravity;
		}
		return true;
	}

	public bool setSoftParams(int _ptn, int _bone, float _damping, float _elasticity, float _stiffness, bool _isNowParam = true)
	{
		if (Particles == null || Patterns == null)
		{
			return false;
		}
		if (Particles.Count == 0 || Patterns.Count == 0)
		{
			return false;
		}
		if (Patterns.Count <= _ptn)
		{
			return false;
		}
		if (_isNowParam)
		{
			if (_bone == -1)
			{
				for (int i = 0; i < Particles.Count; i++)
				{
					Particles[i].Damping = _damping;
					Particles[i].Elasticity = _elasticity;
					Particles[i].Stiffness = _stiffness;
				}
			}
			else if (Particles.Count > _bone)
			{
				Particles[_bone].Damping = _damping;
				Particles[_bone].Elasticity = _elasticity;
				Particles[_bone].Stiffness = _stiffness;
			}
		}
		if (_ptn < 0)
		{
			for (int j = 0; j < Patterns.Count; j++)
			{
				if (_bone == -1)
				{
					for (int k = 0; k < Patterns[j].ParticlePtns.Count; k++)
					{
						setSoftParams(Patterns[j].ParticlePtns[k], _damping, _elasticity, _stiffness);
					}
					for (int l = 0; l < Patterns[j].Params.Count; l++)
					{
						setSoftParams(Patterns[j].Params[l], _damping, _elasticity, _stiffness);
					}
				}
				else
				{
					if (Patterns[j].ParticlePtns.Count > _bone)
					{
						setSoftParams(Patterns[j].ParticlePtns[_bone], _damping, _elasticity, _stiffness);
					}
					if (Patterns[j].Params.Count > _bone)
					{
						setSoftParams(Patterns[j].Params[_bone], _damping, _elasticity, _stiffness);
					}
				}
			}
		}
		else
		{
			if (Patterns.Count <= _ptn)
			{
				return false;
			}
			if (_bone == -1)
			{
				for (int m = 0; m < Patterns[_ptn].ParticlePtns.Count; m++)
				{
					setSoftParams(Patterns[_ptn].ParticlePtns[m], _damping, _elasticity, _stiffness);
				}
				for (int n = 0; n < Patterns[_ptn].Params.Count; n++)
				{
					setSoftParams(Patterns[_ptn].Params[n], _damping, _elasticity, _stiffness);
				}
			}
			else
			{
				if (Patterns[_ptn].ParticlePtns.Count > _bone)
				{
					setSoftParams(Patterns[_ptn].ParticlePtns[_bone], _damping, _elasticity, _stiffness);
				}
				if (Patterns[_ptn].Params.Count > _bone)
				{
					setSoftParams(Patterns[_ptn].Params[_bone], _damping, _elasticity, _stiffness);
				}
			}
		}
		return true;
	}

	private bool setSoftParams(ParticlePtn _ptn, float _damping, float _elasticity, float _stiffness)
	{
		_ptn.Damping = _damping;
		_ptn.Elasticity = _elasticity;
		_ptn.Stiffness = _stiffness;
		return true;
	}

	private bool setSoftParams(BoneParameter _ptn, float _damping, float _elasticity, float _stiffness)
	{
		_ptn.Damping = _damping;
		_ptn.Elasticity = _elasticity;
		_ptn.Stiffness = _stiffness;
		return true;
	}

	public bool SetRotationCalcParams(int _ptn, int _bone, bool _enable, bool _isNowParam = true)
	{
		if (Particles == null || Patterns == null)
		{
			return false;
		}
		if (Particles.Count == 0 || Patterns.Count == 0)
		{
			return false;
		}
		if (Patterns.Count <= _ptn)
		{
			return false;
		}
		if (_isNowParam)
		{
			if (_bone == -1)
			{
				for (int i = 0; i < Particles.Count; i++)
				{
					Particles[i].IsRotationCalc = _enable;
				}
			}
			else if (Particles.Count > _bone)
			{
				Particles[_bone].IsRotationCalc = _enable;
			}
		}
		if (_ptn < 0)
		{
			for (int j = 0; j < Patterns.Count; j++)
			{
				if (_bone == -1)
				{
					for (int k = 0; k < Patterns[j].ParticlePtns.Count; k++)
					{
						Patterns[j].ParticlePtns[k].IsRotationCalc = _enable;
					}
					for (int l = 0; l < Patterns[j].Params.Count; l++)
					{
						Patterns[j].Params[l].IsRotationCalc = _enable;
					}
				}
				else
				{
					if (Patterns[j].ParticlePtns.Count > _bone)
					{
						Patterns[j].ParticlePtns[_bone].IsRotationCalc = _enable;
					}
					if (Patterns[j].Params.Count > _bone)
					{
						Patterns[j].Params[_bone].IsRotationCalc = _enable;
					}
				}
			}
		}
		else
		{
			if (Patterns.Count <= _ptn)
			{
				return false;
			}
			if (_bone == -1)
			{
				for (int m = 0; m < Patterns[_ptn].ParticlePtns.Count; m++)
				{
					Patterns[_ptn].ParticlePtns[m].IsRotationCalc = _enable;
				}
				for (int n = 0; n < Patterns[_ptn].Params.Count; n++)
				{
					Patterns[_ptn].Params[n].IsRotationCalc = _enable;
				}
			}
			else
			{
				if (Patterns[_ptn].ParticlePtns.Count > _bone)
				{
					Patterns[_ptn].ParticlePtns[_bone].IsRotationCalc = _enable;
				}
				if (Patterns[_ptn].Params.Count > _bone)
				{
					Patterns[_ptn].Params[_bone].IsRotationCalc = _enable;
				}
			}
		}
		return true;
	}

	public bool SetMoveLimitParams(int _ptn, int _bone, bool _enable, bool _isNowParam = true)
	{
		if (Particles == null || Patterns == null)
		{
			return false;
		}
		if (Particles.Count == 0 || Patterns.Count == 0)
		{
			return false;
		}
		if (Patterns.Count <= _ptn)
		{
			return false;
		}
		if (_isNowParam)
		{
			if (_bone == -1)
			{
				for (int i = 0; i < Particles.Count; i++)
				{
					Particles[i].IsMoveLimit = _enable;
				}
			}
			else if (Particles.Count > _bone)
			{
				Particles[_bone].IsMoveLimit = _enable;
			}
		}
		if (_ptn < 0)
		{
			for (int j = 0; j < Patterns.Count; j++)
			{
				if (_bone == -1)
				{
					for (int k = 0; k < Patterns[j].ParticlePtns.Count; k++)
					{
						Patterns[j].ParticlePtns[k].IsMoveLimit = _enable;
					}
					for (int l = 0; l < Patterns[j].Params.Count; l++)
					{
						Patterns[j].Params[l].IsMoveLimit = _enable;
					}
				}
				else
				{
					if (Patterns[j].ParticlePtns.Count > _bone)
					{
						Patterns[j].ParticlePtns[_bone].IsMoveLimit = _enable;
					}
					if (Patterns[j].Params.Count > _bone)
					{
						Patterns[j].Params[_bone].IsMoveLimit = _enable;
					}
				}
			}
		}
		else
		{
			if (Patterns.Count <= _ptn)
			{
				return false;
			}
			if (_bone == -1)
			{
				for (int m = 0; m < Patterns[_ptn].ParticlePtns.Count; m++)
				{
					Patterns[_ptn].ParticlePtns[m].IsMoveLimit = _enable;
				}
				for (int n = 0; n < Patterns[_ptn].Params.Count; n++)
				{
					Patterns[_ptn].Params[n].IsMoveLimit = _enable;
				}
			}
			else
			{
				if (Patterns[_ptn].ParticlePtns.Count > _bone)
				{
					Patterns[_ptn].ParticlePtns[_bone].IsMoveLimit = _enable;
				}
				if (Patterns[_ptn].Params.Count > _bone)
				{
					Patterns[_ptn].Params[_bone].IsMoveLimit = _enable;
				}
			}
		}
		return true;
	}

	public bool SetMoveLimitData(int _ptn, int _bone, Vector3 _min, Vector3 _max, bool _isNowParam = true)
	{
		if (Particles == null || Patterns == null)
		{
			return false;
		}
		if (Particles.Count == 0 || Patterns.Count == 0)
		{
			return false;
		}
		if (Patterns.Count <= _ptn)
		{
			return false;
		}
		if (_isNowParam)
		{
			if (_bone == -1)
			{
				for (int i = 0; i < Particles.Count; i++)
				{
					Particles[i].MoveLimitMin = _min;
					Particles[i].MoveLimitMax = _max;
				}
			}
			else if (Particles.Count > _bone)
			{
				Particles[_bone].MoveLimitMin = _min;
				Particles[_bone].MoveLimitMax = _max;
			}
		}
		if (_ptn < 0)
		{
			for (int j = 0; j < Patterns.Count; j++)
			{
				if (_bone == -1)
				{
					for (int k = 0; k < Patterns[j].ParticlePtns.Count; k++)
					{
						Patterns[j].ParticlePtns[k].MoveLimitMin = _min;
						Patterns[j].ParticlePtns[k].MoveLimitMax = _max;
					}
					for (int l = 0; l < Patterns[j].Params.Count; l++)
					{
						Patterns[j].Params[l].MoveLimitMin = _min;
						Patterns[j].Params[l].MoveLimitMax = _max;
					}
				}
				else
				{
					if (Patterns[j].ParticlePtns.Count > _bone)
					{
						Patterns[j].ParticlePtns[_bone].MoveLimitMin = _min;
						Patterns[j].ParticlePtns[_bone].MoveLimitMax = _max;
					}
					if (Patterns[j].Params.Count > _bone)
					{
						Patterns[j].Params[_bone].MoveLimitMin = _min;
						Patterns[j].Params[_bone].MoveLimitMax = _max;
					}
				}
			}
		}
		else
		{
			if (Patterns.Count <= _ptn)
			{
				return false;
			}
			if (_bone == -1)
			{
				for (int m = 0; m < Patterns[_ptn].ParticlePtns.Count; m++)
				{
					Patterns[_ptn].ParticlePtns[m].MoveLimitMin = _min;
					Patterns[_ptn].ParticlePtns[m].MoveLimitMax = _max;
				}
				for (int n = 0; n < Patterns[_ptn].Params.Count; n++)
				{
					Patterns[_ptn].Params[n].MoveLimitMin = _min;
					Patterns[_ptn].Params[n].MoveLimitMax = _max;
				}
			}
			else
			{
				if (Patterns[_ptn].ParticlePtns.Count > _bone)
				{
					Patterns[_ptn].ParticlePtns[_bone].MoveLimitMin = _min;
					Patterns[_ptn].ParticlePtns[_bone].MoveLimitMax = _max;
				}
				if (Patterns[_ptn].Params.Count > _bone)
				{
					Patterns[_ptn].Params[_bone].MoveLimitMin = _min;
					Patterns[_ptn].Params[_bone].MoveLimitMax = _max;
				}
			}
		}
		return true;
	}

	public bool LoadTextList(List<string> _list)
	{
		LoadInfo loadInfo = new LoadInfo();
		int _index = 0;
		while (_list.Count > _index && LoadText(loadInfo, _list, ref _index))
		{
		}
		if (_list.Count > _index)
		{
			return false;
		}
		Comment = loadInfo.Comment;
		ReflectSpeed = loadInfo.ReflectSpeed;
		HeavyLoopMaxCount = loadInfo.HeavyLoopMaxCount;
		Colliders = new List<DynamicBoneCollider>(loadInfo.Colliders);
		Bones = new List<Transform>(loadInfo.Bones);
		Patterns = new List<BonePtn>();
		foreach (BonePtn pattern in loadInfo.Patterns)
		{
			BonePtn bonePtn = new BonePtn();
			bonePtn.Name = pattern.Name;
			bonePtn.Gravity = pattern.Gravity;
			bonePtn.EndOffset = pattern.EndOffset;
			bonePtn.EndOffsetDamping = pattern.EndOffsetDamping;
			bonePtn.EndOffsetElasticity = pattern.EndOffsetElasticity;
			bonePtn.EndOffsetStiffness = pattern.EndOffsetStiffness;
			bonePtn.EndOffsetInert = pattern.EndOffsetInert;
			foreach (BoneParameter param in pattern.Params)
			{
				BoneParameter boneParameter = new BoneParameter();
				boneParameter.Name = param.Name;
				boneParameter.RefTransform = param.RefTransform;
				boneParameter.IsRotationCalc = param.IsRotationCalc;
				boneParameter.Damping = param.Damping;
				boneParameter.Elasticity = param.Elasticity;
				boneParameter.Stiffness = param.Stiffness;
				boneParameter.Inert = param.Inert;
				boneParameter.NextBoneLength = param.NextBoneLength;
				boneParameter.CollisionRadius = param.CollisionRadius;
				boneParameter.IsMoveLimit = param.IsMoveLimit;
				boneParameter.MoveLimitMin = param.MoveLimitMin;
				boneParameter.MoveLimitMax = param.MoveLimitMax;
				boneParameter.KeepLengthLimitMin = param.KeepLengthLimitMin;
				boneParameter.KeepLengthLimitMax = param.KeepLengthLimitMax;
				boneParameter.IsCrush = param.IsCrush;
				boneParameter.CrushMoveAreaMin = param.CrushMoveAreaMin;
				boneParameter.CrushMoveAreaMax = param.CrushMoveAreaMax;
				boneParameter.CrushAddXYMin = param.CrushAddXYMin;
				boneParameter.CrushAddXYMax = param.CrushAddXYMax;
				bonePtn.Params.Add(boneParameter);
			}
			Patterns.Add(bonePtn);
		}
		InitNodeParticle();
		SetupParticles();
		InitLocalPosition();
		if (IsRefTransform())
		{
			setPtn(0, true);
		}
		InitTransforms();
		return true;
	}

	private void UpdateDynamicBones(float _deltaTime)
	{
		if (Root == null)
		{
			return;
		}
		ObjectScale = Mathf.Abs(Root.lossyScale.x);
		ObjectMove = Root.position - ObjectPrevPosition;
		ObjectPrevPosition = Root.position;
		int num = 1;
		if (UpdateRate > 0f)
		{
			float num2 = 1f / UpdateRate;
			UpdateTime += _deltaTime;
			num = 0;
			while (UpdateTime >= num2)
			{
				UpdateTime -= num2;
				if (++num >= HeavyLoopMaxCount)
				{
					UpdateTime = 0f;
					break;
				}
			}
		}
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				UpdateParticles1();
				UpdateParticles2();
				ObjectMove = Vector3.zero;
			}
		}
		else
		{
			SkipUpdateParticles();
		}
		ApplyParticlesToTransforms();
	}

	private void InitNodeParticle()
	{
		if (Patterns == null)
		{
			return;
		}
		foreach (BonePtn pattern in Patterns)
		{
			if (pattern.ParticlePtns != null)
			{
				pattern.ParticlePtns.Clear();
			}
			else
			{
				pattern.ParticlePtns = new List<ParticlePtn>();
			}
			if (pattern.Params.Count != Bones.Count)
			{
				continue;
			}
			foreach (var item in Bones.Select((Transform value, int idx) => new { value, idx }))
			{
				pattern.ParticlePtns.Add(AppendParticlePtn(pattern.Params[item.idx], Vector3.zero));
			}
			BoneParameter boneParameter = new BoneParameter();
			boneParameter.Damping = pattern.EndOffsetDamping;
			boneParameter.Elasticity = pattern.EndOffsetElasticity;
			boneParameter.Stiffness = pattern.EndOffsetStiffness;
			boneParameter.Inert = pattern.EndOffsetInert;
			pattern.ParticlePtns.Add(AppendParticlePtn(boneParameter, pattern.EndOffset));
		}
	}

	private void SetupParticles()
	{
		Particles.Clear();
		if (Root == null && Bones.Count > 0)
		{
			Root = Bones[0];
		}
		if (Root == null || Bones == null || Patterns == null || Bones.Count == 0 || Patterns.Count == 0 || Bones.Count != Patterns[0].Params.Count)
		{
			return;
		}
		ObjectScale = Root.lossyScale.x;
		ObjectPrevPosition = Root.position;
		ObjectMove = Vector3.zero;
		int num = -1;
		foreach (var item in Bones.Select((Transform value, int idx) => new { value, idx }))
		{
			AppendParticles(item.value, Patterns[0].Params[item.idx], Vector3.zero, num);
			num++;
		}
		AppendParticles(null, new BoneParameter(), Patterns[0].EndOffset, num);
	}

	private ParticlePtn AppendParticlePtn(BoneParameter _parameter, Vector3 _endOffset)
	{
		ParticlePtn particlePtn = new ParticlePtn();
		particlePtn.IsRotationCalc = _parameter.IsRotationCalc;
		particlePtn.Damping = _parameter.Damping;
		particlePtn.Elasticity = _parameter.Elasticity;
		particlePtn.Stiffness = _parameter.Stiffness;
		particlePtn.Inert = _parameter.Inert;
		particlePtn.ScaleNextBoneLength = _parameter.NextBoneLength;
		particlePtn.Radius = _parameter.CollisionRadius;
		particlePtn.IsMoveLimit = _parameter.IsMoveLimit;
		particlePtn.MoveLimitMin = _parameter.MoveLimitMin;
		particlePtn.MoveLimitMax = _parameter.MoveLimitMax;
		particlePtn.KeepLengthLimitMin = _parameter.KeepLengthLimitMin;
		particlePtn.KeepLengthLimitMax = _parameter.KeepLengthLimitMax;
		particlePtn.IsCrush = _parameter.IsCrush;
		particlePtn.CrushMoveAreaMin = _parameter.CrushMoveAreaMin;
		particlePtn.CrushMoveAreaMax = _parameter.CrushMoveAreaMax;
		particlePtn.CrushAddXYMin = _parameter.CrushAddXYMin;
		particlePtn.CrushAddXYMax = _parameter.CrushAddXYMax;
		particlePtn.Damping = Mathf.Clamp01(particlePtn.Damping);
		particlePtn.Elasticity = Mathf.Clamp01(particlePtn.Elasticity);
		particlePtn.Stiffness = Mathf.Clamp01(particlePtn.Stiffness);
		particlePtn.Inert = Mathf.Clamp01(particlePtn.Inert);
		particlePtn.ScaleNextBoneLength = Mathf.Max(particlePtn.ScaleNextBoneLength, 0f);
		particlePtn.Radius = Mathf.Max(particlePtn.Radius, 0f);
		if (_parameter.RefTransform != null)
		{
			particlePtn.InitLocalPosition = _parameter.RefTransform.localPosition;
			particlePtn.InitLocalRotation = _parameter.RefTransform.localRotation;
			particlePtn.InitLocalScale = _parameter.RefTransform.localScale;
			particlePtn.refTrans = _parameter.RefTransform;
		}
		else
		{
			particlePtn.EndOffset = _endOffset;
		}
		return particlePtn;
	}

	private Particle AppendParticles(Transform _transform, BoneParameter _parameter, Vector3 _endOffset, int _parentIndex)
	{
		Particle particle = new Particle();
		particle.Transform = _transform;
		particle.IsRotationCalc = _parameter.IsRotationCalc;
		particle.Damping = _parameter.Damping;
		particle.Elasticity = _parameter.Elasticity;
		particle.Stiffness = _parameter.Stiffness;
		particle.Inert = _parameter.Inert;
		particle.ScaleNextBoneLength = _parameter.NextBoneLength;
		particle.Radius = _parameter.CollisionRadius;
		particle.IsMoveLimit = _parameter.IsMoveLimit;
		particle.MoveLimitMin = _parameter.MoveLimitMin;
		particle.MoveLimitMax = _parameter.MoveLimitMax;
		particle.KeepLengthLimitMin = _parameter.KeepLengthLimitMin;
		particle.KeepLengthLimitMax = _parameter.KeepLengthLimitMax;
		particle.IsCrush = _parameter.IsCrush;
		particle.CrushMoveAreaMin = _parameter.CrushMoveAreaMin;
		particle.CrushMoveAreaMax = _parameter.CrushMoveAreaMax;
		particle.CrushAddXYMin = _parameter.CrushAddXYMin;
		particle.CrushAddXYMax = _parameter.CrushAddXYMax;
		particle.ParentIndex = _parentIndex;
		particle.Damping = Mathf.Clamp01(particle.Damping);
		particle.Elasticity = Mathf.Clamp01(particle.Elasticity);
		particle.Stiffness = Mathf.Clamp01(particle.Stiffness);
		particle.Inert = Mathf.Clamp01(particle.Inert);
		particle.ScaleNextBoneLength = Mathf.Max(particle.ScaleNextBoneLength, 0f);
		particle.Radius = Mathf.Max(particle.Radius, 0f);
		if (_transform != null)
		{
			particle.Position = (particle.PrevPosition = _transform.position);
			particle.InitLocalPosition = _transform.localPosition;
			particle.InitLocalRotation = _transform.localRotation;
			particle.refTrans = _transform;
			if (_parentIndex >= 0)
			{
				CalcLocalPosition(particle, Particles[_parentIndex]);
			}
		}
		else
		{
			Transform transform = Particles[_parentIndex].Transform;
			particle.EndOffset = _endOffset;
			particle.Position = (particle.PrevPosition = transform.TransformPoint(particle.EndOffset));
		}
		Particles.Add(particle);
		return particle;
	}

	private void InitTransforms()
	{
		int count = Particles.Count;
		for (int i = 0; i < count; i++)
		{
			Particle particle = Particles[i];
			if (!(particle.Transform == null))
			{
				if (particle.refTrans != null)
				{
					particle.Transform.localPosition = particle.refTrans.localPosition;
					particle.Transform.localRotation = particle.refTrans.localRotation;
					particle.Transform.localScale = particle.refTrans.localScale;
				}
				else
				{
					particle.Transform.localPosition = particle.InitLocalPosition;
					particle.Transform.localRotation = particle.InitLocalRotation;
					particle.Transform.localScale = particle.InitLocalScale;
				}
			}
		}
	}

	private void UpdateParticles1()
	{
		if (Patterns == null || (Patterns != null && Patterns.Count == 0))
		{
			return;
		}
		Vector3 vector = (Gravity + Force) * ObjectScale;
		for (int i = 0; i < Particles.Count; i++)
		{
			Particle particle = Particles[i];
			if (particle.ParentIndex >= 0)
			{
				Vector3 vector2 = (particle.Position - particle.PrevPosition) * ReflectSpeed;
				Vector3 vector3 = ObjectMove * particle.Inert;
				particle.PrevPosition = particle.Position + vector3;
				particle.Position += vector2 * (1f - particle.Damping) + vector + vector3;
			}
			else
			{
				particle.PrevPosition = particle.Position;
				particle.Position = particle.Transform.position;
			}
		}
	}

	private void UpdateParticles2()
	{
		for (int i = 1; i < Particles.Count; i++)
		{
			Particle particle = Particles[i];
			Particle particle2 = Particles[particle.ParentIndex];
			float num = ((!(particle.Transform != null)) ? (particle.EndOffset.magnitude * ObjectScale) : (particle2.Transform.position - particle.Transform.position).magnitude);
			Matrix4x4 localToWorldMatrix = particle2.Transform.localToWorldMatrix;
			localToWorldMatrix.SetColumn(3, new Vector4(particle2.Position.x, particle2.Position.y, particle2.Position.z, 1f));
			Vector3 vector = ((!(particle.Transform != null)) ? localToWorldMatrix.MultiplyPoint3x4(particle.EndOffset) : localToWorldMatrix.MultiplyPoint3x4(particle.LocalPosition));
			float num2 = Mathf.Lerp(1f, particle.Stiffness, Weight);
			if (num2 > 0f || particle.Elasticity > 0f)
			{
				Vector3 vector2 = vector - particle.Position;
				particle.Position += vector2 * particle.Elasticity;
				if (num2 > 0f)
				{
					vector2 = vector - particle.Position;
					float magnitude = vector2.magnitude;
					float num3 = num * (1f - num2) * 2f;
					if (magnitude > num3)
					{
						particle.Position += vector2 * ((magnitude - num3) / magnitude);
					}
				}
			}
			float particleRadius = particle.Radius * ObjectScale;
			foreach (DynamicBoneCollider collider in Colliders)
			{
				if (collider != null && collider.enabled && collider.gameObject.activeInHierarchy)
				{
					collider.Collide(ref particle.Position, particleRadius);
				}
			}
			Vector3 vector3 = particle2.Position - particle.Position;
			float magnitude2 = vector3.magnitude;
			if (magnitude2 > 0f)
			{
				float num4 = (magnitude2 - num) / magnitude2;
				if (particle.KeepLengthLimitMin >= num4)
				{
					particle.Position += vector3 * (num4 - particle.KeepLengthLimitMin);
				}
				else if (num4 >= particle.KeepLengthLimitMax)
				{
					particle.Position += vector3 * (num4 - particle.KeepLengthLimitMax);
				}
			}
			if ((bool)particle.Transform && particle.IsMoveLimit)
			{
				Matrix4x4 localToWorldMatrix2 = particle.Transform.localToWorldMatrix;
				localToWorldMatrix2.SetColumn(3, new Vector4(vector.x, vector.y, vector.z, 1f));
				Vector3 v = localToWorldMatrix2.inverse.MultiplyPoint3x4(particle.Position);
				v.x = Mathf.Clamp(v.x, particle.MoveLimitMin.x, particle.MoveLimitMax.x);
				v.y = Mathf.Clamp(v.y, particle.MoveLimitMin.y, particle.MoveLimitMax.y);
				v.z = Mathf.Clamp(v.z, particle.MoveLimitMin.z, particle.MoveLimitMax.z);
				particle.Position = localToWorldMatrix2.MultiplyPoint3x4(v);
			}
		}
	}

	private void SkipUpdateParticles()
	{
		for (int i = 0; i < Particles.Count; i++)
		{
			Particle particle = Particles[i];
			if (particle.ParentIndex >= 0)
			{
				Vector3 vector = ObjectMove * particle.Inert;
				particle.PrevPosition += vector;
				particle.Position += vector;
				Particle particle2 = Particles[particle.ParentIndex];
				float num = ((!(particle.Transform != null)) ? (particle.EndOffset.magnitude * ObjectScale) : (particle2.Transform.position - particle.Transform.position).magnitude);
				Matrix4x4 localToWorldMatrix = particle2.Transform.localToWorldMatrix;
				localToWorldMatrix.SetColumn(3, new Vector4(particle2.Position.x, particle2.Position.y, particle2.Position.z, 1f));
				Vector3 vector2 = ((!(particle.Transform != null)) ? localToWorldMatrix.MultiplyPoint3x4(particle.EndOffset) : localToWorldMatrix.MultiplyPoint3x4(particle.LocalPosition));
				float num2 = Mathf.Lerp(1f, particle.Stiffness, Weight);
				if (num2 > 0f)
				{
					Vector3 vector3 = vector2 - particle.Position;
					float magnitude = vector3.magnitude;
					float num3 = num * (1f - num2) * 2f;
					if (magnitude > num3)
					{
						particle.Position += vector3 * ((magnitude - num3) / magnitude);
					}
				}
				Vector3 vector4 = particle2.Position - particle.Position;
				float magnitude2 = vector4.magnitude;
				if (magnitude2 > 0f)
				{
					float num4 = (magnitude2 - num) / magnitude2;
					if (particle.KeepLengthLimitMin >= num4)
					{
						particle.Position += vector4 * (num4 - particle.KeepLengthLimitMin);
					}
					else if (num4 >= particle.KeepLengthLimitMax)
					{
						particle.Position += vector4 * (num4 - particle.KeepLengthLimitMax);
					}
				}
				if ((bool)particle.Transform && particle.IsMoveLimit)
				{
					Matrix4x4 localToWorldMatrix2 = particle.Transform.localToWorldMatrix;
					localToWorldMatrix2.SetColumn(3, new Vector4(vector2.x, vector2.y, vector2.z, 1f));
					Vector3 v = localToWorldMatrix2.inverse.MultiplyPoint3x4(particle.Position);
					v.x = Mathf.Clamp(v.x, particle.MoveLimitMin.x, particle.MoveLimitMax.x);
					v.y = Mathf.Clamp(v.y, particle.MoveLimitMin.y, particle.MoveLimitMax.y);
					v.z = Mathf.Clamp(v.z, particle.MoveLimitMin.z, particle.MoveLimitMax.z);
					particle.Position = localToWorldMatrix2.MultiplyPoint3x4(v);
				}
			}
			else
			{
				particle.PrevPosition = particle.Position;
				particle.Position = particle.Transform.position;
			}
		}
	}

	private void ApplyParticlesToTransforms()
	{
		for (int i = 1; i < Particles.Count; i++)
		{
			Particle particle = Particles[i];
			Particle particle2 = Particles[particle.ParentIndex];
			if (particle2.IsRotationCalc)
			{
				Vector3 direction = ((!(particle.Transform != null)) ? particle.EndOffset : particle.LocalPosition);
				Vector3 vector = particle2.Transform.TransformDirection(direction);
				Vector3 toDirection = particle.Position - particle2.Position;
				float magnitude = direction.magnitude;
				if (magnitude != 0f)
				{
					toDirection = particle.Position + vector * -1f * (1f - particle2.ScaleNextBoneLength) - particle2.Position;
				}
				Quaternion quaternion = Quaternion.FromToRotation(vector, toDirection);
				particle2.Transform.rotation = quaternion * particle2.Transform.rotation;
			}
			if (!particle.Transform)
			{
				continue;
			}
			Vector3 vector2 = particle.Transform.localToWorldMatrix.inverse.MultiplyPoint3x4(particle.Position);
			if (particle.IsCrush)
			{
				float num = 0f;
				if (vector2.z <= 0f)
				{
					float num2 = Mathf.Clamp01(Mathf.InverseLerp(particle.CrushMoveAreaMin, 0f, vector2.z));
					num = particle.CrushAddXYMin * (1f - num2);
				}
				else
				{
					float num3 = Mathf.Clamp01(Mathf.InverseLerp(0f, particle.CrushMoveAreaMax, vector2.z));
					num = particle.CrushAddXYMax * num3;
				}
				particle.Transform.localScale = particle.InitLocalScale + new Vector3(num, num, 0f);
			}
			particle.Transform.position = particle.Position;
		}
	}

	private void CalcLocalPosition(Particle _particle, Particle _parent)
	{
		_particle.LocalPosition = _parent.Transform.InverseTransformPoint(_particle.Position);
	}

	private Vector3 CalcLocalPosition(Vector3 _particle, Transform _parent)
	{
		return _parent.InverseTransformPoint(_particle);
	}

	private bool IsRefTransform()
	{
		if (Patterns == null)
		{
			return false;
		}
		foreach (BonePtn pattern in Patterns)
		{
			if (pattern.Params == null)
			{
				return false;
			}
			foreach (BoneParameter param in pattern.Params)
			{
				if (param.RefTransform == null)
				{
					return false;
				}
			}
		}
		return true;
	}

	private Transform FindLoop(Transform transform, string name)
	{
		if (string.Compare(name, transform.name) == 0)
		{
			return transform;
		}
		foreach (Transform item in transform)
		{
			Transform transform3 = FindLoop(item, name);
			if (null != transform3)
			{
				return transform3;
			}
		}
		return null;
	}

	private bool LoadText(LoadInfo _info, List<string> _list, ref int _index)
	{
		string[] array = _list[_index].Split('\t');
		int num = array.Length;
		if (num == 0)
		{
			return false;
		}
		string text = array[0].Substring(0, 2);
		if (text.Equals("//"))
		{
			_index++;
			return true;
		}
		switch (array[0])
		{
		case "#Comment":
			_info.Comment = array[1];
			break;
		case "#ReflectSpeed":
		{
			float result;
			if (!float.TryParse(array[1], out result))
			{
				return false;
			}
			_info.ReflectSpeed = result;
			break;
		}
		case "#HeavyLoopMaxCount":
		{
			int result2;
			if (!int.TryParse(array[1], out result2))
			{
				return false;
			}
			_info.HeavyLoopMaxCount = result2;
			break;
		}
		case "#Colliders name":
		{
			for (int i = 1; i < num && !(array[i] == string.Empty) && !(array[i] == " "); i++)
			{
				Transform transform = FindLoop(base.transform, array[i]);
				if (transform == null)
				{
					return false;
				}
				DynamicBoneCollider component = transform.GetComponent<DynamicBoneCollider>();
				if (component == null)
				{
					return false;
				}
				_info.Colliders.Add(component);
			}
			break;
		}
		case "#Bone name":
		{
			for (int j = 1; j < num && !(array[j] == string.Empty) && !(array[j] == " "); j++)
			{
				Transform transform2 = FindLoop(base.transform, array[j]);
				if (transform2 == null)
				{
					return false;
				}
				_info.Bones.Add(transform2);
			}
			break;
		}
		case "#PtnClassMember":
		{
			BonePtn bonePtn = new BonePtn();
			if (!LoadPtnClassMember(bonePtn, array, _index))
			{
				return false;
			}
			_index++;
			if (!LoadParamClassMember(bonePtn, _list, ref _index))
			{
				return false;
			}
			_info.Patterns.Add(bonePtn);
			return true;
		}
		default:
			return false;
		}
		_index++;
		return true;
	}

	private bool LoadPtnClassMember(BonePtn _ptn, string[] _str, int _index)
	{
		int length = _str.Length;
		int _index2 = 0;
		if (!ChekcLength(length, ref _index2, _index, "[PtnClassMember] 表示する名前", string.Empty))
		{
			return false;
		}
		_ptn.Name = _str[_index2];
		float _param;
		if (!GetMemberFloat(length, _str, ref _index2, _index, out _param, "[PtnClassMember] 重力X", string.Empty))
		{
			return false;
		}
		Vector3 gravity = _ptn.Gravity;
		gravity.x = _param;
		_ptn.Gravity = gravity;
		if (!GetMemberFloat(length, _str, ref _index2, _index, out _param, "[PtnClassMember] 重力Y", string.Empty))
		{
			return false;
		}
		gravity = _ptn.Gravity;
		gravity.y = _param;
		_ptn.Gravity = gravity;
		if (!GetMemberFloat(length, _str, ref _index2, _index, out _param, "[PtnClassMember] 重力Z", string.Empty))
		{
			return false;
		}
		gravity = _ptn.Gravity;
		gravity.z = _param;
		_ptn.Gravity = gravity;
		if (!GetMemberFloat(length, _str, ref _index2, _index, out _param, "[PtnClassMember] EndOffsetX", string.Empty))
		{
			return false;
		}
		gravity = _ptn.EndOffset;
		gravity.x = _param;
		_ptn.EndOffset = gravity;
		if (!GetMemberFloat(length, _str, ref _index2, _index, out _param, "[PtnClassMember] EndOffsetY", string.Empty))
		{
			return false;
		}
		gravity = _ptn.EndOffset;
		gravity.y = _param;
		_ptn.EndOffset = gravity;
		if (!GetMemberFloat(length, _str, ref _index2, _index, out _param, "[PtnClassMember] EndOffsetZ", string.Empty))
		{
			return false;
		}
		gravity = _ptn.EndOffset;
		gravity.z = _param;
		_ptn.EndOffset = gravity;
		if (!GetMemberFloat(length, _str, ref _index2, _index, out _param, "[PtnClassMember] EndOffsetの空気抵抗", string.Empty))
		{
			return false;
		}
		_ptn.EndOffsetDamping = _param;
		if (!GetMemberFloat(length, _str, ref _index2, _index, out _param, "[PtnClassMember] EndOffsetの弾力", string.Empty))
		{
			return false;
		}
		_ptn.EndOffsetElasticity = _param;
		if (!GetMemberFloat(length, _str, ref _index2, _index, out _param, "[PtnClassMember] EndOffsetの硬さ", string.Empty))
		{
			return false;
		}
		_ptn.EndOffsetStiffness = _param;
		if (!GetMemberFloat(length, _str, ref _index2, _index, out _param, "[PtnClassMember] EndOffsetの惰性", string.Empty))
		{
			return false;
		}
		_ptn.EndOffsetInert = _param;
		return true;
	}

	private bool LoadParamClassMember(BonePtn _ptn, List<string> _list, ref int _index)
	{
		while (_list.Count > _index)
		{
			string[] array = _list[_index].Split('\t');
			int num = array.Length;
			int _index2 = 0;
			float _param = 0f;
			bool _param2 = false;
			if (num <= _index2)
			{
				return false;
			}
			string text = array[_index2].Substring(0, 2);
			if (text.Equals("//"))
			{
				_index++;
				continue;
			}
			if (array[_index2] != "#ParamClassMember")
			{
				break;
			}
			BoneParameter boneParameter = new BoneParameter();
			if (!ChekcLength(num, ref _index2, _index, "[ParamClassMember] 表示する名前", string.Empty))
			{
				return false;
			}
			boneParameter.Name = array[_index2];
			if (!ChekcLength(num, ref _index2, _index, "[ParamClassMember] 参照するフレーム", string.Empty))
			{
				return false;
			}
			Transform transform = FindLoop(base.transform, array[_index2]);
			if (transform == null)
			{
				return false;
			}
			boneParameter.RefTransform = transform;
			if (!GetMemberBool(num, array, ref _index2, _index, out _param2, "[ParamClassMember] 回転するか ", string.Empty))
			{
				return false;
			}
			boneParameter.IsRotationCalc = _param2;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 空気抵抗", string.Empty))
			{
				return false;
			}
			boneParameter.Damping = _param;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 弾力", string.Empty))
			{
				return false;
			}
			boneParameter.Elasticity = _param;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 硬さ", string.Empty))
			{
				return false;
			}
			boneParameter.Stiffness = _param;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 惰性", string.Empty))
			{
				return false;
			}
			boneParameter.Inert = _param;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 次の骨までの距離補正", string.Empty))
			{
				return false;
			}
			boneParameter.NextBoneLength = _param;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 当たり判定の半径", string.Empty))
			{
				return false;
			}
			boneParameter.CollisionRadius = _param;
			if (!GetMemberBool(num, array, ref _index2, _index, out _param2, "[ParamClassMember] 移動制限するか ", string.Empty))
			{
				return false;
			}
			boneParameter.IsMoveLimit = _param2;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 移動制限最小X", string.Empty))
			{
				return false;
			}
			Vector3 moveLimitMin = boneParameter.MoveLimitMin;
			moveLimitMin.x = _param;
			boneParameter.MoveLimitMin = moveLimitMin;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 移動制限最小Y", string.Empty))
			{
				return false;
			}
			moveLimitMin = boneParameter.MoveLimitMin;
			moveLimitMin.y = _param;
			boneParameter.MoveLimitMin = moveLimitMin;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 移動制限最小Z", string.Empty))
			{
				return false;
			}
			moveLimitMin = boneParameter.MoveLimitMin;
			moveLimitMin.z = _param;
			boneParameter.MoveLimitMin = moveLimitMin;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 移動制限最大X", string.Empty))
			{
				return false;
			}
			moveLimitMin = boneParameter.MoveLimitMax;
			moveLimitMin.x = _param;
			boneParameter.MoveLimitMax = moveLimitMin;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 移動制限最大Y", string.Empty))
			{
				return false;
			}
			moveLimitMin = boneParameter.MoveLimitMax;
			moveLimitMin.y = _param;
			boneParameter.MoveLimitMax = moveLimitMin;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 移動制限最大Z", string.Empty))
			{
				return false;
			}
			moveLimitMin = boneParameter.MoveLimitMax;
			moveLimitMin.z = _param;
			boneParameter.MoveLimitMax = moveLimitMin;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 親からの長さの補正しない範囲最小", string.Empty))
			{
				return false;
			}
			boneParameter.KeepLengthLimitMin = _param;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 親からの長さの補正しない範囲最大", string.Empty))
			{
				return false;
			}
			boneParameter.KeepLengthLimitMax = _param;
			if (!GetMemberBool(num, array, ref _index2, _index, out _param2, "[ParamClassMember] 潰すか ", string.Empty))
			{
				return false;
			}
			boneParameter.IsCrush = _param2;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 潰す移動判断範囲最小", string.Empty))
			{
				return false;
			}
			boneParameter.CrushMoveAreaMin = _param;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 潰す移動判断範囲最大", string.Empty))
			{
				return false;
			}
			boneParameter.CrushMoveAreaMax = _param;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 潰れた時に加算するXYスケール", string.Empty))
			{
				return false;
			}
			boneParameter.CrushAddXYMin = _param;
			if (!GetMemberFloat(num, array, ref _index2, _index, out _param, "[ParamClassMember] 伸びた時に加算するXYスケール", string.Empty))
			{
				return false;
			}
			boneParameter.CrushAddXYMax = _param;
			_ptn.Params.Add(boneParameter);
			_index++;
		}
		return true;
	}

	private bool ChekcLength(int _length, ref int _index, int _line, string _warning = "", string _warning1 = "")
	{
		if (_length <= ++_index)
		{
			return false;
		}
		return true;
	}

	private bool GetMemberFloat(int _length, string[] _str, ref int _index, int _line, out float _param, string _warning = "", string _warning1 = "")
	{
		_param = 0f;
		if (!ChekcLength(_length, ref _index, _line, _warning, string.Empty))
		{
			return false;
		}
		if (!float.TryParse(_str[_index], out _param))
		{
			return false;
		}
		return true;
	}

	private bool GetMemberInt(int _length, string[] _str, ref int _index, int _line, out int _param, string _warning = "", string _warning1 = "")
	{
		_param = 0;
		if (!ChekcLength(_length, ref _index, _line, _warning, string.Empty))
		{
			return false;
		}
		if (!int.TryParse(_str[_index], out _param))
		{
			return false;
		}
		return true;
	}

	private bool GetMemberBool(int _length, string[] _str, ref int _index, int _line, out bool _param, string _warning = "", string _warning1 = "")
	{
		_param = false;
		if (!ChekcLength(_length, ref _index, _line, _warning, string.Empty))
		{
			return false;
		}
		if (_str[_index] == "false" || _str[_index] == "FALSE" || _str[_index] == "False")
		{
			_param = false;
			return true;
		}
		if (_str[_index] == "true" || _str[_index] == "TRUE" || _str[_index] == "True")
		{
			_param = true;
			return true;
		}
		return false;
	}

	private void SaveText(StreamWriter _writer)
	{
		_writer.Write("//コメント\n");
		_writer.Write("#Comment\t" + Comment + "\n");
		_writer.Write("//粒子のスピード\n");
		_writer.Write("#ReflectSpeed\t" + ReflectSpeed + "\n");
		_writer.Write("//重い時に何回まで回すか\u3000回数多いと正確になるけど更に重くなるよ\n");
		_writer.Write("#HeavyLoopMaxCount\t" + HeavyLoopMaxCount + "\n");
		_writer.Write("//登録する当たり判定の名前\n");
		_writer.Write("#Colliders name\t");
		foreach (DynamicBoneCollider collider in Colliders)
		{
			_writer.Write(collider.gameObject.name + "\t");
		}
		_writer.Write("\n");
		_writer.Write("//登録する骨の名前\n");
		_writer.Write("#Bone name\t");
		foreach (Transform bone in Bones)
		{
			_writer.Write(bone.name + "\t");
		}
		_writer.Write("\n");
		foreach (BonePtn pattern in Patterns)
		{
			_writer.Write("//パターンの設定\n");
			_writer.Write("//PtnClass\t");
			_writer.Write("表示する名前\t");
			_writer.Write("重力 X\t");
			_writer.Write("重力 Y\t");
			_writer.Write("重力 Z\t");
			_writer.Write("EndOffset x\t");
			_writer.Write("EndOffset y\t");
			_writer.Write("EndOffset z\t");
			_writer.Write("EndOffsetの空気抵抗\t");
			_writer.Write("EndOffsetの弾力\t");
			_writer.Write("EndOffsetの硬さ\t");
			_writer.Write("EndOffsetの惰性\t");
			_writer.Write("\n");
			_writer.Write("#PtnClassMember\t");
			_writer.Write(pattern.Name + "\t");
			_writer.Write(pattern.Gravity.x + "\t");
			_writer.Write(pattern.Gravity.y + "\t");
			_writer.Write(pattern.Gravity.z + "\t");
			_writer.Write(pattern.EndOffset.x + "\t");
			_writer.Write(pattern.EndOffset.y + "\t");
			_writer.Write(pattern.EndOffset.z + "\t");
			_writer.Write(pattern.EndOffsetDamping + "\t");
			_writer.Write(pattern.EndOffsetElasticity + "\t");
			_writer.Write(pattern.EndOffsetStiffness + "\t");
			_writer.Write(pattern.EndOffsetInert + "\t");
			_writer.Write("\n");
			_writer.Write("//そのパターンの骨に対するパラメーター\n");
			_writer.Write("//ParamClass\t");
			_writer.Write("表示する名前\t");
			_writer.Write("参照するフレーム名\t");
			_writer.Write("回転する？\t");
			_writer.Write("空気抵抗\t");
			_writer.Write("弾力\t");
			_writer.Write("硬さ\t");
			_writer.Write("惰性\t");
			_writer.Write("次の骨までの距離補正\t");
			_writer.Write("当たり判定の半径\t");
			_writer.Write("移動制限する？\t");
			_writer.Write("移動制限最小X\t");
			_writer.Write("移動制限最小Y\t");
			_writer.Write("移動制限最小Z\t");
			_writer.Write("移動制限最大X\t");
			_writer.Write("移動制限最大Y\t");
			_writer.Write("移動制限最大Z\t");
			_writer.Write("親からの長さを補正しない範囲最小値\t");
			_writer.Write("親からの長さを補正しない範囲最大値\t");
			_writer.Write("潰す？\t");
			_writer.Write("潰す移動判断範囲最小\t");
			_writer.Write("潰す移動判断範囲最大\t");
			_writer.Write("潰れた時に加算するXYスケール\t");
			_writer.Write("伸びた時に加算するXYスケール\t");
			_writer.Write("\n");
			foreach (BoneParameter param in pattern.Params)
			{
				_writer.Write("#ParamClassMember\t");
				_writer.Write(param.Name + "\t");
				string empty = string.Empty;
				if (param.RefTransform != null)
				{
					empty = param.RefTransform.name;
				}
				_writer.Write(empty + "\t");
				_writer.Write(param.IsRotationCalc + "\t");
				_writer.Write(param.Damping + "\t");
				_writer.Write(param.Elasticity + "\t");
				_writer.Write(param.Stiffness + "\t");
				_writer.Write(param.Inert + "\t");
				_writer.Write(param.NextBoneLength + "\t");
				_writer.Write(param.CollisionRadius + "\t");
				_writer.Write(param.IsMoveLimit + "\t");
				_writer.Write(param.MoveLimitMin.x + "\t");
				_writer.Write(param.MoveLimitMin.y + "\t");
				_writer.Write(param.MoveLimitMin.z + "\t");
				_writer.Write(param.MoveLimitMax.x + "\t");
				_writer.Write(param.MoveLimitMax.y + "\t");
				_writer.Write(param.MoveLimitMax.z + "\t");
				_writer.Write(param.KeepLengthLimitMin + "\t");
				_writer.Write(param.KeepLengthLimitMax + "\t");
				_writer.Write(param.IsCrush + "\t");
				_writer.Write(param.CrushMoveAreaMin + "\t");
				_writer.Write(param.CrushMoveAreaMin + "\t");
				_writer.Write(param.CrushAddXYMin + "\t");
				_writer.Write(param.CrushAddXYMax + "\t");
				_writer.Write("\n");
			}
		}
	}
}
