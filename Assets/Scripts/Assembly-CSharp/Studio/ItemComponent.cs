using System;
using System.Linq;
using Illusion.CustomAttributes;
using UnityEngine;

namespace Studio
{
	public class ItemComponent : MonoBehaviour
	{
		[Serializable]
		public class Info
		{
			public bool useColor = true;

			public Color defColor = Color.white;

			public bool usePattern = true;

			public Color defColorPattern = Color.white;

			public bool defClamp = true;

			public Vector4 defUV = Vector4.zero;

			public float defRot;

			public float ut
			{
				get
				{
					return defUV.x;
				}
				set
				{
					defUV.x = value;
				}
			}

			public float vt
			{
				get
				{
					return defUV.y;
				}
				set
				{
					defUV.y = value;
				}
			}

			public float us
			{
				get
				{
					return defUV.z;
				}
				set
				{
					defUV.z = value;
				}
			}

			public float vs
			{
				get
				{
					return defUV.w;
				}
				set
				{
					defUV.w = value;
				}
			}
		}

		[Header("通常パーツ")]
		public Renderer[] rendNormal;

		[Header("半透明パーツ")]
		public Renderer[] rendAlpha;

		[Header("ガラスパーツ")]
		public Renderer[] rendGlass;

		[Space]
		[Header("構成情報")]
		public Info[] info;

		public Color defShadow = Color.white;

		public float alpha = 1f;

		public Color defGlass = Color.white;

		public Color defLineColor = Color.white;

		public float defLineWidth = 1f;

		public Color defEmissionColor = Color.white;

		public float defEmissionPower;

		public float defLightCancel;

		[Button("SetColor", "初期色を設定", new object[] { })]
		[Space]
		public int setcolor;

		public bool check
		{
			get
			{
				return !rendNormal.IsNullOrEmpty() || !rendAlpha.IsNullOrEmpty();
			}
		}

		public bool checkAlpha
		{
			get
			{
				return !rendAlpha.IsNullOrEmpty();
			}
		}

		public bool checkGlass
		{
			get
			{
				return !rendGlass.IsNullOrEmpty();
			}
		}

		public bool checkLine
		{
			get
			{
				return rendNormal.Any((Renderer r) => r.material.HasProperty(ItemShader._LineColor));
			}
		}

		public bool checkEmissionColor
		{
			get
			{
				return HasProperty(rendNormal, ItemShader._EmissionColor) | HasProperty(rendAlpha, ItemShader._EmissionColor);
			}
		}

		public bool checkEmissionPower
		{
			get
			{
				return HasProperty(rendNormal, ItemShader._EmissionPower) | HasProperty(rendAlpha, ItemShader._EmissionPower);
			}
		}

		public bool checkEmission
		{
			get
			{
				return checkEmissionColor | checkEmissionPower;
			}
		}

		public bool checkLightCancel
		{
			get
			{
				return HasProperty(rendNormal, ItemShader._LightCancel) | HasProperty(rendAlpha, ItemShader._LightCancel);
			}
		}

		public Color[] defColorMain
		{
			get
			{
				return info.Select((Info i) => i.defColor).ToArray();
			}
		}

		public Color[] defColorPattern
		{
			get
			{
				return info.Select((Info i) => i.defColorPattern).ToArray();
			}
		}

		public bool[] useColor
		{
			get
			{
				return info.Select((Info i) => i.useColor).ToArray();
			}
		}

		public bool[] usePattern
		{
			get
			{
				return info.Select((Info i) => i.usePattern).ToArray();
			}
		}

		public Info this[int _idx]
		{
			get
			{
				return info.SafeGet(_idx);
			}
		}

