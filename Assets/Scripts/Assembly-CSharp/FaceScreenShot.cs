using System.Collections;
using System.IO;
using System.Linq;
using Illusion.CustomAttributes;
using UnityEngine;

public class FaceScreenShot : MonoBehaviour
{
	public enum CapType
	{
		Face = 0,
		Eyes = 1,
		Mouth = 2
	}

	[SerializeField]
	private CapType capType;

	[SerializeField]
	private GameScreenShotAssist[] scriptGssAssist;

	[SerializeField]
	private ChaControl chaCtrl;

	[SerializeField]
	[NamedArray(typeof(CapType))]
	private Vector2[] capPosition = new Vector2[3]
	{
		new Vector2(80f, 300f),
		new Vector2(150f, 100f),
		new Vector2(250f, 100f)
	};

	[SerializeField]
	private Rect texRect = new Rect(490f, 0f, 220f, 0f);

	[NotEditable]
	[SerializeField]
	private int renderCount;

	[SerializeField]
	[NotEditable]
	private int maxSize;

	private string savePath = string.Empty;

	[Button("Capture", "キャプチャー", new object[] { })]
	public int excuteCapture;

	[Button("GetGSS", "GSS取得", new object[] { })]
	public int getGSS;

	[Button("GetChara", "キャラ取得", new object[] { })]
	public int getChara;

	private void GetGSS()
	{
		scriptGssAssist = Object.FindObjectsOfType<GameScreenShotAssist>();
	}

	private void GetChara()
	{
		chaCtrl = Object.FindObjectOfType<ChaControl>();
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Capture()
	{
		if (scriptGssAssist.Length != 0 && !(chaCtrl == null))
		{
			renderCount = 0;
			StartCoroutine(CapFace());
		}
	}

	private IEnumerator CapFace()
	{
		bool bkBlink = chaCtrl.GetEyesBlinkFlag();
		chaCtrl.ChangeEyesBlinkFlag(false);
		string path = UserData.Create("exp_cap/" + capType);
		string ext = ".png";
		int eyeMax = chaCtrl.eyesCtrl.GetMaxPtn();
		int mouthMax = chaCtrl.mouthCtrl.GetMaxPtn();
		int index = (int)capType;
		texRect.y = capPosition[index].x;
		texRect.height = capPosition[index].y;
		switch (capType)
		{
		case CapType.Face:
		{
			renderCount = 0;
			maxSize = eyeMax * mouthMax;
			for (int i = 0; i < eyeMax; i++)
			{
				chaCtrl.ChangeEyesPtn(i, false);
				for (int j = 0; j < mouthMax; j++)
				{
					chaCtrl.ChangeMouthPtn(j, false);
					string localPath = path + chaCtrl.GetEyesPtn();
					if (!Directory.Exists(localPath))
					{
						Directory.CreateDirectory(localPath);
					}
					string fileName = string.Format("{0}/{1}", chaCtrl.GetEyesPtn(), chaCtrl.GetMouthPtn());
					savePath = path + fileName + ext;
					renderCount++;
					yield return null;
					yield return StartCoroutine(CaptureFunc());
				}
			}
			break;
		}
		case CapType.Eyes:
			renderCount = -1;
			maxSize = eyeMax;
			while (++renderCount < eyeMax)
			{
				chaCtrl.ChangeEyesPtn(renderCount, false);
				savePath = path + renderCount + ext;
				yield return null;
				yield return StartCoroutine(CaptureFunc());
			}
			break;
		case CapType.Mouth:
			renderCount = -1;
			maxSize = mouthMax;
			while (++renderCount < mouthMax)
			{
				chaCtrl.ChangeMouthPtn(renderCount, false);
				savePath = path + renderCount + ext;
				yield return null;
				yield return StartCoroutine(CaptureFunc());
			}
			break;
		}
		chaCtrl.ChangeEyesBlinkFlag(bkBlink);
		yield return null;
	}

	private IEnumerator CaptureFunc()
	{
		yield return new WaitForEndOfFrame();
		Texture2D tex = new Texture2D((int)texRect.width, (int)texRect.height, TextureFormat.RGB24, false);
		RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, (QualitySettings.antiAliasing == 0) ? 1 : QualitySettings.antiAliasing);
		Graphics.Blit(scriptGssAssist[0].rtCamera, rt);
		foreach (GameScreenShotAssist item in scriptGssAssist.Skip(1))
		{
			Graphics.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), item.rtCamera);
		}
		RenderTexture.active = rt;
		tex.ReadPixels(texRect, 0, 0);
		tex.Apply();
		RenderTexture.active = null;
		byte[] bytes = tex.EncodeToPNG();
		RenderTexture.ReleaseTemporary(rt);
		Object.Destroy(tex);
		tex = null;
		File.WriteAllBytes(savePath, bytes);
		yield return null;
	}
}
