using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour {

	public CharacterData data;
	[SerializeField]
	GameObject ui;
	[SerializeField]
	TextMeshPro text;

	// playing sounds (level up etc.)
	public AudioManager audioManager;

	// shown at level up
	public GameObject levelUpUI;
	public GameObject levelUpParticle;


	public GameObject castPoint;

    public List<Character> charactersGainingXPFromThisCharacter = new List<Character>();

    Inventory inventory;

	public bool canAttack = true;
	public bool canCast = true;


	// all skills the player may have
	public Skill[] skillbook;

	// all skills the player possesses at the moment
	public Skill[] availableSkills;

	// skill that is active right now
	public Skill activeSkill;

	// for active magic effects
	public int maximalActiveEffects = 3;
	float[] magicEffectTimer;
	float[] damageOverTimeTimer;
	MagicEffect[] magicEffects;

    NavMeshAgent agent;

    // movement values
    float speed;
    float angularSpeed;
	float acceleration;



	void Awake()
	{
		data = new CharacterData(this);

		// create timers according to the maximal number of active effects on this character
		magicEffectTimer = new float[maximalActiveEffects];
		damageOverTimeTimer = new float[maximalActiveEffects];
		magicEffects = new MagicEffect[maximalActiveEffects];

        agent = GetComponentInParent<NavMeshAgent>();

		// save movement values in case they need to be restored
		speed = agent.speed;
		angularSpeed = agent.angularSpeed;
		acceleration = agent.acceleration;

    }




    void Start()
    {
		UpdateAvailableSkills();
       // activeSkill.character = this;
        inventory = GetComponent<Inventory>();

		FindObjectOfType<SkillbarUI>().FillSkillSlots();
    }

	public void UpdateSkillbook(Skill[] newSkillBook)
	{
		skillbook = newSkillBook;
		UpdateAvailableSkills();
	}

	void UpdateAvailableSkills()
	{
		availableSkills = new Skill[skillbook.Length];
		for (int i = 0; i < skillbook.Length; i++)
		{
			availableSkills[i] = Instantiate(skillbook[i]);
			availableSkills[i].name = skillbook[i].name;
			availableSkills[i].character = this;
		}
		if (activeSkill == null)
		{
			activeSkill = availableSkills[0];
		}

		FindObjectOfType<SkillbarUI>().FillSkillSlots();

	}

	public void Attack (Character other) {

		if (canAttack) {
            if(GetComponent<BoandlAnimation>()!=null)
                GetComponent<BoandlAnimation>().Trigger("Attack");
			StartCoroutine (AttackCooldown (CalculateAttackSpeed ()));
			other.TakeDamage (CalculateDamage(other));
		}
	}

	public void AddMagicEffect(MagicEffect magicEffect)
	{
		Debug.Log("Trying to add magic effect to " + this.name);
		// check if a magic effect slot is available
		int index = -1;
		for (int i = 0; i < magicEffectTimer.Length; i++)
		{
			if (magicEffectTimer[i] <= 0f)
			{
				index = i;
				break;
			}
		}

		// we can add a new magic effect to this character
		if (index >= 0)
		{
			Debug.Log("Added Effect to " + this.name);
			magicEffectTimer[index] = magicEffect.totalTime;
			damageOverTimeTimer[index] = 0f;
			magicEffects[index] = magicEffect;

            // add mesh effect
            var effectInstance = Instantiate(magicEffect.meshModifier);
            effectInstance.name = magicEffect.meshModifier.name;
            effectInstance.transform.parent = this.gameObject.transform;
            effectInstance.transform.localPosition = Vector3.zero;
            effectInstance.transform.localRotation = new Quaternion();
            var meshUpdater = effectInstance.GetComponent<PSMeshRendererUpdater>();
            meshUpdater.UpdateMeshEffect(this.gameObject);

        }
	}

	void Update()
	{
		// damage over time etc. caused by magic spells
        UpdateMagicEffects();
	}

    protected virtual void UpdateMagicEffects()
    {
        // update magic effects timers and apply over time effects
        for (int i = 0; i < magicEffectTimer.Length; i++)
        {
            if (magicEffectTimer[i] > 0f)
            {
				// time is running
				magicEffectTimer[i] -= Time.deltaTime;
				damageOverTimeTimer[i] += Time.deltaTime;

				// Apply magic effect
				ApplyEffect(magicEffects[i], i);



				// if the effect has timed out, remove it from this character
				if (magicEffectTimer[i] <= 0f)
				{
					RemoveEffect(magicEffects[i]);
					damageOverTimeTimer[i] = 0f;
                }
            }
        }

    }

    void ApplyEffect(MagicEffect effect, int timerID)
    {
		
		if (effect.movementMultiplier != 1.0f)
		{
			// change movement speed
			agent.speed = speed * effect.movementMultiplier;
			agent.angularSpeed = angularSpeed * effect.movementMultiplier;
			agent.acceleration = acceleration * effect.movementMultiplier;
		}

		if (effect.baseDamageOverTime != 0f)
		{
			// Add damage over time: every damageOverTimeTickRate seconds we apply baseDamageOverTime damage to this character
			if (damageOverTimeTimer[timerID] >= effect.damageOverTimeTickRate)
			{
				TakeDamage(effect.baseDamageOverTime, effect.damageOverTimeType);
				damageOverTimeTimer[timerID] = 0f;
			}

		}
	}


	void RemoveEffect(MagicEffect effect)
	{
		// remove mesh effect and restore all values if time is over
		GameObject _effect = transform.Find(effect.meshModifier.name).gameObject;
		if (_effect)
		{
			Destroy(_effect);
		}


		// restore speed values
		agent.speed = speed;
		agent.angularSpeed = angularSpeed;
		agent.acceleration = acceleration;
	}


    public void SecondaryAttack(Vector3 target, Character other)
    {
		if (canCast) {
			if (other != null) {
				if (activeSkill.CastCheck (target, other.gameObject)) {
					StartCoroutine (Cast (target, other, 0.33f));
					if (GetComponent<BoandlAnimation> () != null)
						GetComponent<BoandlAnimation> ().Trigger ("Cast");
				}
			}
			else {
				if (activeSkill.CastCheck (target, null)) {
					StartCoroutine (Cast (target, other, 0.33f));
					if (GetComponent<BoandlAnimation> () != null)
						GetComponent<BoandlAnimation> ().Trigger ("Cast");

				}
			}	
		}
	}

	public void TakeDamage (int amount, DamageType dmgType = DamageType.None) {

		int rd = ReducedDamage (amount, dmgType);
		if (ui != null && text != null) {
			TextMeshPro instance = Instantiate (text, ui.transform) as TextMeshPro;
			instance.text = rd.ToString ();
		}

		data.stats["health"].Current -= rd;

        if (data.stats["health"].Current <= 0)
        {
            Death();
        }
    }

	// Calculates the chance for a critical hit of this character on a character of level "levelOther"
	public int CalculateCrit(int levelOther)
	{
		return (data.attributes["dexterity"].GetValue() - 2 - 2 * levelOther);
	}

	public int GetDamage(int otherLevel)
	{
		int wpnDamage;
		Weapon wpn = (Weapon)inventory.equipment[EquipLocation.Hands];
		if (wpn != null)
		{
			wpnDamage = wpn.damage;
		}
		else
		{
			wpnDamage = data.baseDamage;
		}

		return (int)((wpnDamage + data.attributes["strength"].GetValue()));
	}
	protected virtual int CalculateDamage(Character other)
    {
		int dmg =  GetDamage(other.data.level);
		int critMult = 1;
		int rng = (int)UnityEngine.Random.Range(0, 100);

		// 2: Attribute base - 1 - 2, 2: Depends on attribute points per level
		if (rng < 10 * CalculateCrit(other.data.level))
		{
			Debug.Log("CRIT!");
			critMult = 2;
		}

		return dmg * critMult;
	}
	
	public float GetAttackSpeed()
	{
		float wpnSpeed;
		Weapon wpn = (Weapon)inventory.equipment[EquipLocation.Hands];
		if (wpn != null)
		{
			wpnSpeed = wpn.speed;
		}
		else
		{
			wpnSpeed = data.baseSpeed;
		}

		return 10f / (wpnSpeed + data.attributes["dexterity"].GetValue());
	}

    protected virtual float CalculateAttackSpeed()
    {
		return GetAttackSpeed();
    }

    protected virtual int ReducedDamage (int damage, DamageType dmgType = DamageType.None)
    {
        int armAbs = 0;
        float armRel = 1f;

        EquipLocation[] loc = new EquipLocation[4] { EquipLocation.Chest, EquipLocation.Head, EquipLocation.Gloves, EquipLocation.Boots };

        for (int i = 0; i < 4; i++)
        {
            Armor arm = (Armor)inventory.equipment[loc[i]];
            if (arm != null)
            {
                armAbs += arm.absolut;
                armRel *= (1f - arm.relative);
            }
        }

        return (int) (Mathf.Clamp(damage - armAbs, 0, int.MaxValue) * armRel);
    }

	protected virtual void Death () {

		if (!gameObject.tag.Equals("Player"))
		{
			Destroy(gameObject, 1f);
		}
		// player has died, goto game over scene
		else
		{
			if (GetComponent<BoandlAnimation>() != null)
			{
				// play death animation
				GetComponent<BoandlAnimation>().Trigger("Death");

				// restore health of player
				data.stats["health"].Current = data.stats["health"].Max;
				data.stats["mana"].Current = data.stats["mana"].Max;

				// load game over scene
				PlayerController playerController = GetComponent<PlayerController>();
				playerController.StartCoroutine(playerController.Teleport());
				SceneManager.LoadScene("(F) GameOver");
			}
		}


	}

	IEnumerator AttackCooldown (float amount) {
		canAttack = false;
		yield return new WaitForSeconds (amount);
		canAttack = true;
	}

	IEnumerator Cast (Vector3 target, Character other, float amount) {
		canCast = false;
		yield return new WaitForSeconds (amount);
		if (other != null)
			activeSkill.Cast (target, other.gameObject);
		else
			activeSkill.Cast (target, null);
		canCast = true;
	}
}
