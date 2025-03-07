using UnityEngine;

namespace NodeCanvas.DialogueTrees
{
	[AddComponentMenu("NodeCanvas/Dialogue Actor")]
	public class DialogueActor : MonoBehaviour, IDialogueActor
	{
		[SerializeField]
		protected string _name;

		[SerializeField]
		protected Texture2D _portrait;

		[SerializeField]
		protected Color _dialogueColor = Color.white;

		[SerializeField]
		protected Vector3 _dialogueOffset;

		private Sprite _portraitSprite;

		public new string name
		{
			get
			{
				return _name;
			}
		}

		public Texture2D portrait
		{
			get
			{
				return _portrait;
			}
		}

		public Sprite portraitSprite
		{
			get
			{
				if (_portraitSprite == null && portrait != null)
				{
					_portraitSprite = Sprite.Create(portrait, new Rect(0f, 0f, portrait.width, portrait.height), new Vector2(0.5f, 0.5f));
				}
				return _portraitSprite;
			}
		}

		public Color dialogueColor
		{
			get
			{
				return _dialogueColor;
			}
		}

		public Vector3 dialoguePosition
		{
			get
			{
				return Vector3.Scale(base.transform.position + _dialogueOffset, base.transform.localScale);
			}
		}

		//Transform IDialogueActor.get_transform()
		//{
		//	return base.transform;
		//}
	}
}
