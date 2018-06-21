using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public GameObject player;

    // item stored at this slot
    protected Item item;

    // picture in this slot
    public Image icon;

    // reference to the pickup prefab for spawning the object when kicking it out of the inventory
    public GameObject spawn;

    // for showing item information on mouse over
    public GameObject infoCanvas;

    // the text boxes that are filled with the item data
    public Text textName;
    public Text textStats;
    public Text textDescription;

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (eventData.clickCount == 1)
                OnLeftClick();
            else if (eventData.clickCount == 2)
                OnLeftDoubleClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }

    }

    // On Mouse over event for this slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show description of item if there is any
        if (item != null)
        {
            // show info box
            infoCanvas.SetActive(true);

            // fill item data
            textName.text = item.name;
			string attributes = "";
			string defenses = "";
			string attacks = "";
			
			foreach (AttributeModifier atr in item.attributeMods)
			{
				if (atr.amount > 0)
				{
					attributes += atr.name + ": +" + atr.amount.ToString() + "\n";
				}
			}

			foreach (DefenseModifier def in item.defenseMods)
			{
				if (def.absolute > 0 || def.relative > 0)
				{
					defenses += def.name + ": +" + def.absolute.ToString() + ", + " + def.relative.ToString() + "% \n";
				}
			}

			foreach (AttackModifier att in item.attackMods)
			{
				if (att.damage > 0)
				{
					attacks += att.name + ": +" + att.damage.ToString() + "\n";
				}
			}
			textStats.text = attributes + "\n" + defenses + "\n" + attacks;
			textDescription.text = item.description;

            // adjust info box position
            Vector3 pos = icon.transform.position;
            pos.x -= icon.rectTransform.rect.width / 2;
            pos.y += icon.rectTransform.rect.height / 2;

            infoCanvas.transform.position = pos;

        }
    }

    // Mouse has left the slot
    public void OnPointerExit(PointerEventData eventData)
    {
        // hide description of item
        infoCanvas.SetActive(false);
    }

    public virtual void OnRightClick()
    {
        // remove item from inventory if there is any and place it in the world
        ThrowItemAway();
    }
    public virtual void OnLeftClick()
    {
        Debug.Log("Left click");
    }

    public virtual void OnLeftDoubleClick()
    {
        // cast item to equipment
        Item eq = (Item)item;

        // if cast was succesfull, we will equip the item
        if (eq != null)
        {
            // if item is not equipable, return
            if (eq.equipTo == EquipLocation.None)
                return;

            Item formerEquipment = null;
            // first we check if there is already some item in the corresponding equipment slot
            if (Inventory.instance.equipment.ContainsKey(eq.equipTo))
            {
                formerEquipment = Inventory.instance.equipment[eq.equipTo];

                if (formerEquipment != null)
                {
                    Debug.Log("We need to swap equipment");
                    Debug.Log(formerEquipment.name);
                    // remove equipment from inventory
                    Inventory.instance.Remove(item);

                    // put off currently worn equipment and put it to the inventory
                    Inventory.instance.Unequip(formerEquipment);

                    // equip the item we just removed from the inventory
                    Inventory.instance.Equip(eq);

                }
                else
                {
                    // apparently, nothing is on this slot yet, just equip the item
                    Inventory.instance.Equip(eq);
                }
            }
            else
            {
                // nothing is on this slot yet, just equip the item
                Inventory.instance.Equip(eq);
            }

            // close description window
            infoCanvas.SetActive(false);

        }
    }


    protected void ThrowItemAway()
    {
        // check if there is an item on this slot
        if (item != null)
        {
			// if this is the case, spawn a pickup object in the world carrying exactly this item
			item.Spawn (new Vector3 (Inventory.instance.transform.position.x, 0f, Inventory.instance.transform.position.z));

            // now the item is not in the inventory any more
            Inventory.instance.Remove(item);

            // close description window
            infoCanvas.SetActive(false);
        }
    }


}
