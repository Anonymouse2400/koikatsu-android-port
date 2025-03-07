using ActionGame.MapObject;
using Illusion.CustomAttributes;
using IllusionUtility.GetUtility;
using UnityEngine;

namespace H
{
	public class HPointData : MonoBehaviour
	{
		public MeshRenderer _meshRender;

		[Header("Hシーンでのカテゴリー番号")]
		[SerializeField]
		private int[] _categorys;

		private Transform _selfTransform;

		[SerializeField]
		[Header("対")]
		private string[] _targets;

		private Transform[] _objTargets = new Transform[0];

		[SerializeField]
		[Header("グループ")]
		private string[] _groups;

		private Transform[] _objGroups = new Transform[0];

		[Header("オフセット位置")]
		[SerializeField]
		private Vector3 _offsetPos;

		[SerializeField]
		[Header("オフセット回転")]
		private Vector3 _offsetAngle;

		[Button("SetWorldToLocalOffset", "Offsetのワールド座標をローカル座標に変換する", new object[] { })]
		[Space]
		public int SetWorldToLocalOffsetButton;

		[Header("デバッグ用")]
		[Space]
		[Button("BackUpPosition", "今の位置を保存する(エディタ用)", new object[] { })]
		public int BackUpPositionButton;

		[Space]
		[Button("MoveOffset", "Offsetの位置を参照した場所にしてみる", new object[] { })]
		public int MoveOffsetButton;

		[Button("ResetPosition", "元の位置に戻す", new object[] { })]
		public int ResetPositionButton;

		[SerializeField]
		[Header("慣れ判断 0:判定しない 1:慣れ以上")]
		[Space]
		private int _experience;

		[Button("SetMapNameSame", "Kind探してコピーする", new object[] { })]
		[Space]
		public int SetMapNameSameButton;

		private Vector3 backupPos;

		private Vector3 backupAngle;

		public int[] category
		{
			get
			{
				return _categorys;
			}
			set
			{
				_categorys = value;
			}
		}

		public string[] targets
		{
			get
			{
				return _targets;
			}
			private set
			{
				_targets = value;
			}
		}

		public Transform selfTransform
		{
			get
			{
				return _selfTransform;
			}
		}

		public Transform[] objTargets
		{
			get
			{
				return _objTargets;
			}
		}

		public string[] groups
		{
			get
			{
				return _groups;
			}
			private set
			{
				_groups = value;
			}
		}

		public Transform[] objGroups
		{
			get
			{
				return _objGroups;
			}
		}

		public MeshRenderer meshRender
		{
			get
			{
				return _meshRender;
			}
		}

		public int Experience
		{
			get
			{
				return _experience;
			}
		}

		private void Start()
		{
			backupPos = base.transform.position;
			backupAngle = base.transform.eulerAngles;
			if ((bool)meshRender && (bool)meshRender.material)
			{
				meshRender.material.SetTexture(ChaShader._MainTex, GlobalMethod.LoadAllFolderInOneFile<Texture2D>("h/common/", "category" + ((_categorys.Length == 0) ? string.Empty : _categorys[0].ToString("00"))));
			}
		}

		public bool GetKindObject(Transform _map)
		{
			if (!_map)
			{
				return false;
			}
			GameObject gameObject = _map.FindLoop(base.name);
			if ((bool)gameObject)
			{
				_selfTransform = gameObject.transform;
			}
			_objTargets = new Transform[_targets.Length];
			for (int i = 0; i < _objTargets.Length; i++)
			{
				GameObject gameObject2 = _map.FindLoop(_targets[i]);
				if ((bool)gameObject2)
				{
					_objTargets[i] = gameObject2.transform;
				}
			}
			_objGroups = new Transform[_groups.Length];
			for (int j = 0; j < _objGroups.Length; j++)
			{
				GameObject gameObject3 = _map.FindLoop(_groups[j]);
				if ((bool)gameObject3)
				{
					_objGroups[j] = gameObject3.transform;
				}
			}
			return true;
		}

		private void BackUpPosition()
		{
			backupPos = base.transform.position;
			backupAngle = base.transform.eulerAngles;
		}

		public void MoveOffset()
		{
			ResetPosition();
			base.transform.position += _offsetPos;
			base.transform.Rotate(_offsetAngle);
		}

		public void ResetPosition()
		{
			base.transform.position = backupPos;
			base.transform.eulerAngles = backupAngle;
		}

		private void SetMapNameSame()
		{
			GameObject gameObject = GameObject.Find(base.name);
			if (!gameObject)
			{
				return;
			}
			Kind component = gameObject.GetComponent<Kind>();
			if ((bool)component)
			{
				base.transform.position = gameObject.transform.position;
				base.transform.rotation = gameObject.transform.rotation;
				category = component.categoryes;
				targets = new string[component.targets.Length];
				for (int i = 0; i < targets.Length; i++)
				{
					targets[i] = component.targets[i].name;
				}
				groups = new string[component.groups.Length];
				for (int j = 0; j < groups.Length; j++)
				{
					groups[j] = component.groups[j].name;
				}
				_offsetPos = component.offsetPos;
				_offsetAngle = component.offsetAngle;
				if ((bool)meshRender && (bool)meshRender.material)
				{
					meshRender.material.SetTexture(ChaShader._MainTex, GlobalMethod.LoadAllFolderInOneFile<Texture2D>("h/common/", "category" + ((_categorys.Length == 0) ? string.Empty : _categorys[0].ToString("00"))));
				}
			}
		}

		private void SetWorldToLocalOffset()
		{
			if (_offsetPos.x != 0f)
			{
				_offsetPos.x = base.transform.position.x - _offsetPos.x;
			}
			if (_offsetPos.y != 0f)
			{
				_offsetPos.y = base.transform.position.y - _offsetPos.y;
			}
			if (_offsetPos.z != 0f)
			{
				_offsetPos.z = base.transform.position.z - _offsetPos.z;
			}
			if (_offsetAngle.x != 0f)
			{
				_offsetAngle.x -= base.transform.rotation.eulerAngles.x;
			}
			if (_offsetAngle.y != 0f)
			{
				_offsetAngle.y -= base.transform.rotation.eulerAngles.y;
			}
			if (_offsetAngle.z != 0f)
			{
				_offsetAngle.z -= base.transform.rotation.eulerAngles.z;
			}
		}
	}
}
