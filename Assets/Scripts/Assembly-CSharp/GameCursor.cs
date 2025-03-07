using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameCursor : Singleton<GameCursor>
{
	private struct RECT
	{
		public int left;

		public int top;

		public int right;

		public int bottom;
	}

	public struct POINT
	{
		public int X;

		public int Y;

		public POINT(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	public Texture2D[] iconDefalutTextures = new Texture2D[3];

	public static Vector3 pos = Vector3.zero;

	public static float speed = 2000f;

	public static bool isLock = false;

	private POINT lockPos = default(POINT);

	private bool GUICheckFlag;

	private int sizeWindow;

	private int nowCursorKind;

	private const int MOUSEEVENTF_LEFTDOWN = 2;

	private const int MOUSEEVENTF_LEFTUP = 4;

	private IntPtr windowPtr = GetForegroundWindow();

	private RECT winRect = default(RECT);

	public static bool isDraw
	{
		get
		{
			return Cursor.visible;
		}
		set
		{
			Cursor.visible = value;
		}
	}

	private void Start()
	{
		pos = Input.mousePosition;
		GetCursorPos(out lockPos);
		windowPtr = GetForegroundWindow();
		SetCursorTexture(-1, null, true);
	}

	private void Update()
	{
		if (isLock)
		{
			SetCursorPos(lockPos.X, lockPos.Y);
			return;
		}
		pos = Input.mousePosition;
		GetCursorPos(out lockPos);
	}

	[DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
	private static extern void SetCursorPos(int X, int Y);

	[DllImport("USER32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetCursorPos(out POINT lpPoint);

	[DllImport("USER32.dll")]
	private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

	[DllImport("USER32.dll")]
	private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

	[DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
	private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

	[DllImport("user32.dll")]
	private static extern int MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, int bRepaint);

	[DllImport("user32.dll")]
	private static extern int GetWindowRect(IntPtr hwnd, ref RECT lpRect);

	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll")]
	private static extern IntPtr FindWindow(string className, string windowName);

	[DllImport("user32.dll")]
	public static extern bool SetWindowText(IntPtr hwnd, string lpString);

	public void SetCursorTexture(int _kind, Texture2D[] _texs = null, bool _forceChange = false)
	{
		if (nowCursorKind == _kind && !_forceChange)
		{
			return;
		}
		switch (_kind)
		{
		case -1:
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			break;
		case -2:
			Cursor.SetCursor(iconDefalutTextures[sizeWindow], Vector2.zero, CursorMode.ForceSoftware);
			break;
		default:
			if (_texs == null)
			{
				return;
			}
			Cursor.SetCursor(_texs[sizeWindow], new Vector2((float)_texs[sizeWindow].width * 0.5f, (float)_texs[sizeWindow].height * 0.5f), CursorMode.ForceSoftware);
			break;
		}
		nowCursorKind = _kind;
	}

	public void SetCoursorPosition(Vector3 mousePos)
	{
		POINT lpPoint = new POINT(0, 0);
		ClientToScreen(windowPtr, ref lpPoint);
		GetWindowRect(windowPtr, ref winRect);
		POINT pOINT = new POINT(lpPoint.X - winRect.left, lpPoint.Y - winRect.top);
		lpPoint.X = (int)mousePos.x;
		lpPoint.Y = Screen.height - (int)mousePos.y;
		ClientToScreen(windowPtr, ref lpPoint);
		SetCursorPos(lpPoint.X + pOINT.X, lpPoint.Y + pOINT.Y);
	}

	public void SetCursorLock(bool setLockFlag)
	{
		if (setLockFlag)
		{
			if (!isLock)
			{
				GetCursorPos(out lockPos);
				isLock = true;
				Cursor.visible = false;
			}
		}
		else if (isLock)
		{
			SetCursorPos(lockPos.X, lockPos.Y);
			isLock = false;
			Cursor.visible = true;
		}
	}

	public void UnLockCursor()
	{
		if (isLock)
		{
			isLock = false;
			Cursor.visible = true;
		}
	}

	public void UpdateCursorLock()
	{
		if (isLock)
		{
			SetCursorPos(lockPos.X, lockPos.Y);
		}
	}
}
