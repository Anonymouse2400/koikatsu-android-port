using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using UniRx;
using UnityEngine;

namespace ADV.EventCG
{
	public class CameraData : MonoBehaviour
	{
		[SerializeField]
		[Header("カメラデータ")]
		private float _fieldOfView;

		private float? baseFieldOfView;

		[Header("身長補正座標")]
		[SerializeField]
		private Vector3 _minPos;

		[SerializeField]
		private Vector3 _maxPos;

		[SerializeField]
		[Header("身長補正角度")]
		private Vector3 _minAng;

		[SerializeField]
		private Vector3 _maxAng;

		private ReactiveCollection<ChaControl> _chaCtrlList = new ReactiveCollection<ChaControl>();

		private Vector3 basePos;

		private Vector3 baseAng;

		public bool initialized { get; private set; }

		public float fieldOfView
		{
			get
			{
				return _fieldOfView;
			}
			set
			{
				_fieldOfView = value;
			}
		}

		public ReactiveCollection<ChaControl> chaCtrlList
		{
			get
			{
				return _chaCtrlList;
			}
		}

		public void SetCameraData(Camera cam)
		{
			baseFieldOfView = cam.fieldOfView;
		}

		public void RepairCameraData(Camera cam)
		{
			if (baseFieldOfView.HasValue)
			{
				cam.fieldOfView = baseFieldOfView.Value;
			}
		}

		private void Calculate()
		{
			if (_chaCtrlList.Any())
			{
				float shape = _chaCtrlList.Average((ChaControl p) => p.GetShapeBodyValue(0));
				Vector3 shapeLerpPositionValue = MotionIK.GetShapeLerpPositionValue(shape, _minPos, _maxPos);
				Vector3 shapeLerpAngleValue = MotionIK.GetShapeLerpAngleValue(shape, _minAng, _maxAng);
				base.transform.SetPositionAndRotation(basePos + shapeLerpPositionValue, Quaternion.Euler(baseAng + shapeLerpAngleValue));
			}
		}

		private void OnEnable()
		{
			basePos = base.transform.position;
			baseAng = base.transform.eulerAngles;
		}

		private void OnDisable()
		{
			base.transform.position = basePos;
			base.transform.eulerAngles = baseAng;
		}

		private IEnumerator Start()
		{
			base.enabled = false;
			Data componentInParent = GetComponentInParent<Data>();
			List<ChaControl> list = (from p in componentInParent.chaRoot.Children()
				select p.GetComponent<ChaControl>() into p
				where p != null
				select p).ToList();
			list.ForEach(delegate(ChaControl item)
			{
				_chaCtrlList.Add(item);
			});
			_chaCtrlList.ObserveAdd().Subscribe(delegate
			{
				Calculate();
			});
			_chaCtrlList.ObserveRemove().Subscribe(delegate
			{
				Calculate();
			});
			_chaCtrlList.AddTo(this);
			base.enabled = true;
			initialized = true;
			Calculate();
			yield break;
		}
	}
}
