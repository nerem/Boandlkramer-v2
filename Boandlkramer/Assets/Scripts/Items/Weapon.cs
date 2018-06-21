using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Weapon", menuName = "Items/Weapon", order = 3)]
public class Weapon : Item {

	public DamageType type;
	public int damage;
	public int range;
	public float speed;
}

public enum DamageType { None, Slashing, Piercing, Blunt, Fire, Ice, Lightning }
