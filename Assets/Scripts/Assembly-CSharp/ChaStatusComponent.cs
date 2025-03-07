using System;
using System.Linq;
using Illusion.Extensions;
using TMPro;
using UnityEngine;

public class ChaStatusComponent : MonoBehaviour
{
	[Serializable]
	public class InfoObject
	{
		private bool _enabled = true;

		[SerializeField]
		private GameObject _obj;

		[SerializeField]
		private TextMeshProUGUI[] _infos;

		public bool enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}

		public GameObject obj
		{
			get
			{
				return _obj;
			}
		}

		public TextMeshProUGUI[] infos
		{
			get
			{
				return _infos;
			}
		}
	}

	public GameObject objHeart;

	public GameObject objMember;

	public StudentCardControlComponent cmpStudentCard;

	[SerializeField]
	private InfoObject[] _infoObject;

	public GameObject[] objInfo { get; set; }

	public SaveData.Heroine heroine { get; set; }

	public InfoObject[] infoObject
	{
		get
		{
			return _infoObject;
		}
	}

	public void SetActiveInfo(int index, bool active)
	{
		_infoObject.SafeProc(index, delegate(InfoObject o)
		{
			o.obj.SetActiveIfDifferent(o.enabled && active);
		});
	}

	public void SetActiveInfoAll(int count)
	{
		if (_infoObject != null)
		{
			int num = _infoObject.Count((InfoObject o) => o.enabled);
			for (int i = 0; i < num; i++)
			{
				SetActiveInfo(i, count % num == i);
			}
		}
	}

	public TextMeshProUGUI[] GetinfoTexts(int index)
	{
		return ((uint)index < _infoObject.Length) ? _infoObject[index].infos : null;
	}
}
