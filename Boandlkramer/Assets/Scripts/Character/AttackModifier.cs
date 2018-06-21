using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Attack Modifier", menuName = "Modifiers/Attack Modifier", order = 2)]
public class AttackModifier : ScriptableObject {

	public string description;

	public DamageType type;
	public int damage;
}
