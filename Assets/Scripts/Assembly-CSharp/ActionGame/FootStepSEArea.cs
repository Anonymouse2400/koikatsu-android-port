using System.Collections.Generic;
using System.Linq;
using ActionGame.Chara;
using Illusion.Game;
using Manager;
using UnityEngine;

namespace ActionGame
{
	public class FootStepSEArea : MonoBehaviour
	{
		public enum CollisionState
		{
			None = 0,
			Enter = 1,
			Stay = 2,
			Exit = 3
		}

		[SerializeField]
		[Tooltip("Compatible collider is Box, Sphere, Capsule only.")]
		private Collider[] _colliders;

		[SerializeField]
		private FootStepSE _area;

		[SerializeField]
		private LayerMask _mask = 0;

		private Dictionary<int, CollisionState> _collisionStateTable = new Dictionary<int, CollisionState>();

		private Dictionary<int, FootStepSE> _prevSETable = new Dictionary<int, FootStepSE>();

		public void ForceUpdate()
		{
			int[] array = _collisionStateTable.Keys.ToArray();
			int[] array2 = array;
			foreach (int key in array2)
			{
				_collisionStateTable[key] = CollisionState.None;
			}
			OnUpdate();
		}

		private void Update()
		{
			if (Singleton<Scene>.IsInstance() && (!Singleton<Scene>.IsInstance() || !Singleton<Scene>.Instance.IsNowLoadingFade))
			{
				OnUpdate();
			}
		}

		private void OnUpdate()
		{
			if (!Singleton<Character>.IsInstance())
			{
				return;
			}
			List<int> list = ListPool<int>.Get();
			Collider[] colliders = _colliders;
			foreach (Collider collider in colliders)
			{
				if (collider is BoxCollider)
				{
					BoxCollider boxCollider = collider as BoxCollider;
					Vector3 center = collider.transform.position + Vector3.Scale(boxCollider.center, collider.transform.lossyScale);
					Vector3 halfExtents = Vector3.Scale(boxCollider.size, collider.transform.lossyScale) / 2f;
					Collider[] array = Physics.OverlapBox(center, halfExtents, Quaternion.identity, _mask, QueryTriggerInteraction.Collide);
					Collider[] array2 = array;
					foreach (Collider collider2 in array2)
					{
						Base component = collider2.GetComponent<Base>();
						if (!(component == null) && !list.Contains(component.chaCtrl.loadNo))
						{
							list.Add(component.chaCtrl.loadNo);
						}
					}
				}
				else if (collider is SphereCollider)
				{
					SphereCollider sphereCollider = collider as SphereCollider;
					Vector3 position = collider.transform.position + Vector3.Scale(sphereCollider.center, collider.transform.lossyScale);
					float radius = sphereCollider.radius * Mathf.Max(collider.transform.lossyScale.x, Mathf.Max(collider.transform.lossyScale.y, collider.transform.lossyScale.z));
					Collider[] array3 = Physics.OverlapSphere(position, radius, _mask, QueryTriggerInteraction.Collide);
					Collider[] array4 = array3;
					foreach (Collider collider3 in array4)
					{
						Base component2 = collider3.GetComponent<Base>();
						if (!(component2 == null) && !list.Contains(component2.chaCtrl.loadNo))
						{
							list.Add(component2.chaCtrl.loadNo);
						}
					}
				}
				else
				{
					if (!(collider is CapsuleCollider))
					{
						continue;
					}
					CapsuleCollider capsuleCollider = collider as CapsuleCollider;
					Vector3 vector = collider.transform.position + Vector3.Scale(capsuleCollider.center, collider.transform.lossyScale);
					Vector3 point = vector + Quaternion.identity * new Vector3(0f, capsuleCollider.height * collider.transform.lossyScale.y, 0f);
					float radius2 = capsuleCollider.radius * Mathf.Max(collider.transform.lossyScale.x, collider.transform.lossyScale.z);
					Collider[] array5 = Physics.OverlapCapsule(vector, point, radius2, _mask, QueryTriggerInteraction.Collide);
					Collider[] array6 = array5;
					foreach (Collider collider4 in array6)
					{
						Base component3 = collider4.GetComponent<Base>();
						if (!(component3 == null) && !list.Contains(component3.chaCtrl.loadNo))
						{
							list.Add(component3.chaCtrl.loadNo);
						}
					}
				}
			}
			KeyValuePair<int, ChaControl>[] array7 = Singleton<Character>.Instance.dictEntryChara.Where((KeyValuePair<int, ChaControl> x) => x.Value.GetActiveTop()).ToArray();
			KeyValuePair<int, ChaControl>[] array8 = array7;
			for (int m = 0; m < array8.Length; m++)
			{
				KeyValuePair<int, ChaControl> keyValuePair = array8[m];
				CollisionStateUpdate(keyValuePair.Key, list.Contains(keyValuePair.Key));
			}
			ListPool<int>.Release(list);
		}

		private void CollisionStateUpdate(int id, bool condition)
		{
			CollisionState value;
			if (!_collisionStateTable.TryGetValue(id, out value))
			{
				CollisionState collisionState = CollisionState.None;
				_collisionStateTable[id] = collisionState;
				value = collisionState;
			}
			CollisionState collisionState2 = CollisionState.None;
			if (condition)
			{
				switch (value)
				{
				case CollisionState.None:
				case CollisionState.Exit:
					collisionState2 = CollisionState.Enter;
					break;
				case CollisionState.Enter:
				case CollisionState.Stay:
					collisionState2 = CollisionState.Stay;
					break;
				}
				if (collisionState2 == CollisionState.Enter)
				{
					OnEnterArea(id);
				}
			}
			else
			{
				switch (value)
				{
				case CollisionState.None:
				case CollisionState.Exit:
					collisionState2 = CollisionState.None;
					break;
				case CollisionState.Enter:
				case CollisionState.Stay:
					collisionState2 = CollisionState.Exit;
					break;
				}
				if (collisionState2 == CollisionState.Exit)
				{
					OnExitArea(id);
				}
			}
			_collisionStateTable[id] = collisionState2;
		}

		private void OnEnterArea(int id)
		{
			FootStepSE value;
			if (!Utils.Sound.FootStepAreaTypes.TryGetValue(id, out value))
			{
				FootStepSE footStepAreaDefaultType = Utils.Sound.FootStepAreaDefaultType;
				Utils.Sound.FootStepAreaTypes[id] = footStepAreaDefaultType;
				value = footStepAreaDefaultType;
			}
			_prevSETable[id] = value;
			Utils.Sound.FootStepAreaTypes[id] = _area;
		}

		private void OnExitArea(int id)
		{
			Utils.Sound.FootStepAreaTypes[id] = _prevSETable[id];
		}
	}
}
