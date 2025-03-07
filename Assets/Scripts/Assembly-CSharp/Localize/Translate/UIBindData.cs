using System.Collections.Generic;
using UnityEngine;

namespace Localize.Translate
{
	public class UIBindData : MonoBehaviour
	{
		[SerializeField]
		private int ID = -1;

		[SerializeField]
		private GameObject target;

		private bool isUnload;

		public Data.Param data { get; private set; }

		public Data.Param Bind(Dictionary<int, Data.Param> dic, bool isUnload)
		{
			Unload();
			this.isUnload = isUnload;
			if (target == null)
			{
				return null;
			}
			if (ID == -100)
			{
				Manager.BindFont(target);
				return null;
			}
			Data.Param value;
			if (!dic.TryGetValue(ID, out value))
			{
				return null;
			}
			Manager.Bind(target, value, isUnload);
			return data = value;
		}

		private void Unload()
		{
			if (isUnload)
			{
				Manager.Unload(data);
				data = null;
			}
		}

		private void OnDestroy()
		{
			Unload();
		}
	}
}
