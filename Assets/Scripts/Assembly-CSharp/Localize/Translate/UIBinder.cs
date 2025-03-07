using System.Collections.Generic;
using UnityEngine;

namespace Localize.Translate
{
	[DefaultExecutionOrder(-1)]
	public class UIBinder : BaseLoader
	{
		[SerializeField]
		private int sceneID = -1;

		[SerializeField]
		private int category = -1;

		[SerializeField]
		private UIBindData[] binds;

		private List<Data.Param> useList = new List<Data.Param>();

		private void Load()
		{
			Manager.LoadScene(sceneID, base.gameObject);
			Dictionary<int, Data.Param> dic = Manager.GetCategory(sceneID, category);
			UIBindData[] array = binds;
			foreach (UIBindData uIBindData in array)
			{
				useList.Add(uIBindData.Bind(dic, false));
			}
		}

		private void Unload()
		{
			Manager.Unload(useList);
			useList.Clear();
		}

		private void Start()
		{
			Load();
		}

		private void OnDestroy()
		{
			Unload();
		}
	}
}
