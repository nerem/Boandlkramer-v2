
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class CharacterData {

	// owner of this character data
	Character owner;

    public int level = 1;

    // Attribute points per level
    int attributePointsPerLevel = 5;

	// Skill points per level
	int skillPointsPerLevel = 1;

	public Dictionary<string, Stat> stats;
	public Dictionary<string, Attribute> attributes;
	public List<Perk> perks;

    public int baseDamage = 3;
    public float baseSpeed = 10f;
    public int baseArmor = 0;


    // current XP the character has
	int experience = 0;

    // remaining attribute points to spend
    int remainingAttributePoints = 0;

	// remaining skill points to spend
	int remainingSkillPoints = 0;

    // for updating UI
    public CharacterUI charUI;

	// scaling drop of XP
	public int xpDropMultiplier = 50;

	// scaling for xp needed for level up
	public int xpForLevelMultiplier = 50;


	public CharacterData (Character owningCharacter) {

        attributes = new Dictionary<string, Attribute>() { { "strength", new Attribute ()}, { "dexterity", new Attribute ()},
            { "vitality", new Attribute ()}, { "intelligence", new Attribute ()} };
        stats = new Dictionary<string, Stat> () { { "health", new Stat (100, attributes["vitality"]) },
			{ "mana", new Stat (100, attributes["intelligence"]) } };
		perks = new List<Perk> ();

		owner = owningCharacter;

	}


    // for UI
	public int GetCurrentExperiencePoints()
	{
		return experience;
	}

    // calculates the amount of xp needed for a certain level
	public int CalculateExperienceForLevel(int lvl)
	{
		if (lvl == 1)
			return 0;
		else
			return xpForLevelMultiplier*(lvl * lvl);
	}

    // adds an amount of XP points and increases the level if enough xp was gained
	public void IncreaseExperience(int expAmount)
	{
		experience += expAmount;

        // adjust the level
		if (experience >= CalculateExperienceForLevel(level + 1))
		{
			// play a sound
			owner.audioManager.Play("LevelUp");

			// increase level, add attribute / skill points and restore health / mana
			level++;
            stats["health"].Current = stats["health"].Max;
            stats["mana"].Current = stats["mana"].Max;
            remainingAttributePoints += attributePointsPerLevel;
			remainingSkillPoints += skillPointsPerLevel;

            // this is for testing if we gained that much experience to level up more than one level
            IncreaseExperience(0);

			// show level up UI
			owner.levelUpUI.SetActive(true);
			owner.levelUpUI.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "~ Level " + level.ToString() + " ~";
			owner.levelUpUI.GetComponent<FadeOut>().Fade(2f);
			GameObject go = GameObject.Instantiate(owner.levelUpParticle, owner.gameObject.transform);
			go.GetComponent<ParticleSystem>().Play();
			GameObject.Destroy(go, 3f);
		}

		if (charUI != null)
			charUI.UpdateCharacterUI();
	}

    // for UI
    public int GetRemainingAttributePoints()
    {
        return remainingAttributePoints;
    }

	public int GetRemainingSkillPoints()
	{
		return remainingSkillPoints;
	}

    // increases an attribute if possible
    public void SpendAttributePoint(string attribute)
    {
        if (remainingAttributePoints > 0)
        {
            attributes[attribute].IncreaseBase();
            remainingAttributePoints--;
        }
    }


	// increases level of a / learn new skill if possible
	public bool SpendSkillPoint(Skill skill)
	{
		if (remainingSkillPoints > 0)
		{
			List<Skill> currentSkills = new List<Skill>(owner.skillbook);
			if (currentSkills.Contains(skill))
			{
				// skill is already available, increase level if there is one with a higher level
				if (skill.nextLevelSkill != null)
				{
					int index = currentSkills.IndexOf(skill);
					currentSkills[index] = skill.nextLevelSkill;
					owner.UpdateSkillbook(currentSkills.ToArray());
					remainingSkillPoints--;

					// we have successfully upgraded this skill
					return true;
				}
			}
		}

		return false;
	}

    // calculates the amount of XP this character "drops" if he dies
    public int XPDropping()
    {
		Debug.Log("XP Drop: " + (level * xpDropMultiplier).ToString());
        return level * xpDropMultiplier;
    }
	
}

[System.Serializable]
public class Stat {

	int m_max;
	int m_current;
    Attribute m_attribute;

	public int Max {
		get {
            if (m_attribute != null)
                return m_max + m_attribute.GetValue() * 10;
            else
                return m_max;
		}
		set {
			if (value >= 0)
				m_max = value;
		}
	}

	public int Current {
		get {
			return m_current;
		}
		set {
            m_current = Mathf.Clamp(value, 0, Max);
;		}
	}

	public Stat () {

		m_max = 100;
        m_attribute = null;
		m_current = Max;
	}

	public Stat (int max, Attribute attribute) {

		m_max = Mathf.Clamp(max, 0, int.MaxValue);
        m_attribute = attribute;
		m_current = Max;
	}

	public Stat (int max, int current, Attribute attribute) {

		m_max = Mathf.Clamp (max, 0, int.MaxValue);
		m_attribute = attribute;
		m_current = Mathf.Clamp(current, 0, Max);
    }
}

[System.Serializable]
public class Attribute {

	int m_base;
	List<AttributeModifier> modifiers = new List<AttributeModifier> ();

	public Attribute () {

		m_base = 5;
	}

	public Attribute (int value) {

		m_base = value;
	}

	public int GetValue () {

		int value = m_base;
		foreach (AttributeModifier modifier in modifiers)
			value += modifier.amount;
		return value;
	}

	public int GetBaseValue()
	{
		return m_base;
	}

	public int GetModifierValue()
	{
		int value = 0;
		foreach (AttributeModifier modifier in modifiers)
			value += modifier.amount;

		return value;
	}

	public void SetBase (int value) {

		if (value >= 0)
			m_base = value;
	}

	public void AddModifier (AttributeModifier modifier) {

		modifiers.Add (modifier);
	}

	public void RemoveModifier (AttributeModifier modifier) {

		modifiers.Remove (modifier);
	}

    public void IncreaseBase()
    {
        m_base++;
    }
}
