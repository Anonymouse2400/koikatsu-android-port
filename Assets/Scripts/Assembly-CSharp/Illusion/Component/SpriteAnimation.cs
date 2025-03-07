using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace Illusion.Component
{
	public class SpriteAnimation : MonoBehaviour
	{
		public enum Mode
		{
			None = 0,
			Play = 1,
			Loop = 2
		}

		[SerializeField]
		private Mode _mode;

		[SerializeField]
		private float _fps = 24f;

		[SerializeField]
		private Sprite[] _frames;

		public UnityEvent onFinish = new UnityEvent();

		private SpriteRenderer _renderer;

		private IDisposable stopDisposable;

		public Mode mode
		{
			get
			{
				return _mode;
			}
			set
			{
				_mode = value;
			}
		}

		public float fps
		{
			get
			{
				return Mathf.Max(_fps, 0.1f);
			}
			set
			{
				_fps = Mathf.Max(value, 0.1f);
			}
		}

		public Sprite[] frames
		{
			get
			{
				return _frames;
			}
			set
			{
				_frames = value;
			}
		}

		public Sprite sprite
		{
			get
			{
				if (cachedRenderer == null)
				{
					return null;
				}
				return cachedRenderer.sprite;
			}
			set
			{
				if (!(cachedRenderer == null))
				{
					cachedRenderer.sprite = value;
				}
			}
		}

		private SpriteRenderer cachedRenderer
		{
			get
			{
				return this.GetCache(ref _renderer, () => GetComponentInChildren<SpriteRenderer>());
			}
		}

		private void Initialize()
		{
			Stop();
			sprite = null;
		}

		public void Play()
		{
			Initialize();
			stopDisposable = Observable.Interval(TimeSpan.FromMilliseconds(1000f / _fps)).Take(_frames.Length).Subscribe((Action<long>)delegate(long index)
			{
				sprite = _frames[index];
			}, (Action)delegate
			{
				onFinish.Invoke();
				if (mode == Mode.Loop)
				{
					Play();
				}
				else
				{
					sprite = null;
				}
			});
		}

		public void Stop()
		{
			if (stopDisposable != null)
			{
				stopDisposable.Dispose();
			}
			stopDisposable = null;
		}

		private void OnEnable()
		{
			Mode mode = this.mode;
			if (mode == Mode.Play || mode == Mode.Loop)
			{
				Play();
			}
			else
			{
				Initialize();
			}
		}

		private void OnDisable()
		{
			Stop();
		}
	}
}
