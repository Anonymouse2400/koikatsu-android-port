using System;
using System.Collections.Generic;
using System.Linq;
using RootMotion.FinalIK;
using UnityEngine;

public class HitReaction : MonoBehaviour
{
	[Serializable]
	public abstract class HitPoint
	{
		public string name;

		[SerializeField]
		private float crossFadeTime = 0.1f;

		private float length;

		private float crossFadeSpeed;

		private float lastTime;

		protected float crossFader { get; private set; }

		protected float timer { get; private set; }

		protected Vector3 force { get; private set; }

		public void Hit(Vector3 force)
		{
			if (length == 0f)
			{
				length = GetLength();
			}
			if (!(length <= 0f))
			{
				if (timer < 1f)
				{
					crossFader = 0f;
				}
				crossFadeSpeed = ((!(crossFadeTime > 0f)) ? 0f : (1f / crossFadeTime));
				CrossFadeStart();
				timer = 0f;
				this.force = force;
			}
		}

		public void Apply(IKSolverFullBodyBiped solver, float weight)
		{
			float num = Time.time - lastTime;
			lastTime = Time.time;
			if (!(timer >= length))
			{
				timer = Mathf.Clamp(timer + num, 0f, length);
				if (crossFadeSpeed > 0f)
				{
					crossFader = Mathf.Clamp(crossFader + num * crossFadeSpeed, 0f, 1f);
				}
				else
				{
					crossFader = 1f;
				}
				OnApply(solver, weight);
			}
		}

		public bool IsPlay()
		{
			if (timer >= length)
			{
				return false;
			}
			return true;
		}

		public float GetLerpPlay()
		{
			return (length == 0f) ? 1f : (timer / length);
		}

		protected abstract float GetLength();

		protected abstract void CrossFadeStart();

		protected abstract void OnApply(IKSolverFullBodyBiped solver, float weight);
	}

	[Serializable]
	public class HitPointEffector : HitPoint
	{
		[Serializable]
		public class EffectorLink
		{
			public FullBodyBipedEffector effector;

			public float weight;

			private Vector3 lastValue;

			private Vector3 current;

			public void Apply(IKSolverFullBodyBiped solver, Vector3 offset, float crossFader)
			{
				current = Vector3.Lerp(lastValue, offset * weight, crossFader);
				solver.GetEffector(effector).positionOffset += current;
			}

			public void CrossFadeStart()
			{
				lastValue = current;
			}
		}

		public AnimationCurve offsetInForceDirection;

		public AnimationCurve offsetInUpDirection;

		public EffectorLink[] effectorLinks;

		protected override float GetLength()
		{
			float num = ((offsetInForceDirection.keys.Length <= 0) ? 0f : offsetInForceDirection.keys[offsetInForceDirection.length - 1].time);
			float min = ((offsetInUpDirection.keys.Length <= 0) ? 0f : offsetInUpDirection.keys[offsetInUpDirection.length - 1].time);
			return Mathf.Clamp(num, min, num);
		}

		protected override void CrossFadeStart()
		{
			EffectorLink[] array = effectorLinks;
			foreach (EffectorLink effectorLink in array)
			{
				effectorLink.CrossFadeStart();
			}
		}

		protected override void OnApply(IKSolverFullBodyBiped solver, float weight)
		{
			Vector3 vector = solver.GetRoot().up * base.force.magnitude;
			Vector3 offset = offsetInForceDirection.Evaluate(base.timer) * base.force + offsetInUpDirection.Evaluate(base.timer) * vector;
			offset *= weight;
			EffectorLink[] array = effectorLinks;
			foreach (EffectorLink effectorLink in array)
			{
				effectorLink.Apply(solver, offset, base.crossFader);
			}
		}
	}

	[Serializable]
	public class HitPointEffectorParent
	{
		public string name;

		public int id;

		public HitPointEffector[] effectorHitPoints;

		public void Hit(Vector3 force)
		{
			HitPointEffector[] array = effectorHitPoints;
			foreach (HitPointEffector hitPointEffector in array)
			{
				hitPointEffector.Hit(force);
			}
		}

		public void Apply(IKSolverFullBodyBiped solver, float weight)
		{
			HitPointEffector[] array = effectorHitPoints;
			foreach (HitPointEffector hitPointEffector in array)
			{
				hitPointEffector.Apply(solver, weight);
			}
		}

