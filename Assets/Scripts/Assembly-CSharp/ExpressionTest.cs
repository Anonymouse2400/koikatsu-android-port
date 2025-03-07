using System.Collections.Generic;
using CustomUtility;
using Illusion.CustomAttributes;
using IllusionUtility.GetUtility;
using Manager;
using RootMotion.FinalIK;
using UniRx;
using UnityEngine;

public class ExpressionTest : BaseLoader
{
	[Button("ResetPosition", "リセット", new object[] { })]
	public int resetPosition;

	[SerializeField]
	private CustomGuideObject ikEffectorHandL;

	[SerializeField]
	private CustomGuideObject ikEffectorHandR;

	[SerializeField]
	private CustomGuideObject ikGoalArmL;

	[SerializeField]
	private CustomGuideObject ikGoalArmR;

	[SerializeField]
	private CustomGuideObject ikEffectorFootL;

	[SerializeField]
	private CustomGuideObject ikEffectorFootR;

	[SerializeField]
	private CustomGuideObject ikGoalLegL;

	[SerializeField]
	private CustomGuideObject ikGoalLegR;

	[SerializeField]
	private Animator anim;

	[SerializeField]
	[Range(0f, 1f)]
	private float animationPos;

	[SerializeField]
	private bool wireframe;

	[SerializeField]
	private bool enableIK = true;

	private FullBodyBipedIK ik;

	private ChaControl female;

	private Transform[] trfBone;

	private CustomGuideObject[] cgo;

	[SerializeField]
	private Transform trfArrow;

	public BoolReactiveProperty enableMark = new BoolReactiveProperty(true);

	public ConvertRotation.RotationOrder calctype;

	private List<GameObject> lstCheckObj = new List<GameObject>();

	private void ResetPosition()
	{
		if (cgo != null && trfBone != null)
		{
			for (int i = 0; i < cgo.Length; i++)
			{
				cgo[i].amount.position = trfBone[i].position;
			}
		}
	}

	public void Start()
	{
		Singleton<Character>.Instance.BeginLoadAssetBundle();
		female = Singleton<Character>.Instance.CreateFemale(null, 0);
		female.Load();
		string[] array = new string[8] { "cf_j_hand_L", "cf_j_hand_R", "cf_j_forearm01_L", "cf_j_forearm01_R", "cf_j_leg03_L", "cf_j_leg03_R", "cf_j_leg01_L", "cf_j_leg01_R" };
		trfBone = new Transform[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = female.objBodyBone.transform.FindLoop(array[i]);
			trfBone[i] = ((!gameObject) ? null : gameObject.transform);
		}
		cgo = new CustomGuideObject[array.Length];
		cgo[0] = ikEffectorHandL;
		cgo[1] = ikEffectorHandR;
		cgo[2] = ikGoalArmL;
		cgo[3] = ikGoalArmR;
		cgo[4] = ikEffectorFootL;
		cgo[5] = ikEffectorFootR;
		cgo[6] = ikGoalLegL;
		cgo[7] = ikGoalLegR;
		female.UpdateBustSoftnessAndGravity();
		female.ChangeEyesPtn(0);
		female.ChangeLookEyesPtn(1);
		female.ChangeEyesOpenMax(1f);
		female.ChangeMouthPtn(1);
		female.ChangeLookNeckPtn(3);
		female.ChangeMouthOpenMax(0f);
		female.hideMoz = true;
		female.SetClothesStateAll(3);
		if (null != anim)
		{
			female.animBody.runtimeAnimatorController = anim.runtimeAnimatorController;
		}
		ResetPosition();
		ik = female.animBody.GetComponent<FullBodyBipedIK>();
		ik.enabled = enableIK;
		ik.solver.leftHandEffector.target = ikEffectorHandL.transform;
		ik.solver.leftHandEffector.positionWeight = 1f;
		ik.solver.leftArmChain.bendConstraint.bendGoal = ikGoalArmL.transform;
		ik.solver.leftArmChain.bendConstraint.weight = 1f;
		ik.solver.rightHandEffector.target = ikEffectorHandR.transform;
		ik.solver.rightHandEffector.positionWeight = 1f;
		ik.solver.rightArmChain.bendConstraint.bendGoal = ikGoalArmR.transform;
		ik.solver.rightArmChain.bendConstraint.weight = 1f;
		ik.solver.leftFootEffector.target = ikEffectorFootL.transform;
		ik.solver.leftFootEffector.positionWeight = 1f;
		ik.solver.leftLegChain.bendConstraint.bendGoal = ikGoalLegL.transform;
		ik.solver.leftLegChain.bendConstraint.weight = 1f;
		ik.solver.rightFootEffector.target = ikEffectorFootR.transform;
		ik.solver.rightFootEffector.positionWeight = 1f;
		ik.solver.rightLegChain.bendConstraint.bendGoal = ikGoalLegR.transform;
		ik.solver.rightLegChain.bendConstraint.weight = 1f;
		female.InitializeExpression();
		lstCheckObj.Clear();
		foreach (KeyValuePair<int, Transform> item in female.expression.dictBone)
		{
			if (!(null == item.Value))
			{
				GameObject gameObject2 = Object.Instantiate(trfArrow.gameObject, item.Value, false);
				gameObject2.name = "mark_" + item.Value.name;
				gameObject2.SetActive(true);
				lstCheckObj.Add(gameObject2);
			}
		}
		Singleton<Character>.Instance.EndLoadAssetBundle();
		enableMark.Subscribe(delegate(bool flag)
		{
			if (lstCheckObj != null)
			{
				lstCheckObj.ForEach(delegate(GameObject x)
				{
					x.SetActive(flag);
				});
			}
		});
	}

	public void Update()
	{
		if ((bool)female.animBody && (bool)female.animBody.runtimeAnimatorController)
		{
			female.animBody.Play("Take 001", 0, animationPos);
		}
		if ((bool)Camera.main)
		{
			WireFrameRender component = Camera.main.GetComponent<WireFrameRender>();
			if ((bool)component)
			{
				component.wireFrameDraw = wireframe;
			}
		}
		if ((bool)ik)
		{
			ik.enabled = enableIK;
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			female.enableExpression = !female.enableExpression;
		}
	}

	public void OnDestroy()
	{
		if (Singleton<Character>.IsInstance())
		{
			Singleton<Character>.Instance.DeleteCharaAll();
			Singleton<Character>.Instance.EndLoadAssetBundle();
		}
	}
}
