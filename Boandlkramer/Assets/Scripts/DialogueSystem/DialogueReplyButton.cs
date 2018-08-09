using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueReplyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler{

	Text txtBox;
	string textDefault;
	public Dialogue next;
	public int key;
	public Reply reply;

	// set to true if answer was given
	public bool bGiven = false;

	public GameObject contentParent;

	void Start()
	{
		txtBox = GetComponent<Text>();
		textDefault = txtBox.text;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ReplyClick();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!bGiven)
		{
			// highlight answer on mouse over
			txtBox.text = "<color=white>" + textDefault + "</color>";
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		txtBox.text = textDefault;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha0 + key))
		{
			ReplyClick();
		}
	}

	void ReplyClick()
	{
		if (!bGiven)
		{

			// answer leads to end of dialogue
			if (next == null || next.text == "")
			{
				Debug.Log("TEST!!!!!!");
				FindObjectOfType<DialogueManager>().EndDialogue();
				return;
			}

			// this answer is marked as given
			bGiven = true;

			// delete any other replies and make the given answer default (mouse is probably still over the text...)
			int iElements = contentParent.transform.childCount;
			for (int i = 0; i < iElements; i++)
			{
				var child = contentParent.transform.GetChild(i);
				DialogueReplyButton replyButton = child.GetComponent<DialogueReplyButton>();
				if (replyButton)
				{
					
					if (!replyButton.bGiven)
					{
						Destroy(child.gameObject);
					}
					else
					{
						replyButton.txtBox.text = replyButton.textDefault;
					}
				}
			}

			// add next dialogue node
			FindObjectOfType<DialogueManager>().StartDialogue(next);
		}
	}

}
