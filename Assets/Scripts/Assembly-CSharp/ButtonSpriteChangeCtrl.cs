using System;
using Localize.Translate;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteChangeCtrl : MonoBehaviour
{
	[Serializable]
	public class ButtonSprite
	{
		public Sprite main;

		public Sprite sel;

		public Sprite disable;
	}

	public ButtonSprite on = new ButtonSprite();

	public ButtonSprite off = new ButtonSprite();

	private Button button;

	private void Start()
	{
		button = base.gameObject.GetComponent<Button>();
	}

	public void OnChangeValue(bool _isOn)
	{
		if (!(button == null))
		{
			IButtonSpriteChangable component = GetComponent<IButtonSpriteChangable>();
			if (component != null)
			{
				component.OnChange(this);
			}
			ButtonSprite buttonSprite = ((!_isOn) ? off : on);
			if (!(buttonSprite.main == null) && !(buttonSprite.sel == null))
			{
				Image image = button.targetGraphic as Image;
				image.sprite = buttonSprite.main;
				SpriteState spriteState = default(SpriteState);
				spriteState.highlightedSprite = buttonSprite.sel;
				spriteState.pressedSprite = buttonSprite.sel;
				spriteState.disabledSprite = buttonSprite.disable;
				SpriteState spriteState2 = spriteState;
				button.spriteState = spriteState2;
			}
		}
	}
}
