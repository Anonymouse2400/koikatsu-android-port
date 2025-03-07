using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraEffector : MonoBehaviour
{
	[SerializeField]
	private GlobalFog _fog;

	[SerializeField]
	private AmplifyColorEffect _amplifyColor;

	[SerializeField]
	private AmplifyOcclusionEffect _amplifyOcclus;

	[SerializeField]
	private BloomAndFlares _bloom;

	[SerializeField]
	private SunShafts _sunShafts;

	[SerializeField]
	private VignetteAndChromaticAberration _vignette;

	[SerializeField]
	private DepthOfField _dof;

	[SerializeField]
	private Blur _blur;

	[SerializeField]
	private CrossFade _crossFade;

	[SerializeField]
	private SepiaTone _sepia;

	private CameraEffectorConfig _config;

	public bool useFog
	{
		get
		{
			return !(_fog == null) && _fog.enabled;
		}
		set
		{
			if (!(_fog == null))
			{
				_fog.enabled = value;
			}
		}
	}

	public bool useAmplifyColor
	{
		get
		{
			return !(_amplifyColor == null) && _amplifyColor.enabled;
		}
		set
		{
			if (!(_amplifyColor == null))
			{
				_amplifyColor.enabled = value;
			}
		}
	}

	public bool useAmplifyOcclus
	{
		get
		{
			return !(_amplifyOcclus == null) && _amplifyOcclus.enabled;
		}
		set
		{
			if (!(_amplifyOcclus == null))
			{
				_amplifyOcclus.enabled = value;
			}
		}
	}

	public bool useBloom
	{
		get
		{
			return !(_bloom == null) && _bloom.enabled;
		}
		set
		{
			if (!(_bloom == null))
			{
				_bloom.enabled = value;
			}
		}
	}

	public bool useSunShafts
	{
		get
		{
			return !(_sunShafts == null) && _sunShafts.enabled;
		}
		set
		{
			if (!(_sunShafts == null))
			{
				_sunShafts.enabled = value;
			}
		}
	}

	public bool useVignette
	{
		get
		{
			return !(_vignette == null) && _vignette.enabled;
		}
		set
		{
			if (!(_vignette == null))
			{
				_vignette.enabled = value;
			}
		}
	}

	public bool useDOF
	{
		get
		{
			return !(_dof == null) && _dof.enabled;
		}
		set
		{
			if (!(_dof == null))
			{
				_dof.enabled = value;
			}
		}
	}

	public bool useBlur
	{
		get
		{
			return !(_blur == null) && _blur.enabled;
		}
		set
		{
			if (!(_blur == null))
			{
				_blur.enabled = value;
			}
		}
	}

	public bool useCrossFade
	{
		get
		{
			return !(_crossFade == null) && _crossFade.enabled;
		}
		set
		{
			if (!(_crossFade == null))
			{
				_crossFade.enabled = value;
			}
		}
	}

	public bool useSepia
	{
		get
		{
			return !(_sepia == null) && _sepia.enabled;
		}
		set
		{
			if (!(_sepia == null))
			{
				_sepia.enabled = value;
			}
		}
	}

	public GlobalFog fog
	{
		get
		{
			return _fog;
		}
	}

	public AmplifyColorEffect amplifyColor
	{
		get
		{
			return _amplifyColor;
		}
	}

	public AmplifyOcclusionEffect amplifyOcclus
	{
		get
		{
			return _amplifyOcclus;
		}
	}

	public BloomAndFlares bloom
	{
		get
		{
			return _bloom;
		}
	}

	public SunShafts sunShafts
	{
		get
		{
			return _sunShafts;
		}
	}

	public VignetteAndChromaticAberration vignette
	{
		get
		{
			return _vignette;
		}
	}

	public DepthOfField dof
	{
		get
		{
			return _dof;
		}
	}

	public Blur blur
	{
		get
		{
			return _blur;
		}
	}

	public CrossFade crossFade
	{
		get
		{
			return _crossFade;
		}
	}

	public SepiaTone sepia
	{
		get
		{
			return _sepia;
		}
	}

	public CameraEffectorConfig config
	{
		get
		{
			return _config;
		}
	}

	public void SetDOFTarget(Transform target)
	{
		if (!(_dof == null))
		{
			_dof.focalTransform = target;
		}
	}

	private void Awake()
	{
		_config = GetComponent<CameraEffectorConfig>();
	}

	[ContextMenu("Setup")]
	private void Setup()
	{
		_fog = GetComponent<GlobalFog>();
		_amplifyColor = GetComponent<AmplifyColorEffect>();
		_amplifyOcclus = GetComponent<AmplifyOcclusionEffect>();
		_bloom = GetComponent<BloomAndFlares>();
		_sunShafts = GetComponent<SunShafts>();
		_vignette = GetComponent<VignetteAndChromaticAberration>();
		_dof = GetComponent<DepthOfField>();
		_blur = GetComponent<Blur>();
		_crossFade = GetComponent<CrossFade>();
		_sepia = GetComponent<SepiaTone>();
	}
}
