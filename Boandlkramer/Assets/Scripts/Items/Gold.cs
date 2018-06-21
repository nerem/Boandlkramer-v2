using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gold", menuName = "Items/Gold", order = 2)]

public class Gold : Item {

    // Amount of gold
    public int amount = 1;

	// factor for gold amount dependent on enemy level
	int factor = 10;
	
	public override void Init(int level)
	{
		amount = Random.Range(level * factor, (level + 2) * factor);
		description = amount.ToString();
	}

}
