using UnityEngine;

namespace Sound
{
	[RequireComponent(typeof(AudioSource))]
	public class FadePlayer : MonoBehaviour
	{
		public abstract class State
		{
			protected FadePlayer player;

			public State(FadePlayer player)
			{
				this.player = player;
			}

			public virtual void Play()
			{
			}

			public virtual void Pause()
			{
			}

			public virtual void Stop()
			{
			}

			public virtual bool Update()
			{
				return true;
			}
		}

		public class Wait : State
		{
			public Wait(FadePlayer player)
				: base(player)
			{
			}

			public override void Play()
			{
				if (player.fadeInTime > 0f)
				{
					player.state = new FadeIn(player);
				}
				else
				{
					player.state = new Playing(player);
				}
			}
		}

		public class FadeIn : State
		{
			private float t;

			public FadeIn(FadePlayer player)
				: base(player)
			{
				player.audioSource.Play();
				player.audioSource.volume = 0f;
			}

			public override void Pause()
			{
				player.state = new Paused(player);
			}

			public override void Stop()
			{
				player.state = new FadeOut(player);
			}

			public override bool Update()
			{
				t += Time.deltaTime;
				player.audioSource.volume = Mathf.Lerp(0f, player.currentVolume, t / player.fadeInTime);
				if (t >= player.fadeInTime)
				{
					player.state = new Playing(player);
					return true;
				}
				return false;
			}
		}

		public class Playing : State
		{
			public Playing(FadePlayer player)
				: base(player)
			{
				if (!player.audioSource.isPlaying)
				{
					player.audioSource.Play();
				}
			}

			public override void Pause()
			{
				player.state = new Paused(player);
			}

			public override void Stop()
			{
				player.state = new FadeOut(player);
			}
		}

		public class Paused : State
		{
			private State preState;

			public Paused(FadePlayer player)
				: base(player)
			{
				preState = player.state;
				player.audioSource.Pause();
			}

			public override void Stop()
			{
				player.audioSource.Stop();
				player.state = new Wait(player);
			}

			public override void Play()
			{
				player.state = preState;
				player.audioSource.Play();
			}
		}

		public class FadeOut : State
		{
			private float t;

			public FadeOut(FadePlayer player)
				: base(player)
			{
				player.currentVolume = player.audioSource.volume;
			}

			public override void Pause()
			{
				player.state = new Paused(player);
			}

			public override bool Update()
			{
				t += Time.deltaTime;
				player.audioSource.volume = player.currentVolume * (1f - t / player.fadeOutTime);
				if (t >= player.fadeOutTime)
				{
					player.audioSource.volume = 0f;
					player.audioSource.Stop();
					player.state = new Wait(player);
					return true;
				}
				return false;
			}
		}

		public float currentVolume = 1f;

		private State state;

		private float fadeInTime;

		private float fadeOutTime;

		public State nowState
		{
			get
			{
				return state;
			}
		}

		public AudioSource audioSource { get; private set; }

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
			state = new Wait(this);
		}

		private void Update()
		{
			state.Update();
			if (state is Wait && !audioSource.isPlaying)
			{
				Object.Destroy(base.gameObject);
			}
		}

		public void Play(float fadeTime = 0f)
		{
			fadeInTime = fadeTime;
			state.Play();
		}

		public void Pause()
		{
			state.Pause();
		}

		public void Stop(float fadeTime)
		{
			fadeOutTime = fadeTime;
			state.Stop();
		}
	}
}
