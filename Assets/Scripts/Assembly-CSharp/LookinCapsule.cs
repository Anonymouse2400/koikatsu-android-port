using System.Text;
using UnityEngine;

public class LookinCapsule : CollisionCamera
{
	public float scaleRate = 5f;

	private GameObject lookCap;

	private new void Start()
	{
		base.Start();
		lookCap = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		lookCap.GetComponent<CapsuleCollider>().isTrigger = true;
		lookCap.GetComponent<Renderer>().enabled = false;
		lookCap.transform.position = (camCtrl.TargetPos + camCtrl.transform.position) * 0.5f;
		lookCap.transform.parent = camCtrl.transform;
		Vector3 cameraAngle = camCtrl.CameraAngle;
		cameraAngle.x += 90f;
		lookCap.transform.rotation = Quaternion.Euler(cameraAngle);
		cameraAngle = lookCap.transform.localScale;
		cameraAngle.y = (camCtrl.TargetPos - camCtrl.transform.position).magnitude;
		lookCap.transform.localScale = cameraAngle;
		lookCap.AddComponent<LookHit>();
		Rigidbody rigidbody = lookCap.AddComponent<Rigidbody>();
		rigidbody.useGravity = false;
	}

	private void Update()
	{
		lookCap.transform.position = (camCtrl.TargetPos + camCtrl.transform.position) * 0.5f;
		Vector3 localScale = default(Vector3);
		localScale.y = Vector3.Distance(camCtrl.TargetPos, camCtrl.transform.position) * 0.5f;
		localScale.x = (localScale.z = scaleRate);
		lookCap.transform.localScale = localScale;
	}

	private void OnGUI()
	{
		StringBuilder stringBuilder = new StringBuilder();
		float height = 1000f;
		if (objDels != null)
		{
			GameObject[] array = objDels;
			foreach (GameObject gameObject in array)
			{
				if (!gameObject.GetComponent<Renderer>().enabled)
				{
					stringBuilder.Append(gameObject.name);
					stringBuilder.Append("\n");
				}
			}
		}
		GUI.Box(new Rect(5f, 5f, 300f, height), string.Empty);
		GUI.Label(new Rect(10f, 5f, 1000f, height), stringBuilder.ToString());
	}
}
