using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

	// sliding UI in and out
	public Animator animator;

	// add dialogue text and replys to this game object
	public GameObject contentParent;

	// Texts and Replys to create
	[SerializeField]
	GameObject dialogueTextBoxPrefab;
	[SerializeField]
	GameObject replyPrefab;

	List<GameObject> currentReplies = new List<GameObject>();

	public void StartDialogue(Dialogue DialogueNode)
	{
		animator.SetBool("isOpen", true);
		AddDialogue(DialogueNode);
	}


	public void EndDialogue()
	{
		// delete everything such that we can start from a plain dialogue ui next time...
		int iElements = contentParent.transform.childCount;
		for (int i = 0; i < iElements; i++)
		{
			var child = contentParent.transform.GetChild(i);
			Destroy(child.gameObject);

		}
		currentReplies.Clear();
		animator.SetBool("isOpen", false);
		Debug.Log("close dialogue animation");
	}

	void AddDialogue(Dialogue dialogue)
	{

		// create a text box containing the sentence
		string text = "<color=yellow>" + dialogue.name + ":</color> " + dialogue.text;

		GameObject go = Instantiate(dialogueTextBoxPrefab, contentParent.transform);
		go.GetComponent<Text>().text = text + "\n";

		// next, add possible replies
		for (int i = 0; i < dialogue.replies.Count; i++)
		{
			GameObject replyGO = Instantiate(replyPrefab, contentParent.transform);
			replyGO.GetComponent<DialogueReplyButton>().reply = dialogue.replies[i];
			replyGO.GetComponent<DialogueReplyButton>().contentParent = contentParent;
			replyGO.GetComponent<Text>().text = (i+1) + ") " + dialogue.replies[i].reply;

			if (dialogue.replies[i].replyDialogue.text == "")
			{
				// if there is no next dialogue to this reply, we add and "[Ende]" tag in order to indicate that this answer will 
				// end the dialogue.
				replyGO.GetComponent<Text>().text += " [Ende]";
			}

			replyGO.GetComponent<DialogueReplyButton>().key = i + 1;								// set hotkey to associated keyboard key
			replyGO.GetComponent<DialogueReplyButton>().next = dialogue.replies[i].replyDialogue;	// link next dialogue

			// for the last element we add a new line
			if (i == dialogue.replies.Count - 1)
			{
				replyGO.GetComponent<Text>().text += "\n";

			}
			currentReplies.Add(replyGO);
		}
	}
}