		public void UpdateColor(OIItemInfo _info)
		{
			Renderer[] array = rendNormal;
			foreach (Renderer renderer in array)
			{
				for (int j = 0; j < 3; j++)
				{
					if (!info[j].useColor)
					{
						continue;
					}
					Material[] materials = renderer.materials;
					foreach (Material material in materials)
					{
						switch (j)
						{
						case 0:
							material.SetColor(ItemShader._Color, _info.color[0]);
							if (info[j].usePattern)
							{
								material.SetColor(ItemShader._Color1_2, _info.color[3]);
								material.SetVector(ItemShader._Patternuv1, _info.pattern[0].uv);
								material.SetFloat(ItemShader._patternrotator1, _info.pattern[0].rot);
								material.SetFloat(ItemShader._patternclamp1, (!_info.pattern[0].clamp) ? 0f : 1f);
							}
							break;
						case 1:
							material.SetColor(ItemShader._Color2, _info.color[1]);
							if (info[j].usePattern)
							{
								material.SetColor(ItemShader._Color2_2, _info.color[4]);
								material.SetVector(ItemShader._Patternuv2, _info.pattern[1].uv);
								material.SetFloat(ItemShader._patternrotator2, _info.pattern[1].rot);
								material.SetFloat(ItemShader._patternclamp2, (!_info.pattern[1].clamp) ? 0f : 1f);
							}
							break;
						case 2:
							material.SetColor(ItemShader._Color3, _info.color[2]);
							if (info[j].usePattern)
							{
								material.SetColor(ItemShader._Color3_2, _info.color[5]);
								material.SetVector(ItemShader._Patternuv3, _info.pattern[2].uv);
								material.SetFloat(ItemShader._patternrotator3, _info.pattern[2].rot);
								material.SetFloat(ItemShader._patternclamp3, (!_info.pattern[2].clamp) ? 0f : 1f);
							}
							break;
						}
					}
				}
				Material[] materials2 = renderer.materials;
				foreach (Material material2 in materials2)
				{
					material2.SetColor(ItemShader._ShadowColor, _info.color[6]);
				}
				Material[] materials3 = renderer.materials;
				foreach (Material material3 in materials3)
				{
					if (material3.HasProperty(ItemShader._LineColor))
					{
						material3.SetColor(ItemShader._LineColor, _info.lineColor);
					}
					if (material3.HasProperty(ItemShader._LineWidthS))
					{
						material3.SetFloat(ItemShader._LineWidthS, _info.lineWidth);
					}
					if (material3.HasProperty(ItemShader._EmissionColor))
					{
						material3.SetColor(ItemShader._EmissionColor, _info.emissionColor);
					}
					if (material3.HasProperty(ItemShader._EmissionPower))
					{
						material3.SetFloat(ItemShader._EmissionPower, _info.emissionPower);
					}
					if (material3.HasProperty(ItemShader._LightCancel))
					{
						material3.SetFloat(ItemShader._LightCancel, _info.lightCancel);
					}
				}
			}
			Renderer[] array2 = rendAlpha;
			foreach (Renderer renderer2 in array2)
			{
				renderer2.material.SetFloat(ItemShader._alpha, _info.alpha);
				Material[] materials4 = renderer2.materials;
				foreach (Material material4 in materials4)
				{
					if (material4.HasProperty(ItemShader._EmissionColor))
					{
						material4.SetColor(ItemShader._EmissionColor, _info.emissionColor);
					}
					if (material4.HasProperty(ItemShader._EmissionPower))
					{
						material4.SetFloat(ItemShader._EmissionPower, _info.emissionPower);
					}
					if (material4.HasProperty(ItemShader._LightCancel))
					{
						material4.SetFloat(ItemShader._LightCancel, _info.lightCancel);
					}
				}
			}
			Renderer[] array3 = rendGlass;
			foreach (Renderer renderer3 in array3)
			{
				renderer3.material.SetColor(ItemShader._Color4, _info.color[7]);
			}
		}

		public void SetPatternTex(int _idx, Texture2D _texture)
		{
			int[] array = new int[3]
			{
				ItemShader._PatternMask1,
				ItemShader._PatternMask2,
				ItemShader._PatternMask3
			};
			Renderer[] array2 = rendNormal;
			foreach (Renderer renderer in array2)
			{
				renderer.material.SetTexture(array[_idx], _texture);
			}
		}

