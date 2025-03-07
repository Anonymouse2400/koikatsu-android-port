using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Localize.Translate
{
	public class ButtonSpriteChangable : MonoBehaviour, IButtonSpriteChangable
	{
		[SerializeField]
		private int sceneID = -1;

		[SerializeField]
		private int category = -1;

		[SerializeField]
		private string on = "on";

		[SerializeField]
		private string off = "off";

		[Header("開始時の適用")]
		[SerializeField]
		private bool isOn;

		private Dictionary<int, Dictionary<int, Data.Param>> _categorys;

		private Dictionary<int, Data.Param> _data;

		private List<Data.Param> useList = new List<Data.Param>();

		private ButtonSpriteChangeCtrl.ButtonSprite _onSprite;

		private ButtonSpriteChangeCtrl.ButtonSprite _offSprite;

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

		private ButtonSpriteChangeCtrl.ButtonSprite onSprite
		{
			get
			{
				return this.GetCache(ref _onSprite, () => Load(on));
			}
		}

		private ButtonSpriteChangeCtrl.ButtonSprite offSprite
		{
			get
			{
				return this.GetCache(ref _offSprite, () => Load(off));
			}
		}

		private void Start()
		{
			Button component = GetComponent<Button>();
			if (!(component == null))
			{
				ButtonSpriteChangeCtrl.ButtonSprite buttonSprite = ((!isOn) ? offSprite : onSprite);
				Manager.Replace(component, buttonSprite.main);
				component.spriteState = new SpriteState
				{
					highlightedSprite = buttonSprite.sel,
					pressedSprite = buttonSprite.sel,
					disabledSprite = buttonSprite.disable
				};
			}
		}

		private void OnDestroy()
		{
			Manager.Unload(useList);
		}

		void IButtonSpriteChangable.OnChange(ButtonSpriteChangeCtrl changeCtrl)
		{
			Set(changeCtrl.on, onSprite);
			Set(changeCtrl.off, offSprite);
		}

		private void Set(ButtonSpriteChangeCtrl.ButtonSprite src, ButtonSpriteChangeCtrl.ButtonSprite dst)
		{
			dst.main.SafeProc(delegate(Sprite sp)
			{
				src.main = sp;
			});
			dst.sel.SafeProc(delegate(Sprite sp)
			{
				src.sel = sp;
			});
			dst.disable.SafeProc(delegate(Sprite sp)
			{
				src.disable = sp;
			});
		}

		private ButtonSpriteChangeCtrl.ButtonSprite Load(string tagName)
		{
			ButtonSpriteChangeCtrl.ButtonSprite btSprite = new ButtonSpriteChangeCtrl.ButtonSprite();
			Data.Param[] array = data.Values.FindTags(tagName).ToArray();
			useList.AddRange(array);
			Sprite[] array2 = array.Select((Data.Param p) => Manager.Convert(p.Load(false))).ToArray();
			int num = 0;
			array2.SafeProc(num++, delegate(Sprite sprite)
			{
				btSprite.main = sprite;
			});
			array2.SafeProc(num++, delegate(Sprite sprite)
			{
				btSprite.sel = sprite;
			});
			array2.SafeProc(num++, delegate(Sprite sprite)
			{
				btSprite.disable = sprite;
			});
			return btSprite;
		}
	}
}
