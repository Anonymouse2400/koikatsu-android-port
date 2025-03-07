using UnityEngine;

public class MetaballJoint : MonoBehaviour
{
	public Transform transformJoint;

	public float limitLength;

	[SerializeField]
	[Tooltip("確認用表示")]
	private float length;

	private void Start()
	{
		if ((bool)transformJoint)
		{
			length = Vector3.Distance(transformJoint.position, base.transform.position);
		}
	}

	private void Update()
	{
		Vector3 vector = transformJoint.position - base.transform.position;
		float magnitude = vector.magnitude;
		float num = magnitude - length;
		if (num > 0f)
		{
			base.transform.Translate(vector * ((num - limitLength) / magnitude), Space.World);
		}
		else if (num < 0f)
		{
			base.transform.Translate(vector * ((num + limitLength) / magnitude), Space.World);
		}
	}
}