		public void SetColor()
		{
			if (!rendNormal.IsNullOrEmpty())
			{
				Material sharedMaterial = rendNormal[0].sharedMaterial;
				if (null != sharedMaterial)
				{
					info[0].defColor = sharedMaterial.GetColor("_Color");
					info[1].defColor = sharedMaterial.GetColor("_Color2");
					info[2].defColor = sharedMaterial.GetColor("_Color3");
					info[0].defColorPattern = sharedMaterial.GetColor("_Color1_2");
					info[1].defColorPattern = sharedMaterial.GetColor("_Color2_2");
					info[2].defColorPattern = sharedMaterial.GetColor("_Color3_2");
					info[0].defUV = sharedMaterial.GetVector("_Patternuv1");
					info[1].defUV = sharedMaterial.GetVector("_Patternuv2");
					info[2].defUV = sharedMaterial.GetVector("_Patternuv3");
					info[0].defRot = sharedMaterial.GetFloat("_patternrotator1");
					info[1].defRot = sharedMaterial.GetFloat("_patternrotator2");
					info[2].defRot = sharedMaterial.GetFloat("_patternrotator3");
					info[0].defClamp = sharedMaterial.GetFloat("_patternclamp1") != 0f;
					info[1].defClamp = sharedMaterial.GetFloat("_patternclamp2") != 0f;
					info[2].defClamp = sharedMaterial.GetFloat("_patternclamp3") != 0f;
					defShadow = sharedMaterial.GetColor("_ShadowColor");
					defLineColor = sharedMaterial.GetColor("_LineColor");
					defLineWidth = sharedMaterial.GetFloat("_LineWidthS");
				}
			}
			if (!rendAlpha.IsNullOrEmpty())
			{
				Material sharedMaterial2 = rendAlpha[0].sharedMaterial;
				if (null != sharedMaterial2)
				{
					alpha = sharedMaterial2.GetFloat("_alpha");
				}
			}
			if (!rendGlass.IsNullOrEmpty())
			{
				Material sharedMaterial3 = rendGlass[0].sharedMaterial;
				if (null != sharedMaterial3)
				{
					defGlass = sharedMaterial3.GetColor("_Color4");
				}
			}
		}

		public void SetLine()
		{
			if (rendNormal.IsNullOrEmpty())
			{
				return;
			}
			Material material = rendNormal[0].material;
			if (null != material)
			{
				if (material.HasProperty("_LineColor"))
				{
					defLineColor = material.GetColor("_LineColor");
				}
				if (material.HasProperty("_LineWidthS"))
				{
					defLineWidth = material.GetFloat("_LineWidthS");
				}
			}
		}

		public void SetEmission()
		{
			bool[] result = new bool[3];
			Func<Renderer[], bool> func = delegate(Renderer[] _rend)
			{
				if (_rend.IsNullOrEmpty())
				{
					return false;
				}
				foreach (Renderer renderer in _rend)
				{
					Material material = renderer.material;
					if (!result[0] && material.HasProperty("_EmissionColor"))
					{
						defEmissionColor = material.GetColor("_EmissionColor");
						result[0] = true;
					}
					if (!result[1] && material.HasProperty("_EmissionPower"))
					{
						defEmissionPower = material.GetFloat("_EmissionPower");
						result[1] = true;
					}
					if (!result[2] && material.HasProperty("_LightCancel"))
					{
						defLightCancel = material.GetFloat("_LightCancel");
						result[2] = true;
					}
					if (result[0] & result[1] & result[2])
					{
						break;
					}
				}
				return result[0] & result[1] & result[2];
			};
			if (!func(rendNormal))
			{
				func(rendAlpha);
			}
		}

		public void SetFlag(bool[] _color, bool[] _pattern)
		{
			for (int i = 0; i < info.Length; i++)
			{
				info[i].useColor = _color[i];
				info[i].usePattern = _pattern[i];
			}
		}

		private bool HasProperty(Renderer[] _renderer, int _nameID)
		{
			return _renderer.Any((Renderer r) => r.materials.Any((Material m) => m.HasProperty(_nameID)));
		}

		private void Reset()
		{
			info = new Info[3];
			for (int i = 0; i < 3; i++)
			{
				info[i] = new Info();
			}
			SetColor();
		}
	}
}
