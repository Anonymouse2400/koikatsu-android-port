using System;
using System.Linq;
using UnityEngine;

public static class GUIDrawer
{
	public class Window
	{
		public enum Type
		{
			None = 0,
			Normal = 1,
			Layout = 2,
			Custom = 3
		}

		public string title;

		public Rect rect;

		public Action<int> DoWindow;

		public Type type = Type.Layout;

		public Action HideEvent;

		private bool _isHide;

		public Action CloseEvent;

		private bool _isClose;

		private static Vector2 buttonSize = new Vector2(20f, 20f);

		private string defaultTitle;

		private Rect? backupRect;

		public bool isHide
		{
			get
			{
				return _isHide;
			}
			set
			{
				_isHide = value;
				if (_isHide)
				{
					HideEvent.Call();
				}
			}
		}

		public bool isClose
		{
			get
			{
				return _isClose;
			}
			set
			{
				_isClose = value;
				if (_isClose)
				{
					CloseEvent.Call();
					title = defaultTitle;
				}
			}
		}

		public Window()
		{
			type = Type.None;
			defaultTitle = (title = string.Empty);
		}

		public Window(string title)
		{
			this.title = title ?? string.Empty;
			defaultTitle = this.title;
			rect = new Rect(0f, 0f, 300f, 0f);
		}

		public Window(string title, Rect rect)
		{
			this.title = title ?? string.Empty;
			defaultTitle = this.title;
			this.rect = rect;
		}

		public void Open()
		{
			isClose = false;
		}

		public void Close()
		{
			isClose = true;
		}

		public void Hide()
		{
			isHide = true;
		}

		public void View()
		{
			isHide = false;
		}

		public void Draw(int windowID)
		{
			if (!backupRect.HasValue)
			{
				backupRect = rect;
			}
			if (isClose)
			{
				return;
			}
			CloseButton();
			Type type = this.type;
			switch (type)
			{
			case Type.None:
				DoWindow(-1);
				break;
			case Type.Custom:
			{
				Rect screenRect = rect;
				using (new GUILayout.AreaScope(screenRect))
				{
					GUIContent content = new GUIContent(title);
					using (new GUILayout.VerticalScope("box"))
					{
						if (GUILayout.RepeatButton(content, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
						{
							float y = new GUIStyle().CalcSize(content).y;
							rect.x = Input.mousePosition.x - screenRect.width * 0.5f;
							rect.y = (float)Screen.height - Input.mousePosition.y - y * 0.5f;
						}
						DoWindow(-1);
						break;
					}
				}
			}
			case Type.Normal:
			case Type.Layout:
			{
				Action<int> doWindow = delegate(int unusedWindowID)
				{
					using (new GUILayout.VerticalScope())
					{
						if (isHide)
						{
							GUILayout.Label(string.Empty, GUILayout.Height(rect.height));
						}
						DoWindow(unusedWindowID);
					}
					GUI.DragWindow();
				};
				if (this.type == Type.Normal || isHide)
				{
					if (isHide)
					{
						rect.height = 50f;
					}
					rect = GUI.Window(windowID, rect, delegate(int unusedWindowID)
					{
						doWindow(unusedWindowID);
					}, title);
				}
				else
				{
					rect = GUILayout.Window(windowID, rect, delegate(int unusedWindowID)
					{
						doWindow(unusedWindowID);
					}, title);
				}
				HideButton();
				break;
			}
			}
		}

		private void HideButton()
		{
			Rect position = rect;
			float x = buttonSize.x;
			float y = buttonSize.y;
			position.x = position.x + position.width - x * 2f;
			position.y -= y;
			position.width = x;
			position.height = y;
			if (GUI.Button(position, (!isHide) ? "-" : "□"))
			{
				isHide = !isHide;
				if (!isHide)
				{
					rect.height = backupRect.Value.height;
				}
			}
		}

		private void CloseButton()
		{
			Rect position = rect;
			float x = buttonSize.x;
			float y = buttonSize.y;
			position.x = position.x + position.width - x;
			position.y -= y;
			position.width = x;
			position.height = y;
			if (GUI.Button(position, "X"))
			{
				isClose = true;
			}
		}
	}

	public static void Draw(float x, float y, string str, float baseW = 7f, float baseH = 15f, bool isLeftUp = true)
	{
		Draw(ref x, ref y, str, baseW, baseH);
	}

	public static void Draw(ref float x, float y, string str, float baseW = 7f, float baseH = 15f, bool isLeftUp = true)
	{
		Draw(ref x, ref y, str, baseW, baseH);
	}

	public static void Draw(float x, ref float y, string str, float baseW = 7f, float baseH = 15f, bool isLeftUp = true)
	{
		Draw(ref x, ref y, str, baseW, baseH);
	}

	public static void Draw(ref float x, ref float y, string str, float baseW = 7f, float baseH = 15f, bool isLeftUp = true)
	{
		if (!(str == string.Empty))
		{
			string[] array = str.Replace(Environment.NewLine, "\n").Split('\n');
			float num = (float)array.Max((string max) => max.Length) * baseW;
			float num2 = (float)array.Length * baseH;
			GUI.Box(new Rect(x, y, num + 10f, num2 + 5f), string.Empty);
			GUI.Label(new Rect(x + 5f, y, Screen.width, Screen.height), str);
			x += num;
			if (isLeftUp)
			{
				y += num2 + baseH;
			}
			else
			{
				y -= num2 + baseH;
			}
		}
	}

	public static float GetWidth(string str, float baseW = 7f)
	{
		return (!(str == string.Empty)) ? ((float)str.Replace(Environment.NewLine, "\n").Split('\n').Max((string max) => max.Length) * baseW) : 0f;
	}

	public static float GetHeight(string str, float baseH = 15f)
	{
		return (!(str == string.Empty)) ? ((float)str.Replace(Environment.NewLine, "\n").Split('\n').Length * baseH + baseH) : 0f;
	}
}
