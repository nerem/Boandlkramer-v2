using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Pickup : Interactable {

    public Item item;

    public override void Interact(Character other)
    {
        Debug.Log("Picking up " + item.name);
        Debug.Log("Description: " + item.description);

        // Add the item to the inventory if possible
        if (Inventory.instance.Add(item))
        {
			if (item.bAutoInteract) {
				GameObject instance = Instantiate (Inventory.instance.pickupUI, transform.position, Quaternion.identity) as GameObject;
				if (item is Gold)
					instance.GetComponentInChildren<TextMeshPro> ().text = ((Gold)item).amount.ToString() +  " Gold";
				if (item is Potion)
					instance.GetComponentInChildren<TextMeshPro> ().text = "Potion";
			}
			Destroy (this.gameObject);
        }
    }

}
