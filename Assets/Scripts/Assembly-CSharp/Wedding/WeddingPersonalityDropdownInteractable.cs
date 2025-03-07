using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Wedding
{
	internal class WeddingPersonalityDropdownInteractable : MonoBehaviour
	{
		[SerializeField]
		private Transform content;

		[SerializeField]
		private WeddingScene scene;

		private void Start()
		{
			HashSet<int> personality = Singleton<Game>.Instance.weddingData.personality;
			foreach (var item in content.Children().Skip(1).Select((Transform t, int i) => new { t, i }))
			{
				item.t.GetComponent<Selectable>().interactable = personality.Contains(scene.personalityes[item.i].No);
			}
		}
	}
}
