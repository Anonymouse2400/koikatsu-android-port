using System;
using SkinnedMetaballBuilder;
using UnityEngine;

public class FPControl : MonoBehaviour
{
	public Camera myCamera;

	public CharacterController cc;

	public float walkSpeed = 3f;

	private float _theta;

	private float _phi;

	public float rotSpeed = 1f;

	private float _mx;

	private float _my;

	private void Start()
	{
		Vector3 forward = myCamera.transform.forward;
		_phi = Mathf.Asin(forward.y);
		_theta = Mathf.Atan2(forward.x, forward.z);
		_mx = Input.mousePosition.x;
		_my = Input.mousePosition.y;
	}

	private void Update()
	{
		float axis = Input.GetAxis("Horizontal");
		float axis2 = Input.GetAxis("Vertical");
		float x = Input.mousePosition.x;
		float y = Input.mousePosition.y;
		float num = x - _mx;
		float num2 = y - _my;
		_mx = x;
		_my = y;
		Vector3 forward = myCamera.transform.forward;
		forward.y = 0f;
		forward.Normalize();
		Vector3 vector = Vector3.Cross(Vector3.up, forward);
		Vector3 vector2 = walkSpeed * (forward * axis2 + vector * axis) + Vector3.down * 3f;
		cc.Move(vector2 * Time.deltaTime);
		_theta -= num * rotSpeed;
		if (_theta > (float)Math.PI)
		{
			_theta -= (float)Math.PI * 2f;
		}
		else if (_theta <= -(float)Math.PI)
		{
			_theta += (float)Math.PI * 2f;
		}
		_phi += num2 * rotSpeed;
		if (_phi > 1f)
		{
			_phi = 1f;
		}
		else if (_phi < -1f)
		{
			_phi = -1f;
		}
		Vector3 vector3 = new Vector3(Mathf.Cos(_phi) * Mathf.Cos(_theta), Mathf.Sin(_phi), Mathf.Cos(_phi) * Mathf.Sin(_theta));
		myCamera.transform.LookAt(myCamera.transform.position + vector3, Vector3.up);
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Shoot();
		}
	}

	public void Shoot()
	{
		Ray ray = myCamera.ScreenPointToRay(new Vector3((float)myCamera.pixelWidth * 0.5f, (float)myCamera.pixelHeight * 0.5f, 0f));
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo))
		{
			Transform t = hitInfo.collider.transform;
			DungeonControl dungeonControl = Utils.FindComponentInParents<DungeonControl>(t);
			if (dungeonControl != null)
			{
				dungeonControl.AddCell(hitInfo.point, 2f);
			}
		}
	}
}
