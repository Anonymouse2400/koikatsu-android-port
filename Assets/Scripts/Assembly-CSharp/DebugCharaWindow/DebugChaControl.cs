using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DebugCharaWindow
{
	public class DebugChaControl : Singleton<DebugChaControl>
	{
		public class DebugChaValue
		{
			public float[] shapeFace = new float[ChaFileDefine.cf_headshapename.Length];

			public float[] shapeBody = new float[ChaFileDefine.cf_bodyshapename.Length];

			public float[] bustEtc = new float[3];

			public bool disableMaskFace;

			public bool[,] disableMaskBody = new bool[2, ChaFileDefine.cf_BustShapeMaskID.Length];

			public void Initialize(ChaControl _chaCtrl)
			{
				UpdateParam(_chaCtrl);
				disableMaskFace = _chaCtrl.chaFile.status.disableMouthShapeMask;
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < ChaFileDefine.cf_BustShapeMaskID.Length; j++)
					{
						disableMaskBody[i, j] = _chaCtrl.chaFile.status.disableBustShapeMask[i, j];
					}
				}
			}

			public void UpdateParam(ChaControl _chaCtrl)
			{
				for (int i = 0; i < shapeFace.Length; i++)
				{
					shapeFace[i] = _chaCtrl.chaFile.custom.face.shapeValueFace[i];
				}
				for (int j = 0; j < shapeBody.Length; j++)
				{
					shapeBody[j] = _chaCtrl.chaFile.custom.body.shapeValueBody[j];
				}
				bustEtc[0] = _chaCtrl.chaFile.custom.body.bustSoftness;
				bustEtc[1] = _chaCtrl.chaFile.custom.body.bustWeight;
				bustEtc[2] = _chaCtrl.chaFile.custom.body.areolaSize;
			}
		}

		[HideInInspector]
		public bool drawOnGUI;

		[HideInInspector]
		public bool enableShape;

		[HideInInspector]
		public int[] selectChara = new int[2];

		[HideInInspector]
		public int selectSex = 1;

		[HideInInspector]
		public Camera[] camView = new Camera[4];

		private Camera camMain;

		[HideInInspector]
		public int viewType = 5;

		[HideInInspector]
		public bool useAnotherDisplay;

		public GameObject objAudio;

		public Dictionary<string, string> dictGuiStr = new Dictionary<string, string>();

		public Dictionary<ChaControl, DebugChaValue> dictChaValue = new Dictionary<ChaControl, DebugChaValue>();

		public GameScreenShot screenShot { get; private set; }

		public GameScreenShotSerial screenShotSerial { get; private set; }

		private void Start()
		{
		}

		private void Update()
		{
			if (null != Camera.main)
			{
				camMain = Camera.main;
			}
			if (enableShape)
			{
				foreach (KeyValuePair<ChaControl, DebugChaValue> item in dictChaValue)
				{
					if (!item.Key.loadEnd)
					{
						continue;
					}
					for (int i = 0; i < item.Value.shapeFace.Length; i++)
					{
						item.Key.SetShapeFaceValue(i, item.Value.shapeFace[i]);
					}
					for (int j = 0; j < item.Value.shapeBody.Length; j++)
					{
						item.Key.SetShapeBodyValue(j, item.Value.shapeBody[j]);
					}
					item.Key.ChangeBustSoftness(item.Value.bustEtc[0]);
					item.Key.ChangeBustGravity(item.Value.bustEtc[1]);
					if (item.Value.bustEtc[2] != item.Key.chaFile.custom.body.areolaSize)
					{
						item.Key.chaFile.custom.body.areolaSize = item.Value.bustEtc[2];
						item.Key.ChangeSettingAreolaSize();
					}
					item.Key.DisableShapeMouth(item.Value.disableMaskFace);
					for (int k = 0; k < 2; k++)
					{
						for (int l = 0; l < ChaFileDefine.cf_BustShapeMaskID.Length; l++)
						{
							item.Key.DisableShapeBodyID(k, l, item.Value.disableMaskBody[k, l]);
						}
					}
					item.Key.updateBustSize = true;
				}
			}
			if ((bool)camView[0] && camView[0].enabled && (bool)camMain)
			{
				camView[0].transform.localPosition = camMain.transform.position;
				camView[0].transform.localRotation = camMain.transform.rotation;
			}
		}

		private void OnGUI()
		{
			if (drawOnGUI)
			{
				string[] array = dictGuiStr.Values.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					GUI.TextField(new Rect(0f, 20 * i, 1000f, 20f), array[i]);
				}
			}
		}

		protected new void Awake()
		{
			if (CheckInstance())
			{
				camMain = Camera.main;
				screenShot = base.gameObject.AddComponent<GameScreenShot>();
				screenShotSerial = base.gameObject.AddComponent<GameScreenShotSerial>();
				CreateCamera();
				UpdateCameraSetting();
				objAudio = new GameObject("objAudio");
				if ((bool)objAudio)
				{
					objAudio.AddComponent<AudioSource>();
				}
			}
		}

		public void UpdateCameraSetting()
		{
			Camera[] array = camView;
			foreach (Camera camera in array)
			{
				camera.targetDisplay = (useAnotherDisplay ? 1 : 0);
			}
			if (null != camMain)
			{
				camMain.enabled = (useAnotherDisplay ? true : false);
			}
			if (MathfEx.RangeEqualOn(0, viewType, 3))
			{
				Camera[] array2 = camView;
				foreach (Camera camera2 in array2)
				{
					camera2.enabled = false;
				}
				if (viewType == 0)
				{
					camView[0].enabled = true;
					camView[0].rect = new Rect(0f, 0f, 1f, 1f);
				}
				else if (viewType == 1)
				{
					camView[1].enabled = true;
					camView[1].rect = new Rect(0f, 0f, 1f, 1f);
				}
				else if (viewType == 2)
				{
					camView[2].enabled = true;
					camView[2].rect = new Rect(0f, 0f, 1f, 1f);
				}
				else if (viewType == 3)
				{
					camView[3].enabled = true;
					camView[3].rect = new Rect(0f, 0f, 1f, 1f);
				}
			}
			else if (viewType == 4)
			{
				Camera[] array3 = camView;
				foreach (Camera camera3 in array3)
				{
					camera3.enabled = true;
				}
				camView[0].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
				camView[1].rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
				camView[2].rect = new Rect(0f, 0f, 0.5f, 0.5f);
				camView[3].rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
			}
			else
			{
				Camera[] array4 = camView;
				foreach (Camera camera4 in array4)
				{
					camera4.enabled = false;
				}
				if (null != camMain)
				{
					camMain.enabled = true;
				}
			}
		}

		public Vector2 GetCameraPosition(int no)
		{
			Vector2 zero = Vector2.zero;
			switch (no)
			{
			case 1:
				zero.x = 0f - camView[no].transform.position.x;
				zero.y = 0f - camView[no].transform.position.z;
				break;
			case 2:
				zero.x = camView[no].transform.position.x;
				zero.y = 0f - camView[no].transform.position.y;
				break;
			case 3:
				zero.x = camView[no].transform.position.z;
				zero.y = 0f - camView[no].transform.position.y;
				break;
			}
			return zero;
		}

		public void SetCameraPosition(int no, Vector2 v)
		{
			switch (no)
			{
			case 0:
				break;
			case 1:
				camView[no].transform.position = new Vector3(0f - v.x, camView[no].transform.position.y, 0f - v.y);
				break;
			case 2:
				camView[no].transform.position = new Vector3(v.x, 0f - v.y, camView[no].transform.position.z);
				break;
			case 3:
				camView[no].transform.position = new Vector3(camView[no].transform.position.x, 0f - v.y, v.x);
				break;
			}
		}

		public float GetCameraSize(int no)
		{
			if (no == 4)
			{
				return 1f;
			}
			return 1f - camView[no].orthographicSize;
		}

		public void SetCameraSize(int no, float size)
		{
			if (no != 4)
			{
				camView[no].orthographicSize = 1f - size;
			}
		}

		public bool GetWireframeRender(int no)
		{
			if (no == 4)
			{
				return false;
			}
			WireFrameRender component = camView[no].GetComponent<WireFrameRender>();
			return component.wireFrameDraw;
		}

		public void SetWireframeRender(int no, bool wire)
		{
			if (no != 4)
			{
				WireFrameRender component = camView[no].GetComponent<WireFrameRender>();
				component.wireFrameDraw = wire;
			}
		}

		private void CreateCamera()
		{
			string[] array = new string[4] { "Persp", "Top", "Front", "Side" };
			int cullingMask = LayerMask.NameToLayer("Chara") | LayerMask.NameToLayer("Map");
			if (null != camMain)
			{
				cullingMask = camMain.cullingMask;
			}
			GameObject gameObject = new GameObject(array[0]);
			gameObject.transform.SetParent(base.transform);
			camView[0] = gameObject.AddComponent<Camera>();
			if (null != camMain)
			{
				camView[0].CopyFrom(camMain);
			}
			camView[0].clearFlags = CameraClearFlags.Color;
			camView[0].backgroundColor = new Color(0.6f, 0.6f, 0.6f, 1f);
			gameObject.AddComponent<WireFrameRender>();
			Color[] array2 = new Color[4]
			{
				new Color(0.6f, 0.6f, 0.6f, 1f),
				new Color(0.65f, 0.65f, 0.65f, 1f),
				new Color(0.7f, 0.7f, 0.7f, 1f),
				new Color(0.75f, 0.75f, 0.75f, 1f)
			};
			Vector3[] array3 = new Vector3[4]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 1f, 0f),
				new Vector3(0f, 1f, 0f)
			};
			Vector3[] array4 = new Vector3[4]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(90f, 0f, 0f),
				new Vector3(0f, 180f, 0f),
				new Vector3(0f, 90f, 0f)
			};
			for (int i = 1; i < 4; i++)
			{
				GameObject gameObject2 = new GameObject(string.Format(array[i], i));
				gameObject2.transform.SetParent(base.transform);
				gameObject2.transform.localPosition = array3[i];
				gameObject2.transform.localEulerAngles = array4[i];
				camView[i] = gameObject2.AddComponent<Camera>();
				camView[i].cullingMask = cullingMask;
				camView[i].clearFlags = CameraClearFlags.Color;
				camView[i].backgroundColor = array2[i];
				camView[i].orthographic = true;
				camView[i].orthographicSize = 0.5f;
				camView[i].nearClipPlane = -10f;
				camView[i].farClipPlane = 10f;
				camView[i].depth = 1000f;
				camView[i].useOcclusionCulling = true;
				camView[i].allowHDR = false;
				camView[i].allowMSAA = true;
				gameObject2.AddComponent<WireFrameRender>();
			}
		}
	}
}
