using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Localize.Translate
{
	public class SpriteChangable : MonoBehaviour, IHSpriteChangable
	{
		[SerializeField]
		private int sceneID = -1;

		[SerializeField]
		private int category = -1;

		[SerializeField]
		private string tagName = "group";

		[SerializeField]
		private int startIndex = -1;

		private Dictionary<int, Dictionary<int, Data.Param>> _categorys;

		private Dictionary<int, Data.Param> _data;

		private List<Data.Param> useList = new List<Data.Param>();

		private Sprite[] _sprites;

		private Image image;

		private Dictionary<int, Dictionary<int, Data.Param>> categorys
		{
			get
			{
				return this.GetCache(ref _categorys, () => _categorys = Manager.LoadScene(sceneID, base.gameObject));
			}
		}

		private Dictionary<int, Data.Param> data
		{
			get
			{
				return this.GetCache(ref _data, () => categorys.Get(category));
			}
		}

		private Sprite[] sprites
		{
			get
			{
				return this.GetCache(ref _sprites, () => Load(tagName));
			}
		}

		public void OnChange(int num)
		{
			sprites.SafeProc(num, delegate(Sprite sprite)
			{
				image.sprite = sprite;
			});
		}

		private void Start()
		{
			image = GetComponent<Image>();
			if (image == null)
			{
				Object.Destroy(this);
			}
			else
			{
				OnChange(startIndex);
			}
		}

		private void OnDestroy()
		{
			Manager.Unload(useList);
		}

		private Sprite[] Load(string tagName)
		{
			Data.Param[] array = data.Values.FindTags(tagName).ToArray();
			useList.AddRange(array);
			return array.Select((Data.Param p) => Manager.Convert(p.Load(false))).ToArray();
		}
	}
}
