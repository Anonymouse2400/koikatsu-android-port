using Manager;
using UnityEngine;

namespace ActionGame
{
	public class ClassRoomSelectScene : BaseLoader
	{
		[SerializeField]
		private Canvas _background;

		[SerializeField]
		private ClassRoomList _classRoomList;

		[SerializeField]
		private PreviewCharaList _previewCharaList;

		public bool isVisible
		{
			set
			{
				_background.enabled = value;
			}
		}

		public ClassRoomList classRoomList
		{
			get
			{
				return _classRoomList;
			}
		}

		public PreviewCharaList previewCharaList
		{
			get
			{
				return _previewCharaList;
			}
		}

		public void OpenClassRoom()
		{
			_classRoomList.isVisible = true;
			_previewCharaList.isVisible = false;
		}

		public void OpenPreview()
		{
			_classRoomList.isVisible = false;
			_previewCharaList.isVisible = true;
		}

		private void Start()
		{
			Singleton<Scene>.Instance.sceneFade.SortingOrder();
		}
	}
}
