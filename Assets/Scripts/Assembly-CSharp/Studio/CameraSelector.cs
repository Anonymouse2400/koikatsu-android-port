using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Studio
{
	public class CameraSelector : MonoBehaviour
	{
		[SerializeField]
		private TMP_Dropdown dropdown;

		private List<OCICamera> listCamera;

		private int index;

		private void OnValueChanged(int _index)
		{
			if (_index == 0)
			{
				Singleton<Studio>.Instance.ChangeCamera(Singleton<Studio>.Instance.ociCamera, false);
			}
			else
			{
				Singleton<Studio>.Instance.ChangeCamera(listCamera[_index - 1], true);
			}
		}

		public void SetCamera(OCICamera _ocic)
		{
			int num = ((_ocic != null) ? listCamera.FindIndex((OCICamera c) => c == _ocic) : (-1));
			dropdown.value = num + 1;
		}

		public void NextCamera()
		{
			if (!listCamera.IsNullOrEmpty())
			{
				index = (index + 1) % (listCamera.Count + 1);
				OnValueChanged(index);
			}
		}

		public void Init()
		{
			dropdown.ClearOptions();
			List<ObjectInfo> list = ObjectInfoAssist.Find(5);
			listCamera = list.Select((ObjectInfo i) => Studio.GetCtrlInfo(i.dicKey) as OCICamera).ToList();
			index = 0;
			List<TMP_Dropdown.OptionData> list2 = ((!list.IsNullOrEmpty()) ? list.Select((ObjectInfo c) => new TMP_Dropdown.OptionData((Studio.GetCtrlInfo(c.dicKey) as OCICamera).name)).ToList() : new List<TMP_Dropdown.OptionData>());
			list2.Insert(0, new TMP_Dropdown.OptionData("-"));
			dropdown.options = list2;
			dropdown.interactable = !list.IsNullOrEmpty();
			SetCamera(Singleton<Studio>.Instance.ociCamera);
		}

		private void Awake()
		{
			dropdown.onValueChanged.AddListener(OnValueChanged);
			dropdown.interactable = false;
		}
	}
}
