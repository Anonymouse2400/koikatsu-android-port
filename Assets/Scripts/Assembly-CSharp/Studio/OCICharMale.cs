using Manager;
using UnityEngine;

namespace Studio
{
	public class OCICharMale : OCIChar
	{
		public ChaControl male
		{
			get
			{
				return charInfo;
			}
		}

		public override void OnDelete()
		{
			base.OnDelete();
			Singleton<Character>.Instance.DeleteChara(male);
		}

		public override void SetCoordinateInfo(ChaFileDefine.CoordinateType _type, bool _force = false)
		{
			base.SetCoordinateInfo(_type, _force);
		}

		public override void SetClothesStateAll(int _state)
		{
			base.SetClothesStateAll(_state);
			male.SetClothesStateAll((byte)_state);
		}

		public override void ChangeChara(string _path)
		{
			base.ChangeChara(_path);
		}

		public override void SetVisibleSimple(bool _flag)
		{
			base.oiCharInfo.visibleSimple = _flag;
			male.ChangeSimpleBodyDraw(_flag);
		}

		public override void SetSimpleColor(Color _color)
		{
			base.SetSimpleColor(_color);
			male.ChangeSimpleBodyColor(_color);
		}

		public override void LoadClothesFile(string _path)
		{
			base.LoadClothesFile(_path);
		}
	}
}
