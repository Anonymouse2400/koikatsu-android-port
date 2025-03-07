using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Dynamic Bone/Dynamic Bone Ver01")]
public class DynamicBone_Ver01 : MonoBehaviour
{
	[Serializable]
	public class BoneNode
	{
		public Transform Transform;

		public bool RotationCalc = true;
	}

	private class Particle
	{
		public Transform m_Transform;

		public int m_ParentIndex = -1;

		public float m_Damping;

		public float m_Elasticity;

		public float m_Stiffness;

		public float m_Inert;

		public float m_Radius;

		public float m_BoneLength;

		public float m_AddAngleScale;

		public bool m_bRotationCalc = true;

		public Vector3 m_Position = Vector3.zero;

		public Vector3 m_PrevPosition = Vector3.zero;

		public Vector3 m_EndOffset = Vector3.zero;

		public Vector3 m_InitLocalPosition = Vector3.zero;

		public Quaternion m_InitLocalRotation = Quaternion.identity;

		public Vector3 m_InitEuler = Vector3.zero;

		public Vector3 m_LocalPosition = Vector3.zero;
	}

	public string comment = string.Empty;

	public Transform m_Root;

	public float m_UpdateRate = 60f;

	[Range(0f, 1f)]
	public float m_Damping = 0.1f;

	public AnimationCurve m_DampingDistrib;

	[Range(0f, 1f)]
	public float m_Elasticity = 0.1f;

	public AnimationCurve m_ElasticityDistrib;

	[Range(0f, 1f)]
	public float m_Stiffness = 0.1f;

	public AnimationCurve m_StiffnessDistrib;

	[Range(0f, 1f)]
	public float m_Inert;

	public AnimationCurve m_InertDistrib;

	public float m_Radius;

	public AnimationCurve m_RadiusDistrib;

	public float m_AddAngleScale = 1f;

	public AnimationCurve m_AddAngleScaleDistrib;

	public float m_EndLength;

	public Vector3 m_EndOffset = Vector3.zero;

	public Vector3 m_Gravity = Vector3.zero;

	public Vector3 m_Force = Vector3.zero;

	public List<DynamicBoneCollider> m_Colliders;

	public List<BoneNode> m_Nodes;

	private Vector3 m_ObjectMove = Vector3.zero;

	private Vector3 m_ObjectPrevPosition = Vector3.zero;

	private float m_BoneTotalLength;

	private float m_ObjectScale = 1f;

	private float m_Time;

	private float m_Weight = 1f;

	private List<Particle> m_Particles = new List<Particle>();

	private void Start()
	{
		SetupParticles();
	}

	private void Update()
	{
		if (m_Weight > 0f)
		{
			InitTransforms();
		}
	}

	private void LateUpdate()
	{
		if (m_Weight > 0f)
		{
			UpdateDynamicBones(Time.deltaTime);
		}
	}

	private void OnEnable()
	{
		ResetParticlesPosition();
		m_ObjectPrevPosition = base.transform.position;
	}

	private void OnDisable()
	{
		InitTransforms();
	}

	private void OnValidate()
	{
		m_UpdateRate = Mathf.Max(m_UpdateRate, 0f);
		m_Damping = Mathf.Clamp01(m_Damping);
		m_Elasticity = Mathf.Clamp01(m_Elasticity);
		m_Stiffness = Mathf.Clamp01(m_Stiffness);
		m_Inert = Mathf.Clamp01(m_Inert);
		m_Radius = Mathf.Max(m_Radius, 0f);
		m_AddAngleScale = Mathf.Max(m_AddAngleScale, 0f);
		if (Application.isEditor && Application.isPlaying)
		{
			InitTransforms();
			SetupParticles();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!base.enabled || m_Root == null)
		{
			return;
		}
		if (Application.isEditor && !Application.isPlaying && base.transform.hasChanged)
		{
			InitTransforms();
			SetupParticles();
		}
		Gizmos.color = Color.white;
		foreach (Particle particle2 in m_Particles)
		{
			if (particle2.m_ParentIndex >= 0)
			{
				Particle particle = m_Particles[particle2.m_ParentIndex];
				Gizmos.DrawLine(particle2.m_Position, particle.m_Position);
			}
			if (particle2.m_Radius > 0f)
			{
				Gizmos.DrawWireSphere(particle2.m_Position, particle2.m_Radius * m_ObjectScale);
			}
		}
	}

