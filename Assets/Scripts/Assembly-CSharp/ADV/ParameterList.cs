using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;

namespace ADV
{
	public static class ParameterList
	{
		private static readonly List<SceneParameter> list = new List<SceneParameter>();

		public static void Add(SceneParameter param)
		{
			list.Add(param);
		}

		public static void Remove(MonoBehaviour mono)
		{
			list.RemoveAll((SceneParameter p) => p.mono == null || p.mono == mono);
		}

		public static void Init()
		{
			Data data = null;
			if (Singleton<Scene>.IsInstance())
			{
				GameObject commonSpace = Singleton<Scene>.Instance.commonSpace;
				if (commonSpace != null)
				{
					Component destroyComponent = null;
					if (!commonSpace.GetComponent<CommandData>().SafeProc(delegate(CommandData p)
					{
						data = p.data;
						destroyComponent = p;
					}))
					{
						commonSpace.GetComponent<OpenData>().SafeProc(delegate(OpenData p)
						{
							(p.data as Data).SafeProc(delegate(Data castData)
							{
								data = castData;
								destroyComponent = p;
							});
						});
					}
					if (destroyComponent != null)
					{
						Object.Destroy(destroyComponent);
					}
					if (data != null)
					{
						SceneParameter.advScene.nowScene = data.scene;
					}
				}
			}
			if (data == null)
			{
				return;
			}
			foreach (SceneParameter item in list.Where((SceneParameter p) => SceneParameter.IsNowScene(p.mono)))
			{
				item.Init(data);
			}
		}

		public static void Release()
		{
			foreach (SceneParameter item in list.Where((SceneParameter p) => SceneParameter.IsNowScene(p.mono)))
			{
				item.Release();
			}
			if (SceneParameter.advScene != null)
			{
				SceneParameter.advScene.nowScene = null;
			}
		}

		public static void WaitEndProc()
		{
			foreach (SceneParameter item in list.Where((SceneParameter p) => SceneParameter.IsNowScene(p.mono)))
			{
				item.WaitEndProc();
			}
		}
	}
}
