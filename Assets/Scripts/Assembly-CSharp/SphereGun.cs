using UnityEngine;

public class SphereGun : Weapon
{
	public ParticleSystem hitPS;

	protected override void DoShoot(DungeonControl2 dungeon, Vector3 from, Vector3 to)
	{
		weaponBody.transform.position = to;
		dungeon.Attack(brush);
		Object.Instantiate(hitPS.gameObject, to, Quaternion.identity);
	}
}
