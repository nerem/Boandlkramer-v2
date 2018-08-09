using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable
{

	public Dialogue startNode;

	bool isBusy = false;
	Character busyWith = null;

	public override void Interact(Character other)
	{
		Debug.Log(other.gameObject.name + " begins to talk to " + this.name);
		FindObjectOfType<DialogueManager>().StartDialogue(startNode);
		isBusy = true;
		busyWith = other;
	}


	void Update()
	{
		if (isBusy && busyWith)
		{
			// out of range, stop conversation
			if (Vector3.Distance(busyWith.transform.position, this.transform.position) > interactionRange)
			{
				isBusy = false;
				busyWith = null;
				FindObjectOfType<DialogueManager>().EndDialogue();
			}
		}
	}
}
