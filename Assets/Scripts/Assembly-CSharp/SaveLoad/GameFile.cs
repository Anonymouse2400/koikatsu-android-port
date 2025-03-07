using System;
using System.IO;
using System.Linq;
using Illusion.Game;
using Illusion.Game.Extensions;
using Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SaveLoad
{
	[Serializable]
	public class GameFile : IDisposable
	{
		[Serializable]
		public class FileButton
		{
			[SerializeField]
			private Button _button;

			[SerializeField]
			private Text _name;

			[SerializeField]
			private Text _info;

			private string initName;

			private string initInfo;

			public string file { get; private set; }

			public Button button
			{
				get
				{
					return _button;
				}
			}

			public Text name
			{
				get
				{
					return _name;
				}
			}

			public Text info
			{
				get
				{
					return _info;
				}
			}

			public FileButton(Button button, Text name, Text info)
			{
				_button = button;
				_name = name;
				_info = info;
			}

			public void Initialize()
			{
				initName = _name.text;
				initInfo = _info.text;
			}

			public void SetName()
			{
				name.text = SaveData.LoadTitle(file);
				info.text = File.GetLastWriteTime(SaveData.Path + '/' + file).ToString("yyyy/MM/dd/HH:mm");
			}

			public void SetName(bool interactable, int page, params string[] files)
			{
				string text = string.Format("{0}{1:00}", "file", page + int.Parse(_button.name));
				file = text + ".dat";
				bool flag = files.Contains(text);
				if (flag)
				{
					SetName();
				}
				else
				{
					ResetName();
				}
				if (interactable)
				{
					button.interactable = flag;
				}
			}

			private void ResetName()
			{
				_name.text = initName;
				_info.text = initInfo;
			}
		}

		[SerializeField]
		private Button _next;

		[SerializeField]
		private Button _prev;

		[SerializeField]
		private Button _nextTop;

		[SerializeField]
		private Button _prevTop;

		[SerializeField]
		private int _pageMax = 9;

		private static IntReactiveProperty _page = new IntReactiveProperty(0);

		[SerializeField]
		private TextMeshProUGUI _pageText;

		[SerializeField]
		private FileButton[] _fileButtons;

		private CompositeDisposable disposables = new CompositeDisposable();

		public FileButton[] fileButtons
		{
			get
			{
				return _fileButtons;
			}
		}

		public void Initialize(bool interactable)
		{
			FileButton[] array = _fileButtons;
			foreach (FileButton fileButton in array)
			{
				fileButton.Initialize();
			}
			_page.Select((int i) => i < _pageMax).SubscribeToInteractable(_next).AddTo(disposables);
			_page.Select((int i) => i > 0).SubscribeToInteractable(_prev).AddTo(disposables);
			_page.Select((int i) => i < _pageMax).SubscribeToInteractable(_nextTop).AddTo(disposables);
			_page.Select((int i) => i > 0).SubscribeToInteractable(_prevTop).AddTo(disposables);
			_next.OnClickAsObservable().Subscribe(delegate
			{
				_page.Value++;
			}).AddTo(disposables);
			_prev.OnClickAsObservable().Subscribe(delegate
			{
				_page.Value--;
			}).AddTo(disposables);
			_nextTop.OnClickAsObservable().Subscribe(delegate
			{
				_page.Value = _pageMax;
			}).AddTo(disposables);
			_prevTop.OnClickAsObservable().Subscribe(delegate
			{
				_page.Value = 0;
			}).AddTo(disposables);
			new Button[4] { _next, _prev, _nextTop, _prevTop }.Select(UnityUIComponentExtensions.OnClickAsObservable).Merge().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
			})
				.AddTo(disposables);
			_page.Select((int i) => i * 10).Subscribe(delegate(int i)
			{
				string[] files = Game.SaveFiles.Select(Path.GetFileNameWithoutExtension).ToArray();
				FileButton[] array2 = _fileButtons;
				foreach (FileButton fileButton2 in array2)
				{
					fileButton2.SetName(interactable, i, files);
				}
			}).AddTo(disposables);
			_page.Select((int i) => string.Format("{0}/{1}", i + 1, _pageMax + 1)).SubscribeToText(_pageText).AddTo(disposables);
		}

		public void Dispose()
		{
			disposables.Clear();
		}

		public void SetupFiles(FileButton[] fileButtons)
		{
			_fileButtons = fileButtons;
		}
	}
}
