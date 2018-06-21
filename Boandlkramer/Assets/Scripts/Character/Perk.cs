using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Perk", menuName = "Character/Perk", order = 2)]
public class Perk : ScriptableObject {

	public string description;

	public List<AttributeModifier> attributeMods = new List<AttributeModifier> ();
}
