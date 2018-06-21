using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Potion", menuName = "Items/Potion", order = 5)]
public class Potion : Item {

	public string target;
	public int amount;
}
