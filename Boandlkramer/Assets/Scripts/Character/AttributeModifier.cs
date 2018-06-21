using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Attribute Modifier", menuName = "Modifiers/Attribute Modifier", order = 1)]
public class AttributeModifier : ScriptableObject {

	public string description;
	public string target;

	public int amount;
}
