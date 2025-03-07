using System.Linq;
using Illusion.Extensions;
using IllusionUtility.GetUtility;
using Localize.Translate;
using UnityEngine;
using UnityEngine.UI;

public class SaveFrameAssist
{
	private Localize.Translate.Manager.FileInfo[] _dirFrameBacks;

	private Localize.Translate.Manager.FileInfo[] _dirFrameFronts;

	public RectTransform rtfSpBack;

	public RectTransform rtfSpFront;

	private GameObject objSaveFrameTop;

	private Localize.Translate.Manager.FileInfo[] dirFrameBacks
	{
		get
		{
			return this.GetCache(ref _dirFrameBacks, () => GetFiles("cardframe/Back"));
		}
	}

	private Localize.Translate.Manager.FileInfo[] dirFrameFronts
	{
		get
		{
			return this.GetCache(ref _dirFrameFronts, () => GetFiles("cardframe/Front"));
		}
	}

	private int selBack { get; set; }

	private int selFront { get; set; }

	public Camera backFrameCam { get; private set; }

	public Camera frontFrameCam { get; private set; }

	public bool backFrameDraw { get; private set; }

	public bool frontFrameDraw { get; private set; }

	private static Localize.Translate.Manager.FileInfo[] GetFiles(string path)
	{
		Localize.Translate.Manager.FileInfo[] source = Localize.Translate.Manager.DefaultData.UserDataCommonAssist(path, "*.png");
		IOrderedEnumerable<Localize.Translate.Manager.FileInfo> first = from p in source
			where p.isDefault
			orderby p.info.FileName
			select p;
		IOrderedEnumerable<Localize.Translate.Manager.FileInfo> second = from p in source
			where !p.isDefault
			orderby p.info.FileName
			select p;
		return first.Concat(second).ToArray();
	}

