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
		// highlight answer on mouse over
		txtBox.text = "<color=white>" + textDefault + "</color>";
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		// restore highlighting
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
		// use this anser


		// answer leads to end of dialogue
		if (next == null || next.text == "")
		{
			Debug.Log("TEST!!!!!!");
			FindObjectOfType<DialogueManager>().EndDialogue();
			return;
		}

		// add next dialogue node
		FindObjectOfType<DialogueManager>().StartDialogue(next);
	}

}
