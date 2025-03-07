using System;
using Illusion.CustomAttributes;
using UnityEngine;
using UnityEngine.UI;

public class EyeLookMaterialControll : MonoBehaviour
{
	[Serializable]
	public class TexState
	{
		[HideInInspector]
		public int texID = -1;

		public string texName = string.Empty;

		public bool isYure;
	}

	[Button("Initialize", "左目の設定で初期化", new object[] { 0 })]
	public int initializeL;

	[Button("Initialize", "右目の設定で初期化", new object[] { 1 })]
	public int initializeR;

	[Space]
	public EyeLookCalc script;

	public Renderer _renderer;

	private Material _material;

	public int InsideWait;

	public int OutsideWait;

	public int UpWait;

	public int DownWait;

	public float InsideLimit;

	public float OutsideLimit;

	public float UpLimit;

	public float DownLimit;

	public EYE_LR eyeLR;

	public Rect Limit;

	[Range(0.0001f, 0.001f)]
	public float power = 0.001f;

	private int textureWidth;

	private int textureHeight;

	[SerializeField]
	private Vector2 offset = default(Vector2);

	[SerializeField]
	private float hlUpOffsetY;

	[SerializeField]
	private float hlDownOffsetY;

	[SerializeField]
	private Vector2 scale = default(Vector2);

	public TexState[] texStates;

	public int YureInside;

	public int YureOutside;

	public int YureUp;

	public int YureDown;

	public float YureTime = 1f;

	private float YureTimer;

	private Vector2 YureAddVec = new Vector2(0f, 0f);

	private Vector2 YureAddScale = new Vector2(1f, 1f);

	public Text text;

	public void Initialize(int lr)
	{
		texStates = new TexState[3];
		for (int i = 0; i < texStates.Length; i++)
		{
			texStates[i] = new TexState();
		}
		texStates[0].texName = "_MainTex";
		texStates[1].texName = "_overtex1";
		texStates[2].texName = "_overtex2";
		InsideWait = -100;
		OutsideWait = 100;
		UpWait = -100;
		DownWait = 100;
		InsideLimit = -100f;
		OutsideLimit = 100f;
		UpLimit = -80f;
		DownLimit = 80f;
		eyeLR = ((lr != 0) ? EYE_LR.EYE_R : EYE_LR.EYE_L);
		Limit.x = 80f;
		Limit.y = 80f;
		Limit.width = 100f;
		Limit.height = 100f;
		power = 0.001f;
		offset.x = ((lr != 0) ? 0.2f : (-0.2f));
		offset.y = -0.2f;
		hlUpOffsetY = 0f;
		hlDownOffsetY = 0f;
		scale.x = 0f;
		scale.y = 0f;
		YureInside = 4;
		YureOutside = -4;
		YureUp = 4;
		YureDown = -4;
		YureTime = 0.3f;
	}

	private void Start()
	{
		ReSetupMaterial();
		if (texStates != null)
		{
			TexState[] array = texStates;
			foreach (TexState texState in array)
			{
				texState.texID = Shader.PropertyToID(texState.texName);
			}
		}
	}

	private void Update()
	{
		if (null == script)
		{
			return;
		}
		float x = script.GetAngleHRate(eyeLR) + offset.x;
		float y = script.GetAngleVRate() + offset.y;
		Vector2 vector = new Vector2(x, y);
		if (vector.magnitude > 1f)
		{
			vector = vector.normalized;
		}
		float num = Mathf.Lerp(InsideWait, OutsideWait, Mathf.InverseLerp(-1f, 1f, vector.x));
		float num2 = Mathf.Lerp(DownWait, UpWait, Mathf.InverseLerp(-1f, 1f, vector.y));
		float num3 = Mathf.Lerp(1f, 5f, scale.x);
		float num4 = Mathf.Lerp(1f, 5f, scale.y);
		bool flag = false;
		if (YureTime < YureTimer)
		{
			flag = true;
			YureTimer = 0f;
		}
		YureTimer += Time.deltaTime;
		Vector2 vector2 = scale;
		vector2.x *= YureAddScale.x;
		vector2.y *= YureAddScale.y;
		for (int i = 0; i < 3; i++)
		{
			TexState texState = texStates[i];
			Vector2 vector3 = new Vector2(power, power);
			if (texState.isYure)
			{
				vector3.x *= 0.8f;
				vector3.y *= 0.5f;
			}
			Vector2 vector4 = new Vector2(Mathf.Clamp(num * (vector3.x * num3), InsideLimit, OutsideLimit), Mathf.Clamp(num2 * (vector3.y * num4), UpLimit, DownLimit));
			switch (i)
			{
			case 1:
				vector4 = new Vector2(vector4.x, vector4.y + hlUpOffsetY);
				break;
			case 2:
				vector4 = new Vector2(vector4.x, vector4.y + hlDownOffsetY);
				break;
			}
			Vector2 value = vector4;
			if (texState.isYure)
			{
				value += vector2 * -0.5f;
				if (flag)
				{
					YureAddScale.x = UnityEngine.Random.Range(1f, 2f);
					YureAddScale.y = UnityEngine.Random.Range(1f, 1.5f);
				}
				value += YureAddVec;
				YureTimer += Time.deltaTime;
			}
			else
			{
				value += scale * -0.5f;
			}
			if (texState.texID != -1)
			{
				_material.SetTextureOffset(texState.texID, value);
				if (texState.isYure)
				{
					_material.SetTextureScale(texState.texID, new Vector2(1f + vector2.x, 1f + vector2.y));
				}
				else
				{
					_material.SetTextureScale(texState.texID, new Vector2(1f + scale.x, 1f + scale.y));
				}
			}
		}
		if (text != null)
		{
			text.text = "X:" + num + " Y:" + num2;
		}
	}

	public Vector2 GetEyeTexOffset()
	{
		return offset;
	}

	public void SetEyeTexOffsetX(float value)
	{
		if (eyeLR == EYE_LR.EYE_R)
		{
			value *= -1f;
		}
		offset.x = value;
	}

	public void SetEyeTexOffsetY(float value)
	{
		offset.y = value;
	}

	public float GetEyeTexHLUpOffsetY()
	{
		return hlUpOffsetY;
	}

	public float GetEyeTexHLDownOffsetY()
	{
		return hlDownOffsetY;
	}

	public void SetEyeTexHLUpOffsetY(float value)
	{
		hlUpOffsetY = value;
	}

	public void SetEyeTexHLDownOffsetY(float value)
	{
		hlDownOffsetY = value;
	}

	public Vector2 GetEyeTexScale()
	{
		return scale;
	}

	public void SetEyeTexScaleX(float value)
	{
		scale.x = value;
	}

	public void SetEyeTexScaleY(float value)
	{
		scale.y = value;
	}

	public void SetEyeRot(float value)
	{
		if (eyeLR == EYE_LR.EYE_R)
		{
			value *= -1f;
		}
		_material.SetFloat("_rotation", value);
	}

	public void ReSetupMaterial()
	{
		_material = null;
		if (null == _renderer)
		{
			_renderer = GetComponent<Renderer>();
			if ((bool)_renderer)
			{
				_material = _renderer.material;
			}
		}
		else
		{
			_material = _renderer.material;
		}
	}

	public void ChangeShaking(bool enable)
	{
		if (texStates == null)
		{
			return;
		}
		TexState[] array = texStates;
		foreach (TexState texState in array)
		{
			if (texState.texName == "_MainTex")
			{
				texState.isYure = false;
			}
			else
			{
				texState.isYure = enable;
			}
		}
	}
}
