using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "NPCs/Enemy", order = 1)]
public class EnemyType : ScriptableObject {

	public GameObject Object;

	public int health = 100;
	public int mana = 100;

	[HideInInspector]
	public int strength = 0;
	[HideInInspector]
	public int dexterity = 0;
	[HideInInspector]
	public int intelligence = 0;
	[HideInInspector]
	public int vitality = 0;

	public int damage = 10;
	public float speed = 10f;
	public int armor = 2;

    public int level = 1;

	// chance to drop something from loot list
	public float dropchance = .5f;

	// chance to drop multiple items from loot list
	public float incrementalDropchance = .2f;

	// maximum items can be dropped by this enemy
	public int maxDrops = 3;

	// lootlist
    public Item[] loot;

	// number of randomly generated items (dependant on level) added to lootlist
	// public int nRandomlyGeneratedLoot = 1;
}
