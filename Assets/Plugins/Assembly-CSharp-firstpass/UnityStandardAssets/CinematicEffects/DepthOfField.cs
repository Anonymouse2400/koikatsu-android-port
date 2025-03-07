using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Other/DepthOfField")]
	[RequireComponent(typeof(Camera))]
	public class DepthOfField : MonoBehaviour
	{
		[AttributeUsage(AttributeTargets.Field)]
		public sealed class GradientRangeAttribute : PropertyAttribute
		{
			public readonly float max;

			public readonly float min;

			public GradientRangeAttribute(float min, float max)
			{
				this.min = min;
				this.max = max;
			}
		}

		private enum Passes
		{
			BlurAlphaWeighted = 0,
			BoxBlur = 1,
			DilateFgCocFromColor = 2,
			DilateFgCoc = 3,
			CaptureCoc = 4,
			CaptureCocExplicit = 5,
			VisualizeCoc = 6,
			VisualizeCocExplicit = 7,
			CocPrefilter = 8,
			CircleBlur = 9,
			CircleBlurWithDilatedFg = 10,
			CircleBlurLowQuality = 11,
			CircleBlowLowQualityWithDilatedFg = 12,
			Merge = 13,
			MergeExplicit = 14,
			MergeBicubic = 15,
			MergeExplicitBicubic = 16,
			ShapeLowQuality = 17,
			ShapeLowQualityDilateFg = 18,
			ShapeLowQualityMerge = 19,
			ShapeLowQualityMergeDilateFg = 20,
			ShapeMediumQuality = 21,
			ShapeMediumQualityDilateFg = 22,
			ShapeMediumQualityMerge = 23,
			ShapeMediumQualityMergeDilateFg = 24,
			ShapeHighQuality = 25,
			ShapeHighQualityDilateFg = 26,
			ShapeHighQualityMerge = 27,
			ShapeHighQualityMergeDilateFg = 28
		}

		public enum MedianPasses
		{
			Median3 = 0,
			Median3X3 = 1
		}

		public enum BokehTexturesPasses
		{
			Apply = 0,
			Collect = 1
		}

		public enum UIMode
		{
			Basic = 0,
			Advanced = 1,
			Explicit = 2
		}

		public enum ApertureShape
		{
			Circular = 0,
			Hexagonal = 1,
			Octogonal = 2
		}

		public enum FilterQuality
		{
			None = 0,
			Normal = 1,
			High = 2
		}

		private const float kMaxBlur = 35f;

		[Tooltip("Allow to view where the blur will be applied. Yellow for near blur, Blue for far blur.")]
		public bool visualizeBluriness;

		[Tooltip("When enabled quality settings can be hand picked, rather than being driven by the quality slider.")]
		public bool customizeQualitySettings;

		public bool prefilterBlur = true;

		public FilterQuality medianFilter = FilterQuality.High;

		public bool dilateNearBlur = true;

		public bool highQualityUpsampling = true;

		[GradientRange(0f, 100f)]
		[Tooltip("Color represent relative performance. From green (faster) to yellow (slower).")]
		public float quality = 100f;

		[Range(0f, 1f)]
		public float focusPlane = 0.225f;

		[Range(0f, 1f)]
		public float focusRange = 0.9f;

		[Range(0f, 1f)]
		public float nearPlane;

		[Range(0f, 35f)]
		public float nearRadius = 20f;

		[Range(0f, 1f)]
		public float farPlane = 1f;

		[Range(0f, 35f)]
		public float farRadius = 20f;

		[Range(0f, 35f)]
		public float radius = 20f;

		[Range(0.5f, 4f)]
		public float boostPoint = 0.75f;

		[Range(0f, 1f)]
		public float nearBoostAmount;

		[Range(0f, 1f)]
		public float farBoostAmount;

		[Range(0f, 32f)]
		public float fStops = 5f;

		[Range(0.01f, 5f)]
		public float textureBokehScale = 1f;

		[Range(0.01f, 100f)]
		public float textureBokehIntensity = 50f;

		[Range(0.01f, 50f)]
		public float textureBokehThreshold = 2f;

		[Range(0.01f, 1f)]
		public float textureBokehSpawnHeuristic = 0.15f;

		public Transform focusTransform;

		public Texture2D bokehTexture;

		public ApertureShape apertureShape;

		[Range(0f, 179f)]
		public float apertureOrientation;

		[Tooltip("Use with care Bokeh texture are only available on shader model 5, and performance scale with the number of bokehs.")]
		public bool useBokehTexture;

		public UIMode uiMode;

		public Shader filmicDepthOfFieldShader;

		public Shader medianFilterShader;

		public Shader textureBokehShader;

		[NonSerialized]
		private RenderTextureUtility m_RTU = new RenderTextureUtility();

		private ComputeBuffer m_ComputeBufferDrawArgs;

		private ComputeBuffer m_ComputeBufferPoints;

		private Material m_FilmicDepthOfFieldMaterial;

		private Material m_MedianFilterMaterial;

		private Material m_TextureBokehMaterial;

		private float m_LastApertureOrientation;

		private Vector4 m_OctogonalBokehDirection1;

		private Vector4 m_OctogonalBokehDirection2;

		private Vector4 m_OctogonalBokehDirection3;

		private Vector4 m_OctogonalBokehDirection4;

		private Vector4 m_HexagonalBokehDirection1;

		private Vector4 m_HexagonalBokehDirection2;

		private Vector4 m_HexagonalBokehDirection3;

		public Material filmicDepthOfFieldMaterial
		{
			get
			{
				if (m_FilmicDepthOfFieldMaterial == null)
				{
					m_FilmicDepthOfFieldMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(filmicDepthOfFieldShader);
				}
				return m_FilmicDepthOfFieldMaterial;
			}
		}

		public Material medianFilterMaterial
		{
			get
			{
				if (m_MedianFilterMaterial == null)
				{
					m_MedianFilterMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(medianFilterShader);
				}
				return m_MedianFilterMaterial;
			}
		}

		public Material textureBokehMaterial
		{
			get
			{
				if (m_TextureBokehMaterial == null)
				{
					m_TextureBokehMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(textureBokehShader);
				}
				return m_TextureBokehMaterial;
			}
		}

		public ComputeBuffer computeBufferDrawArgs
		{
			get
			{
				if (m_ComputeBufferDrawArgs == null)
				{
					m_ComputeBufferDrawArgs = new ComputeBuffer(1, 16, ComputeBufferType.IndirectArguments);
					int[] data = new int[4] { 0, 1, 0, 0 };
					m_ComputeBufferDrawArgs.SetData(data);
				}
				return m_ComputeBufferDrawArgs;
			}
		}

		public ComputeBuffer computeBufferPoints
		{
			get
			{
				if (m_ComputeBufferPoints == null)
				{
					m_ComputeBufferPoints = new ComputeBuffer(90000, 28, ComputeBufferType.Append);
				}
				return m_ComputeBufferPoints;
			}
		}

		private bool shouldPerformBokeh
		{
			get
			{
				return ImageEffectHelper.supportsDX11 && useBokehTexture && (bool)textureBokehMaterial;
			}
		}

		protected void OnEnable()
		{
			if (filmicDepthOfFieldShader == null)
			{
				filmicDepthOfFieldShader = Shader.Find("Hidden/DepthOfField/DepthOfField");
			}
			if (medianFilterShader == null)
			{
				medianFilterShader = Shader.Find("Hidden/DepthOfField/MedianFilter");
			}
			if (textureBokehShader == null)
			{
				textureBokehShader = Shader.Find("Hidden/DepthOfField/BokehSplatting");
			}
			if (!ImageEffectHelper.IsSupported(filmicDepthOfFieldShader, true, true, this) || !ImageEffectHelper.IsSupported(medianFilterShader, true, true, this))
			{
				base.enabled = false;
				Debug.LogWarning("The image effect " + ToString() + " has been disabled as it's not supported on the current platform.");
			}
			else if (ImageEffectHelper.supportsDX11 && !ImageEffectHelper.IsSupported(textureBokehShader, true, true, this))
			{
				base.enabled = false;
				Debug.LogWarning("The image effect " + ToString() + " has been disabled as it's not supported on the current platform.");
			}
			else
			{
				ComputeBlurDirections(true);
				GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
			}
		}

		protected void OnDisable()
		{
			ReleaseComputeResources();
			if ((bool)m_FilmicDepthOfFieldMaterial)
			{
				UnityEngine.Object.DestroyImmediate(m_FilmicDepthOfFieldMaterial);
			}
			if ((bool)m_TextureBokehMaterial)
			{
				UnityEngine.Object.DestroyImmediate(m_TextureBokehMaterial);
			}
			if ((bool)m_MedianFilterMaterial)
			{
				UnityEngine.Object.DestroyImmediate(m_MedianFilterMaterial);
			}
			m_TextureBokehMaterial = null;
			m_FilmicDepthOfFieldMaterial = null;
			m_MedianFilterMaterial = null;
			m_RTU.ReleaseAllTemporaryRenderTextures();
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (medianFilterMaterial == null || filmicDepthOfFieldMaterial == null)
			{
				Graphics.Blit(source, destination);
				return;
			}
			if (visualizeBluriness)
			{
				Vector4 blurParams;
				Vector4 blurCoe;
				ComputeCocParameters(out blurParams, out blurCoe);
				filmicDepthOfFieldMaterial.SetVector("_BlurParams", blurParams);
				filmicDepthOfFieldMaterial.SetVector("_BlurCoe", blurCoe);
				Graphics.Blit(null, destination, filmicDepthOfFieldMaterial, (uiMode != UIMode.Explicit) ? 6 : 7);
			}
			else
			{
				DoDepthOfField(source, destination);
			}
			m_RTU.ReleaseAllTemporaryRenderTextures();
		}

		private void DoDepthOfField(RenderTexture source, RenderTexture destination)
		{
			float num = (float)source.height / 720f;
			float num2 = num;
			float num3 = Mathf.Max(nearRadius, farRadius) * num2 * 0.75f;
			float num4 = nearRadius * num;
			float num5 = farRadius * num;
			float num6 = Mathf.Max(num4, num5);
			switch (apertureShape)
			{
			case ApertureShape.Hexagonal:
				num6 *= 1.2f;
				break;
			case ApertureShape.Octogonal:
				num6 *= 1.15f;
				break;
			}
			if (num6 < 0.5f)
			{
				Graphics.Blit(source, destination);
				return;
			}
			int width = source.width / 2;
			int height = source.height / 2;
			Vector4 value = new Vector4(num4 * 0.5f, num5 * 0.5f, 0f, 0f);
			RenderTexture temporaryRenderTexture = m_RTU.GetTemporaryRenderTexture(width, height);
			RenderTexture temporaryRenderTexture2 = m_RTU.GetTemporaryRenderTexture(width, height);
			Vector4 blurParams;
			Vector4 blurCoe;
			ComputeCocParameters(out blurParams, out blurCoe);
			filmicDepthOfFieldMaterial.SetVector("_BlurParams", blurParams);
			filmicDepthOfFieldMaterial.SetVector("_BlurCoe", blurCoe);
			filmicDepthOfFieldMaterial.SetVector("_BoostParams", new Vector4(num4 * nearBoostAmount * -0.5f, num5 * farBoostAmount * 0.5f, boostPoint, 0f));
			Graphics.Blit(source, temporaryRenderTexture2, filmicDepthOfFieldMaterial, (uiMode != UIMode.Explicit) ? 4 : 5);
			RenderTexture src = temporaryRenderTexture2;
			RenderTexture dst = temporaryRenderTexture;
			if (shouldPerformBokeh)
			{
				RenderTexture temporaryRenderTexture3 = m_RTU.GetTemporaryRenderTexture(width, height);
				Graphics.Blit(src, temporaryRenderTexture3, filmicDepthOfFieldMaterial, 1);
				filmicDepthOfFieldMaterial.SetVector("_Offsets", new Vector4(0f, 1.5f, 0f, 1.5f));
				Graphics.Blit(temporaryRenderTexture3, dst, filmicDepthOfFieldMaterial, 0);
				filmicDepthOfFieldMaterial.SetVector("_Offsets", new Vector4(1.5f, 0f, 0f, 1.5f));
				Graphics.Blit(dst, temporaryRenderTexture3, filmicDepthOfFieldMaterial, 0);
				textureBokehMaterial.SetTexture("_BlurredColor", temporaryRenderTexture3);
				textureBokehMaterial.SetFloat("_SpawnHeuristic", textureBokehSpawnHeuristic);
				textureBokehMaterial.SetVector("_BokehParams", new Vector4(textureBokehScale * num2, textureBokehIntensity, textureBokehThreshold, num3));
				Graphics.SetRandomWriteTarget(1, computeBufferPoints);
				Graphics.Blit(src, dst, textureBokehMaterial, 1);
				Graphics.ClearRandomWriteTargets();
				SwapRenderTexture(ref src, ref dst);
				m_RTU.ReleaseTemporaryRenderTexture(temporaryRenderTexture3);
			}
			filmicDepthOfFieldMaterial.SetVector("_BlurParams", blurParams);
			filmicDepthOfFieldMaterial.SetVector("_BlurCoe", value);
			filmicDepthOfFieldMaterial.SetVector("_BoostParams", new Vector4(num4 * nearBoostAmount * -0.5f, num5 * farBoostAmount * 0.5f, boostPoint, 0f));
			RenderTexture renderTexture = null;
			if (dilateNearBlur)
			{
				RenderTexture temporaryRenderTexture4 = m_RTU.GetTemporaryRenderTexture(width, height, 0, RenderTextureFormat.RGHalf);
				renderTexture = m_RTU.GetTemporaryRenderTexture(width, height, 0, RenderTextureFormat.RGHalf);
				filmicDepthOfFieldMaterial.SetVector("_Offsets", new Vector4(0f, num4 * 0.75f, 0f, 0f));
				Graphics.Blit(src, temporaryRenderTexture4, filmicDepthOfFieldMaterial, 2);
				filmicDepthOfFieldMaterial.SetVector("_Offsets", new Vector4(num4 * 0.75f, 0f, 0f, 0f));
				Graphics.Blit(temporaryRenderTexture4, renderTexture, filmicDepthOfFieldMaterial, 3);
				m_RTU.ReleaseTemporaryRenderTexture(temporaryRenderTexture4);
			}
			if (prefilterBlur)
			{
				Graphics.Blit(src, dst, filmicDepthOfFieldMaterial, 8);
				SwapRenderTexture(ref src, ref dst);
			}
			switch (apertureShape)
			{
			case ApertureShape.Circular:
				DoCircularBlur(renderTexture, ref src, ref dst, num6);
				break;
			case ApertureShape.Hexagonal:
				DoHexagonalBlur(renderTexture, ref src, ref dst, num6);
				break;
			case ApertureShape.Octogonal:
				DoOctogonalBlur(renderTexture, ref src, ref dst, num6);
				break;
			}
			switch (medianFilter)
			{
			case FilterQuality.Normal:
				medianFilterMaterial.SetVector("_Offsets", new Vector4(1f, 0f, 0f, 0f));
				Graphics.Blit(src, dst, medianFilterMaterial, 0);
				SwapRenderTexture(ref src, ref dst);
				medianFilterMaterial.SetVector("_Offsets", new Vector4(0f, 1f, 0f, 0f));
				Graphics.Blit(src, dst, medianFilterMaterial, 0);
				SwapRenderTexture(ref src, ref dst);
				break;
			case FilterQuality.High:
				Graphics.Blit(src, dst, medianFilterMaterial, 1);
				SwapRenderTexture(ref src, ref dst);
				break;
			}
			filmicDepthOfFieldMaterial.SetVector("_BlurCoe", value);
			filmicDepthOfFieldMaterial.SetVector("_Convolved_TexelSize", new Vector4(src.width, src.height, 1f / (float)src.width, 1f / (float)src.height));
			filmicDepthOfFieldMaterial.SetTexture("_SecondTex", src);
			int pass = ((uiMode != UIMode.Explicit) ? 13 : 14);
			if (highQualityUpsampling)
			{
				pass = ((uiMode != UIMode.Explicit) ? 15 : 16);
			}
			if (shouldPerformBokeh)
			{
				RenderTexture temporaryRenderTexture5 = m_RTU.GetTemporaryRenderTexture(source.height, source.width, 0, source.format);
				Graphics.Blit(source, temporaryRenderTexture5, filmicDepthOfFieldMaterial, pass);
				Graphics.SetRenderTarget(temporaryRenderTexture5);
				ComputeBuffer.CopyCount(computeBufferPoints, computeBufferDrawArgs, 0);
				textureBokehMaterial.SetBuffer("pointBuffer", computeBufferPoints);
				textureBokehMaterial.SetTexture("_MainTex", bokehTexture);
				textureBokehMaterial.SetVector("_Screen", new Vector3(1f / (1f * (float)source.width), 1f / (1f * (float)source.height), num3));
				textureBokehMaterial.SetPass(0);
				Graphics.DrawProceduralIndirect(MeshTopology.Points, computeBufferDrawArgs, 0);
				Graphics.Blit(temporaryRenderTexture5, destination);
			}
			else
			{
				Graphics.Blit(source, destination, filmicDepthOfFieldMaterial, pass);
			}
		}

		private void DoHexagonalBlur(RenderTexture blurredFgCoc, ref RenderTexture src, ref RenderTexture dst, float maxRadius)
		{
			ComputeBlurDirections(false);
			int blurPass;
			int blurAndMergePass;
			GetDirectionalBlurPassesFromRadius(blurredFgCoc, maxRadius, out blurPass, out blurAndMergePass);
			filmicDepthOfFieldMaterial.SetTexture("_SecondTex", blurredFgCoc);
			RenderTexture temporaryRenderTexture = m_RTU.GetTemporaryRenderTexture(src.width, src.height, 0, src.format);
			filmicDepthOfFieldMaterial.SetVector("_Offsets", m_HexagonalBokehDirection1);
			Graphics.Blit(src, temporaryRenderTexture, filmicDepthOfFieldMaterial, blurPass);
			filmicDepthOfFieldMaterial.SetVector("_Offsets", m_HexagonalBokehDirection2);
			Graphics.Blit(temporaryRenderTexture, src, filmicDepthOfFieldMaterial, blurPass);
			filmicDepthOfFieldMaterial.SetVector("_Offsets", m_HexagonalBokehDirection3);
			filmicDepthOfFieldMaterial.SetTexture("_ThirdTex", src);
			Graphics.Blit(temporaryRenderTexture, dst, filmicDepthOfFieldMaterial, blurAndMergePass);
			m_RTU.ReleaseTemporaryRenderTexture(temporaryRenderTexture);
			SwapRenderTexture(ref src, ref dst);
		}

		private void DoOctogonalBlur(RenderTexture blurredFgCoc, ref RenderTexture src, ref RenderTexture dst, float maxRadius)
		{
			ComputeBlurDirections(false);
			int blurPass;
			int blurAndMergePass;
			GetDirectionalBlurPassesFromRadius(blurredFgCoc, maxRadius, out blurPass, out blurAndMergePass);
			filmicDepthOfFieldMaterial.SetTexture("_SecondTex", blurredFgCoc);
			RenderTexture temporaryRenderTexture = m_RTU.GetTemporaryRenderTexture(src.width, src.height, 0, src.format);
			filmicDepthOfFieldMaterial.SetVector("_Offsets", m_OctogonalBokehDirection1);
			Graphics.Blit(src, temporaryRenderTexture, filmicDepthOfFieldMaterial, blurPass);
			filmicDepthOfFieldMaterial.SetVector("_Offsets", m_OctogonalBokehDirection2);
			Graphics.Blit(temporaryRenderTexture, dst, filmicDepthOfFieldMaterial, blurPass);
			filmicDepthOfFieldMaterial.SetVector("_Offsets", m_OctogonalBokehDirection3);
			Graphics.Blit(src, temporaryRenderTexture, filmicDepthOfFieldMaterial, blurPass);
			filmicDepthOfFieldMaterial.SetVector("_Offsets", m_OctogonalBokehDirection4);
			filmicDepthOfFieldMaterial.SetTexture("_ThirdTex", dst);
			Graphics.Blit(temporaryRenderTexture, src, filmicDepthOfFieldMaterial, blurAndMergePass);
			m_RTU.ReleaseTemporaryRenderTexture(temporaryRenderTexture);
		}

		private void DoCircularBlur(RenderTexture blurredFgCoc, ref RenderTexture src, ref RenderTexture dst, float maxRadius)
		{
			int pass;
			if (blurredFgCoc != null)
			{
				filmicDepthOfFieldMaterial.SetTexture("_SecondTex", blurredFgCoc);
				pass = ((!(maxRadius > 10f)) ? 12 : 10);
			}
			else
			{
				pass = ((!(maxRadius > 10f)) ? 11 : 9);
			}
			Graphics.Blit(src, dst, filmicDepthOfFieldMaterial, pass);
			SwapRenderTexture(ref src, ref dst);
		}

		private void ComputeCocParameters(out Vector4 blurParams, out Vector4 blurCoe)
		{
			Camera component = GetComponent<Camera>();
			float num = ((!focusTransform) ? (focusPlane * focusPlane * focusPlane * focusPlane) : (component.WorldToViewportPoint(focusTransform.position).z / component.farClipPlane));
			if (uiMode == UIMode.Basic || uiMode == UIMode.Advanced)
			{
				float w = focusRange * focusRange * focusRange * focusRange;
				float num2 = 4f / Mathf.Tan(0.5f * component.fieldOfView * ((float)Math.PI / 180f));
				float x = num2 / fStops;
				blurCoe = new Vector4(0f, 0f, 1f, 1f);
				blurParams = new Vector4(x, num2, num, w);
				return;
			}
			float num3 = nearPlane * nearPlane * nearPlane * nearPlane;
			float num4 = farPlane * farPlane * farPlane * farPlane;
			float num5 = focusRange * focusRange * focusRange * focusRange;
			float num6 = num5;
			if (num <= num3)
			{
				num = num3 + 1E-07f;
			}
			if (num >= num4)
			{
				num = num4 - 1E-07f;
			}
			if (num - num5 <= num3)
			{
				num5 = num - num3 - 1E-07f;
			}
			if (num + num6 >= num4)
			{
				num6 = num4 - num - 1E-07f;
			}
			float num7 = 1f / (num3 - num + num5);
			float num8 = 1f / (num4 - num - num6);
			float num9 = 1f - num7 * num3;
			float num10 = 1f - num8 * num4;
			blurParams = new Vector4(-1f * num7, -1f * num9, 1f * num8, 1f * num10);
			blurCoe = new Vector4(0f, 0f, (num10 - num9) / (num7 - num8), 0f);
		}

		private void ReleaseComputeResources()
		{
			if (m_ComputeBufferDrawArgs != null)
			{
				m_ComputeBufferDrawArgs.Release();
			}
			m_ComputeBufferDrawArgs = null;
			if (m_ComputeBufferPoints != null)
			{
				m_ComputeBufferPoints.Release();
			}
			m_ComputeBufferPoints = null;
		}

		private void ComputeBlurDirections(bool force)
		{
			if (force || !(Math.Abs(m_LastApertureOrientation - apertureOrientation) < float.Epsilon))
			{
				m_LastApertureOrientation = apertureOrientation;
				float num = apertureOrientation * ((float)Math.PI / 180f);
				float cosinus = Mathf.Cos(num);
				float sinus = Mathf.Sin(num);
				m_OctogonalBokehDirection1 = new Vector4(0.5f, 0f, 0f, 0f);
				m_OctogonalBokehDirection2 = new Vector4(0f, 0.5f, 1f, 0f);
				m_OctogonalBokehDirection3 = new Vector4(-0.353553f, 0.353553f, 1f, 0f);
				m_OctogonalBokehDirection4 = new Vector4(0.353553f, 0.353553f, 1f, 0f);
				m_HexagonalBokehDirection1 = new Vector4(0.5f, 0f, 0f, 0f);
				m_HexagonalBokehDirection2 = new Vector4(0.25f, 0.433013f, 1f, 0f);
				m_HexagonalBokehDirection3 = new Vector4(0.25f, -0.433013f, 1f, 0f);
				if (num > float.Epsilon)
				{
					Rotate2D(ref m_OctogonalBokehDirection1, cosinus, sinus);
					Rotate2D(ref m_OctogonalBokehDirection2, cosinus, sinus);
					Rotate2D(ref m_OctogonalBokehDirection3, cosinus, sinus);
					Rotate2D(ref m_OctogonalBokehDirection4, cosinus, sinus);
					Rotate2D(ref m_HexagonalBokehDirection1, cosinus, sinus);
					Rotate2D(ref m_HexagonalBokehDirection2, cosinus, sinus);
					Rotate2D(ref m_HexagonalBokehDirection3, cosinus, sinus);
				}
			}
		}

		private static void Rotate2D(ref Vector4 direction, float cosinus, float sinus)
		{
			Vector4 vector = direction;
			direction.x = vector.x * cosinus - vector.y * sinus;
			direction.y = vector.x * sinus + vector.y * cosinus;
		}

		private static void SwapRenderTexture(ref RenderTexture src, ref RenderTexture dst)
		{
			RenderTexture renderTexture = dst;
			dst = src;
			src = renderTexture;
		}

		private static void GetDirectionalBlurPassesFromRadius(RenderTexture blurredFgCoc, float maxRadius, out int blurPass, out int blurAndMergePass)
		{
			if (blurredFgCoc == null)
			{
				if (maxRadius > 10f)
				{
					blurPass = 25;
					blurAndMergePass = 27;
				}
				else if (maxRadius > 5f)
				{
					blurPass = 21;
					blurAndMergePass = 23;
				}
				else
				{
					blurPass = 17;
					blurAndMergePass = 19;
				}
			}
			else if (maxRadius > 10f)
			{
				blurPass = 26;
				blurAndMergePass = 28;
			}
			else if (maxRadius > 5f)
			{
				blurPass = 22;
				blurAndMergePass = 24;
			}
			else
			{
				blurPass = 18;
				blurAndMergePass = 20;
			}
		}
	}
}
