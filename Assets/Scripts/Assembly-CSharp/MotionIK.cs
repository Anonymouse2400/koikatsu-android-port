using System;
using System.Collections.Generic;
using System.Linq;
using Illusion;
using Illusion.Component.Correct;
using Illusion.Component.Correct.Process;
using Illusion.Extensions;
using RootMotion.FinalIK;
using UnityEngine;

public class MotionIK
{
	public class IKTargetPair
	{
		public enum IKTarget
		{
			LeftHand = 0,
			RightHand = 1,
			LeftFoot = 2,
			RightFoot = 3
		}

		public IKEffector effector { get; private set; }

		public IKConstraintBend bend { get; private set; }

		public static int IKTargetLength
		{
			get
			{
				return Utils.Enum<IKTarget>.Length * 2;
			}
		}

		public IKTargetPair(IKTarget target, IKSolverFullBodyBiped solver)
		{
			switch (target)
			{
			case IKTarget.LeftHand:
				effector = solver.leftHandEffector;
				bend = solver.leftArmChain.bendConstraint;
				break;
			case IKTarget.RightHand:
				effector = solver.rightHandEffector;
				bend = solver.rightArmChain.bendConstraint;
				break;
			case IKTarget.LeftFoot:
				effector = solver.leftFootEffector;
				bend = solver.leftLegChain.bendConstraint;
				break;
			case IKTarget.RightFoot:
				effector = solver.rightFootEffector;
				bend = solver.rightLegChain.bendConstraint;
				break;
			}
		}

		public static IKTargetPair[] GetPairs(IKSolverFullBodyBiped solver)
		{
			return (from index in Enumerable.Range(0, Utils.Enum<IKTarget>.Length)
				select new IKTargetPair((IKTarget)index, solver)).ToArray();
		}
	}

	public ChaControl info { get; private set; }

	public FullBodyBipedIK ik { get; private set; }

	public MotionIK[] partners { get; private set; }

	public IKTargetPair[] ikTargetPairs
	{
		get
		{
			return (!(ik == null)) ? IKTargetPair.GetPairs(ik.solver) : null;
		}
	}

	public MotionIKData data { get; private set; }

	public FrameCorrect frameCorrect { get; private set; }

	public IKCorrect ikCorrect { get; private set; }

	public bool enabled
	{
		get
		{
			return ik != null && ik.enabled;
		}
		set
		{
			if (!(ik == null))
			{
				ik.enabled = value;
				ikCorrect.isEnabled = value;
			}
		}
	}

	public string[] stateNames
	{
		get
		{
			return (data.states != null) ? data.states.Select((MotionIKData.State p) => p.name).ToArray() : new string[0];
		}
	}

	public MotionIK(ChaControl info, MotionIKData data = null)
	{
		this.info = info;
		this.data = data ?? new MotionIKData();
		Animator animBody = info.animBody;
		ik = animBody.GetComponent<FullBodyBipedIK>();
		if (ik != null)
		{
			frameCorrect = animBody.GetComponent<FrameCorrect>();
			ikCorrect = animBody.GetComponent<IKCorrect>();
		}
		SetPartners();
		Reset();
	}

	public static List<MotionIK> Setup(List<ChaControl> infos)
	{
		List<MotionIK> ret = (from i in Enumerable.Range(0, infos.Count)
			select new MotionIK(infos[i])).ToList();
		ret.ForEach(delegate(MotionIK p)
		{
			p.SetPartners(ret);
		});
		return ret;
	}

	public static Vector3 GetShapeLerpPositionValue(float shape, Vector3 min, Vector3 max)
	{
		return (!(shape >= 0.5f)) ? Vector3.Lerp(min, Vector3.zero, Mathf.InverseLerp(0f, 0.5f, shape)) : Vector3.Lerp(Vector3.zero, max, Mathf.InverseLerp(0.5f, 1f, shape));
	}

	public static Vector3 GetShapeLerpAngleValue(float shape, Vector3 min, Vector3 max)
	{
		Vector3 zero = Vector3.zero;
		if (shape >= 0.5f)
		{
			float t = Mathf.InverseLerp(0.5f, 1f, shape);
			for (int i = 0; i < 3; i++)
			{
				zero[i] = Mathf.LerpAngle(0f, max[i], t);
			}
		}
		else
		{
			float t2 = Mathf.InverseLerp(0f, 0.5f, shape);
			for (int j = 0; j < 3; j++)
			{
				zero[j] = Mathf.LerpAngle(min[j], 0f, t2);
			}
		}
		return zero;
	}

	public void SetPartners(params MotionIK[] partners)
	{
		this.partners = new MotionIK[1] { this }.Concat(partners.Where((MotionIK p) => p != this)).ToArray();
	}

	public void SetPartners(IEnumerable<MotionIK> partners)
	{
		SetPartners(partners.ToArray());
	}

	public void Reset()
	{
		InitFrameCalc();
		enabled = false;
	}

	public void Release()
	{
		data.Release();
	}

	public bool LoadData(TextAsset ta)
	{
		return data.Read(ta);
	}

	public bool LoadData(string path)
	{
		return data.Read(path);
	}

	public void InitFrameCalc()
	{
		if (frameCorrect != null)
		{
			foreach (BaseCorrect.Info item in frameCorrect.list)
			{
				item.enabled = false;
				item.pos = Vector3.zero;
				item.ang = Vector3.zero;
			}
		}
		if (!(ikCorrect != null))
		{
			return;
		}
		foreach (BaseCorrect.Info item2 in ikCorrect.list)
		{
			item2.enabled = false;
			item2.pos = Vector3.zero;
			item2.ang = Vector3.zero;
			item2.bone = null;
		}
	}