		public bool IsPlay()
		{
			bool flag = false;
			HitPointEffector[] array = effectorHitPoints;
			foreach (HitPointEffector hitPointEffector in array)
			{
				flag |= hitPointEffector.IsPlay();
			}
			return flag;
		}
	}

	public float weight = 1f;

	public FullBodyBipedIK ik;

	public HitPointEffectorParent[] effectorHit;

	private List<int> lstEffector = new List<int>();

	public void LateUpdate()
	{
		if (ik == null)
		{
			return;
		}
		HitPointEffectorParent[] array = effectorHit;
		foreach (HitPointEffectorParent hitPointEffectorParent in array)
		{
			hitPointEffectorParent.Apply(ik.solver, weight);
		}
		float num = 1f;
		HitPointEffectorParent[] array2 = effectorHit;
		foreach (HitPointEffectorParent hitPointEffectorParent2 in array2)
		{
			if (!hitPointEffectorParent2.IsPlay())
			{
				continue;
			}
			HitPointEffector[] effectorHitPoints = hitPointEffectorParent2.effectorHitPoints;
			foreach (HitPointEffector hitPointEffector in effectorHitPoints)
			{
				if (hitPointEffector.IsPlay())
				{
					num = hitPointEffector.GetLerpPlay();
					break;
				}
			}
		}
		if (lstEffector.Count == 0)
		{
			return;
		}
		using (List<int>.Enumerator enumerator = lstEffector.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current)
				{
				case 0:
					ik.solver.leftHandEffector.positionWeight = num;
					break;
				case 1:
					ik.solver.rightHandEffector.positionWeight = num;
					break;
				case 2:
					ik.solver.leftFootEffector.positionWeight = num;
					ik.solver.leftFootEffector.rotationWeight = num;
					break;
				case 3:
					ik.solver.rightFootEffector.positionWeight = num;
					ik.solver.rightFootEffector.rotationWeight = num;
					break;
				}
			}
		}
		if (num >= 1f)
		{
			lstEffector.Clear();
		}
	}

	public void Hit(int _nid, Vector3 force)
	{
		if (ik == null)
		{
			return;
		}
		HitPointEffectorParent[] array = effectorHit;
		foreach (HitPointEffectorParent hitPointEffectorParent in array)
		{
			if (hitPointEffectorParent.id == _nid)
			{
				hitPointEffectorParent.Hit(force);
			}
		}
	}

	public void HitsEffector(int _nid, Vector3[] forces)
	{
		if (ik == null)
		{
			return;
		}
		HitPointEffectorParent[] array = effectorHit;
		foreach (HitPointEffectorParent hitPointEffectorParent in array)
		{
			if (hitPointEffectorParent.id != _nid || hitPointEffectorParent.effectorHitPoints.Length != forces.Length)
			{
				continue;
			}
			foreach (var item in hitPointEffectorParent.effectorHitPoints.Select((HitPointEffector value, int index) => new { value, index }))
			{
				item.value.Hit(forces[item.index]);
			}
		}
	}

	public bool IsPlay()
	{
		if (ik == null)
		{
			return false;
		}
		bool flag = false;
		HitPointEffectorParent[] array = effectorHit;
		foreach (HitPointEffectorParent hitPointEffectorParent in array)
		{
			flag |= hitPointEffectorParent.IsPlay();
		}
		return flag;
	}

	public void SetEffector(int _effector)
	{
		lstEffector.Add(_effector);
	}

	public void SetEffector(List<int> _effector)
	{
		lstEffector.AddRange(_effector);
	}

	public void ReleaseEffector()
	{
		using (List<int>.Enumerator enumerator = lstEffector.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current)
				{
				case 0:
					ik.solver.leftHandEffector.positionWeight = 1f;
					break;
				case 1:
					ik.solver.rightHandEffector.positionWeight = 1f;
					break;
				case 2:
					ik.solver.leftFootEffector.positionWeight = 1f;
					ik.solver.leftFootEffector.rotationWeight = 1f;
					break;
				case 3:
					ik.solver.rightFootEffector.positionWeight = 1f;
					ik.solver.rightFootEffector.rotationWeight = 1f;
					break;
				}
			}
		}
		lstEffector.Clear();
	}
}
