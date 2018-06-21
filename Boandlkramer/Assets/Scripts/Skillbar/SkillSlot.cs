using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

	public GameObject player;

	// skill stored in this slot
	public Skill skillInSlot;

	// index of this slot
	[SerializeField]
	int index;

	// reference to the info panel to display
	[SerializeField]
	protected GameObject infoCanvas;

	[SerializeField]
	protected GameObject textSkillName;

	[SerializeField]
	protected GameObject textDescription;

	[SerializeField]
	protected GameObject textManaCost;

	// for drag and drop
	[SerializeField]
	private GameObject dragObject;

	// slot that is being dragged. -1 if not dragging anything
	public static int dragSlotIndex = -1;

	// slot we are currently pointing at
	public static int currentHoverSlotIndex = -1;



	// update visuals of the slot
	public void UpdateSlot()
	{
		Image image = GetComponentsInChildren<Image>()[1];
		if (skillInSlot != null)
		{
			image.sprite = skillInSlot.icon;
			image.enabled = true;
		}
		else
		{
			image.enabled = false;
		}
	}


	public void AddItem(Item newItem)
	{

	}

	public void ClearSlot()
	{

	}


	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if (eventData.clickCount == 1)
				OnLeftClick();
		}
		else if (eventData.button == PointerEventData.InputButton.Right)
		{
			OnRightClick();
		}

	}

	// On Mouse over event for this slot
	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		// show info box
		if (infoCanvas != null)
		{
			// fill item data
			if (skillInSlot != null && textSkillName != null && textDescription != null)
			{
				infoCanvas.SetActive(true);
				// adjust info box position
				Vector3 pos = transform.position;
				pos.x += GetComponent<RectTransform>().rect.width / 2;
				pos.y += GetComponent<RectTransform>().rect.height / 2 + 1.1f * infoCanvas.GetComponent<RectTransform>().rect.height;

				infoCanvas.transform.position = pos;

				// store slot we are hovering right now
				currentHoverSlotIndex = index;

				textSkillName.GetComponent<TextMeshProUGUI>().text = skillInSlot.name.Remove(skillInSlot.name.Length - 1) + " Level " + skillInSlot.skillLevel;
				textDescription.GetComponent<TextMeshProUGUI>().text = skillInSlot.description;

				// offensive skill, add damage information
				OffensiveSkill off = skillInSlot as OffensiveSkill;
				if (off)
				{
					if (off.damage > 0)
					{
						textDescription.GetComponent<TextMeshProUGUI>().text += "\n" + off.dmgType.ToString() + " Damage: " + off.damage.ToString();
					}
				}

				textManaCost.GetComponent<TextMeshProUGUI>().text = "Mana cost: " + skillInSlot.manaCost.ToString() + "\n";
			}
		}


	}

	// Mouse has left the slot
	public void OnPointerExit(PointerEventData eventData)
	{
		// hide description of skill
		if (infoCanvas != null)
		{
			infoCanvas.SetActive(false);

		}

		// not pointing at a skill slot anymore
		currentHoverSlotIndex = -1;
	}


	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		// use image for the spell stored in this slot
		dragObject.SetActive(true);
		dragObject.GetComponentsInChildren<Image>()[1].sprite = GetComponentsInChildren<Image>()[1].sprite;
		dragSlotIndex = index;

	}


	public virtual void OnDrag(PointerEventData eventData)
	{
		dragObject.transform.position = eventData.position;
	}


	public virtual void OnEndDrag(PointerEventData eventData)
	{

		if (currentHoverSlotIndex > -1)
		{
			Debug.Log("Switch skill from slot " + dragSlotIndex + " with skill from slot " + currentHoverSlotIndex);
			FindObjectOfType<SkillbarUI>().SwitchSkills(dragSlotIndex, currentHoverSlotIndex);
		}

		dragObject.SetActive(false);
		// end dragging
		dragSlotIndex = -1;

	}

	public virtual void OnRightClick()
	{
		// ?
	}

	public virtual void OnLeftClick()
	{
		// assign skill as active
		if (skillInSlot != null)
		{
			player.GetComponent<Character>().activeSkill = skillInSlot;
			SkillbarUI _bar = FindObjectOfType<SkillbarUI>();
			_bar.activeSkillIndex = index;
			_bar.UpdateSkillbarUI();
		}
		
	}

}
