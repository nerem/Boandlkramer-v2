using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Skill", menuName = "Skills/AOE", order = 3)]
public class AOESkill : OffensiveSkill {


	public override bool Cast (Vector3 target, GameObject target_obj) {

		if (!base.Cast (target, target_obj))
			return false;

		return true;
	}
}
