using System;
using System.Collections;
using ChaCustom;
using Illusion.Game;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Localize.Translate
{
	public class CustomFileListSelecter : MonoBehaviour
	{
		[SerializeField]
		private CustomFileListCtrlPointerEvent listCtrl;

		[Header("-1:standby,0:Male,1:Female")]
		[SerializeField]
		private int _sex = -1;

		private ReactiveProperty<ChaFileControl> info = new ReactiveProperty<ChaFileControl>();

		private Manager.ChaFileInfo[] files;

		public Button[] enter { get; private set; }

		public Button[] cancel { get; private set; }

		public int sex
		{
			get
			{
				return _sex;
			}
			set
			{
				_sex = value;
			}
		}

		public event Action<ChaFileControl> onEnter = delegate
		{
		};

		public event Action onCancel = delegate
		{
		};

		public void Initialize()
		{
			files = Manager.CreateChaFileInfo(sex, true);
			FileListControler.Execute(sex, files, listCtrl);
			listCtrl.Create(null);
		}

		private void Awake()
		{
			enter = listCtrl.enter ?? new Button[0];
			cancel = listCtrl.cancel ?? new Button[0];
		}

		private IEnumerator Start()
		{
			while (_sex == -1)
			{
				yield return null;
			}
			Initialize();
			listCtrl.eventOnPointerClick += delegate(CustomFileInfo info)
			{
				this.info.Value = ((info != null) ? files[info.index].chaFile : null);
				Utils.Sound.Play(SystemSE.sel);
			};
			Button[] array = enter;
			foreach (Button button in array)
			{
				info.Select((ChaFileControl p) => p != null).SubscribeToInteractable(button);
				button.OnClickAsObservable().Subscribe(delegate
				{
					this.onEnter(info.Value);
					Utils.Sound.Play(SystemSE.ok_s);
				});
			}
			Button[] array2 = cancel;
			foreach (Button button2 in array2)
			{
				button2.OnClickAsObservable().Subscribe(delegate
				{
					this.onCancel();
					Utils.Sound.Play(SystemSE.cancel);
				});
			}
		}
	}
}
