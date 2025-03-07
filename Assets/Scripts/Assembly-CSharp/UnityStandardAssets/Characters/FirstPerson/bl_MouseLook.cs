using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson
{
	[Serializable]
	public class bl_MouseLook
	{
		public float XSensitivity = 2f;

		public float YSensitivity = 2f;

		public bool clampVerticalRotation = true;

		public float MinimumX = -90f;

		public float MaximumX = 90f;

		public bool smooth;

		public float smoothTime = 5f;

		private Quaternion m_CharacterTargetRot;

		private Quaternion m_CameraTargetRot;

		public void Init(Transform character, Transform camera)
		{
			m_CharacterTargetRot = character.localRotation;
			m_CameraTargetRot = camera.localRotation;
		}

		public void LookRotation(Transform character, Transform camera)
		{
			float y = Input.GetAxis("Mouse X") * XSensitivity;
			float num = Input.GetAxis("Mouse Y") * YSensitivity;
			m_CharacterTargetRot *= Quaternion.Euler(0f, y, 0f);
			m_CameraTargetRot *= Quaternion.Euler(0f - num, 0f, 0f);
			if (clampVerticalRotation)
			{
				m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
			}
			if (smooth)
			{
				character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot, smoothTime * Time.deltaTime);
				camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot, smoothTime * Time.deltaTime);
			}
			else
			{
				character.localRotation = m_CharacterTargetRot;
				camera.localRotation = m_CameraTargetRot;
			}
		}

		private Quaternion ClampRotationAroundXAxis(Quaternion q)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1f;
			float value = 114.59156f * Mathf.Atan(q.x);
			value = Mathf.Clamp(value, MinimumX, MaximumX);
			q.x = Mathf.Tan((float)Math.PI / 360f * value);
			return q;
		}
	}
}