	public MotionIKData.State InitState(string stateName)
	{
		return data.InitState(stateName);
	}

	public MotionIKData.State GetNowState(string stateName)
	{
		if (data.states == null)
		{
			return null;
		}
		int num = data.states.Check((MotionIKData.State v) => v.name == stateName);
		if (num == -1)
		{
			return null;
		}
		return data.states[num];
	}

	public MotionIKData.Frame[] GetNowFrames(string stateName)
	{
		MotionIKData.State nowState = GetNowState(stateName);
		return (nowState != null) ? nowState.frames : null;
	}

	public void Calc(string stateName)
	{
		if (frameCorrect == null)
		{
			return;
		}
		InitFrameCalc();
		MotionIKData.State nowState = GetNowState(stateName);
		if (nowState != null)
		{
			int iKTargetLength = IKTargetPair.IKTargetLength;
			MotionIKData.Frame[] frames = nowState.frames;
			foreach (MotionIKData.Frame frame in frames)
			{
				int num = frame.frameNo - iKTargetLength;
				if (num >= 0)
				{
					BaseCorrect.Info info = frameCorrect.list[num];
					info.enabled = true;
					Vector3[] correctShapeValues = GetCorrectShapeValues(partners[frame.editNo].info, frame.shapes);
					info.pos = correctShapeValues[0];
					info.ang = correctShapeValues[1];
				}
			}
		}
		enabled = nowState != null;
		foreach (var item in ikTargetPairs.Select((IKTargetPair target, int index) => new { target, index }))
		{
			LinkIK(item.index, nowState, item.target);
		}
	}

	private Vector3[] GetCorrectShapeValues(ChaControl chara, MotionIKData.Shape[] shapes)
	{
		Vector3[] array = new Vector3[2]
		{
			Vector3.zero,
			Vector3.zero
		};
		foreach (MotionIKData.Shape shape in shapes)
		{
			float shapeBodyValue = chara.GetShapeBodyValue(shape.shapeNo);
			for (int j = 0; j < array.Length; j++)
			{
				if (j == 0)
				{
					array[j] += GetShapeLerpPositionValue(shapeBodyValue, shape.small[j], shape.large[j]);
				}
				else
				{
					array[j] += GetShapeLerpAngleValue(shapeBodyValue, shape.small[j], shape.large[j]);
				}
			}
		}
		return array;
	}

	private void LinkIK(int index, MotionIKData.State state, IKTargetPair pair)
	{
		Func<int, string, Transform> getTarget = (int sex, string frameName) => (!frameName.IsNullOrEmpty()) ? partners[sex].info.GetComponentsInChildren<Transform>().FirstOrDefault((Transform p) => p.name == frameName) : null;
		MotionIKData.Parts parts = ((state != null) ? state[index] : null);
		Func<int, MotionIKData.Frame> FindFrame = (int no) => (state != null) ? state.frames.FirstOrDefault((MotionIKData.Frame p) => p.frameNo == no) : null;
		Transform boneTarget = null;
		MotionIKData.Param2 param2 = ((parts != null) ? parts.param2 : null);
		IKEffector effector = pair.effector;
		effector.target.GetComponent<BaseData>().SafeProc(delegate(BaseData data)
		{
			data.bone = ((param2 != null) ? getTarget(param2.sex, param2.target) : null);
			boneTarget = data.bone;
			data.GetComponent<BaseProcess>().SafeProc(delegate(BaseProcess item)
			{
				item.enabled = data.bone != null;
			});
			MotionIKData.Frame frame = FindFrame(index * 2);
			if (frame == null)
			{
				data.pos = Vector3.zero;
				data.rot = Quaternion.identity;
			}
			else
			{
				Vector3[] correctShapeValues = GetCorrectShapeValues(partners[frame.editNo].info, frame.shapes);
				data.pos = correctShapeValues[0];
				data.rot = Quaternion.Euler(correctShapeValues[1]);
			}
		});
		if (boneTarget == null || param2 == null)
		{
			effector.positionWeight = 0f;
			effector.rotationWeight = 0f;
		}
		else
		{
			effector.positionWeight = param2.weightPos;
			effector.rotationWeight = param2.weightAng;
		}
		boneTarget = null;
		MotionIKData.Param3 param3 = ((parts != null) ? parts.param3 : null);
		IKConstraintBend bend = pair.bend;
		bend.bendGoal.GetComponent<BaseData>().SafeProc(delegate(BaseData data)
		{
			data.bone = ((param3 != null) ? getTarget(0, param3.chein) : null);
			boneTarget = data.bone;
			data.GetComponent<BaseProcess>().SafeProc(delegate(BaseProcess item)
			{
				item.enabled = data.bone != null;
			});
			MotionIKData.Frame frame2 = FindFrame(index * 2 + 1);
			if (frame2 == null)
			{
				data.pos = Vector3.zero;
				data.rot = Quaternion.identity;
			}
			else
			{
				Vector3[] correctShapeValues2 = GetCorrectShapeValues(partners[frame2.editNo].info, frame2.shapes);
				data.pos = correctShapeValues2[0];
				data.rot = Quaternion.Euler(correctShapeValues2[1]);
			}
		});
		if (boneTarget == null || param3 == null)
		{
			bend.weight = 0f;
		}
		else
		{
			bend.weight = param3.weight;
		}
	}
}
