using UnityEngine;

namespace ADV
{
	public static class KeyInput
	{
		public struct Data
		{
			public bool isMouse;

			public bool isKey;

			public bool isCheck
			{
				get
				{
					return isMouse | isKey;
				}
			}

			public Data(bool isMouse, bool isKey)
			{
				this.isMouse = isMouse;
				this.isKey = isKey;
			}
		}

		private static float MouseWheel
		{
			get
			{
				return Input.GetAxis("Mouse ScrollWheel");
			}
		}

		public static bool SkipButton
		{
			get
			{
				return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
			}
		}

		public static bool BackLogTextPageNext
		{
			get
			{
				return Input.GetKeyDown(KeyCode.PageUp);
			}
		}

		public static bool BackLogTextPageBack
		{
			get
			{
				return Input.GetKeyDown(KeyCode.PageDown);
			}
		}

		public static bool BackLogTextFirst
		{
			get
			{
				return Input.GetKeyDown(KeyCode.Home);
			}
		}

		public static bool BackLogTextLast
		{
			get
			{
				return Input.GetKeyDown(KeyCode.End);
			}
		}

		public static Data TextNext(bool isOnWindow, bool isKeyCondition)
		{
			bool isMouse = false;
			if (isOnWindow)
			{
				isMouse = Input.GetMouseButtonDown(0) || MouseWheel < 0f;
			}
			bool isKey = false;
			if (isKeyCondition)
			{
				isKey = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.DownArrow);
			}
			return new Data(isMouse, isKey);
		}

		public static Data WindowNoneButton(bool isOnWindow, bool isKeyCondition)
		{
			bool isMouse = false;
			if (isOnWindow)
			{
			}
			bool isKey = false;
			if (isKeyCondition)
			{
				isKey = Input.GetKeyDown(KeyCode.Space);
			}
			return new Data(isMouse, isKey);
		}

		public static Data WindowNoneButtonCancel(bool isOnWindow, bool isKeyCondition)
		{
			bool isMouse = false;
			if (isOnWindow)
			{
				isMouse = Input.GetMouseButtonDown(1);
			}
			bool isKey = false;
			if (isKeyCondition)
			{
				isKey = Input.GetKeyDown(KeyCode.Space);
			}
			return new Data(isMouse, isKey);
		}

		public static Data BackLogButton(bool isOnWindow, bool isKeyCondition)
		{
			bool isMouse = false;
			if (isOnWindow)
			{
				isMouse = MouseWheel > 0f;
			}
			bool isKey = false;
			if (isKeyCondition)
			{
				isKey = Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.UpArrow);
			}
			return new Data(isMouse, isKey);
		}

		public static Data BackLogButtonCancel(bool isOnWindow, bool isKeyCondition)
		{
			bool isMouse = false;
			if (isOnWindow)
			{
				isMouse = Input.GetMouseButtonDown(1);
			}
			bool isKey = false;
			if (isKeyCondition)
			{
				isKey = Input.GetKeyDown(KeyCode.Space);
			}
			return new Data(isMouse, isKey);
		}

		public static Data BackLogTextNext(bool isOnWindow, bool isKeyCondition)
		{
			bool isMouse = false;
			if (isOnWindow)
			{
				isMouse = MouseWheel > 0f;
			}
			bool isKey = false;
			if (isKeyCondition)
			{
				isKey = Input.GetKeyDown(KeyCode.UpArrow);
			}
			return new Data(isMouse, isKey);
		}

		public static Data BackLogTextBack(bool isOnWindow, bool isKeyCondition)
		{
			bool isMouse = false;
			if (isOnWindow)
			{
				isMouse = MouseWheel < 0f;
			}
			bool isKey = false;
			if (isKeyCondition)
			{
				isKey = Input.GetKeyDown(KeyCode.DownArrow);
			}
			return new Data(isMouse, isKey);
		}
	}
}
