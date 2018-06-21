using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Skill", menuName = "Skills/Projectile (AOE)", order = 4)]
public class ProjectileAOESkill : OffensiveSkill {

	public float range = 1f;
	public float speed = 3f;
	public float AOERange = 0.5f;

	public GameObject projectile;

	public override bool Cast (Vector3 target, GameObject target_obj) {

		if (!base.Cast (target, target_obj))
			return false;

		GameObject instance = Instantiate (projectile, character.castPoint.transform.position, Quaternion.FromToRotation (Vector3.forward, target - character.transform.position));
		instance.GetComponent<Projectile> ().Initialize (target, speed, damage, dmgType, AOERange, magicEffect);

		return true;
	}
}
