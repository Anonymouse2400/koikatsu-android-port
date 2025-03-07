using System;
using System.Collections.Generic;
using System.Linq;
using Localize.Translate;
using UniRx;
using UnityEngine;

namespace ActionGame
{
	public class ActionChangeUI : ChangeUIBase
	{
		public enum ActionType
		{
			None = -1,
			Onanism = 0,
			Peeping = 1,
			CharaEvent = 2,
			SummonEvent = 3,
			Lesbian = 4
		}

		[Serializable]
		public class TypeReactiveProperty : ReactiveProperty<ActionType>
		{
			public TypeReactiveProperty()
			{
			}

			public TypeReactiveProperty(ActionType initialValue)
				: base(initialValue)
			{
			}
		}

		[SerializeField]
		private Canvas _canvas;

		private TypeReactiveProperty _type;

		private ReactiveCollection<ActionType> typeList = new ReactiveCollection<ActionType>();

		public bool isVisible
		{
			get
			{
				return _canvas.enabled;
			}
			set
			{
				_canvas.enabled = value;
			}
		}

		public void Set(ActionType type)
		{
			if (type == ActionType.None)
			{
				typeList.Clear();
			}
			else
			{
				typeList.Add(type);
			}
		}

		public void Remove(ActionType type)
		{
			typeList.Remove(type);
		}

		protected override void Set(int i)
		{
			if (i == -1)
			{
				target.enabled = false;
				return;
			}
			target.enabled = true;
			target.sprite = sprites[i];
			target.SetNativeSize();
		}

		private void Start()
		{
			List<Data.Param> list = new List<Data.Param>();
			Dictionary<int, Data.Param> self = base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.ACTION).Get(4);
			for (int j = 0; j < sprites.Length; j++)
			{
				Data.Param param = self.Get(j);
				Sprite sprite = Localize.Translate.Manager.Convert(param.Load(false));
				if (!(sprite == null))
				{
					list.Add(param);
					sprites[j] = sprite;
				}
			}
			Localize.Translate.Manager.Unload(list);
			typeList.ObserveReset().Subscribe(delegate
			{
				_type.Value = ActionType.None;
			});
			typeList.ObserveAdd().Subscribe(delegate(CollectionAddEvent<ActionType> x)
			{
				_type.Value = x.Value;
			});
			typeList.ObserveRemove().Subscribe(delegate
			{
				_type.Value = (typeList.Any() ? typeList[typeList.Count - 1] : ActionType.None);
			});
			_type = new TypeReactiveProperty(ActionType.None);
			_type.TakeUntilDestroy(this).Subscribe(delegate(ActionType i)
			{
				Set((int)i);
			});
		}
	}
}
