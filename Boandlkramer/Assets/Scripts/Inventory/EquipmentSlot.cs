using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : InventorySlot {


    public EquipLocation equipmentSlotType;

    public override void OnRightClick()
    {
        // unequip item and put it back to the inventory (if possible)
        Debug.Log("Equipment Right Click");
        if (item != null)
        {
            Inventory.instance.Unequip((Item)item);
            // close description window
            infoCanvas.SetActive(false);
        }
        else
            Debug.Log("equip is null");
            
    }

    public override void OnLeftClick()
    {
        Debug.Log("Equipment Left Click");
    }

    public override void OnLeftDoubleClick()
    {
        // double click also unequips item
        if (item != null)
        {
            Inventory.instance.Unequip((Item)item);
            // close description window
            infoCanvas.SetActive(false);
        }
    }

}
