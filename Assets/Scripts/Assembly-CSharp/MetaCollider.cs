using System.Collections.Generic;
using UnityEngine;

public class MetaCollider : MonoBehaviour
{
	public enum Hit
	{
		air = 0,
		hit = 1,
		ground = 2,
		stay = 3,
		exit = 4
	}

	[Tooltip("こいつに当たったらLayerの名前を変更したりジョイントを切り離す dragWaitの値になる")]
	public string[] nameJudgeTags;

	[Tooltip("変更するLayerの名")]
	public string nameChangeLayer = string.Empty;

	[Tooltip("こいつに当たったらdragHitの値になる 拘束判定も含む")]
	public string[] nameWaitTags;

	[Tooltip("こいつに当たったらdragHitの値になる\n離れた瞬間dragAirが入る\nウエイトのついてる当たり判定？\nnameJudgeTagsより判定強い")]
	public string nameBodytagName;

	[Tooltip("nameJudgeTagsのColliderに当たった瞬間")]
	public float dragWait = 30f;

	[Tooltip("nameWaitTagsのColliderに当たった瞬間")]
	public float dragHit = 30f;

	[Tooltip("nameWaitTagsのColliderから離れた瞬間")]
	public float dragAir;

	[Tooltip("当たったコライダーと拘束する？")]
	public bool isConstraint = true;

	[Tooltip("当たったコライダーと拘束するまでの時間")]
	public float constTime = 0.5f;

	[Tooltip("強制的にスリープにするまでの時間")]
	public float sleepTime = 10f;

	[Tooltip("後ろのオブジェクトのRigidBody")]
	public Rigidbody nextRigidBody;

	[Tooltip("Air時に重力付けて,それ以外の時は切る")]
	public bool isEndGravity;

	[Tooltip("Air中にこの時間でDragが変化する dragAirの値になる")]
	public float timeDropDown;

	[SerializeField]
	[Tooltip("確認用")]
	private float timeLerpDrag;

	[SerializeField]
	[Tooltip("確認用")]
	private float dragTempAir;

	[SerializeField]
	[Tooltip("確認用 拘束されている先")]
	private Transform parentTransfrom;

	private HashSet<string> judgeTags;

	private HashSet<string> judgeWaitTags;

	[Tooltip("確認用表示 拘束計算するまで")]
	[SerializeField]
	private float stayTime;

	[Tooltip("確認用表示 強制的にスリープにするまで")]
	[SerializeField]
	private float sleepForceTime;

	[Tooltip("確認用表示 拘束計算が終わった？")]
	[SerializeField]
	private bool isConstraintNow;

	private Vector3 posConstLocal = Vector3.zero;

	private Hit objhit;

	[Tooltip("確認用表示")]
	[SerializeField]
	private Hit objnowhit;

	[SerializeField]
	[Tooltip("確認用表示")]
	private Hit nowhit;

	[Tooltip("確認用表示")]
	[SerializeField]
	private bool isNextAddForce;

	private Rigidbody rigid;

	private bool isGroundHit;

	private MetaballJoint metajoint;

	private ConfigurableJoint joint;

	private void Start()
	{
		judgeTags = new HashSet<string>(nameJudgeTags);
		if (judgeTags == null)
		{
			judgeTags = new HashSet<string>();
		}
		judgeWaitTags = new HashSet<string>(nameWaitTags);
		if (judgeWaitTags == null)
		{
			judgeWaitTags = new HashSet<string>();
		}
		rigid = GetComponent<Rigidbody>();
		metajoint = GetComponent<MetaballJoint>();
		joint = GetComponent<ConfigurableJoint>();
	}

	private void FixedUpdate()
	{
		if (!isGroundHit)
		{
			if (objhit == Hit.hit)
			{
				if (objnowhit == Hit.air || objnowhit == Hit.exit)
				{
					objnowhit = Hit.hit;
					rigid.drag = dragHit;
				}
				else if (objnowhit == Hit.hit)
				{
					objnowhit = Hit.stay;
				}
			}
			else if (objnowhit == Hit.stay || objnowhit == Hit.hit)
			{
				objnowhit = Hit.exit;
				dragTempAir = dragHit;
				timeLerpDrag = 0f;
			}
			if ((objnowhit == Hit.air || objnowhit == Hit.exit) && nowhit == Hit.air)
			{
				lerpDrag();
				if (isEndGravity && (bool)rigid && !rigid.useGravity)
				{
					rigid.useGravity = true;
				}
			}
		}
		objhit = Hit.air;
	}

