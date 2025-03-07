using System.Collections;
using Manager;
using UniRx;
using UnityEngine;

namespace Wedding
{
	public class WeddingCharaSelect : BaseLoader
	{
		[SerializeField]
		private WeddingPreviewCharaList _previewCharaList;

		[Header("-1:standby,0:Male,1:Female")]
		[SerializeField]
		private int _sex = -1;

		private Subject<ChaFileControl> _chaFileSubject = new Subject<ChaFileControl>();

		private CameraControl_Ver2 _camCtrl;

		private BaseCameraControl_Ver2.NoCtrlFunc _NoCtrlCondition;

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

		public Subject<ChaFileControl> chaFileSubject
		{
			get
			{
				return _chaFileSubject;
			}
		}

		public CameraControl_Ver2 camCtrl
		{
			set
			{
				_camCtrl = value;
			}
		}

		public BaseCameraControl_Ver2.NoCtrlFunc NoCtrlCondition
		{
			set
			{
				_NoCtrlCondition = value;
			}
		}

		private IEnumerator Start()
		{
			_previewCharaList.canvas.enabled = false;
			while (_sex == -1)
			{
				yield return null;
			}
			bool processed = false;
			_previewCharaList.canvas.enabled = true;
			_previewCharaList.CharFile.sex = _sex;
			_previewCharaList.onEnter += delegate(ChaFileControl file)
			{
				if (!processed)
				{
					processed = true;
					_chaFileSubject.OnNext(file);
					if (_camCtrl != null)
					{
						_camCtrl.NoCtrlCondition = _NoCtrlCondition;
					}
					Singleton<Scene>.Instance.UnLoad();
				}
			};
			_previewCharaList.onCancel += delegate
			{
				if (!processed)
				{
					processed = true;
					if (_camCtrl != null)
					{
						_camCtrl.NoCtrlCondition = _NoCtrlCondition;
					}
					Singleton<Scene>.Instance.UnLoad();
				}
			};
		}
	}
}
