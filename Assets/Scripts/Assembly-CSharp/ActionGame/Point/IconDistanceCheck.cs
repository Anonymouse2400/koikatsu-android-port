using ActionGame.Chara;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ActionGame.Point
{
	internal class IconDistanceCheck : MonoBehaviour
	{
		[SerializeField]
		private SpriteRenderer icon;

		[SerializeField]
		private float distance = 2f;

		private float? calcY;

		private void Start()
		{
			if (icon == null)
			{
				Object.Destroy(this);
				return;
			}
			if (!Singleton<Game>.IsInstance() || Singleton<Game>.Instance.actScene == null)
			{
				Object.Destroy(this);
				return;
			}
			Player player = Singleton<Game>.Instance.actScene.Player;
			if (player == null)
			{
				Object.Destroy(this);
				return;
			}
			Transform iconT = icon.transform;
			BoolReactiveProperty visible = new BoolReactiveProperty();
			visible.TakeUntilDestroy(icon).TakeUntilDestroy(player).Subscribe(delegate(bool isVisible)
			{
				icon.enabled = isVisible;
			});
			this.UpdateAsObservable().TakeUntilDestroy(icon).TakeUntilDestroy(player)
				.Subscribe(delegate
				{
					Vector3 position = iconT.position;
					calcY = (position.y = player.position.y);
					visible.Value = !Singleton<Game>.Instance.IsRegulate(true) && (player.position - position).sqrMagnitude < distance * distance;
				});
		}

		private void OnDrawGizmos()
		{
			if (!(icon == null))
			{
				Color blue = Color.blue;
				blue.a = 0.5f;
				Gizmos.color = blue;
				float y = 0f;
				Vector3 position = icon.transform.position;
				if (calcY.HasValue)
				{
					y = calcY.Value;
				}
				position.y = y;
				Gizmos.DrawSphere(position, distance);
			}
		}
	}
}
