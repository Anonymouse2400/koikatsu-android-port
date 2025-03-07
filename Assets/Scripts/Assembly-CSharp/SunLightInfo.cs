using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Component.UI;
using Illusion.CustomAttributes;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class SunLightInfo : MonoBehaviour
{
	[Serializable]
	public class Info
	{
		public enum Type
		{
			DayTime = 0,
			Evening = 1,
			Night = 2
		}

		[Label("時間帯")]
		public Type type;

		[Header("2D背景画像の設定")]
		[Label("BackTexture")]
		public Texture2D backTexture;

		[Header("表示オブジェクトの設定")]
		public List<GameObject> visibleList = new List<GameObject>();

		[Label("角度")]
		[Header("サンライトの設定")]
		public Vector3 angle;

		[Label("色")]
		public Color color;

		[Label("強さ")]
		public float intensity;

		[Header("フォグの設定")]
		[Label("使用するか")]
		public bool fogUse;

		[Label("色")]
		public Color fogColor;

		[Label("開始距離")]
		public float fogStart;

		[Label("終了距離")]
		public float fogEnd;

		[Header("カメラのサンシャフト(SunShafts)設定")]
		[Label("色")]
		public Color sunShaftsColor;

		[Label("Shafts caster")]
		public Transform sunShaftsTransform;

		[Label("LutTexture")]
		[Header("カメラのカラーエフェクト(AmplifyColorEffect)設定")]
		public Texture aceLutTexture;

		[Label("LutBlendTexture")]
		public Texture aceLutBlendTexture;

		[Label("BlendAmount")]
		public float aceBlendAmount;

		public void SetBackTexture(BackGroundParam param)
		{
			if (!(param == null))
			{
				param.SetTexture(backTexture);
			}
		}

		public void CopyBackTexture(BackGroundParam param)
		{
			if (!(param == null) && !(param.image == null) && !(param.image.sprite == null))
			{
				backTexture = param.image.sprite.texture;
			}
		}

		public void VisibleChange(bool visible)
		{
			visibleList.ForEach(delegate(GameObject p)
			{
				p.SetActive(visible);
			});
		}

		public void SetLight(Light light)
		{
			light.color = color;
			light.transform.eulerAngles = angle;
			light.intensity = intensity;
		}

		public void CopyLight(Light light)
		{
			color = light.color;
			angle = light.transform.eulerAngles;
			intensity = light.intensity;
		}

		public void SetFog()
		{
			RenderSettings.fog = fogUse;
			RenderSettings.fogMode = FogMode.Linear;
			RenderSettings.fogColor = fogColor;
			RenderSettings.fogStartDistance = fogStart;
			RenderSettings.fogEndDistance = fogEnd;
		}

		public void CopyFog()
		{
			fogUse = RenderSettings.fog;
			fogColor = RenderSettings.fogColor;
			fogStart = RenderSettings.fogStartDistance;
			fogEnd = RenderSettings.fogEndDistance;
		}

		public void SetSunShafts(SunShafts sunShafts)
		{
			sunShafts.sunColor = sunShaftsColor;
			sunShafts.sunTransform = sunShaftsTransform;
		}

		public void CopySunShafts(SunShafts sunShafts)
		{
			sunShaftsColor = sunShafts.sunColor;
			sunShaftsTransform = sunShafts.sunTransform;
		}

		public void SetACE(AmplifyColorEffect ace)
		{
			ace.LutTexture = aceLutTexture;
			ace.LutBlendTexture = aceLutBlendTexture;
			ace.BlendAmount = aceBlendAmount;
		}

		public void CopyACE(AmplifyColorEffect ace)
		{
			aceLutTexture = ace.LutTexture;
			aceLutBlendTexture = ace.LutBlendTexture;
			aceBlendAmount = ace.BlendAmount;
		}
	}

	[Label("サンライト")]
	[SerializeField]
	private Light _targetLight;

	[Label("2D背景パラメータ")]
	[SerializeField]
	private BackGroundParam _bgParam;

	[SerializeField]
	private Info[] _infos;

	public Light targetLight
	{
		get
		{
			return _targetLight;
		}
	}

	public BackGroundParam bgParam
	{
		get
		{
			return _bgParam;
		}
	}

	public Info[] infos
	{
		get
		{
			return _infos;
		}
	}

	public bool Set(Info.Type type, Camera cam)
	{
		if (cam == null)
		{
			return false;
		}
		Info info = infos.FirstOrDefault((Info p) => p.type == type);
		if (info != null)
		{
			Info[] array = infos;
			foreach (Info info2 in array)
			{
				info2.VisibleChange(info2 == info);
			}
			info.SetLight(_targetLight);
			info.SetFog();
			info.SetSunShafts(cam.GetComponent<SunShafts>());
			info.SetACE(cam.GetComponent<AmplifyColorEffect>());
			info.SetBackTexture(_bgParam);
			return true;
		}
		return false;
	}
}
