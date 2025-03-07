using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Other/SMAA")]
	public class AntiAliasing : MonoBehaviour
	{
		public enum DebugDisplay
		{
			Off = 0,
			Edges = 1,
			Weights = 2,
			Depth = 3,
			Accumulation = 4
		}

		public enum EdgeType
		{
			Luminance = 0,
			Color = 1,
			Depth = 2
		}

		public enum TemporalType
		{
			Off = 0,
			SMAA_2x = 1,
			Standard_2x = 2,
			Standard_4x = 3,
			Standard_8x = 4,
			Standard_16x = 5
		}

		private enum Passes
		{
			Copy = 0,
			LumaDetection = 1,
			ClearToBlack = 2,
			WeightCalculation = 3,
			WeightsAndBlend1 = 4,
			WeightsAndBlend2 = 5,
			ColorDetection = 6,
			MergeFrames = 7,
			DepthDetection = 8,
			DebugDepth = 9
		}

		public DebugDisplay displayType;

		public EdgeType edgeType = EdgeType.Depth;

		public Texture2D areaTex;

		public Texture2D searchTex;

		private Matrix4x4 m_BaseProjectionMatrix;

		private Matrix4x4 m_PrevViewProjMat;

		private Camera m_AACamera;

		private int m_SampleIndex;

		[Range(0f, 80f)]
		public float K = 1f;

		public TemporalType temporalType;

		[Range(0f, 1f)]
		public float temporalAccumulationWeight = 0.3f;

		[Range(0.01f, 1f)]
		public float depthThreshold = 0.1f;

		public Shader smaaShader;

		private Material m_SmaaMaterial;

		private RenderTexture m_RtAccum;

		private Camera aaCamera
		{
			get
			{
				if (m_AACamera == null)
				{
					m_AACamera = GetComponent<Camera>();
				}
				return m_AACamera;
			}
		}

		public Material smaaMaterial
		{
			get
			{
				if (m_SmaaMaterial == null)
				{
					m_SmaaMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(smaaShader);
				}
				return m_SmaaMaterial;
			}
		}

		private static Matrix4x4 CalculateViewProjection(Camera camera, Matrix4x4 prjMatrix)
		{
			Matrix4x4 worldToCameraMatrix = camera.worldToCameraMatrix;
			Matrix4x4 gPUProjectionMatrix = GL.GetGPUProjectionMatrix(prjMatrix, true);
			return gPUProjectionMatrix * worldToCameraMatrix;
		}

		private void StoreBaseProjectionMatrix(Matrix4x4 prjMatrix)
		{
			m_BaseProjectionMatrix = prjMatrix;
		}

		private void StorePreviousViewProjMatrix(Matrix4x4 viewPrjMatrix)
		{
			m_PrevViewProjMat = viewPrjMatrix;
		}

		public void UpdateSampleIndex()
		{
			int num = 1;
			if (temporalType == TemporalType.SMAA_2x || temporalType == TemporalType.Standard_2x)
			{
				num = 2;
			}
			else if (temporalType == TemporalType.Standard_4x)
			{
				num = 4;
			}
			else if (temporalType == TemporalType.Standard_8x)
			{
				num = 8;
			}
			else if (temporalType == TemporalType.Standard_16x)
			{
				num = 16;
			}
			m_SampleIndex = (m_SampleIndex + 1) % num;
		}

		private Vector2 GetJitterStandard2X()
		{
			int[,] array = new int[2, 2]
			{
				{ 4, 4 },
				{ -4, -4 }
			};
			int num = array[m_SampleIndex, 0];
			int num2 = array[m_SampleIndex, 1];
			float x = (float)num / 16f;
			float y = (float)num2 / 16f;
			return new Vector2(x, y);
		}

		private Vector2 GetJitterStandard4X()
		{
			int[,] array = new int[4, 2]
			{
				{ -2, -6 },
				{ 6, -2 },
				{ -6, 2 },
				{ 2, 6 }
			};
			int num = array[m_SampleIndex, 0];
			int num2 = array[m_SampleIndex, 1];
			float x = (float)num / 16f;
			float y = (float)num2 / 16f;
			return new Vector2(x, y);
		}

		private Vector2 GetJitterStandard8X()
		{
			int[,] array = new int[8, 2]
			{
				{ 7, -7 },
				{ -3, -5 },
				{ 3, 7 },
				{ -7, -1 },
				{ 5, 1 },
				{ -1, 3 },
				{ 1, -3 },
				{ -5, 5 }
			};
			int num = array[m_SampleIndex, 0];
			int num2 = array[m_SampleIndex, 1];
			float x = (float)num / 16f;
			float y = (float)num2 / 16f;
			return new Vector2(x, y);
		}

		private Vector2 GetJitterStandard16X()
		{
			int[,] array = new int[16, 2]
			{
				{ 7, -4 },
				{ -1, -3 },
				{ 3, -5 },
				{ -5, -2 },
				{ 6, 7 },
				{ -2, 6 },
				{ 2, 5 },
				{ -6, -4 },
				{ 4, -1 },
				{ -3, 2 },
				{ 1, 1 },
				{ -8, 0 },
				{ 5, 3 },
				{ -4, -6 },
				{ 0, -7 },
				{ -7, -8 }
			};
			int num = array[m_SampleIndex, 0];
			int num2 = array[m_SampleIndex, 1];
			float x = ((float)num + 0.5f) / 16f;
			float y = ((float)num2 + 0.5f) / 16f;
			return new Vector2(x, y);
		}

		private Vector2 GetJitterSMAAX2()
		{
			float num = 0.25f;
			num *= ((m_SampleIndex != 0) ? 1f : (-1f));
			float x = num;
			float y = 0f - num;
			return new Vector2(x, y);
		}

		private Vector2 GetCurrentJitter()
		{
			Vector2 result = new Vector2(0f, 0f);
			if (temporalType == TemporalType.SMAA_2x)
			{
				return GetJitterSMAAX2();
			}
			if (temporalType == TemporalType.Standard_2x)
			{
				return GetJitterStandard2X();
			}
			if (temporalType == TemporalType.Standard_4x)
			{
				return GetJitterStandard4X();
			}
			if (temporalType == TemporalType.Standard_8x)
			{
				return GetJitterStandard8X();
			}
			if (temporalType == TemporalType.Standard_16x)
			{
				return GetJitterStandard16X();
			}
			return result;
		}

		private void OnPreCull()
		{
			StoreBaseProjectionMatrix(aaCamera.projectionMatrix);
			if (temporalType != 0)
			{
				UpdateSampleIndex();
				Vector2 currentJitter = GetCurrentJitter();
				Matrix4x4 identity = Matrix4x4.identity;
				identity.m03 = currentJitter.x * 2f / (float)aaCamera.pixelWidth;
				identity.m13 = currentJitter.y * 2f / (float)aaCamera.pixelHeight;
				Matrix4x4 projectionMatrix = identity * m_BaseProjectionMatrix;
				aaCamera.projectionMatrix = projectionMatrix;
			}
		}

		private void OnPostRender()
		{
			aaCamera.ResetProjectionMatrix();
		}

		protected void OnEnable()
		{
			if (smaaShader == null)
			{
				smaaShader = Shader.Find("Hidden/SMAA");
			}
			if (!ImageEffectHelper.IsSupported(smaaShader, true, true, this))
			{
				base.enabled = false;
				Debug.LogWarning("The image effect " + ToString() + " has been disabled as it's not supported on the current platform.");
			}
			else
			{
				aaCamera.depthTextureMode |= DepthTextureMode.Depth;
			}
		}

		private void OnDisable()
		{
			aaCamera.ResetProjectionMatrix();
			if ((bool)m_SmaaMaterial)
			{
				Object.DestroyImmediate(m_SmaaMaterial);
				m_SmaaMaterial = null;
			}
			if ((bool)m_RtAccum)
			{
				Object.DestroyImmediate(m_RtAccum);
				m_RtAccum = null;
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (smaaMaterial == null)
			{
				Graphics.Blit(source, destination);
				return;
			}
			bool flag = false;
			if (m_RtAccum == null || m_RtAccum.width != source.width || m_RtAccum.height != source.height)
			{
				if (m_RtAccum != null)
				{
					RenderTexture.ReleaseTemporary(m_RtAccum);
				}
				m_RtAccum = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
				m_RtAccum.hideFlags = HideFlags.DontSave;
				flag = true;
			}
			int value = 0;
			if (temporalType == TemporalType.SMAA_2x)
			{
				value = ((m_SampleIndex < 1) ? 1 : 2);
			}
			int width = source.width;
			int height = source.height;
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
			RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
			Matrix4x4 matrix4x = CalculateViewProjection(aaCamera, m_BaseProjectionMatrix);
			Matrix4x4 matrix4x2 = Matrix4x4.Inverse(matrix4x);
			smaaMaterial.SetMatrix("_ToPrevViewProjCombined", m_PrevViewProjMat * matrix4x2);
			smaaMaterial.SetInt("_JitterOffset", value);
			smaaMaterial.SetTexture("areaTex", areaTex);
			smaaMaterial.SetTexture("searchTex", searchTex);
			smaaMaterial.SetTexture("colorTex", source);
			smaaMaterial.SetVector("_PixelSize", new Vector4(1f / (float)source.width, 1f / (float)source.height, 0f, 0f));
			Vector2 currentJitter = GetCurrentJitter();
			smaaMaterial.SetVector("_PixelOffset", new Vector4(currentJitter.x / (float)source.width, currentJitter.y / (float)source.height, 0f, 0f));
			smaaMaterial.SetTexture("edgesTex", temporary);
			smaaMaterial.SetTexture("blendTex", temporary2);
			smaaMaterial.SetFloat("K", K);
			smaaMaterial.SetFloat("_TemporalAccum", temporalAccumulationWeight);
			Graphics.Blit(source, temporary, smaaMaterial, 2);
			if (edgeType == EdgeType.Luminance)
			{
				Graphics.Blit(source, temporary, smaaMaterial, 1);
			}
			else if (edgeType == EdgeType.Color)
			{
				Graphics.Blit(source, temporary, smaaMaterial, 6);
			}
			else
			{
				smaaMaterial.SetFloat("_DepthThreshold", 0.01f * depthThreshold);
				Graphics.Blit(source, temporary, smaaMaterial, 8);
			}
			Graphics.Blit(temporary, temporary2, smaaMaterial, 3);
			if (temporalType == TemporalType.Off)
			{
				Graphics.Blit(source, destination, smaaMaterial, 4);
			}
			else
			{
				RenderTexture temporary3 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
				if (temporalType == TemporalType.SMAA_2x)
				{
					smaaMaterial.SetTexture("accumTex", m_RtAccum);
					if (flag)
					{
						Graphics.Blit(source, temporary3, smaaMaterial, 4);
					}
					else
					{
						Graphics.Blit(source, temporary3, smaaMaterial, 5);
					}
					Graphics.Blit(temporary3, m_RtAccum, smaaMaterial, 0);
					Graphics.Blit(temporary3, destination, smaaMaterial, 0);
				}
				else
				{
					Graphics.Blit(source, temporary3, smaaMaterial, 4);
					if (flag)
					{
						Graphics.Blit(temporary3, m_RtAccum, smaaMaterial, 0);
					}
					smaaMaterial.SetTexture("accumTex", m_RtAccum);
					smaaMaterial.SetTexture("smaaTex", temporary3);
					temporary3.filterMode = FilterMode.Bilinear;
					RenderTexture temporary4 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
					Graphics.Blit(temporary3, temporary4, smaaMaterial, 7);
					Graphics.Blit(temporary4, m_RtAccum, smaaMaterial, 0);
					Graphics.Blit(temporary4, destination, smaaMaterial, 0);
					RenderTexture.ReleaseTemporary(temporary4);
				}
				RenderTexture.ReleaseTemporary(temporary3);
			}
			if (displayType == DebugDisplay.Edges)
			{
				Graphics.Blit(temporary, destination, smaaMaterial, 0);
			}
			else if (displayType == DebugDisplay.Weights)
			{
				Graphics.Blit(temporary2, destination, smaaMaterial, 0);
			}
			else if (displayType == DebugDisplay.Depth)
			{
				Graphics.Blit(null, destination, smaaMaterial, 9);
			}
			else if (displayType == DebugDisplay.Accumulation)
			{
				Graphics.Blit(m_RtAccum, destination);
			}
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
			StorePreviousViewProjMatrix(matrix4x);
		}
	}
}
