using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Skill", menuName = "Skills/Projectile (Homing)", order = 4)]
public class ProjectileHomingSkill : OffensiveSkill {

	public float range = 1f;
	public float speed = 3f;

	public GameObject projectile;

	public override bool CastCheck (Vector3 target, GameObject target_obj) {
		if (target_obj == null)
			return false;

		if (!base.CastCheck (target, target_obj))
			return false;

		return true;
	}

	public override bool Cast (Vector3 target, GameObject target_obj) {

		if (target_obj == null)
			return false;

		if (!base.Cast (target, target_obj))
			return false;

		GameObject instance = Instantiate (projectile, character.castPoint.transform.position, Quaternion.FromToRotation (Vector3.forward, target - character.transform.position));
		instance.GetComponent<HomingProjectile> ().Initialize (target_obj, speed, damage, dmgType, magicEffect);

		return true;
	}
}
