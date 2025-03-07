using System;
using System.Collections;
using Manager;
using UniRx;
using UnityEngine;

namespace ActionGame
{
	public class BGMVolume : MonoBehaviour
	{
		private static IDisposable _subscriber;

		[Range(0f, 1f)]
		[SerializeField]
		private float _volume = 1f;

		public float Volume
		{
			get
			{
				return _volume;
			}
			set
			{
				_volume = value;
			}
		}

		private void Start()
		{
			SetCall(_volume);
		}

		private void SetCall(float volume)
		{
			if (_subscriber != null)
			{
				_subscriber.Dispose();
			}
			_subscriber = Observable.FromCoroutine(() => FadeVolume(volume)).Subscribe();
		}

		private IEnumerator FadeVolume(float volume)
		{
			AudioSource audioSource = Singleton<Manager.Sound>.Instance.currentBGM.GetComponent<AudioSource>();
			float startVolume = audioSource.volume;
			float t = 0f;
			float duration = 0.1f;
			float tVelocity = 1f / duration;
			while (t < 1f)
			{
				audioSource.volume = Mathf.Lerp(startVolume, volume, t);
				t += tVelocity * Time.deltaTime;
				yield return null;
			}
			audioSource.volume = volume;
			if (_subscriber != null)
			{
				_subscriber = null;
			}
		}
	}
}
