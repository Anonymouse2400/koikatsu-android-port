using Config;
using Manager;
using UnityEngine;

[RequireComponent(typeof(CameraEffector))]
public class CameraEffectorConfig : MonoBehaviour
{
	[SerializeField]
	private CameraEffector effector;

	private bool _useDOF = true;

	private bool _useFog = true;

	private bool _useBloom = true;

	private bool _useVignette = true;

	private bool _useAmplifyOcclus = true;

	private bool _useSunShafts = true;

	public bool useDOF
	{
		get
		{
			return _useDOF;
		}
		set
		{
			_useDOF = value;
		}
	}

	public bool useFog
	{
		get
		{
			return _useFog;
		}
		set
		{
			_useFog = value;
		}
	}

	public bool useBloom
	{
		get
		{
			return _useBloom;
		}
		set
		{
			_useBloom = value;
		}
	}

	public bool useVignette
	{
		get
		{
			return _useVignette;
		}
		set
		{
			_useVignette = value;
		}
	}

	public bool useAmplifyOcclus
	{
		get
		{
			return _useAmplifyOcclus;
		}
		set
		{
			_useAmplifyOcclus = value;
		}
	}

	public bool useSunShafts
	{
		get
		{
			return _useSunShafts;
		}
		set
		{
			_useSunShafts = value;
		}
	}

	public void SetDOF(bool enable)
	{
		if (!enable)
		{
			effector.useDOF = false;
		}
		_useDOF = enable;
	}

	public void SetFog(bool enable)
	{
		if (!enable)
		{
			effector.useFog = false;
		}
		_useFog = enable;
	}

	public void SetBloom(bool enable)
	{
		if (!enable)
		{
			effector.useBloom = false;
		}
		_useBloom = enable;
	}

	public void SetVignette(bool enable)
	{
		if (!enable)
		{
			effector.useVignette = false;
		}
		_useVignette = enable;
	}

	public void SetAmplifyOcclus(bool enable)
	{
		if (!enable)
		{
			effector.useAmplifyOcclus = false;
		}
		_useAmplifyOcclus = enable;
	}

	public void SetSunShafts(bool enable)
	{
		if (!enable)
		{
			effector.useSunShafts = false;
		}
		_useSunShafts = enable;
	}

	private void Awake()
	{
	}

	private void Update()
	{
		EtceteraSystem etcData = Manager.Config.EtcData;
		if (_useDOF)
		{
			effector.useDOF = etcData.DepthOfField;
		}
		if (_useFog)
		{
			effector.useFog = etcData.Fog;
		}
		if (_useBloom)
		{
			effector.useBloom = etcData.Bloom;
		}
		if (_useVignette)
		{
			effector.useVignette = etcData.Vignette;
		}
		if (_useAmplifyOcclus)
		{
			effector.useAmplifyOcclus = etcData.AmplifyOcclus;
		}
		if (_useSunShafts)
		{
			effector.useSunShafts = etcData.SunShafts;
		}
	}
}