	public bool CreateSaveFrameToHierarchy(Transform parent = null)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("SpriteTop");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.name == "SaveFrame")
			{
				objSaveFrameTop = gameObject;
			}
		}
		if ((bool)objSaveFrameTop)
		{
			return true;
		}
		objSaveFrameTop = new GameObject("SaveFrame");
		if ((bool)parent)
		{
			objSaveFrameTop.transform.parent = parent;
		}
		objSaveFrameTop.tag = "SpriteTop";
		string[] array3 = new string[2] { "BackSpCam", "FrontSpCam" };
		int num = LayerMask.NameToLayer("UI");
		string[] array4 = new string[2]
		{
			string.Empty,
			string.Empty
		};
		int[] array5 = new int[2] { -20, 20 };
		CameraClearFlags[] array6 = new CameraClearFlags[2]
		{
			CameraClearFlags.Depth,
			CameraClearFlags.Depth
		};
		bool[] array7 = new bool[2];
		Camera[] array8 = new Camera[2];
		for (int j = 0; j < array3.Length; j++)
		{
			GameObject gameObject2 = null;
			if (string.Empty != array4[j])
			{
				gameObject2 = GameObject.FindGameObjectWithTag(array4[j]);
			}
			if (null == gameObject2)
			{
				gameObject2 = new GameObject(array3[j]);
			}
			gameObject2.transform.parent = objSaveFrameTop.transform;
			gameObject2.layer = num;
			if (string.Empty != array4[j])
			{
				gameObject2.tag = array4[j];
			}
			array8[j] = gameObject2.GetComponent<Camera>();
			if (null == array8[j])
			{
				array8[j] = gameObject2.AddComponent<Camera>();
			}
			array8[j].cullingMask = 1 << num;
			array8[j].clearFlags = array6[j];
			array8[j].backgroundColor = new Color(0f, 0f, 0f, 1f);
			array8[j].orthographic = true;
			array8[j].orthographicSize = 1f;
			array8[j].nearClipPlane = -10f;
			array8[j].farClipPlane = 10f;
			array8[j].depth = array5[j];
			array8[j].enabled = array7[j];
			array8[j].allowHDR = false;
		}
		backFrameCam = array8[0];
		frontFrameCam = array8[1];
		string[] array9 = new string[2] { "BackSpCanvas", "FrontSpCanvas" };
		string[] array10 = new string[2] { "spback", "spfront" };
		RectTransform[] array11 = new RectTransform[2];
		Vector2[] array12 = new Vector2[2]
		{
			new Vector2(528f, 720f),
			new Vector2(528f, 720f)
		};
		Camera[] array13 = new Camera[2]
		{
			array8[0],
			array8[1]
		};
		for (int k = 0; k < array9.Length; k++)
		{
			GameObject gameObject3 = new GameObject(array9[k]);
			gameObject3.transform.SetParent(objSaveFrameTop.transform, false);
			gameObject3.layer = num;
			Canvas canvas = gameObject3.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.pixelPerfect = false;
			canvas.worldCamera = array13[k];
			canvas.planeDistance = 0f;
			CanvasScaler canvasScaler = gameObject3.AddComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.referenceResolution = new Vector2(1280f, 720f);
			canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			canvasScaler.matchWidthOrHeight = 0f;
			canvasScaler.referencePixelsPerUnit = 100f;
			GameObject gameObject4 = new GameObject(array10[k]);
			gameObject4.AddComponent<CanvasRenderer>();
			gameObject4.transform.SetParent(gameObject3.transform, false);
			gameObject4.layer = num;
			gameObject4.AddComponent<RawImage>();
			RectTransform rectTransform = gameObject4.transform as RectTransform;
			rectTransform.sizeDelta = array12[k];
			array11[k] = rectTransform;
		}
		rtfSpBack = array11[0];
		rtfSpFront = array11[1];
		ChangeSaveFrameBack(2);
		ChangeSaveFrameFront(2);
		ShowSaveFrameBack(false);
		ShowSaveFrameFront(false);
		return true;
	}

	public bool ChangeSaveFrameBack(byte changeNo)
	{
		int num = dirFrameBacks.Length;
		if (num == 0)
		{
			return false;
		}
		int num2 = selBack;
		switch (changeNo)
		{
		case 0:
			num2 = (num2 + 1) % num;
			break;
		case 1:
			num2 = (num2 + num - 1) % num;
			break;
		}
		Texture changeTex = PngAssist.LoadTexture(dirFrameBacks[num2].info.FullPath);
		ChangeSaveFrameBackTexture(changeTex);
		selBack = num2;
		return true;
	}

	public bool ChangeSaveFrameFront(byte changeNo)
	{
		int num = dirFrameFronts.Length;
		if (num == 0)
		{
			return false;
		}
		int num2 = selFront;
		switch (changeNo)
		{
		case 0:
			num2 = (num2 + 1) % num;
			break;
		case 1:
			num2 = (num2 + num - 1) % num;
			break;
		}
		Texture changeTex = PngAssist.LoadTexture(dirFrameFronts[num2].info.FullPath);
		ChangeSaveFrameFrontTexture(changeTex);
		selFront = num2;
		return true;
	}

	public bool ChangeSaveFrameBackTexture(Texture changeTex)
	{
		return ChangeSaveFrameTexture(objSaveFrameTop, "spback", changeTex);
	}

	public bool ChangeSaveFrameFrontTexture(Texture changeTex)
	{
		return ChangeSaveFrameTexture(objSaveFrameTop, "spfront", changeTex);
	}

	public string GetNowPositionStringBack()
	{
		return GetNowPositionString(selBack, dirFrameBacks.Length);
	}

	public string GetNowPositionStringFront()
	{
		return GetNowPositionString(selFront, dirFrameFronts.Length);
	}

	private static string GetNowPositionString(int sel, int fileFig)
	{
		if (fileFig == 0)
		{
			if (Localize.Translate.Manager.isTranslate)
			{
				return "NoData";
			}
			return "ファイルがありません";
		}
		return string.Format("{0:000} / {1:000}", sel + 1, fileFig);
	}

	private static bool ChangeSaveFrameTexture(GameObject objSaveFrameTop, string searchName, Texture changeTex)
	{
		if (objSaveFrameTop == null)
		{
			return false;
		}
		GameObject gameObject = objSaveFrameTop.transform.FindLoop(searchName);
		if (gameObject == null)
		{
			return false;
		}
		RawImage orAddComponent = gameObject.GetOrAddComponent<RawImage>();
		if (orAddComponent.texture != null)
		{
			Object.Destroy(orAddComponent.texture);
		}
		orAddComponent.texture = changeTex;
		return true;
	}

	public bool SetActiveSaveFrameTop(bool active)
	{
		if (null == objSaveFrameTop)
		{
			return false;
		}
		objSaveFrameTop.SetActiveIfDifferent(active);
		return true;
	}

	public bool ShowSaveFrameBack(bool visible = true)
	{
		if (null == objSaveFrameTop)
		{
			return false;
		}
		GameObject gameObject = objSaveFrameTop.transform.FindLoop("spback");
		if (null == gameObject)
		{
			return false;
		}
		gameObject.SetActiveIfDifferent(visible);
		backFrameCam.enabled = visible;
		backFrameDraw = visible;
		return true;
	}

	public bool ShowSaveFrameFront(bool visible = true)
	{
		if (null == objSaveFrameTop)
		{
			return false;
		}
		GameObject gameObject = objSaveFrameTop.transform.FindLoop("spfront");
		if (null == gameObject)
		{
			return false;
		}
		gameObject.SetActiveIfDifferent(visible);
		frontFrameCam.enabled = visible;
		frontFrameDraw = visible;
		return true;
	}
}