	private void Update()
	{
		if (isConstraintNow && parentTransfrom != null)
		{
			base.transform.position = parentTransfrom.localToWorldMatrix.MultiplyPoint3x4(posConstLocal);
		}
		if ((bool)rigid && !rigid.isKinematic && !rigid.IsSleeping())
		{
			sleepForceTime += Time.deltaTime;
			if (sleepTime <= sleepForceTime)
			{
				rigid.isKinematic = true;
				rigid.Sleep();
			}
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		if (isEndGravity && (bool)rigid && rigid.useGravity)
		{
			rigid.useGravity = false;
		}
		if (judgeTags.Contains(col.gameObject.tag))
		{
			isGroundHit = true;
			base.gameObject.layer = LayerMask.NameToLayer(nameChangeLayer);
			if ((bool)rigid)
			{
				rigid.mass = 1f;
				rigid.useGravity = true;
				rigid.drag = dragWait;
			}
			if ((bool)metajoint)
			{
				Object.Destroy(metajoint);
			}
			if ((bool)joint)
			{
				Object.Destroy(joint);
			}
		}
		if (judgeWaitTags.Contains(col.gameObject.tag))
		{
			nowhit = Hit.hit;
			if ((bool)rigid)
			{
				rigid.drag = dragHit;
			}
			Constraint(col);
		}
		ChangeJoint();
		NextAddForce();
		if (col.gameObject.tag == nameBodytagName)
		{
			objhit = Hit.hit;
		}
	}

	private void OnCollisionStay(Collision col)
	{
		if (judgeWaitTags.Contains(col.gameObject.tag))
		{
			Constraint(col);
		}
	}

	private void OnCollisionExit(Collision col)
	{
		if (judgeWaitTags.Contains(col.gameObject.tag))
		{
			nowhit = Hit.air;
			timeLerpDrag = 0f;
			dragTempAir = dragHit;
		}
	}

	public bool ChangeJoint()
	{
		if (!metajoint || !joint)
		{
			return false;
		}
		if (!metajoint.isActiveAndEnabled)
		{
			return true;
		}
		metajoint.enabled = false;
		joint.xMotion = ConfigurableJointMotion.Limited;
		joint.yMotion = ConfigurableJointMotion.Limited;
		joint.zMotion = ConfigurableJointMotion.Limited;
		return true;
	}

	private bool Constraint(Collision col)
	{
		if (!isConstraint)
		{
			return true;
		}
		if (isConstraintNow)
		{
			return true;
		}
		stayTime += Time.deltaTime;
		if (stayTime < constTime)
		{
			return true;
		}
		if ((bool)rigid)
		{
			rigid.isKinematic = true;
			rigid.Sleep();
		}
		isConstraintNow = true;
		return true;
	}

	private bool NextAddForce()
	{
		if (!nextRigidBody || isNextAddForce)
		{
			return true;
		}
		Vector3 vector = base.transform.position - nextRigidBody.transform.position;
		nextRigidBody.velocity = Vector3.zero;
		nextRigidBody.AddForce(vector.normalized, ForceMode.Impulse);
		isNextAddForce = true;
		return true;
	}

	private bool lerpDrag()
	{
		timeLerpDrag += Time.deltaTime;
		timeLerpDrag = Mathf.Clamp(timeLerpDrag, 0f, timeDropDown);
		float t = Mathf.InverseLerp(0f, timeDropDown, timeLerpDrag);
		if (timeDropDown == 0f)
		{
			t = 1f;
		}
		if ((bool)rigid)
		{
			rigid.drag = Mathf.Lerp(dragTempAir, dragAir, t);
		}
		return true;
	}
}
