using Illusion.Game;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Wedding
{
	public abstract class WeddingSelectBaseView : MonoBehaviour
	{
		[SerializeField]
		private Canvas _canvas;

		[SerializeField]
		private Button _btCharaFile;

		[SerializeField]
		private Button _btCharaDefault;

		[SerializeField]
		private Button _btSchool;

		[SerializeField]
		private Button _btFromSchool;

		[SerializeField]
		private Button _btGymsuit;

		[SerializeField]
		private Button _btSwimsuit;

		[SerializeField]
		private Button _btClub;

		[SerializeField]
		private Button _btPlain;

		[SerializeField]
		private Button _btPajamas;

		[SerializeField]
		private Button _btClothFile;

		[SerializeField]
		private Button _btDefCloth;

		[SerializeField]
		private Button _btInner;

		[SerializeField]
		private Button _btOuter;

		public Canvas canvas
		{
			get
			{
				return _canvas;
			}
		}

		public Button btCharaFile
		{
			get
			{
				return _btCharaFile;
			}
		}

		public Button btCharaDefault
		{
			get
			{
				return _btCharaDefault;
			}
		}

		public Button btSchool
		{
			get
			{
				return _btSchool;
			}
		}

		public Button btFromSchool
		{
			get
			{
				return _btFromSchool;
			}
		}

		public Button btGymsuit
		{
			get
			{
				return _btGymsuit;
			}
		}

		public Button btSwimsuit
		{
			get
			{
				return _btSwimsuit;
			}
		}

		public Button btClub
		{
			get
			{
				return _btClub;
			}
		}

		public Button btPlain
		{
			get
			{
				return _btPlain;
			}
		}

		public Button btPajamas
		{
			get
			{
				return _btPajamas;
			}
		}

		public Button btClothFile
		{
			get
			{
				return _btClothFile;
			}
		}

		public Button btDefCloth
		{
			get
			{
				return _btDefCloth;
			}
		}

		public Button btInner
		{
			get
			{
				return _btInner;
			}
		}

		public Button btOuter
		{
			get
			{
				return _btOuter;
			}
		}

		public void Initialize()
		{
			Button[] array = new Button[2] { _btCharaFile, _btClothFile };
			foreach (Button button in array)
			{
				button.OnClickAsObservable().Subscribe(delegate
				{
					Utils.Sound.Play(SystemSE.window_o);
				});
			}
			Button[] array2 = new Button[11]
			{
				_btCharaDefault, _btSchool, _btFromSchool, _btGymsuit, _btSwimsuit, _btClub, _btPlain, _btPajamas, _btDefCloth, _btInner,
				_btOuter
			};
			foreach (Button button2 in array2)
			{
				button2.OnClickAsObservable().Subscribe(delegate
				{
					Utils.Sound.Play(SystemSE.sel);
				});
			}
		}
	}
}
