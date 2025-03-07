using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Studio
{
	public class LateUpdateManager : Singleton<LateUpdateManager>
	{
		private const int InitializeSize = 16;

		private int tail;

		private ILateUpdatable[] arrayUpdatable = new ILateUpdatable[16];

		[SerializeField]
		private bool m_ReduceArraySizeWhenNeed;

		public static bool reduceArraySizeWhenNeed
		{
			get
			{
				return Singleton<LateUpdateManager>.IsInstance() && Singleton<LateUpdateManager>.Instance.m_ReduceArraySizeWhenNeed;
			}
			set
			{
				if (Singleton<LateUpdateManager>.IsInstance())
				{
					Singleton<LateUpdateManager>.Instance.m_ReduceArraySizeWhenNeed = value;
				}
			}
		}

		public static void AddUpdatableST(ILateUpdatable _updatable)
		{
			if (_updatable != null && Singleton<LateUpdateManager>.IsInstance())
			{
				Singleton<LateUpdateManager>.Instance.AddUpdatable(_updatable);
			}
		}

		private void AddUpdatable(ILateUpdatable _updatable)
		{
			if (arrayUpdatable.Length == tail)
			{
				Array.Resize(ref arrayUpdatable, checked(tail * 2));
			}
			arrayUpdatable[tail++] = _updatable;
		}

		public static void RemoveUpdatableST(ILateUpdatable _updatable)
		{
			if (_updatable != null && Singleton<LateUpdateManager>.IsInstance())
			{
				Singleton<LateUpdateManager>.Instance.RemoveUpdatable(_updatable);
			}
		}

		private void RemoveUpdatable(ILateUpdatable _updatable)
		{
			for (int i = 0; i < arrayUpdatable.Length; i++)
			{
				if (arrayUpdatable[i] == _updatable)
				{
					arrayUpdatable[i] = null;
					break;
				}
			}
		}

		public static void RefreshArrayUpdatableST()
		{
			if (Singleton<LateUpdateManager>.IsInstance())
			{
				Singleton<LateUpdateManager>.Instance.RefreshArrayUpdatable();
			}
		}

		private void RefreshArrayUpdatable()
		{
			int num = tail - 1;
			for (int i = 0; i < arrayUpdatable.Length; i++)
			{
				if (arrayUpdatable[i] != null)
				{
					continue;
				}
				ILateUpdatable lateUpdatable;
				while (i < num)
				{
					lateUpdatable = arrayUpdatable[num];
					if (lateUpdatable == null)
					{
						num--;
						continue;
					}
					goto IL_0031;
				}
				tail = i;
				break;
				IL_0031:
				arrayUpdatable[i] = lateUpdatable;
				arrayUpdatable[num] = null;
				num--;
			}
			if (m_ReduceArraySizeWhenNeed && tail < arrayUpdatable.Length / 2)
			{
				Array.Resize(ref arrayUpdatable, arrayUpdatable.Length / 2);
			}
		}

		protected override void Awake()
		{
			if (!CheckInstance())
			{
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			this.LateUpdateAsObservable().Subscribe(delegate
			{
				for (int i = 0; i < tail; i++)
				{
					if (arrayUpdatable[i] != null)
					{
						arrayUpdatable[i].LateUpdateFunc();
					}
				}
			}).AddTo(this);
		}
	}
}
