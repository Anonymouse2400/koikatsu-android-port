using System.Collections;
using System.Linq;
using Manager;
using StrayTech;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame.Chara.Mover
{
	public class Main : State
	{
		private bool mouseMoving;

		private static readonly KeyCode[] moveKeyCodes = new KeyCode[4]
		{
			KeyCode.W,
			KeyCode.D,
			KeyCode.S,
			KeyCode.A
		};

		private float[] moveKeyAngles;

		private PlayerMover mover { get; set; }

		private NavMeshAgent agent
		{
			get
			{
				return chara.agent;
			}
		}

		private Player chara
		{
			get
			{
				return mover.chara;
			}
		}

		private Transform transform
		{
			get
			{
				return chara.cachedTransform;
			}
		}

		public bool isOnNavMeshLink { get; private set; }

		public Main(PlayerMover mover)
			: base(mover)
		{
			this.mover = mover;
		}

		public override IEnumerator Initialize()
		{
			isOnNavMeshLink = false;
			moveKeyAngles = (from i in Enumerable.Range(0, moveKeyCodes.Length * 2)
				select 45f * (float)i).ToArray();
			base.initialized = true;
			yield break;
		}

		public override void Release()
		{
		}

		public override void Update()
		{
			ActionInput.PlayerMove(ref mouseMoving);
			bool isCursorLock = chara.actScene.isCursorLock;
			isCursorLock &= !mover.isReglateMove;
			isCursorLock &= !Singleton<Game>.Instance.IsRegulate(true);
			mouseMoving &= isCursorLock;
			Vector3 vector = Vector3.zero;
			float? num = null;
			if (!mouseMoving && isCursorLock)
			{
				int num2 = moveKeyCodes.Length;
				bool[] array = (from i in Enumerable.Range(0, num2)
					select Input.GetKey(moveKeyCodes[i])).ToArray();
				for (int j = 0; j < num2; j++)
				{
					int num3 = j * 2;
					if (array[j])
					{
						num = moveKeyAngles[num3];
					}
					if (array[j] && ((j != num2 - 1) ? array[j + 1] : array[0]))
					{
						num = moveKeyAngles[num3 + 1];
						break;
					}
				}
			}
			switch (chara.actScene.CameraState.Mode)
			{
			case CameraMode.TPS:
				if (mouseMoving)
				{
					Transform transform = MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCamera.transform;
					chara.eulerAngles = transform.eulerAngles;
					vector = transform.forward;
				}
				else if (num.HasValue)
				{
					Transform transform2 = MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCamera.transform;
					chara.eulerAngles = new Vector3(0f, transform2.eulerAngles.y + num.Value, 0f);
					vector = this.transform.forward;
				}
				break;
			case CameraMode.FPS:
				if (mouseMoving)
				{
					vector = this.transform.forward;
				}
				else if (num.HasValue)
				{
					vector = Quaternion.AngleAxis(num.Value, Vector3.up) * this.transform.forward;
				}
				break;
			default:
				if (mouseMoving)
				{
					this.transform.Rotate(0f, Input.GetAxis("Mouse X") * mover.agentSpeeder.rotateSpeed, 0f);
					vector = this.transform.forward;
				}
				break;
			}
			vector.y = 0f;
			vector = vector.normalized * agent.speed;
			mover.animParamSpeed = vector.magnitude;
			if (agent.enabled && agent.isOnNavMesh)
			{
				if (!isOnNavMeshLink)
				{
					agent.Move(vector * Time.deltaTime);
					return;
				}
				vector = agent.velocity;
				vector.y = 0f;
				chara.rotation = Quaternion.LookRotation(vector);
			}
			else
			{
				chara.position += vector * Time.deltaTime;
			}
		}

		public void NavMeshLinkIn(Vector3 pos)
		{
			if (agent.isOnNavMesh)
			{
				NavMeshPath path = new NavMeshPath();
				isOnNavMeshLink = agent.CalculatePath(pos, path);
				if (isOnNavMeshLink)
				{
					agent.SetPath(path);
				}
			}
		}

		public void NavMeshLinkOut()
		{
			isOnNavMeshLink = false;
			if (agent.isOnNavMesh)
			{
				agent.CompleteOffMeshLink();
				agent.ResetPath();
			}
		}
	}
}
