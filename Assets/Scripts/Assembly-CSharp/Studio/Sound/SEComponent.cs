using ActionGame.MapSound;
using Illusion.Game;
using Manager;
using UnityEngine;

namespace Studio.Sound
{
	public class SEComponent : MonoBehaviour
	{
		public enum RolloffType
		{
			対数関数 = 0,
			線形 = 1
		}

		[SerializeField]
		private AudioClip _clip;

		[SerializeField]
		private Manager.Sound.Type _soundType = Manager.Sound.Type.GameSE3D;

		[SerializeField]
		private bool _isLoop;

		[SerializeField]
		private RolloffType _type = RolloffType.線形;

		[SerializeField]
		private Threshold _rolloffDistance = new Threshold(0f, 1f);

		[SerializeField]
		[Range(0f, 1f)]
		private float _volume = 1f;

		private AudioSource _audioSource;

		public AudioClip Clip
		{
			get
			{
				return _clip;
			}
			set
			{
				_clip = value;
			}
		}

		public Manager.Sound.Type SoundType
		{
			get
			{
				return _soundType;
			}
			set
			{
				_soundType = value;
			}
		}

		public bool IsLoop
		{
			get
			{
				return _isLoop;
			}
			set
			{
				_isLoop = value;
				if (_audioSource != null)
				{
					_audioSource.loop = value;
				}
			}
		}

		public RolloffType DecayType
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
				if (_audioSource != null)
				{
					if (_type == RolloffType.線形)
					{
						_audioSource.rolloffMode = AudioRolloffMode.Linear;
					}
					else
					{
						_audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
					}
				}
			}
		}

		public Threshold RolloffDistance
		{
			get
			{
				return _rolloffDistance;
			}
			set
			{
				Threshold rolloffDistance = new Threshold(Mathf.Max(0f, value.min), value.max);
				_rolloffDistance = rolloffDistance;
				if (_audioSource != null)
				{
					_audioSource.minDistance = _rolloffDistance.min;
					_audioSource.maxDistance = _rolloffDistance.max;
				}
			}
		}

		public float Volume
		{
			get
			{
				return _volume;
			}
			set
			{
				float volume = (_volume = Mathf.Max(0f, Mathf.Min(1f, value)));
				if (_audioSource != null)
				{
					_audioSource.volume = volume;
				}
			}
		}

		private void OnEnable()
		{
			if (_audioSource == null)
			{
				_audioSource = Utils.Sound.Play(_soundType, _clip);
			}
			if (!_audioSource.isPlaying)
			{
				_audioSource.Play();
			}
			if (_type == RolloffType.線形)
			{
				_audioSource.rolloffMode = AudioRolloffMode.Linear;
			}
			else
			{
				_audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
			}
			_audioSource.loop = _isLoop;
			_audioSource.minDistance = _rolloffDistance.min;
			_audioSource.maxDistance = _rolloffDistance.max;
			_audioSource.volume = _volume;
		}

		private void OnDisable()
		{
			if (_audioSource != null && _audioSource.isPlaying)
			{
				_audioSource.Stop();
			}
		}

		private void Update()
		{
			if (!(_audioSource == null))
			{
				_audioSource.transform.position = base.transform.position;
			}
		}
	}
}
