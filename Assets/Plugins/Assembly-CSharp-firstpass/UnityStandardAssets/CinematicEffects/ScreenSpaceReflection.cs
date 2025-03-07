using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Rendering/Screen Space Reflection")]
	public class ScreenSpaceReflection : MonoBehaviour
	{
		public enum SSRDebugMode
		{
			None = 0,
			IncomingRadiance = 1,
			SSRResult = 2,
			FinalGlossyTerm = 3,
			SSRMask = 4,
			Roughness = 5,
			BaseColor = 6,
			SpecColor = 7,
			Reflectivity = 8,
			ReflectionProbeOnly = 9,
			ReflectionProbeMinusSSR = 10,
			SSRMinusReflectionProbe = 11,
			NoGlossy = 12,
			NegativeNoGlossy = 13,
			MipLevel = 14
		}

		public enum SSRResolution
		{
			FullResolution = 0,
			HalfTraceFullResolve = 1,
			HalfResolution = 2
		}

		[Serializable]
		public struct SSRSettings
		{
			[AttributeUsage(AttributeTargets.Field)]
			public class LayoutAttribute : PropertyAttribute
			{
			}

			[Layout]
			public BasicSettings basicSettings;

			[Layout]
			public ReflectionSettings reflectionSettings;

			[Layout]
			public AdvancedSettings advancedSettings;

			[Layout]
			public DebugSettings debugSettings;

			private static readonly SSRSettings s_Performance = new SSRSettings
			{
				basicSettings = new BasicSettings
				{
					screenEdgeFading = 0f,
					maxDistance = 10f,
					fadeDistance = 10f,
					reflectionMultiplier = 1f,
					enableHDR = false,
					additiveReflection = false
				},
				reflectionSettings = new ReflectionSettings
				{
					maxSteps = 64,
					rayStepSize = 4,
					widthModifier = 0.5f,
					smoothFallbackThreshold = 0.4f,
					distanceBlur = 1f,
					fresnelFade = 0.2f,
					fresnelFadePower = 2f,
					smoothFallbackDistance = 0.05f
				},
				advancedSettings = new AdvancedSettings
				{
					useTemporalConfidence = false,
					temporalFilterStrength = 0f,
					treatBackfaceHitAsMiss = false,
					allowBackwardsRays = false,
					traceBehindObjects = true,
					highQualitySharpReflections = false,
					traceEverywhere = false,
					resolution = SSRResolution.HalfResolution,
					bilateralUpsample = false,
					improveCorners = false,
					reduceBanding = false,
					highlightSuppression = false
				},
				debugSettings = new DebugSettings
				{
					debugMode = SSRDebugMode.None
				}
			};

			private static readonly SSRSettings s_Default = new SSRSettings
			{
				basicSettings = new BasicSettings
				{
					screenEdgeFading = 0.03f,
					maxDistance = 100f,
					fadeDistance = 100f,
					reflectionMultiplier = 1f,
					enableHDR = true,
					additiveReflection = false
				},
				reflectionSettings = new ReflectionSettings
				{
					maxSteps = 128,
					rayStepSize = 3,
					widthModifier = 0.5f,
					smoothFallbackThreshold = 0.2f,
					distanceBlur = 1f,
					fresnelFade = 0.2f,
					fresnelFadePower = 2f,
					smoothFallbackDistance = 0.05f
				},
				advancedSettings = new AdvancedSettings
				{
					useTemporalConfidence = true,
					temporalFilterStrength = 0.7f,
					treatBackfaceHitAsMiss = false,
					allowBackwardsRays = false,
					traceBehindObjects = true,
					highQualitySharpReflections = true,
					traceEverywhere = true,
					resolution = SSRResolution.HalfTraceFullResolve,
					bilateralUpsample = true,
					improveCorners = true,
					reduceBanding = true,
					highlightSuppression = false
				},
				debugSettings = new DebugSettings
				{
					debugMode = SSRDebugMode.None
				}
			};

			private static readonly SSRSettings s_HighQuality = new SSRSettings
			{
				basicSettings = new BasicSettings
				{
					screenEdgeFading = 0.03f,
					maxDistance = 100f,
					fadeDistance = 100f,
					reflectionMultiplier = 1f,
					enableHDR = true,
					additiveReflection = false
				},
				reflectionSettings = new ReflectionSettings
				{
					maxSteps = 512,
					rayStepSize = 1,
					widthModifier = 0.5f,
					smoothFallbackThreshold = 0.2f,
					distanceBlur = 1f,
					fresnelFade = 0.2f,
					fresnelFadePower = 2f,
					smoothFallbackDistance = 0.05f
				},
				advancedSettings = new AdvancedSettings
				{
					useTemporalConfidence = true,
					temporalFilterStrength = 0.7f,
					treatBackfaceHitAsMiss = false,
					allowBackwardsRays = false,
					traceBehindObjects = true,
					highQualitySharpReflections = true,
					traceEverywhere = true,
					resolution = SSRResolution.HalfTraceFullResolve,
					bilateralUpsample = true,
					improveCorners = true,
					reduceBanding = true,
					highlightSuppression = false
				},
				debugSettings = new DebugSettings
				{
					debugMode = SSRDebugMode.None
				}
			};

			public static SSRSettings performanceSettings
			{
				get
				{
					return s_Performance;
				}
			}

			public static SSRSettings defaultSettings
			{
				get
				{
					return s_Default;
				}
			}

			public static SSRSettings highQualitySettings
			{
				get
				{
					return s_HighQuality;
				}
			}
		}

		[Serializable]
		public struct BasicSettings
		{
			[Tooltip("Nonphysical multiplier for the SSR reflections. 1.0 is physically based.")]
			[Range(0f, 2f)]
			public float reflectionMultiplier;

			[Tooltip("Maximum reflection distance in world units.")]
			[Range(0.5f, 1000f)]
			public float maxDistance;

			[Tooltip("How far away from the maxDistance to begin fading SSR.")]
			[Range(0f, 1000f)]
			public float fadeDistance;

			[Tooltip("Higher = fade out SSRR near the edge of the screen so that reflections don't pop under camera motion.")]
			[Range(0f, 1f)]
			public float screenEdgeFading;

			[Tooltip("Enable for better reflections of very bright objects at a performance cost")]
			public bool enableHDR;

			[Tooltip("Add reflections on top of existing ones. Not physically correct.")]
			public bool additiveReflection;
		}

		[Serializable]
		public struct ReflectionSettings
		{
			[Tooltip("Max raytracing length.")]
			[Range(16f, 2048f)]
			public int maxSteps;

			[Tooltip("Log base 2 of ray tracing coarse step size. Higher traces farther, lower gives better quality silhouettes.")]
			[Range(0f, 4f)]
			public int rayStepSize;

			[Tooltip("Typical thickness of columns, walls, furniture, and other objects that reflection rays might pass behind.")]
			[Range(0.01f, 10f)]
			public float widthModifier;

			[Tooltip("Increase if reflections flicker on very rough surfaces.")]
			[Range(0f, 1f)]
			public float smoothFallbackThreshold;

			[Tooltip("Start falling back to non-SSR value solution at smoothFallbackThreshold - smoothFallbackDistance, with full fallback occuring at smoothFallbackThreshold.")]
			[Range(0f, 0.2f)]
			public float smoothFallbackDistance;

			[Tooltip("Amplify Fresnel fade out. Increase if floor reflections look good close to the surface and bad farther 'under' the floor.")]
			[Range(0f, 1f)]
			public float fresnelFade;

			[Tooltip("Higher values correspond to a faster Fresnel fade as the reflection changes from the grazing angle.")]
			[Range(0.1f, 10f)]
			public float fresnelFadePower;

			[Tooltip("Controls how blurry reflections get as objects are further from the camera. 0 is constant blur no matter trace distance or distance from camera. 1 fully takes into account both factors.")]
			[Range(0f, 1f)]
			public float distanceBlur;
		}

		[Serializable]
		public struct AdvancedSettings
		{
			[Range(0f, 0.99f)]
			[Tooltip("Increase to decrease flicker in scenes; decrease to prevent ghosting (especially in dynamic scenes). 0 gives maximum performance.")]
			public float temporalFilterStrength;

			[Tooltip("Enable to limit ghosting from applying the temporal filter.")]
			public bool useTemporalConfidence;

			[Tooltip("Enable to allow rays to pass behind objects. This can lead to more screen-space reflections, but the reflections are more likely to be wrong.")]
			public bool traceBehindObjects;

			[Tooltip("Enable to increase quality of the sharpest reflections (through filtering), at a performance cost.")]
			public bool highQualitySharpReflections;

			[Tooltip("Improves quality in scenes with varying smoothness, at a potential performance cost.")]
			public bool traceEverywhere;

			[Tooltip("Enable to force more surfaces to use reflection probes if you see streaks on the sides of objects or bad reflections of their backs.")]
			public bool treatBackfaceHitAsMiss;

			[Tooltip("Enable for a performance gain in scenes where most glossy objects are horizontal, like floors, water, and tables. Leave on for scenes with glossy vertical objects.")]
			public bool allowBackwardsRays;

			[Tooltip("Improve visual fidelity of reflections on rough surfaces near corners in the scene, at the cost of a small amount of performance.")]
			public bool improveCorners;

			[Tooltip("Half resolution SSRR is much faster, but less accurate. Quality can be reclaimed for some performance by doing the resolve at full resolution.")]
			public SSRResolution resolution;

			[Tooltip("Drastically improves reflection reconstruction quality at the expense of some performance.")]
			public bool bilateralUpsample;

			[Tooltip("Improve visual fidelity of mirror reflections at the cost of a small amount of performance.")]
			public bool reduceBanding;

			[Tooltip("Enable to limit the effect a few bright pixels can have on rougher surfaces")]
			public bool highlightSuppression;
		}

		[Serializable]
		public struct DebugSettings
		{
			[Tooltip("Various Debug Visualizations")]
			public SSRDebugMode debugMode;
		}

		private enum PassIndex
		{
			RayTraceStep1 = 0,
			RayTraceStep2 = 1,
			RayTraceStep4 = 2,
			RayTraceStep8 = 3,
			RayTraceStep16 = 4,
			CompositeFinal = 5,
			Blur = 6,
			CompositeSSR = 7,
			Blit = 8,
			EdgeGeneration = 9,
			MinMipGeneration = 10,
			HitPointToReflections = 11,
			BilateralKeyPack = 12,
			BlitDepthAsCSZ = 13,
			TemporalFilter = 14,
			AverageRayDistanceGeneration = 15,
			PoissonBlur = 16
		}

		[SerializeField]
		public SSRSettings settings = SSRSettings.defaultSettings;

		[Tooltip("Enable to try and bypass expensive bilateral upsampling away from edges. There is a slight performance hit for generating the edge buffers, but a potentially high performance savings from bypassing bilateral upsampling where it is unneeded. Test on your target platforms to see if performance improves.")]
		private bool useEdgeDetector;

		[Range(-4f, 4f)]
		private float mipBias;

		private bool useOcclusion = true;

		private bool fullResolutionFiltering;

		private bool fallbackToSky;

		private bool computeAverageRayDistance;

		private bool m_HasInformationFromPreviousFrame;

		private Matrix4x4 m_PreviousWorldToCameraMatrix;

		private RenderTexture m_PreviousDepthBuffer;

		private RenderTexture m_PreviousHitBuffer;

		private RenderTexture m_PreviousReflectionBuffer;

		public Shader ssrShader;

		private Material m_SSRMaterial;

		[NonSerialized]
		private RenderTextureUtility m_RTU = new RenderTextureUtility();

		public Material ssrMaterial
		{
			get
			{
				if (m_SSRMaterial == null)
				{
					m_SSRMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(ssrShader);
				}
				return m_SSRMaterial;
			}
		}

		protected void OnEnable()
		{
			if (ssrShader == null)
			{
				ssrShader = Shader.Find("Hidden/ScreenSpaceReflection");
			}
			if (!ImageEffectHelper.IsSupported(ssrShader, true, true, this))
			{
				base.enabled = false;
				Debug.LogWarning("The image effect " + ToString() + " has been disabled as it's not supported on the current platform.");
			}
			else
			{
				GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
			}
		}

		private void OnDisable()
		{
			if ((bool)m_SSRMaterial)
			{
				UnityEngine.Object.DestroyImmediate(m_SSRMaterial);
			}
			if ((bool)m_PreviousDepthBuffer)
			{
				UnityEngine.Object.DestroyImmediate(m_PreviousDepthBuffer);
			}
			if ((bool)m_PreviousHitBuffer)
			{
				UnityEngine.Object.DestroyImmediate(m_PreviousHitBuffer);
			}
			if ((bool)m_PreviousReflectionBuffer)
			{
				UnityEngine.Object.DestroyImmediate(m_PreviousReflectionBuffer);
			}
			m_SSRMaterial = null;
			m_PreviousDepthBuffer = null;
			m_PreviousHitBuffer = null;
			m_PreviousReflectionBuffer = null;
		}

		private void PreparePreviousBuffers(int w, int h)
		{
			if (m_PreviousDepthBuffer != null && (m_PreviousDepthBuffer.width != w || m_PreviousDepthBuffer.height != h))
			{
				UnityEngine.Object.DestroyImmediate(m_PreviousDepthBuffer);
				UnityEngine.Object.DestroyImmediate(m_PreviousHitBuffer);
				UnityEngine.Object.DestroyImmediate(m_PreviousReflectionBuffer);
				m_PreviousDepthBuffer = null;
				m_PreviousHitBuffer = null;
				m_PreviousReflectionBuffer = null;
			}
			if (m_PreviousDepthBuffer == null)
			{
				m_PreviousDepthBuffer = new RenderTexture(w, h, 0, RenderTextureFormat.RFloat);
				m_PreviousHitBuffer = new RenderTexture(w, h, 0, RenderTextureFormat.ARGBHalf);
				m_PreviousReflectionBuffer = new RenderTexture(w, h, 0, RenderTextureFormat.ARGBHalf);
			}
		}

		[ImageEffectOpaque]
		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (ssrMaterial == null)
			{
				Graphics.Blit(source, destination);
				return;
			}
			if (m_HasInformationFromPreviousFrame)
			{
				m_HasInformationFromPreviousFrame = m_PreviousDepthBuffer != null && source.width == m_PreviousDepthBuffer.width && source.height == m_PreviousDepthBuffer.height;
			}
			bool flag = m_HasInformationFromPreviousFrame && (double)settings.advancedSettings.temporalFilterStrength > 0.0;
			m_HasInformationFromPreviousFrame = false;
			if (Camera.current.actualRenderingPath != RenderingPath.DeferredShading)
			{
				Graphics.Blit(source, destination);
				return;
			}
			int width = source.width;
			int height = source.height;
			RenderTexture temporaryRenderTexture = m_RTU.GetTemporaryRenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
			temporaryRenderTexture.filterMode = FilterMode.Point;
			Graphics.Blit(source, temporaryRenderTexture, ssrMaterial, 12);
			ssrMaterial.SetTexture("_NormalAndRoughnessTexture", temporaryRenderTexture);
			float num = source.width;
			float num2 = source.height;
			Vector2 vector = new Vector2(num / (float)width, num2 / (float)height);
			int num3 = ((settings.advancedSettings.resolution == SSRResolution.FullResolution) ? 1 : 2);
			width /= num3;
			height /= num3;
			ssrMaterial.SetVector("_SourceToTempUV", new Vector4(vector.x, vector.y, 1f / vector.x, 1f / vector.y));
			Matrix4x4 projectionMatrix = GetComponent<Camera>().projectionMatrix;
			Vector4 value = new Vector4(-2f / (num * projectionMatrix[0]), -2f / (num2 * projectionMatrix[5]), (1f - projectionMatrix[2]) / projectionMatrix[0], (1f + projectionMatrix[6]) / projectionMatrix[5]);
			float value2 = num / (-2f * (float)Math.Tan((double)GetComponent<Camera>().fieldOfView / 180.0 * Math.PI * 0.5));
			ssrMaterial.SetFloat("_PixelsPerMeterAtOneMeter", value2);
			float num4 = num / 2f;
			float num5 = num2 / 2f;
			Matrix4x4 matrix4x = default(Matrix4x4);
			matrix4x.SetRow(0, new Vector4(num4, 0f, 0f, num4));
			matrix4x.SetRow(1, new Vector4(0f, num5, 0f, num5));
			matrix4x.SetRow(2, new Vector4(0f, 0f, 1f, 0f));
			matrix4x.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
			Matrix4x4 value3 = matrix4x * projectionMatrix;
			ssrMaterial.SetVector("_ScreenSize", new Vector2(num, num2));
			ssrMaterial.SetVector("_ReflectionBufferSize", new Vector2(width, height));
			Vector2 vector2 = new Vector2((float)(1.0 / (double)num), (float)(1.0 / (double)num2));
			Matrix4x4 worldToCameraMatrix = GetComponent<Camera>().worldToCameraMatrix;
			Matrix4x4 inverse = GetComponent<Camera>().worldToCameraMatrix.inverse;
			ssrMaterial.SetVector("_InvScreenSize", vector2);
			ssrMaterial.SetVector("_ProjInfo", value);
			ssrMaterial.SetMatrix("_ProjectToPixelMatrix", value3);
			ssrMaterial.SetMatrix("_WorldToCameraMatrix", worldToCameraMatrix);
			ssrMaterial.SetMatrix("_CameraToWorldMatrix", inverse);
			ssrMaterial.SetInt("_EnableRefine", settings.advancedSettings.reduceBanding ? 1 : 0);
			ssrMaterial.SetInt("_AdditiveReflection", settings.basicSettings.additiveReflection ? 1 : 0);
			ssrMaterial.SetInt("_ImproveCorners", settings.advancedSettings.improveCorners ? 1 : 0);
			ssrMaterial.SetFloat("_ScreenEdgeFading", settings.basicSettings.screenEdgeFading);
			ssrMaterial.SetFloat("_MipBias", mipBias);
			ssrMaterial.SetInt("_UseOcclusion", useOcclusion ? 1 : 0);
			ssrMaterial.SetInt("_BilateralUpsampling", settings.advancedSettings.bilateralUpsample ? 1 : 0);
			ssrMaterial.SetInt("_FallbackToSky", fallbackToSky ? 1 : 0);
			ssrMaterial.SetInt("_TreatBackfaceHitAsMiss", settings.advancedSettings.treatBackfaceHitAsMiss ? 1 : 0);
			ssrMaterial.SetInt("_AllowBackwardsRays", settings.advancedSettings.allowBackwardsRays ? 1 : 0);
			ssrMaterial.SetInt("_TraceEverywhere", settings.advancedSettings.traceEverywhere ? 1 : 0);
			float farClipPlane = GetComponent<Camera>().farClipPlane;
			float nearClipPlane = GetComponent<Camera>().nearClipPlane;
			Vector3 vector3 = ((!float.IsPositiveInfinity(farClipPlane)) ? new Vector3(nearClipPlane * farClipPlane, nearClipPlane - farClipPlane, farClipPlane) : new Vector3(nearClipPlane, -1f, 1f));
			ssrMaterial.SetVector("_CameraClipInfo", vector3);
			ssrMaterial.SetFloat("_MaxRayTraceDistance", settings.basicSettings.maxDistance);
			ssrMaterial.SetFloat("_FadeDistance", settings.basicSettings.fadeDistance);
			ssrMaterial.SetFloat("_LayerThickness", settings.reflectionSettings.widthModifier);
			RenderTextureFormat format = (settings.basicSettings.enableHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32);
			RenderTexture[] array = new RenderTexture[5];
			for (int i = 0; i < 5; i++)
			{
				if (fullResolutionFiltering)
				{
					array[i] = m_RTU.GetTemporaryRenderTexture(width, height, 0, format);
				}
				else
				{
					array[i] = m_RTU.GetTemporaryRenderTexture(width >> i, height >> i, 0, format);
				}
				array[i].filterMode = ((!settings.advancedSettings.bilateralUpsample) ? FilterMode.Bilinear : FilterMode.Point);
			}
			ssrMaterial.SetInt("_EnableSSR", 1);
			ssrMaterial.SetInt("_DebugMode", (int)settings.debugSettings.debugMode);
			ssrMaterial.SetInt("_TraceBehindObjects", settings.advancedSettings.traceBehindObjects ? 1 : 0);
			ssrMaterial.SetInt("_MaxSteps", settings.reflectionSettings.maxSteps);
			RenderTexture temporaryRenderTexture2 = m_RTU.GetTemporaryRenderTexture(width, height);
			int pass = Mathf.Clamp(settings.reflectionSettings.rayStepSize, 0, 4);
			Graphics.Blit(source, temporaryRenderTexture2, ssrMaterial, pass);
			ssrMaterial.SetTexture("_HitPointTexture", temporaryRenderTexture2);
			Graphics.Blit(source, array[0], ssrMaterial, 11);
			ssrMaterial.SetTexture("_ReflectionTexture0", array[0]);
			ssrMaterial.SetInt("_FullResolutionFiltering", fullResolutionFiltering ? 1 : 0);
			ssrMaterial.SetFloat("_MaxRoughness", 1f - settings.reflectionSettings.smoothFallbackThreshold);
			ssrMaterial.SetFloat("_RoughnessFalloffRange", settings.reflectionSettings.smoothFallbackDistance);
			ssrMaterial.SetFloat("_SSRMultiplier", settings.basicSettings.reflectionMultiplier);
			RenderTexture[] array2 = new RenderTexture[5];
			if (settings.advancedSettings.bilateralUpsample && useEdgeDetector)
			{
				array2[0] = m_RTU.GetTemporaryRenderTexture(width, height);
				Graphics.Blit(source, array2[0], ssrMaterial, 9);
				for (int j = 1; j < 5; j++)
				{
					array2[j] = m_RTU.GetTemporaryRenderTexture(width >> j, height >> j);
					ssrMaterial.SetInt("_LastMip", j - 1);
					Graphics.Blit(array2[j - 1], array2[j], ssrMaterial, 10);
				}
			}
			if (settings.advancedSettings.highQualitySharpReflections)
			{
				RenderTexture temporaryRenderTexture3 = m_RTU.GetTemporaryRenderTexture(array[0].width, array[0].height, 0, array[0].format);
				temporaryRenderTexture3.filterMode = array[0].filterMode;
				array[0].filterMode = FilterMode.Bilinear;
				Graphics.Blit(array[0], temporaryRenderTexture3, ssrMaterial, 16);
				m_RTU.ReleaseTemporaryRenderTexture(array[0]);
				array[0] = temporaryRenderTexture3;
				ssrMaterial.SetTexture("_ReflectionTexture0", array[0]);
			}
			for (int k = 1; k < 5; k++)
			{
				RenderTexture renderTexture = array[k - 1];
				RenderTexture temporaryRenderTexture4;
				if (fullResolutionFiltering)
				{
					temporaryRenderTexture4 = m_RTU.GetTemporaryRenderTexture(width, height, 0, format);
				}
				else
				{
					int num6 = k;
					temporaryRenderTexture4 = m_RTU.GetTemporaryRenderTexture(width >> num6, height >> k - 1, 0, format);
				}
				for (int l = 0; l < ((!fullResolutionFiltering) ? 1 : (k * k)); l++)
				{
					ssrMaterial.SetVector("_Axis", new Vector4(1f, 0f, 0f, 0f));
					ssrMaterial.SetFloat("_CurrentMipLevel", (float)k - 1f);
					Graphics.Blit(renderTexture, temporaryRenderTexture4, ssrMaterial, 6);
					ssrMaterial.SetVector("_Axis", new Vector4(0f, 1f, 0f, 0f));
					renderTexture = array[k];
					Graphics.Blit(temporaryRenderTexture4, renderTexture, ssrMaterial, 6);
				}
				ssrMaterial.SetTexture("_ReflectionTexture" + k, array[k]);
				m_RTU.ReleaseTemporaryRenderTexture(temporaryRenderTexture4);
			}
			if (settings.advancedSettings.bilateralUpsample && useEdgeDetector)
			{
				for (int m = 0; m < 5; m++)
				{
					ssrMaterial.SetTexture("_EdgeTexture" + m, array2[m]);
				}
			}
			ssrMaterial.SetInt("_UseEdgeDetector", useEdgeDetector ? 1 : 0);
			RenderTexture temporaryRenderTexture5 = m_RTU.GetTemporaryRenderTexture(source.width, source.height, 0, RenderTextureFormat.RHalf);
			if (computeAverageRayDistance)
			{
				Graphics.Blit(source, temporaryRenderTexture5, ssrMaterial, 15);
			}
			ssrMaterial.SetInt("_UseAverageRayDistance", computeAverageRayDistance ? 1 : 0);
			ssrMaterial.SetTexture("_AverageRayDistanceBuffer", temporaryRenderTexture5);
			bool flag2 = settings.advancedSettings.resolution == SSRResolution.HalfTraceFullResolve;
			RenderTexture temporaryRenderTexture6 = m_RTU.GetTemporaryRenderTexture((!flag2) ? width : source.width, (!flag2) ? height : source.height, 0, format);
			ssrMaterial.SetFloat("_FresnelFade", settings.reflectionSettings.fresnelFade);
			ssrMaterial.SetFloat("_FresnelFadePower", settings.reflectionSettings.fresnelFadePower);
			ssrMaterial.SetFloat("_DistanceBlur", settings.reflectionSettings.distanceBlur);
			ssrMaterial.SetInt("_HalfResolution", (settings.advancedSettings.resolution != 0) ? 1 : 0);
			ssrMaterial.SetInt("_HighlightSuppression", settings.advancedSettings.highlightSuppression ? 1 : 0);
			Graphics.Blit(array[0], temporaryRenderTexture6, ssrMaterial, 7);
			ssrMaterial.SetTexture("_FinalReflectionTexture", temporaryRenderTexture6);
			RenderTexture temporaryRenderTexture7 = m_RTU.GetTemporaryRenderTexture((!flag2) ? width : source.width, (!flag2) ? height : source.height, 0, format);
			if (flag)
			{
				ssrMaterial.SetInt("_UseTemporalConfidence", settings.advancedSettings.useTemporalConfidence ? 1 : 0);
				ssrMaterial.SetFloat("_TemporalAlpha", settings.advancedSettings.temporalFilterStrength);
				ssrMaterial.SetMatrix("_CurrentCameraToPreviousCamera", m_PreviousWorldToCameraMatrix * inverse);
				ssrMaterial.SetTexture("_PreviousReflectionTexture", m_PreviousReflectionBuffer);
				ssrMaterial.SetTexture("_PreviousCSZBuffer", m_PreviousDepthBuffer);
				Graphics.Blit(source, temporaryRenderTexture7, ssrMaterial, 14);
				ssrMaterial.SetTexture("_FinalReflectionTexture", temporaryRenderTexture7);
			}
			if ((double)settings.advancedSettings.temporalFilterStrength > 0.0)
			{
				m_PreviousWorldToCameraMatrix = worldToCameraMatrix;
				PreparePreviousBuffers(source.width, source.height);
				Graphics.Blit(source, m_PreviousDepthBuffer, ssrMaterial, 13);
				Graphics.Blit(temporaryRenderTexture2, m_PreviousHitBuffer);
				Graphics.Blit((!flag) ? temporaryRenderTexture6 : temporaryRenderTexture7, m_PreviousReflectionBuffer);
				m_HasInformationFromPreviousFrame = true;
			}
			Graphics.Blit(source, destination, ssrMaterial, 5);
			m_RTU.ReleaseAllTemporaryRenderTextures();
		}
	}
}
