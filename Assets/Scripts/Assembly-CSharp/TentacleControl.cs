using UnityEngine;

public class TentacleControl : MonoBehaviour
{
	public SkinnedMetaballSeed seed;

	private void Start()
	{
		SetupPhysicsBones();
	}

	private void Update()
	{
	}

	private void SetupPhysicsBones()
	{
		MetaballCellObject componentInChildren = seed.boneRoot.GetComponentInChildren<MetaballCellObject>();
		if (componentInChildren != null)
		{
			SetupPhysicsBonesRecursive(componentInChildren, true);
		}
	}

	private void SetupPhysicsBonesRecursive(MetaballCellObject obj, bool bRoot = false)
	{
		Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
		if (rigidbody == null)
		{
			rigidbody = obj.gameObject.AddComponent<Rigidbody>();
		}
		rigidbody.useGravity = false;
		if (bRoot)
		{
			FixedJoint fixedJoint = obj.GetComponent<FixedJoint>();
			if (fixedJoint == null)
			{
				fixedJoint = obj.gameObject.AddComponent<FixedJoint>();
			}
			fixedJoint.connectedBody = seed.GetComponent<Rigidbody>();
		}
		else
		{
			HingeJoint hingeJoint = obj.GetComponent<HingeJoint>();
			if (hingeJoint == null)
			{
				hingeJoint = obj.gameObject.AddComponent<HingeJoint>();
			}
			hingeJoint.connectedBody = obj.transform.parent.GetComponent<Rigidbody>();
			hingeJoint.useLimits = true;
			hingeJoint.limits = new JointLimits
			{
				max = 30f,
				min = -30f
			};
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			Transform child = obj.transform.GetChild(i);
			MetaballCellObject component = child.GetComponent<MetaballCellObject>();
			if (component != null)
			{
				SetupPhysicsBonesRecursive(component);
			}
		}
	}
}
