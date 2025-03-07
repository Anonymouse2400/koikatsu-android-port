using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomFileWindow : MonoBehaviour
	{
		public enum FileWindowType
		{
			CharaSave = 0,
			CharaLoad = 1,
			CoordinateSave = 2,
			CoordinateLoad = 3,
			FaceSave = 4,
			FaceLoad = 5,
			BodySave = 6,
			BodyLoad = 7,
			HairSave = 8,
			HairLoad = 9,
			TopSave = 10,
			TopLoad = 11,
			BotSave = 12,
			BotLoad = 13,
			BraSave = 14,
			BraLoad = 15,
			ShortsSave = 16,
			ShortsLoad = 17,
			GlovesSave = 18,
			GlovesLoad = 19,
			PanstSave = 20,
			PanstLoad = 21,
			SocksSave = 22,
			SocksLoad = 23,
			ShoesSave = 24,
			ShoesLoad = 25,
			AccessorySave = 26,
			AccessoryLoad = 27,
			MakeupSave = 28,
			MakeupLoad = 29
		}

		[Serializable]
		private class TypeReactiveProperty : ReactiveProperty<FileWindowType>
		{
			public TypeReactiveProperty()
			{
			}

			public TypeReactiveProperty(FileWindowType initialValue)
				: base(initialValue)
			{
			}
		}

		[SerializeField]
		private TypeReactiveProperty _fwType = new TypeReactiveProperty(FileWindowType.CharaSave);

		[SerializeField]
		private TextMeshProUGUI textTitle;

		[SerializeField]
		private Button btnClose;

		[SerializeField]
		private GameObject objSave;

		[SerializeField]
		private GameObject objLoad;

		[SerializeField]
		private GameObject objCharaLoad;

		[SerializeField]
		private GameObject objCoordinateLoad;

		[SerializeField]
		private GameObject objClose;

		[SerializeField]
		private GameObject objTglCategory;

		[SerializeField]
		private GameObject objTglTemp;

		[SerializeField]
		private Toggle _tglChaLoadFace;

		[SerializeField]
		private Toggle _tglChaLoadBody;

		[SerializeField]
		private Toggle _tglChaLoadHair;

		[SerializeField]
		private Toggle _tglChaLoadParam;

		[SerializeField]
		private Toggle _tglChaLoadCoorde;

		[SerializeField]
		private Button _btnChaLoadAllOn;

		[SerializeField]
		private Button _btnChaLoadAllOff;

		[SerializeField]
		private Button _btnChaLoadLoad;

		[SerializeField]
		private Button _btnDelete;

		[SerializeField]
		private Button _btnSave;

		[SerializeField]
		private Button _btnOverwrite;

		[SerializeField]
		private Toggle _tglCoordeLoadClothes;

		[SerializeField]
		private Toggle _tglCoordeLoadAcs;

		[SerializeField]
		private Button _btnCoordeLoadAllOn;

		[SerializeField]
		private Button _btnCoordeLoadAllOff;

		[SerializeField]
		private Button _btnCoordeLoadLoad;

		[SerializeField]
		private Button _btnLoad;

		public FileWindowType fwType
		{
			get
			{
				return _fwType.Value;
			}
			set
			{
				_fwType.Value = value;
			}
		}

		public Toggle[] tglCategory { get; private set; }

		public Toggle tglChaLoadFace
		{
			get
			{
				return _tglChaLoadFace;
			}
		}

		public Toggle tglChaLoadBody
		{
			get
			{
				return _tglChaLoadBody;
			}
		}

		public Toggle tglChaLoadHair
		{
			get
			{
				return _tglChaLoadHair;
			}
		}

		public Toggle tglChaLoadParam
		{
			get
			{
				return _tglChaLoadParam;
			}
		}

		public Toggle tglChaLoadCoorde
		{
			get
			{
				return _tglChaLoadCoorde;
			}
		}

		public Button btnChaLoadAllOn
		{
			get
			{
				return _btnChaLoadAllOn;
			}
		}

		public Button btnChaLoadAllOff
		{
			get
			{
				return _btnChaLoadAllOff;
			}
		}

		public Button btnChaLoadLoad
		{
			get
			{
				return _btnChaLoadLoad;
			}
		}

		public Button btnDelete
		{
			get
			{
				return _btnDelete;
			}
		}

		public Button btnSave
		{
			get
			{
				return _btnSave;
			}
		}

		public Button btnOverwrite
		{
			get
			{
				return _btnOverwrite;
			}
		}

		public Toggle tglCoordeLoadClothes
		{
			get
			{
				return _tglCoordeLoadClothes;
			}
		}

		public Toggle tglCoordeLoadAcs
		{
			get
			{
				return _tglCoordeLoadAcs;
			}
		}

		public Button btnCoordeLoadAllOn
		{
			get
			{
				return _btnCoordeLoadAllOn;
			}
		}

		public Button btnCoordeLoadAllOff
		{
			get
			{
				return _btnCoordeLoadAllOff;
			}
		}

		public Button btnCoordeLoadLoad
		{
			get
			{
				return _btnCoordeLoadLoad;
			}
		}

		public Button btnLoad
		{
			get
			{
				return _btnLoad;
			}
		}

		public event Action<FileWindowType> eventUpdateWindow = delegate
		{
		};

		public static bool ToggleCheck(Toggle toggle, bool defaultValue = false)
		{
			return (!(toggle == null)) ? toggle.isOn : defaultValue;
		}

		private IEnumerator Start()
		{
			yield return new WaitUntil(() => null != Singleton<CustomBase>.Instance.lstFileList);
			Initialize();
			_fwType.TakeUntilDestroy(this).Subscribe(delegate
			{
				UpdateWindow();
			});
			if (btnClose != null)
			{
				btnClose.OnClickAsObservable().Subscribe(delegate
				{
					base.gameObject.SetActiveIfDifferent(false);
				});
			}
		}

		private void Initialize()
		{
			objSave.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(false);
			});
			objLoad.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(false);
			});
			objCharaLoad.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(false);
			});
			objCoordinateLoad.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(false);
			});
			objClose.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(false);
			});
		}

		public void UpdateWindow()
		{
			Setting(Singleton<CustomBase>.Instance.lstFileList.FirstOrDefault((ExcelData.Param x) => fwType.ToString() == x.list[1]));
			this.eventUpdateWindow(_fwType.Value);
		}

		private static bool isActive(string s)
		{
			return s == "1";
		}

		private bool Setting(ExcelData.Param setting)
		{
			if (setting == null)
			{
				return false;
			}
			textTitle.SafeProcObject(delegate(TextMeshProUGUI text)
			{
				text.text = setting.list[2];
			});
			objSave.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(isActive(setting.list[3]));
			});
			objLoad.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(isActive(setting.list[4]));
			});
			objCharaLoad.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(isActive(setting.list[5]));
			});
			objCoordinateLoad.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(isActive(setting.list[6]));
			});
			objClose.SafeProcObject(delegate(GameObject o)
			{
				o.SetActiveIfDifferent(isActive(setting.list[7]));
			});
			Transform parent = null;
			if (objTglCategory != null)
			{
				objTglCategory.SetActiveIfDifferent(isActive(setting.list[8]));
				parent = objTglCategory.transform;
			}
			List<Toggle> lstTgl = new List<Toggle>();
			int num = setting.list.Count - 9;
			for (int i = 0; i < num && isActive(setting.list[9 + i]); i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(objTglTemp, parent, false);
				if (gameObject == null)
				{
					break;
				}
				Transform transform = gameObject.transform.Find("textCate");
				if (transform == null)
				{
					break;
				}
				transform.GetComponent<TextMeshProUGUI>().SafeProcObject(delegate(TextMeshProUGUI text)
				{
					text.text = setting.list[9 + i];
				});
				gameObject.SetActiveIfDifferent(true);
				gameObject.GetComponent<Toggle>().SafeProcObject(delegate(Toggle tgl)
				{
					lstTgl.Add(tgl);
				});
			}
			tglCategory = lstTgl.ToArray();
			return true;
		}
	}
}
