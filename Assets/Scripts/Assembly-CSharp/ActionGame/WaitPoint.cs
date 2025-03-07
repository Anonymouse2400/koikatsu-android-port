using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Chara;
using ActionGame.MapObject;
using Illusion;
using Illusion.Extensions;
using Illusion.Game.Elements.EasyLoader;
using Manager;
using UnityEngine;

namespace ActionGame
{
	public class WaitPoint : MonoBehaviour
	{
		[Serializable]
		public class KindMover
		{
			[Header("対象名")]
			public string name;

			[Header("ずらす場所")]
			public Vector3 shiftPos;

			[Header("ずらす角度")]
			public Vector3 shiftAngle;

			[Header("ずらす計算")]
			public bool isLocal;

			public void MoveShift(Kind kind)
			{
				kind.ResetPosition();
				if (isLocal)
				{
					kind.transform.Translate(shiftPos);
					kind.transform.Rotate(shiftAngle);
				}
				else
				{
					kind.transform.position = shiftPos;
					kind.transform.eulerAngles = shiftAngle;
				}
			}
		}

		[Serializable]
		public class Parameter
		{
			[Serializable]
			public class Motion
			{
				[Serializable]
				public class Else
				{
					[Header("再生モーション")]
					public string motion;

					[Header("アイテム")]
					public List<Item.Data> itemList = new List<Item.Data>();

					public void ItemSet(List<GameObject> itemObjList, ChaControl chaCtrl)
					{
						itemObjList.AddRange(itemList.Select(delegate(Item.Data item)
						{
							string asset = item.asset;
							string[] self = item.asset.Split('/');
							item.asset = self.Shuffle().First();
							GameObject result = item.Load(chaCtrl);
							item.asset = asset;
							return result;
						}));
					}
				}

				private const int ErrorID = int.MaxValue;

				[Header("モーションID")]
				public int ID = int.MaxValue;

				[Header("状態(0=>立ち : 1=>座り : 2=>寝ている : 3=>水泳部 : 4=>机 : 5=>壁 : 6=>角オナ : 7=>ハードルオナ)")]
				public int state;

				[Header("再生モーション")]
				public string motion;

				[Header("待機時にずらす場所")]
				public Vector3 offsetPos;

				[Header("待機時にずらす角度")]
				public Vector3 offsetAngle;

				[Header("アイテム")]
				public List<Item.Data> itemList = new List<Item.Data>();

				[Header("特殊モーション")]
				public Else motionElse = new Else();

				public bool isErrorID
				{
					get
					{
						return ID == int.MaxValue;
					}
				}

				public void ItemSet(List<GameObject> itemObjList, ChaControl chaCtrl)
				{
					itemObjList.AddRange(itemList.Select(delegate(Item.Data item)
					{
						string asset = item.asset;
						string[] self = item.asset.Split('/');
						item.asset = self.Shuffle().First();
						GameObject result = item.Load(chaCtrl);
						item.asset = asset;
						return result;
					}));
				}
			}

			[Header("AI行動")]
			public string layer = string.Empty;

			[Header("モーション")]
			public List<Motion> motionList = new List<Motion>();
		}

		public int MapNo;

		[Header("ずらすマップオブジェクト")]
		public List<KindMover> kindList = new List<KindMover>();

		[Header("H時にずらす場所")]
		public Vector3 offsetHPos;

		[Header("H時にずらす角度")]
		public Vector3 offsetHAngle;

		[Header("NavMesh用のOffset座標")]
		public Vector3 navMeshOffsetPoint;

		public List<Parameter> parameterList = new List<Parameter>();

		private Vector3? _position;

		private Vector3? _angle;

		private WaitPoint _Parent;

		private bool isParentChecked;

		private WaitPoint[] _Children;

		public bool isNavMeshOffsetPoint
		{
			get
			{
				return navMeshOffsetPoint.sqrMagnitude > 1E-05f;
			}
		}

		public Vector3 navPosition
		{
			get
			{
				return position + navMeshOffsetPoint;
			}
		}

		public Vector3 position
		{
			get
			{
				Vector3 value;
				if (_position.HasValue)
				{
					value = _position.Value;
				}
				else
				{
					Vector3? vector = (_position = base.transform.position);
					value = vector.Value;
				}
				return value;
			}
		}

		public Vector3 angle
		{
			get
			{
				Vector3 value;
				if (_angle.HasValue)
				{
					value = _angle.Value;
				}
				else
				{
					Vector3? vector = (_angle = base.transform.eulerAngles);
					value = vector.Value;
				}
				return value;
			}
		}

		private Base chara
		{
			get
			{
				return (!(reservedChara != null)) ? waithingChara : reservedChara;
			}
		}

		public Base reservedChara { get; private set; }

		public bool isReserved { get; private set; }

		public Base waithingChara { get; private set; }

		public bool isParent
		{
			get
			{
				return Parent != null;
			}
		}

		public WaitPoint Parent
		{
			get
			{
				if (isParentChecked)
				{
					return _Parent;
				}
				isParentChecked = true;
				base.transform.parent.SafeProc(delegate(Transform parent)
				{
					_Parent = parent.GetComponent<WaitPoint>();
				});
				return _Parent;
			}
		}

		public bool isChild
		{
			get
			{
				return Children.Any();
			}
		}

		public WaitPoint[] Children
		{
			get
			{
				return this.GetCache(ref _Children, () => (from child in base.transform.Children()
					select child.GetComponent<WaitPoint>() into child
					where child != null
					select child).ToArray());
			}
		}

		public void Reserve(Base chara)
		{
			isReserved = true;
			reservedChara = chara;
		}

		public void UnReserve()
		{
			isReserved = false;
			if (chara != null)
			{
				chara.SetWaitPoint(null);
			}
			reservedChara = null;
			waithingChara = null;
			WaitPoint[] children = Children;
			foreach (WaitPoint waitPoint in children)
			{
				if (!(waitPoint.chara == null) && waitPoint.chara.isActiveAndEnabled)
				{
					waitPoint.chara.SetWaitPoint(null);
					(waitPoint.chara as NPC).SafeProc(delegate(NPC npc)
					{
						npc.AI.NextActionNoTarget();
					});
				}
			}
		}

		public int SetWait(int index, params int[] _not)
		{
			if (reservedChara != null)
			{
				waithingChara = chara;
				reservedChara = null;
			}
			chara.mapNo = MapNo;
			Parameter parameter = parameterList.SafeGet(index);
			var anon = (from m in parameter.motionList.Select((Parameter.Motion m, int i) => new { m, i })
				where _not.IsNullOrEmpty() || !_not.Contains(m.m.ID)
				select m).Shuffle().FirstOrDefault();
			chara.state = anon.m.state;
			chara.motion.state = anon.m.motion;
			chara.SetWaitPoint(new Base.WaitPointData(this, index, anon.i));
			chara.SetPositionAndRotation(position + anon.m.offsetPos, angle + anon.m.offsetAngle);
			return anon.i;
		}

		private void OnDrawGizmos()
		{
			if (isNavMeshOffsetPoint && (!Singleton<Game>.IsInstance() || !(Singleton<Game>.Instance.actScene != null) || !(Singleton<Game>.Instance.actScene.Map != null) || Singleton<Game>.Instance.actScene.Map.no == MapNo))
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
				Vector3 vector = base.transform.position + navMeshOffsetPoint;
				float num = 0.25f;
				Gizmos.DrawSphere(vector, num);
				Utils.Gizmos.Axis(vector, Quaternion.identity, num);
			}
		}
	}
}
