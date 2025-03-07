using Manager;
using UnityEngine;

namespace ActionGame
{
	public static class ActionInput
	{
		public static bool isAction
		{
			get
			{
				return Input.GetMouseButtonDown(1);
			}
		}

		public static bool isCursorLock
		{
			get
			{
				return Input.GetMouseButtonDown(2);
			}
		}

		public static bool isWalk
		{
			get
			{
				return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			}
		}

		public static bool isViewChange
		{
			get
			{
				return Input.GetKeyDown(KeyCode.Space);
			}
		}

		public static bool isViewTurn
		{
			get
			{
				if (Manager.Config.ActData.CrouchCtrlKey)
				{
					return Input.GetKeyDown(KeyCode.Z);
				}
				return Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
			}
		}

		public static bool isCrouch
		{
			get
			{
				if (Manager.Config.ActData.CrouchCtrlKey)
				{
					return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
				}
				return Input.GetKey(KeyCode.Z);
			}
		}

		public static bool isViewPlayer
		{
			get
			{
				return Input.GetKeyDown(KeyCode.R);
			}
		}

		public static void PlayerMove(ref bool isMove)
		{
			if (!Manager.Config.ActData.MoveLook)
			{
				isMove = Input.GetMouseButton(0);
			}
			else if (Input.GetMouseButtonDown(0))
			{
				isMove = !isMove;
			}
		}
	}
}
