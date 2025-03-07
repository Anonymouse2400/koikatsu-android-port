using UnityEngine;

public class TunnelGun : Weapon
{
	public ParticleSystem hitPS;

	public ParticleSystem shootPS;

	protected override void DoShoot(DungeonControl2 dungeon, Vector3 from, Vector3 to)
	{
		weaponBody.transform.position = from;
		weaponBody.transform.LookAt(to, Vector3.up);
		dungeon.Attack(brush);
		Object.Instantiate(hitPS.gameObject, to, Quaternion.identity);
		Object.Instantiate(shootPS.gameObject, weaponBody.transform.position, weaponBody.transform.rotation);
	}
}
