using System;
using UnityEngine;

namespace ADV
{
	[Serializable]
	public class Info
	{
		[Serializable]
		public class Audio
		{
			[Serializable]
			public class Eco
			{
				[SerializeField]
				private bool _use;

				[Range(10f, 5000f)]
				[SerializeField]
				private float _delay = 50f;

				[SerializeField]
				[Range(0f, 1f)]
				private float _decayRatio = 0.5f;

				[Range(0f, 1f)]
				[SerializeField]
				private float _wetMix = 1f;

				[Range(0f, 1f)]
				[SerializeField]
				private float _dryMix = 1f;

				public bool use
				{
					get
					{
						return _use;
					}
					set
					{
						_use = value;
					}
				}

				public float delay
				{
					get
					{
						return _delay;
					}
					set
					{
						_delay = value;
					}
				}

				public float decayRatio
				{
					get
					{
						return _decayRatio;
					}
					set
					{
						_decayRatio = value;
					}
				}

				public float wetMix
				{
					get
					{
						return _wetMix;
					}
					set
					{
						_wetMix = value;
					}
				}

				public float dryMix
				{
					get
					{
						return _dryMix;
					}
					set
					{
						_dryMix = value;
					}
				}
			}

			public bool is2D;

			public bool isNotMoveMouth;

			public Eco eco = new Eco();
		}

		[Serializable]
		public class Anime
		{
			[Serializable]
			public class Play
			{
				[SerializeField]
				private int _layerNo;

				[Range(0f, 10f)]
				[SerializeField]
				private float _crossFadeTime = 0.8f;

				[Range(0.001f, 3f)]
				[SerializeField]
				private float _transitionDuration = 0.3f;

				[Range(0f, 1f)]
				[SerializeField]
				private float _normalizedTime;

				public int layerNo
				{
					get
					{
						return _layerNo;
					}
					set
					{
						_layerNo = value;
					}
				}

				public float crossFadeTime
				{
					get
					{
						return _crossFadeTime;
					}
					set
					{
						_crossFadeTime = value;
					}
				}

				public float transitionDuration
				{
					get
					{
						return _transitionDuration;
					}
					set
					{
						_transitionDuration = value;
					}
				}

				public float normalizedTime
				{
					get
					{
						return _normalizedTime;
					}
					set
					{
						_normalizedTime = value;
					}
				}
			}

			public Play play = new Play();
		}

		public Audio audio = new Audio();

		public Anime anime = new Anime();
	}
}
