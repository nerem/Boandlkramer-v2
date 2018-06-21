using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Defense Modifier", menuName = "Modifiers/Defense Modifier", order = 3)]
public class DefenseModifier : ScriptableObject {

	public string description;

	public ArmorType type;
	public int absolute;
	public float relative;
}