	public void SetWeight(float w)
	{
		if (m_Weight != w)
		{
			if (w == 0f)
			{
				InitTransforms();
			}
			else if (m_Weight == 0f)
			{
				ResetParticlesPosition();
				m_ObjectPrevPosition = base.transform.position;
			}
			m_Weight = w;
		}
	}

	public float GetWeight()
	{
		return m_Weight;
	}

	public void setRoot(Transform _transRoot)
	{
		m_Root = _transRoot;
	}

	private void UpdateDynamicBones(float t)
	{
		if (m_Root == null)
		{
			return;
		}
		m_ObjectScale = Mathf.Abs(base.transform.lossyScale.x);
		m_ObjectMove = base.transform.position - m_ObjectPrevPosition;
		m_ObjectPrevPosition = base.transform.position;
		int num = 1;
		if (m_UpdateRate > 0f)
		{
			float num2 = 1f / m_UpdateRate;
			m_Time += t;
			num = 0;
			while (m_Time >= num2)
			{
				m_Time -= num2;
				if (++num >= 3)
				{
					m_Time = 0f;
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
				m_ObjectMove = Vector3.zero;
			}
		}
		else
		{
			SkipUpdateParticles();
		}
		ApplyParticlesToTransforms();
	}

	public void SetupParticles()
	{
		m_Particles.Clear();
		if (m_Root == null && m_Nodes.Count > 0)
		{
			m_Root = m_Nodes[0].Transform;
		}
		if (m_Root == null)
		{
			return;
		}
		m_ObjectScale = base.transform.lossyScale.x;
		m_ObjectPrevPosition = base.transform.position;
		m_ObjectMove = Vector3.zero;
		m_BoneTotalLength = 0f;
		Particle particle = null;
		int num = -1;
		foreach (BoneNode node in m_Nodes)
		{
			float boneLength = ((particle == null) ? 0f : particle.m_BoneLength);
			particle = AppendParticles(node, num, boneLength);
			num++;
		}
		if (m_EndLength > 0f || m_EndOffset.magnitude != 0f)
		{
			AppendParticles(new BoneNode(), num, particle.m_BoneLength);
		}
		float num2 = ((m_Particles.Count > 1) ? (1f / (float)(m_Particles.Count - 1)) : 0f);
		float num3 = 0f;
		foreach (Particle particle2 in m_Particles)
		{
			particle2.m_Damping = m_Damping;
			particle2.m_Elasticity = m_Elasticity;
			particle2.m_Stiffness = m_Stiffness;
			particle2.m_Inert = m_Inert;
			particle2.m_Radius = m_Radius;
			particle2.m_AddAngleScale = m_AddAngleScale;
			if (m_DampingDistrib.keys.Length > 0)
			{
				particle2.m_Damping *= m_DampingDistrib.Evaluate(num3);
			}
			if (m_ElasticityDistrib.keys.Length > 0)
			{
				particle2.m_Elasticity *= m_ElasticityDistrib.Evaluate(num3);
			}
			if (m_StiffnessDistrib.keys.Length > 0)
			{
				particle2.m_Stiffness *= m_StiffnessDistrib.Evaluate(num3);
			}
			if (m_InertDistrib.keys.Length > 0)
			{
				particle2.m_Inert *= m_InertDistrib.Evaluate(num3);
			}
			if (m_RadiusDistrib.keys.Length > 0)
			{
				particle2.m_Radius *= m_RadiusDistrib.Evaluate(num3);
			}
			if (m_AddAngleScaleDistrib.keys.Length > 0)
			{
				particle2.m_AddAngleScale *= m_AddAngleScaleDistrib.Evaluate(num3);
			}
			num3 += num2;
			particle2.m_Damping = Mathf.Clamp01(particle2.m_Damping);
			particle2.m_Elasticity = Mathf.Clamp01(particle2.m_Elasticity);
			particle2.m_Stiffness = Mathf.Clamp01(particle2.m_Stiffness);
			particle2.m_Inert = Mathf.Clamp01(particle2.m_Inert);
			particle2.m_Radius = Mathf.Max(particle2.m_Radius, 0f);
			particle2.m_AddAngleScale = Mathf.Max(particle2.m_AddAngleScale, 0f);
		}
	}

	public void reloadParameter()
	{
		foreach (Particle particle in m_Particles)
		{
			particle.m_Damping = m_Damping;
			particle.m_Elasticity = m_Elasticity;
			particle.m_Stiffness = m_Stiffness;
			particle.m_Inert = m_Inert;
			particle.m_Radius = m_Radius;
			particle.m_AddAngleScale = m_AddAngleScale;
			particle.m_Damping = Mathf.Clamp01(particle.m_Damping);
			particle.m_Elasticity = Mathf.Clamp01(particle.m_Elasticity);
			particle.m_Stiffness = Mathf.Clamp01(particle.m_Stiffness);
			particle.m_Inert = Mathf.Clamp01(particle.m_Inert);
			particle.m_Radius = Mathf.Max(particle.m_Radius, 0f);
			particle.m_AddAngleScale = Mathf.Max(particle.m_AddAngleScale, 0f);
		}
	}

	private Particle AppendParticles(BoneNode b, int parentIndex, float boneLength)
	{
		Particle particle = new Particle();
		particle.m_Transform = b.Transform;
		particle.m_bRotationCalc = b.RotationCalc;
		particle.m_ParentIndex = parentIndex;
		if (b.Transform != null)
		{
			particle.m_Position = (particle.m_PrevPosition = b.Transform.position);
			particle.m_InitLocalPosition = b.Transform.localPosition;
			particle.m_InitLocalRotation = b.Transform.localRotation;
			particle.m_InitEuler = b.Transform.localEulerAngles;
			if (parentIndex >= 0)
			{
				CalcLocalPosition(particle, m_Particles[parentIndex]);
			}
		}
		else
		{
			Transform transform = m_Particles[parentIndex].m_Transform;
			if (m_EndLength > 0f)
			{
				Transform parent = transform.parent;
				if (parent != null)
				{
					particle.m_EndOffset = transform.InverseTransformPoint(transform.position * 2f - parent.position) * m_EndLength;
				}
				else
				{
					particle.m_EndOffset = new Vector3(m_EndLength, 0f, 0f);
				}
			}
			else
			{
				particle.m_EndOffset = m_EndOffset;
			}
			particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
		}
		if (parentIndex >= 0)
		{
			boneLength += (m_Particles[parentIndex].m_Transform.position - particle.m_Position).magnitude;
			particle.m_BoneLength = boneLength;
			m_BoneTotalLength = Mathf.Max(m_BoneTotalLength, boneLength);
		}
		m_Particles.Add(particle);
		return particle;
	}

	public void InitTransforms()
	{
		foreach (Particle particle in m_Particles)
		{
			if (particle.m_Transform != null)
			{
				particle.m_Transform.localPosition = particle.m_InitLocalPosition;
				particle.m_Transform.localRotation = particle.m_InitLocalRotation;
			}
		}
	}

	public void ResetParticlesPosition()
	{
		m_ObjectPrevPosition = base.transform.position;
		foreach (Particle particle in m_Particles)
		{
			if (particle.m_Transform != null)
			{
				particle.m_Position = (particle.m_PrevPosition = particle.m_Transform.position);
				continue;
			}
			Transform transform = m_Particles[particle.m_ParentIndex].m_Transform;
			particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
		}
	}

	private void UpdateParticles1()
	{
		Vector3 gravity = m_Gravity;
		gravity = (gravity + m_Force) * m_ObjectScale;
		foreach (Particle particle in m_Particles)
		{
			if (particle.m_ParentIndex >= 0)
			{
				Vector3 vector = particle.m_Position - particle.m_PrevPosition;
				Vector3 vector2 = m_ObjectMove * particle.m_Inert;
				particle.m_PrevPosition = particle.m_Position + vector2;
				particle.m_Position += vector * (1f - particle.m_Damping) + gravity + vector2;
			}
			else
			{
				particle.m_PrevPosition = particle.m_Position;
				particle.m_Position = particle.m_Transform.position;
			}
		}
	}

	private void UpdateParticles2()
	{
		for (int i = 1; i < m_Particles.Count; i++)
		{
			Particle particle = m_Particles[i];
			Particle particle2 = m_Particles[particle.m_ParentIndex];
			float num = ((!(particle.m_Transform != null)) ? (particle.m_EndOffset.magnitude * m_ObjectScale) : (particle2.m_Transform.position - particle.m_Transform.position).magnitude);
			float num2 = Mathf.Lerp(1f, particle.m_Stiffness, m_Weight);
			if (num2 > 0f || particle.m_Elasticity > 0f)
			{
				Matrix4x4 localToWorldMatrix = particle2.m_Transform.localToWorldMatrix;
				localToWorldMatrix.SetColumn(3, particle2.m_Position);
				Vector3 vector = ((!(particle.m_Transform != null)) ? localToWorldMatrix.MultiplyPoint3x4(particle.m_EndOffset) : localToWorldMatrix.MultiplyPoint3x4(particle.m_LocalPosition));
				Vector3 vector2 = vector - particle.m_Position;
				particle.m_Position += vector2 * particle.m_Elasticity;
				if (num2 > 0f)
				{
					vector2 = vector - particle.m_Position;
					float magnitude = vector2.magnitude;
					float num3 = num * (1f - num2) * 2f;
					if (magnitude > num3)
					{
						particle.m_Position += vector2 * ((magnitude - num3) / magnitude);
					}
				}
			}
			float particleRadius = particle.m_Radius * m_ObjectScale;
			foreach (DynamicBoneCollider collider in m_Colliders)
			{
				if (collider != null && collider.enabled)
				{
					collider.Collide(ref particle.m_Position, particleRadius);
				}
			}
			Vector3 vector3 = particle2.m_Position - particle.m_Position;
			float magnitude2 = vector3.magnitude;
			if (magnitude2 > 0f)
			{
				particle.m_Position += vector3 * ((magnitude2 - num) / magnitude2);
			}
		}
	}

	private void SkipUpdateParticles()
	{
		foreach (Particle particle2 in m_Particles)
		{
			if (particle2.m_ParentIndex >= 0)
			{
				Vector3 vector = m_ObjectMove * particle2.m_Inert;
				particle2.m_PrevPosition += vector;
				particle2.m_Position += vector;
				Particle particle = m_Particles[particle2.m_ParentIndex];
				float num = ((!(particle2.m_Transform != null)) ? (particle2.m_EndOffset.magnitude * m_ObjectScale) : (particle.m_Transform.position - particle2.m_Transform.position).magnitude);
				float num2 = Mathf.Lerp(1f, particle2.m_Stiffness, m_Weight);
				if (num2 > 0f)
				{
					Matrix4x4 localToWorldMatrix = particle.m_Transform.localToWorldMatrix;
					localToWorldMatrix.SetColumn(3, particle.m_Position);
					Vector3 vector2 = ((!(particle2.m_Transform != null)) ? localToWorldMatrix.MultiplyPoint3x4(particle2.m_EndOffset) : localToWorldMatrix.MultiplyPoint3x4(particle2.m_Transform.localPosition));
					Vector3 vector3 = vector2 - particle2.m_Position;
					float magnitude = vector3.magnitude;
					float num3 = num * (1f - num2) * 2f;
					if (magnitude > num3)
					{
						particle2.m_Position += vector3 * ((magnitude - num3) / magnitude);
					}
				}
				Vector3 vector4 = particle.m_Position - particle2.m_Position;
				float magnitude2 = vector4.magnitude;
				if (magnitude2 > 0f)
				{
					particle2.m_Position += vector4 * ((magnitude2 - num) / magnitude2);
				}
			}
			else
			{
				particle2.m_PrevPosition = particle2.m_Position;
				particle2.m_Position = particle2.m_Transform.position;
			}
		}
	}

	private void ApplyParticlesToTransforms()
	{
		for (int i = 1; i < m_Particles.Count; i++)
		{
			Particle particle = m_Particles[i];
			Particle particle2 = m_Particles[particle.m_ParentIndex];
			if (particle2.m_bRotationCalc)
			{
				Vector3 direction = ((!(particle.m_Transform != null)) ? particle.m_EndOffset : particle.m_LocalPosition);
				Vector3 toDirection = particle.m_Position - particle2.m_Position;
				float angle;
				Vector3 axis;
				Quaternion.FromToRotation(particle2.m_Transform.TransformDirection(direction), toDirection).ToAngleAxis(out angle, out axis);
				angle *= particle.m_AddAngleScale;
				Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
				particle2.m_Transform.rotation = quaternion * particle2.m_Transform.rotation;
			}
			if ((bool)particle.m_Transform)
			{
				particle.m_Transform.position = particle.m_Position;
			}
		}
	}

	private void CalcLocalPosition(Particle particle, Particle parent)
	{
		particle.m_LocalPosition = parent.m_Transform.InverseTransformPoint(particle.m_Position);
	}
}
