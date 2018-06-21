using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Skill", menuName = "Skills/Attack Skill (Base)", order = 2)]
public class OffensiveSkill : Skill {

	public int damage;
	public DamageType dmgType;
}
