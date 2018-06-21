using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Skills/Skill (Base)", order = 1)]
public class Skill : ScriptableObject {

    public Sprite icon;
    public string description;

	public int manaCost = 10;

	public int skillLevel = 1;
	public string descriptionNextLevel;
	public Skill nextLevelSkill;

    public Character character;

	public MagicEffect magicEffect;

	public virtual bool CastCheck (Vector3 target, GameObject target_obj) {
		if (character.data.stats["mana"].Current < manaCost)
			return false;

		character.data.stats["mana"].Current -= manaCost;
		return true;
	}

    public virtual bool Cast (Vector3 target, GameObject target_obj) {
		return true;
	}
}
