using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Illusion.Game;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace Illusion.Component
{
	public class ShortcutKey : MonoBehaviour
	{
		[Serializable]
		public class Proc
		{
			public KeyCode keyCode;

			public bool enabled = true;

			public UnityEvent call = new UnityEvent();

			public int refCount { get; set; }
		}

		private class ShortcutKeyList
		{
			private List<ShortcutKey> list = new List<ShortcutKey>();

			public void Add(ShortcutKey sk)
			{
				foreach (ShortcutKey item in list)
				{
					foreach (Proc proc in item.procList)
					{
						foreach (Proc proc2 in sk.procList)
						{
							if (proc2.keyCode == proc.keyCode)
							{
								proc.refCount++;
								proc.enabled = false;
							}
						}
					}
				}
				list.Insert(0, sk);
			}

			public bool Remove(ShortcutKey sk)
			{
				if (!list.Remove(sk))
				{
					return false;
				}
				foreach (ShortcutKey item in list)
				{
					foreach (Proc proc in item.procList)
					{
						foreach (Proc proc2 in sk.procList)
						{
							if (proc2.keyCode == proc.keyCode)
							{
								if (--proc.refCount == 0)
								{
									proc.enabled = true;
								}
								if (proc.refCount >= 0)
								{
								}
							}
						}
					}
				}
				return true;
			}
		}

		private static bool _Enable = true;

		public List<Proc> procList = new List<Proc>();

		private static ShortcutKeyList list = new ShortcutKeyList();

		public static bool Enable
		{
			set
			{
				_Enable = value;
			}
		}

		public static bool IsReglate(string sceneName)
		{
			if (!Singleton<Scene>.IsInstance())
			{
				return true;
			}
			switch (Singleton<Scene>.Instance.LoadSceneName)
			{
			case "Init":
			case "Logo":
				return true;
			default:
				if (Singleton<Scene>.Instance.IsNowLoadingFade)
				{
					return true;
				}
				if (Singleton<Scene>.Instance.NowSceneNames.Contains(sceneName))
				{
					return true;
				}
				return false;
			}
		}

		public void _GameEnd()
		{
			if (!IsReglate("Exit"))
			{
				Illusion.Game.Utils.Scene.GameEnd();
			}
		}

		public void _ReturnTitle()
		{
			if (!IsReglate("Title") && !Singleton<Scene>.Instance.IsOverlap)
			{
				Observable.FromCoroutine((CancellationToken _) => Illusion.Game.Utils.Scene.ReturnTitle()).StartAsCoroutine();
			}
		}

		public void _OpenConfig()
		{
			string text = "Config";
			if (!IsReglate(text) && !Singleton<Scene>.Instance.IsOverlap)
			{
				Singleton<Scene>.Instance.LoadReserve(new Scene.Data
				{
					levelName = text,
					isAdd = true
				}, false);
			}
		}

		public void _OpenShortcutKey()
		{
			string sceneName = "Shortcut";
			if (IsReglate(sceneName) || Singleton<Scene>.Instance.IsOverlap)
			{
				return;
			}
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = sceneName,
				isAdd = true,
				onLoad = delegate
				{
					ShortcutScene rootComponent = Scene.GetRootComponent<ShortcutScene>(sceneName);
					if (!(rootComponent == null))
					{
						rootComponent.page = (Singleton<Scene>.Instance.NowSceneNames.Contains("H") ? 1 : 0);
					}
				}
			}, false);
		}

		public void _OpenTutorial()
		{
			string sceneName = "Tutorial";
			if (!IsReglate(sceneName) && !Singleton<Scene>.Instance.IsOverlap)
			{
				Illusion.Game.Utils.Scene.OpenTutorial(2, true);
			}
		}

		private IEnumerator Start()
		{
			base.enabled = false;
			while (!procList.Any())
			{
				yield return null;
			}
			list.Add(this);
			base.enabled = true;
		}

		private void OnDestroy()
		{
			list.Remove(this);
		}

		private void Update()
		{
			if (!_Enable)
			{
				return;
			}
			foreach (Proc proc in procList)
			{
				if (proc.enabled && Input.GetKeyDown(proc.keyCode))
				{
					proc.call.Invoke();
				}
			}
		}
	}
}
