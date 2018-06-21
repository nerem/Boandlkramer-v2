using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Armor", menuName = "Items/Armor", order = 2)]
public class Armor : Item {

	public ArmorType type;
	public int absolut;
	public float relative;


}

public enum ArmorType { Light, Medium, Heavy, Fire, Ice, Lightning }