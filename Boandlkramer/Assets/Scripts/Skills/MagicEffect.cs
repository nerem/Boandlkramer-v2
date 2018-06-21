using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagicEffect", menuName = "MagicEffect/MagicEffect", order = 1)]
public class MagicEffect : ScriptableObject {


	// time this effect lasts on the character
	public float totalTime = 3f;

	// effect on different stats of the character
	public float movementMultiplier = 1f;							// affect movement speed values by this factor
	public float damageMultiplier = 1f;								// affect damage values by this factor
	public int baseDamageOverTime = 0;								// deal baseDamageOverTime per damageOverTimeTickRate
	public float damageOverTimeTickRate = 1.0f;
	public DamageType damageOverTimeType = DamageType.None;			// damage type for damage over time

    public GameObject meshModifier;									// visual mesh effect while magic effect is active
}
