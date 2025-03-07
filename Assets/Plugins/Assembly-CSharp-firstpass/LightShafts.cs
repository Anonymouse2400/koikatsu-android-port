using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class LightShafts : MonoBehaviour
{
	public LightShaftsShadowmapMode m_ShadowmapMode;

	private LightShaftsShadowmapMode m_ShadowmapModeOld;

	public Camera[] m_Cameras;

	public Camera m_CurrentCamera;

	private bool m_ShadowmapDirty = true;

	public Vector3 m_Size = new Vector3(10f, 10f, 20f);

	public float m_SpotNear = 0.1f;

	public float m_SpotFar = 1f;

	public LayerMask m_CullingMask = -1;

	public LayerMask m_ColorFilterMask = 0;

	public float m_Brightness = 5f;

	public float m_BrightnessColored = 5f;

	public float m_Extinction = 0.5f;

	public float m_MinDistFromCamera;

	public int m_ShadowmapRes = 1024;

	private Camera m_ShadowmapCamera;

	private RenderTexture m_Shadowmap;

	public Shader m_DepthShader;

	private RenderTexture m_ColorFilter;

	public Shader m_ColorFilterShader;

	public bool m_Colored;

	public float m_ColorBalance = 1f;

	public int m_EpipolarLines = 256;

	public int m_EpipolarSamples = 512;

	private RenderTexture m_CoordEpi;

	private RenderTexture m_DepthEpi;

	public Shader m_CoordShader;

	private Material m_CoordMaterial;

	private Camera m_CoordsCamera;

	private RenderTexture m_InterpolationEpi;

	public Shader m_DepthBreaksShader;

	private Material m_DepthBreaksMaterial;

	private RenderTexture m_RaymarchedLightEpi;

	private Material m_RaymarchMaterial;

	public Shader m_RaymarchShader;

	private RenderTexture m_InterpolateAlongRaysEpi;

	public Shader m_InterpolateAlongRaysShader;

	private Material m_InterpolateAlongRaysMaterial;

	private RenderTexture m_SamplePositions;

	public Shader m_SamplePositionsShader;

	private Material m_SamplePositionsMaterial;

	public Shader m_FinalInterpolationShader;

	private Material m_FinalInterpolationMaterial;

	public float m_DepthThreshold = 0.5f;

	public int m_InterpolationStep = 32;

	public bool m_ShowSamples;

	public bool m_ShowInterpolatedSamples;

	public float m_ShowSamplesBackgroundFade = 0.8f;

	public bool m_AttenuationCurveOn;

	public AnimationCurve m_AttenuationCurve;

	private Texture2D m_AttenuationCurveTex;

	private LightType m_LightType = LightType.Directional;

	private bool m_DX11Support;

	private bool m_MinRequirements;

	private Mesh m_SpotMesh;

	private float m_SpotMeshNear = -1f;

	private float m_SpotMeshFar = -1f;

	private float m_SpotMeshAngle = -1f;

	private float m_SpotMeshRange = -1f;

	public bool directional
	{
		get
		{
			return m_LightType == LightType.Directional;
		}
	}

	public bool spot
	{
		get
		{
			return m_LightType == LightType.Spot;
		}
	}

	private void InitLUTs()
	{
		if (!m_AttenuationCurveTex)
		{
			m_AttenuationCurveTex = new Texture2D(256, 1, TextureFormat.ARGB32, false, true);
			m_AttenuationCurveTex.wrapMode = TextureWrapMode.Clamp;
			m_AttenuationCurveTex.hideFlags = HideFlags.HideAndDontSave;
			if (m_AttenuationCurve == null || m_AttenuationCurve.length == 0)
			{
				m_AttenuationCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
			}
			if ((bool)m_AttenuationCurveTex)
			{
				UpdateLUTs();
			}
		}
	}

	public void UpdateLUTs()
	{
		InitLUTs();
		if (m_AttenuationCurve != null)
		{
			for (int i = 0; i < 256; i++)
			{
				float num = Mathf.Clamp(m_AttenuationCurve.Evaluate((float)i / 255f), 0f, 1f);
				m_AttenuationCurveTex.SetPixel(i, 0, new Color(num, num, num, num));
			}
			m_AttenuationCurveTex.Apply();
		}
	}

	private void InitRenderTexture(ref RenderTexture rt, int width, int height, int depth, RenderTextureFormat format, bool temp = true)
	{
		if (temp)
		{
			rt = RenderTexture.GetTemporary(width, height, depth, format);
			return;
		}
		if (rt != null)
		{
			if (rt.width == width && rt.height == height && rt.depth == depth && rt.format == format)
			{
				return;
			}
			rt.Release();
			UnityEngine.Object.DestroyImmediate(rt);
		}
		rt = new RenderTexture(width, height, depth, format);
		rt.hideFlags = HideFlags.HideAndDontSave;
	}

	private void InitShadowmap()
	{
		bool flag = m_ShadowmapMode == LightShaftsShadowmapMode.Dynamic;
		if (flag && m_ShadowmapMode != m_ShadowmapModeOld)
		{
			if ((bool)m_Shadowmap)
			{
				m_Shadowmap.Release();
			}
			if ((bool)m_ColorFilter)
			{
				m_ColorFilter.Release();
			}
		}
		InitRenderTexture(ref m_Shadowmap, m_ShadowmapRes, m_ShadowmapRes, 24, RenderTextureFormat.RFloat, flag);
		m_Shadowmap.filterMode = FilterMode.Point;
		m_Shadowmap.wrapMode = TextureWrapMode.Clamp;
		if (m_Colored)
		{
			InitRenderTexture(ref m_ColorFilter, m_ShadowmapRes, m_ShadowmapRes, 0, RenderTextureFormat.ARGB32, flag);
		}
		m_ShadowmapModeOld = m_ShadowmapMode;
	}

	private void ReleaseShadowmap()
	{
		if (m_ShadowmapMode != LightShaftsShadowmapMode.Static)
		{
			RenderTexture.ReleaseTemporary(m_Shadowmap);
			RenderTexture.ReleaseTemporary(m_ColorFilter);
		}
	}

	private void InitEpipolarTextures()
	{
		m_EpipolarLines = ((m_EpipolarLines >= 8) ? m_EpipolarLines : 8);
		m_EpipolarSamples = ((m_EpipolarSamples >= 4) ? m_EpipolarSamples : 4);
		InitRenderTexture(ref m_CoordEpi, m_EpipolarSamples, m_EpipolarLines, 0, RenderTextureFormat.RGFloat);
		m_CoordEpi.filterMode = FilterMode.Point;
		InitRenderTexture(ref m_DepthEpi, m_EpipolarSamples, m_EpipolarLines, 0, RenderTextureFormat.RFloat);
		m_DepthEpi.filterMode = FilterMode.Point;
		InitRenderTexture(ref m_InterpolationEpi, m_EpipolarSamples, m_EpipolarLines, 0, (!m_DX11Support) ? RenderTextureFormat.RGFloat : RenderTextureFormat.RGInt);
		m_InterpolationEpi.filterMode = FilterMode.Point;
		InitRenderTexture(ref m_RaymarchedLightEpi, m_EpipolarSamples, m_EpipolarLines, 24, RenderTextureFormat.ARGBFloat);
		m_RaymarchedLightEpi.filterMode = FilterMode.Point;
		InitRenderTexture(ref m_InterpolateAlongRaysEpi, m_EpipolarSamples, m_EpipolarLines, 0, RenderTextureFormat.ARGBFloat);
		m_InterpolateAlongRaysEpi.filterMode = FilterMode.Point;
	}

	private void InitMaterial(ref Material material, Shader shader)
	{
		if (!material && (bool)shader)
		{
			material = new Material(shader);
			material.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	private void InitMaterials()
	{
		InitMaterial(ref m_FinalInterpolationMaterial, m_FinalInterpolationShader);
		InitMaterial(ref m_CoordMaterial, m_CoordShader);
		InitMaterial(ref m_SamplePositionsMaterial, m_SamplePositionsShader);
		InitMaterial(ref m_RaymarchMaterial, m_RaymarchShader);
		InitMaterial(ref m_DepthBreaksMaterial, m_DepthBreaksShader);
		InitMaterial(ref m_InterpolateAlongRaysMaterial, m_InterpolateAlongRaysShader);
	}

	private void InitSpotFrustumMesh()
	{
		if (!m_SpotMesh)
		{
			m_SpotMesh = new Mesh();
			m_SpotMesh.hideFlags = HideFlags.HideAndDontSave;
		}
		Light component = GetComponent<Light>();
		if (m_SpotMeshNear != m_SpotNear || m_SpotMeshFar != m_SpotFar || m_SpotMeshAngle != component.spotAngle || m_SpotMeshRange != component.range)
		{
			float num = component.range * m_SpotFar;
			float num2 = component.range * m_SpotNear;
			float num3 = Mathf.Tan(component.spotAngle * ((float)Math.PI / 180f) * 0.5f);
			float num4 = num * num3;
			float num5 = num2 * num3;
			Vector3[] array = ((m_SpotMesh.vertices == null || m_SpotMesh.vertices.Length != 8) ? new Vector3[8] : m_SpotMesh.vertices);
			array[0] = new Vector3(0f - num4, 0f - num4, num);
			array[1] = new Vector3(num4, 0f - num4, num);
			array[2] = new Vector3(num4, num4, num);
			array[3] = new Vector3(0f - num4, num4, num);
			array[4] = new Vector3(0f - num5, 0f - num5, num2);
			array[5] = new Vector3(num5, 0f - num5, num2);
			array[6] = new Vector3(num5, num5, num2);
			array[7] = new Vector3(0f - num5, num5, num2);
			m_SpotMesh.vertices = array;
			if (m_SpotMesh.triangles == null || m_SpotMesh.triangles.Length != 36)
			{
				int[] triangles = new int[36]
				{
					0, 1, 2, 0, 2, 3, 6, 5, 4, 7,
					6, 4, 3, 2, 6, 3, 6, 7, 2, 1,
					5, 2, 5, 6, 0, 3, 7, 0, 7, 4,
					5, 1, 0, 5, 0, 4
				};
				m_SpotMesh.triangles = triangles;
			}
			m_SpotMeshNear = m_SpotNear;
			m_SpotMeshFar = m_SpotFar;
			m_SpotMeshAngle = component.spotAngle;
			m_SpotMeshRange = component.range;
		}
	}

	public void UpdateLightType()
	{
		m_LightType = GetComponent<Light>().type;
	}

	public bool CheckMinRequirements()
	{
		if (m_MinRequirements)
		{
			return true;
		}
		m_DX11Support = SystemInfo.graphicsShaderLevel >= 50;
		m_MinRequirements = SystemInfo.graphicsShaderLevel >= 30;
		m_MinRequirements &= SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGFloat);
		m_MinRequirements &= SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RFloat);
		return m_MinRequirements;
	}

	private void InitResources()
	{
		UpdateLightType();
		InitMaterials();
		InitEpipolarTextures();
		InitLUTs();
		InitSpotFrustumMesh();
	}

	private void ReleaseResources()
	{
		ReleaseShadowmap();
		RenderTexture.ReleaseTemporary(m_CoordEpi);
		RenderTexture.ReleaseTemporary(m_DepthEpi);
		RenderTexture.ReleaseTemporary(m_InterpolationEpi);
		RenderTexture.ReleaseTemporary(m_RaymarchedLightEpi);
		RenderTexture.ReleaseTemporary(m_InterpolateAlongRaysEpi);
	}

	private Bounds GetBoundsLocal()
	{
		if (directional)
		{
			return new Bounds(new Vector3(0f, 0f, m_Size.z * 0.5f), m_Size);
		}
		Light component = GetComponent<Light>();
		Vector3 center = new Vector3(0f, 0f, component.range * (m_SpotFar + m_SpotNear) * 0.5f);
		float z = (m_SpotFar - m_SpotNear) * component.range;
		float num = Mathf.Tan(component.spotAngle * ((float)Math.PI / 180f) * 0.5f) * m_SpotFar * component.range * 2f;
		return new Bounds(center, new Vector3(num, num, z));
	}

	private Matrix4x4 GetBoundsMatrix()
	{
		Bounds boundsLocal = GetBoundsLocal();
		Transform transform = base.transform;
		return Matrix4x4.TRS(transform.position + transform.forward * boundsLocal.center.z, transform.rotation, boundsLocal.size);
	}

	private float GetFrustumApex()
	{
		return (0f - m_SpotNear) / (m_SpotFar - m_SpotNear) - 0.5f;
	}

	private void OnDrawGizmosSelected()
	{
		UpdateLightType();
		Gizmos.color = Color.yellow;
		if (directional)
		{
			Gizmos.matrix = GetBoundsMatrix();
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		}
		else if (spot)
		{
			Transform transform = base.transform;
			Light component = GetComponent<Light>();
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			Gizmos.DrawFrustum(transform.position, component.spotAngle, component.range * m_SpotFar, component.range * m_SpotNear, 1f);
		}
	}

	private void RenderQuadSections(Vector4 lightPos)
	{
		for (int i = 0; i < 4; i++)
		{
			if ((i != 0 || !(lightPos.y > 1f)) && (i != 1 || !(lightPos.x > 1f)) && (i != 2 || !(lightPos.y < -1f)) && (i != 3 || !(lightPos.x < -1f)))
			{
				float num = (float)i / 2f - 1f;
				float y = num + 0.5f;
				GL.Begin(7);
				GL.Vertex3(-1f, num, 0f);
				GL.Vertex3(1f, num, 0f);
				GL.Vertex3(1f, y, 0f);
				GL.Vertex3(-1f, y, 0f);
				GL.End();
			}
		}
	}

	private void RenderQuad()
	{
		GL.Begin(7);
		GL.TexCoord2(0f, 0f);
		GL.Vertex3(-1f, -1f, 0f);
		GL.TexCoord2(0f, 1f);
		GL.Vertex3(-1f, 1f, 0f);
		GL.TexCoord2(1f, 1f);
		GL.Vertex3(1f, 1f, 0f);
		GL.TexCoord2(1f, 0f);
		GL.Vertex3(1f, -1f, 0f);
		GL.End();
	}

	private void RenderSpotFrustum()
	{
		Graphics.DrawMeshNow(m_SpotMesh, base.transform.position, base.transform.rotation);
	}

	private Vector4 GetLightViewportPos()
	{
		Vector3 position = base.transform.position;
		if (directional)
		{
			position = m_CurrentCamera.transform.position + base.transform.forward;
		}
		Vector3 vector = m_CurrentCamera.WorldToViewportPoint(position);
		return new Vector4(vector.x * 2f - 1f, vector.y * 2f - 1f, 0f, 0f);
	}

	private bool IsVisible()
	{
		Matrix4x4 worldToProjectionMatrix = m_CurrentCamera.projectionMatrix * m_CurrentCamera.worldToCameraMatrix * base.transform.localToWorldMatrix;
		return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(worldToProjectionMatrix), GetBoundsLocal());
	}

	private bool IntersectsNearPlane()
	{
		Vector3[] vertices = m_SpotMesh.vertices;
		float num = m_CurrentCamera.nearClipPlane - 0.001f;
		Transform transform = base.transform;
		for (int i = 0; i < vertices.Length; i++)
		{
			float z = m_CurrentCamera.WorldToViewportPoint(transform.TransformPoint(vertices[i])).z;
			if (z < num)
			{
				return true;
			}
		}
		return false;
	}

	private void SetKeyword(bool firstOn, string firstKeyword, string secondKeyword)
	{
		Shader.EnableKeyword((!firstOn) ? secondKeyword : firstKeyword);
		Shader.DisableKeyword((!firstOn) ? firstKeyword : secondKeyword);
	}

	public void SetShadowmapDirty()
	{
		m_ShadowmapDirty = true;
	}

	private void GetFrustumRays(out Matrix4x4 frustumRays, out Vector3 cameraPosLocal)
	{
		float farClipPlane = m_CurrentCamera.farClipPlane;
		Vector3 position = m_CurrentCamera.transform.position;
		Matrix4x4 inverse = GetBoundsMatrix().inverse;
		Vector2[] array = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f)
		};
		frustumRays = default(Matrix4x4);
		for (int i = 0; i < 4; i++)
		{
			Vector3 v = m_CurrentCamera.ViewportToWorldPoint(new Vector3(array[i].x, array[i].y, farClipPlane)) - position;
			v = inverse.MultiplyVector(v);
			frustumRays.SetRow(i, v);
		}
		cameraPosLocal = inverse.MultiplyPoint3x4(position);
	}

	private void SetFrustumRays(Material material)
	{
		Matrix4x4 frustumRays;
		Vector3 cameraPosLocal;
		GetFrustumRays(out frustumRays, out cameraPosLocal);
		material.SetVector("_CameraPosLocal", cameraPosLocal);
		material.SetMatrix("_FrustumRays", frustumRays);
		material.SetFloat("_FrustumApex", GetFrustumApex());
	}

	private float GetDepthThresholdAdjusted()
	{
		return m_DepthThreshold / m_CurrentCamera.farClipPlane;
	}

	private bool CheckCamera()
	{
		if (m_Cameras == null)
		{
			return false;
		}
		Camera[] cameras = m_Cameras;
		foreach (Camera camera in cameras)
		{
			if (camera == m_CurrentCamera)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateCameraDepthMode()
	{
		if (m_Cameras == null)
		{
			return;
		}
		Camera[] cameras = m_Cameras;
		foreach (Camera camera in cameras)
		{
			if ((bool)camera)
			{
				camera.depthTextureMode |= DepthTextureMode.Depth;
			}
		}
	}

	public void Start()
	{
		CheckMinRequirements();
		if (m_Cameras == null || m_Cameras.Length == 0)
		{
			m_Cameras = new Camera[1] { Camera.main };
		}
		UpdateCameraDepthMode();
	}

	private void UpdateShadowmap()
	{
		if (m_ShadowmapMode != LightShaftsShadowmapMode.Static || m_ShadowmapDirty)
		{
			InitShadowmap();
			if (m_ShadowmapCamera == null)
			{
				GameObject gameObject = new GameObject("Depth Camera");
				gameObject.AddComponent(typeof(Camera));
				m_ShadowmapCamera = gameObject.GetComponent<Camera>();
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				m_ShadowmapCamera.enabled = false;
				m_ShadowmapCamera.clearFlags = CameraClearFlags.Color;
			}
			Transform transform = m_ShadowmapCamera.transform;
			transform.position = base.transform.position;
			transform.rotation = base.transform.rotation;
			if (directional)
			{
				m_ShadowmapCamera.orthographic = true;
				m_ShadowmapCamera.nearClipPlane = 0f;
				m_ShadowmapCamera.farClipPlane = m_Size.z;
				m_ShadowmapCamera.orthographicSize = m_Size.y * 0.5f;
				m_ShadowmapCamera.aspect = m_Size.x / m_Size.y;
			}
			else
			{
				m_ShadowmapCamera.orthographic = false;
				m_ShadowmapCamera.nearClipPlane = m_SpotNear * GetComponent<Light>().range;
				m_ShadowmapCamera.farClipPlane = m_SpotFar * GetComponent<Light>().range;
				m_ShadowmapCamera.fieldOfView = GetComponent<Light>().spotAngle;
				m_ShadowmapCamera.aspect = 1f;
			}
			m_ShadowmapCamera.renderingPath = RenderingPath.Forward;
			m_ShadowmapCamera.targetTexture = m_Shadowmap;
			m_ShadowmapCamera.cullingMask = m_CullingMask;
			m_ShadowmapCamera.backgroundColor = Color.white;
			m_ShadowmapCamera.RenderWithShader(m_DepthShader, "RenderType");
			if (m_Colored)
			{
				m_ShadowmapCamera.targetTexture = m_ColorFilter;
				m_ShadowmapCamera.cullingMask = m_ColorFilterMask;
				m_ShadowmapCamera.backgroundColor = new Color(m_ColorBalance, m_ColorBalance, m_ColorBalance);
				m_ShadowmapCamera.RenderWithShader(m_ColorFilterShader, string.Empty);
			}
			m_ShadowmapDirty = false;
		}
	}

	private void RenderCoords(int width, int height, Vector4 lightPos)
	{
		SetFrustumRays(m_CoordMaterial);
		RenderBuffer[] colorBuffers = new RenderBuffer[2] { m_CoordEpi.colorBuffer, m_DepthEpi.colorBuffer };
		Graphics.SetRenderTarget(colorBuffers, m_DepthEpi.depthBuffer);
		m_CoordMaterial.SetVector("_LightPos", lightPos);
		m_CoordMaterial.SetVector("_CoordTexDim", new Vector4(m_CoordEpi.width, m_CoordEpi.height, 1f / (float)m_CoordEpi.width, 1f / (float)m_CoordEpi.height));
		m_CoordMaterial.SetVector("_ScreenTexDim", new Vector4(width, height, 1f / (float)width, 1f / (float)height));
		m_CoordMaterial.SetPass(0);
		RenderQuad();
	}

	private void RenderInterpolationTexture(Vector4 lightPos)
	{
		Graphics.SetRenderTarget(m_InterpolationEpi.colorBuffer, m_RaymarchedLightEpi.depthBuffer);
		if (!m_DX11Support && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer))
		{
			m_DepthBreaksMaterial.SetPass(1);
			RenderQuad();
		}
		else
		{
			GL.Clear(true, true, new Color(0f, 0f, 0f, 1f));
		}
		m_DepthBreaksMaterial.SetFloat("_InterpolationStep", m_InterpolationStep);
		m_DepthBreaksMaterial.SetFloat("_DepthThreshold", GetDepthThresholdAdjusted());
		m_DepthBreaksMaterial.SetTexture("_DepthEpi", m_DepthEpi);
		m_DepthBreaksMaterial.SetVector("_DepthEpiTexDim", new Vector4(m_DepthEpi.width, m_DepthEpi.height, 1f / (float)m_DepthEpi.width, 1f / (float)m_DepthEpi.height));
		m_DepthBreaksMaterial.SetPass(0);
		RenderQuadSections(lightPos);
	}

	private void InterpolateAlongRays(Vector4 lightPos)
	{
		Graphics.SetRenderTarget(m_InterpolateAlongRaysEpi);
		m_InterpolateAlongRaysMaterial.SetFloat("_InterpolationStep", m_InterpolationStep);
		m_InterpolateAlongRaysMaterial.SetTexture("_InterpolationEpi", m_InterpolationEpi);
		m_InterpolateAlongRaysMaterial.SetTexture("_RaymarchedLightEpi", m_RaymarchedLightEpi);
		m_InterpolateAlongRaysMaterial.SetVector("_RaymarchedLightEpiTexDim", new Vector4(m_RaymarchedLightEpi.width, m_RaymarchedLightEpi.height, 1f / (float)m_RaymarchedLightEpi.width, 1f / (float)m_RaymarchedLightEpi.height));
		m_InterpolateAlongRaysMaterial.SetPass(0);
		RenderQuadSections(lightPos);
	}

	private void RenderSamplePositions(int width, int height, Vector4 lightPos)
	{
		InitRenderTexture(ref m_SamplePositions, width, height, 0, RenderTextureFormat.ARGB32, false);
		m_SamplePositions.enableRandomWrite = true;
		m_SamplePositions.filterMode = FilterMode.Point;
		Graphics.SetRenderTarget(m_SamplePositions);
		GL.Clear(false, true, new Color(0f, 0f, 0f, 1f));
		Graphics.ClearRandomWriteTargets();
		Graphics.SetRandomWriteTarget(1, m_SamplePositions);
		Graphics.SetRenderTarget(m_RaymarchedLightEpi);
		m_SamplePositionsMaterial.SetVector("_OutputTexDim", new Vector4(width - 1, height - 1, 0f, 0f));
		m_SamplePositionsMaterial.SetVector("_CoordTexDim", new Vector4(m_CoordEpi.width, m_CoordEpi.height, 0f, 0f));
		m_SamplePositionsMaterial.SetTexture("_Coord", m_CoordEpi);
		m_SamplePositionsMaterial.SetTexture("_InterpolationEpi", m_InterpolationEpi);
		if (m_ShowInterpolatedSamples)
		{
			m_SamplePositionsMaterial.SetFloat("_SampleType", 1f);
			m_SamplePositionsMaterial.SetVector("_Color", new Vector4(0.4f, 0.4f, 0f, 0f));
			m_SamplePositionsMaterial.SetPass(0);
			RenderQuad();
		}
		m_SamplePositionsMaterial.SetFloat("_SampleType", 0f);
		m_SamplePositionsMaterial.SetVector("_Color", new Vector4(1f, 0f, 0f, 0f));
		m_SamplePositionsMaterial.SetPass(0);
		RenderQuadSections(lightPos);
		Graphics.ClearRandomWriteTargets();
	}

	private void ShowSamples(int width, int height, Vector4 lightPos)
	{
		bool flag = m_ShowSamples && m_DX11Support;
		SetKeyword(flag, "SHOW_SAMPLES_ON", "SHOW_SAMPLES_OFF");
		if (flag)
		{
			RenderSamplePositions(width, height, lightPos);
		}
		m_FinalInterpolationMaterial.SetFloat("_ShowSamplesBackgroundFade", m_ShowSamplesBackgroundFade);
	}

	private void Raymarch(int width, int height, Vector4 lightPos)
	{
		SetFrustumRays(m_RaymarchMaterial);
		int width2 = m_Shadowmap.width;
		int height2 = m_Shadowmap.height;
		Graphics.SetRenderTarget(m_RaymarchedLightEpi.colorBuffer, m_RaymarchedLightEpi.depthBuffer);
		GL.Clear(false, true, new Color(0f, 0f, 0f, 1f));
		m_RaymarchMaterial.SetTexture("_Coord", m_CoordEpi);
		m_RaymarchMaterial.SetTexture("_InterpolationEpi", m_InterpolationEpi);
		m_RaymarchMaterial.SetTexture("_Shadowmap", m_Shadowmap);
		float num = ((!m_Colored) ? m_Brightness : (m_BrightnessColored / m_ColorBalance));
		num *= GetComponent<Light>().intensity;
		m_RaymarchMaterial.SetFloat("_Brightness", num);
		m_RaymarchMaterial.SetFloat("_Extinction", 0f - m_Extinction);
		m_RaymarchMaterial.SetVector("_ShadowmapDim", new Vector4(width2, height2, 1f / (float)width2, 1f / (float)height2));
		m_RaymarchMaterial.SetVector("_ScreenTexDim", new Vector4(width, height, 1f / (float)width, 1f / (float)height));
		m_RaymarchMaterial.SetVector("_LightColor", GetComponent<Light>().color.linear);
		m_RaymarchMaterial.SetFloat("_MinDistFromCamera", m_MinDistFromCamera);
		SetKeyword(m_Colored, "COLORED_ON", "COLORED_OFF");
		m_RaymarchMaterial.SetTexture("_ColorFilter", m_ColorFilter);
		SetKeyword(m_AttenuationCurveOn, "ATTENUATION_CURVE_ON", "ATTENUATION_CURVE_OFF");
		m_RaymarchMaterial.SetTexture("_AttenuationCurveTex", m_AttenuationCurveTex);
		Texture cookie = GetComponent<Light>().cookie;
		SetKeyword(cookie != null, "COOKIE_TEX_ON", "COOKIE_TEX_OFF");
		if (cookie != null)
		{
			m_RaymarchMaterial.SetTexture("_Cookie", cookie);
		}
		m_RaymarchMaterial.SetPass(0);
		RenderQuadSections(lightPos);
	}

	private void FlipWorkaround()
	{
		bool flag = Convert.ToSingle(Application.unityVersion.Substring(0, 3)) < 4.5f;
		flag &= m_CurrentCamera.actualRenderingPath != RenderingPath.DeferredLighting;
		if (flag)
		{
			MonoBehaviour monoBehaviour = m_CurrentCamera.GetComponent("PostEffectsBase") as MonoBehaviour;
			flag &= monoBehaviour == null || !monoBehaviour.enabled;
		}
		SetKeyword(flag, "FLIP_WORKAROUND_ON", "FLIP_WORKAROUND_OFF");
	}

	public void OnRenderObject()
	{
		m_CurrentCamera = Camera.current;
		if (m_MinRequirements && CheckCamera() && IsVisible())
		{
			RenderBuffer activeDepthBuffer = Graphics.activeDepthBuffer;
			RenderBuffer activeColorBuffer = Graphics.activeColorBuffer;
			InitResources();
			Vector4 lightViewportPos = GetLightViewportPos();
			bool firstOn = lightViewportPos.x >= -1f && lightViewportPos.x <= 1f && lightViewportPos.y >= -1f && lightViewportPos.y <= 1f;
			SetKeyword(firstOn, "LIGHT_ON_SCREEN", "LIGHT_OFF_SCREEN");
			int width = Screen.width;
			int height = Screen.height;
			UpdateShadowmap();
			SetKeyword(directional, "DIRECTIONAL_SHAFTS", "SPOT_SHAFTS");
			RenderCoords(width, height, lightViewportPos);
			RenderInterpolationTexture(lightViewportPos);
			Raymarch(width, height, lightViewportPos);
			InterpolateAlongRays(lightViewportPos);
			ShowSamples(width, height, lightViewportPos);
			FlipWorkaround();
			SetFrustumRays(m_FinalInterpolationMaterial);
			m_FinalInterpolationMaterial.SetTexture("_InterpolationEpi", m_InterpolationEpi);
			m_FinalInterpolationMaterial.SetTexture("_DepthEpi", m_DepthEpi);
			m_FinalInterpolationMaterial.SetTexture("_Shadowmap", m_Shadowmap);
			m_FinalInterpolationMaterial.SetTexture("_Coord", m_CoordEpi);
			m_FinalInterpolationMaterial.SetTexture("_SamplePositions", m_SamplePositions);
			m_FinalInterpolationMaterial.SetTexture("_RaymarchedLight", m_InterpolateAlongRaysEpi);
			m_FinalInterpolationMaterial.SetVector("_CoordTexDim", new Vector4(m_CoordEpi.width, m_CoordEpi.height, 1f / (float)m_CoordEpi.width, 1f / (float)m_CoordEpi.height));
			m_FinalInterpolationMaterial.SetVector("_ScreenTexDim", new Vector4(width, height, 1f / (float)width, 1f / (float)height));
			m_FinalInterpolationMaterial.SetVector("_LightPos", lightViewportPos);
			m_FinalInterpolationMaterial.SetFloat("_DepthThreshold", GetDepthThresholdAdjusted());
			bool flag = directional || IntersectsNearPlane();
			m_FinalInterpolationMaterial.SetFloat("_ZTest", (!flag) ? 2 : 8);
			SetKeyword(flag, "QUAD_SHAFTS", "FRUSTUM_SHAFTS");
			Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
			m_FinalInterpolationMaterial.SetPass(0);
			if (flag)
			{
				RenderQuad();
			}
			else
			{
				RenderSpotFrustum();
			}
			ReleaseResources();
		}
	}
}
