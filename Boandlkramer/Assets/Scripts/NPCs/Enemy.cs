using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Enemy : Character {

	Item[] loot;
	[SerializeField]
	EnemyType enemyType;

	[SerializeField]
	ParticleSystem dyingEffect;

	// graphic that is displayed when this enemy is focused by the player
	GameObject highlightGraphic;

	// set to true if this enemy is focused by the player
	public bool bHighlighted = false;

	// for changing highlight focus effect
	bool bWasJustHighlighted = false;

	void Start()
	{

		if (enemyType != null)
		{
			LoadCharacterData();
		}
		highlightGraphic = GameObject.FindGameObjectWithTag("highlight");


	}

	void LoadCharacterData()
	{
		data.attributes["strength"].SetBase(enemyType.strength);
		data.attributes["dexterity"].SetBase(enemyType.dexterity);
		data.attributes["intelligence"].SetBase(enemyType.intelligence);
		data.attributes["vitality"].SetBase(enemyType.vitality);

		data.stats["health"] = new Stat(enemyType.health, data.attributes["vitality"]);
		data.stats["mana"] = new Stat(enemyType.mana, data.attributes["intelligence"]);

		data.baseDamage = enemyType.damage;
		data.baseSpeed = enemyType.speed;
		data.baseArmor = enemyType.armor;
        data.level = enemyType.level;

        loot = enemyType.loot;
	}

	void Update()
	{
        UpdateMagicEffects();

		if (bHighlighted)
		{
			//highlightGraphic.transform.localScale = transform.localScale;
			highlightGraphic.transform.position = transform.position;
			bWasJustHighlighted = true;
		}
		else
		{
			// not in focus anymore, check if we have already removed the effect from this enemy
			if (bWasJustHighlighted)
			{
				// if this is not the case, remove highlight graphic and remember that we´ve removed the effect
				highlightGraphic.transform.position = new Vector3(0f, -10f, 0f);
				bWasJustHighlighted = false;
			}
		}

	}

    protected override int CalculateDamage(Character other) {

        return data.baseDamage;
    }

    protected override float CalculateAttackSpeed() {

        return 10f / (data.baseSpeed + 10f);
    }

    protected override int ReducedDamage(int damage, DamageType dmgType = DamageType.None) {

        return damage - data.baseArmor;
    }

	protected override void Death () {

		// increase xp of the player
		foreach (Character character in charactersGainingXPFromThisCharacter) {
			character.data.IncreaseExperience (this.data.XPDropping ());
		}

		DropLoot ();

		// defocus
		bHighlighted = false;
		highlightGraphic.transform.position = new Vector3(0f, -10f, 0f);

		if (dyingEffect != null)
		{
			ParticleSystem go = Instantiate(dyingEffect, transform.position, transform.rotation);
			//this.transform.parent = go.transform;
			Destroy(go.gameObject, 2f);

            // spawn blood decal
            GameObject.FindGameObjectWithTag("blooddecalspawner").GetComponent<SpawnBloodDecals>().SpawnRandomBloodDecal(transform.position);
        }



        Debug.Log("Test");
		Destroy (gameObject);
	}


	void DropLoot () {

		// drops random loot of loot list if the player is lucky
		if (Random.Range(0, 100) < enemyType.dropchance * 100)
		{
			int itemNumber = Random.Range(0, loot.Length);
			loot[itemNumber].Spawn(transform.position, enemyType.level);
			Debug.Log("Dropped " + loot[itemNumber].name);

			int num = 1;
			while (Random.Range(0, 100) < enemyType.incrementalDropchance * 100 && num < enemyType.maxDrops)
			{
				// drop more items
				Debug.Log("additional item");
				loot[Random.Range(0, loot.Length)].Spawn(transform.position, enemyType.level);
				num++;
			}
		}
	}
}
