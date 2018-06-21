
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    #region Singleton
    public static Inventory instance;

    void Awake ()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one inventory created!");
            return;
        }
        instance = this;
    }

    #endregion

    // for updating the graphical representation of the inventory if the inventory has changed
    public delegate void HasChanged();
    public HasChanged hasChangedCallback;

    // for updating the skillbar when amount of potion changes
    public delegate void PotionsHaveChanged();
    public PotionsHaveChanged potionsHaveChangedCallback;

    // size of the inventory
    public int size = 20;

    // list for the items the player carries
    public List<Item> items = new List<Item>();

    // amount of gold the player owns
    public int gold = 0;

    // amount of health potions the player possesses
    public int healthPotions = 0;

    // amound of mana potions the player possesses
    public int manaPotions = 0;

    // for reference the player and access character data
    public Transform Player;
    CharacterData playerData;

    // reference to character UI
    public CharacterUI charUI;

    // for currently equipped items
    public Dictionary<EquipLocation, Item> equipment = new Dictionary<EquipLocation, Item>() { { EquipLocation.Hands, null }, { EquipLocation.Chest, null }, { EquipLocation.Head, null }, { EquipLocation.Gloves, null }, { EquipLocation.Boots, null }};

	public GameObject pickupUI;

    void Start()
    {
        playerData = Player.GetComponent<Character>().data;
    }



    // adds an item to the inventory
    public bool Add(Item item)
    {
        // item is gold
        if (item.GetType() == typeof(Gold))
        {
            AddGold(((Gold)item).amount);
            return true;
        }

        // item is potion
        if (item.GetType() == typeof(Potion))
        {
            Potion pot = (Potion)item;
            
            if (pot.target == "Health")
            {
                AddHealthPotions(pot.amount);
            }
            else if (pot.target == "Mana")
            {
                AddManaPotions(pot.amount);
            }

            return true;
        }

        // check if there is some room left for the new item
        if (items.Count >= size)
        {
            Debug.Log("Inventory is full.");
            return false;
        }

        Item itemToAdd = Instantiate(item);
        itemToAdd.name = item.name;
        itemToAdd.MakeUnique();

        items.Add(itemToAdd);

        if (hasChangedCallback != null)
            hasChangedCallback.Invoke();

        return true;
    }

    // removes an item from the inventory
    public void Remove(Item item)
    {
        items.Remove(item);
        if (hasChangedCallback != null)
            hasChangedCallback.Invoke();
    }

    // adds (substracts for negativ sign) gold to the inventory
    public void AddGold(int amount)
    {
        gold += amount;
        if (gold < 0)
            gold = 0;
        if (hasChangedCallback != null)
            hasChangedCallback.Invoke();
    }

    // adds (subtracts for negative sign) health potions to the inventory
    public void AddHealthPotions(int amount)
    {
        healthPotions += amount;
        if (healthPotions < 0)
            healthPotions = 0;

        if(potionsHaveChangedCallback != null)
        {
            potionsHaveChangedCallback.Invoke();
        }
    }

    // adds (subtracts for negative sign) mana potions to the inventory
    public void AddManaPotions(int amount)
    {
        manaPotions += amount;
        if (manaPotions < 0)
            manaPotions = 0;

        if (potionsHaveChangedCallback != null)
        {
            potionsHaveChangedCallback.Invoke();
        }
    }

    // puts equipment from the inventory to current equipment
    public void Equip(Item equip)
    {
        // and remove it from the inventory
        items.Remove((Item)equip);

        // put the new equipment to the slot
        equipment[equip.equipTo] = equip;
        equip.AddModifier(playerData);

        charUI.UpdateCharacterUI();

        // Update UI
        if (hasChangedCallback != null)
            hasChangedCallback.Invoke();
    }


    public void Unequip(Item equip)
    {
        if (!Add(equip))
        {
            Debug.Log("Inventory is full!");
            return;
        }
        else
        {
            Debug.Log("Putting " + equip.name + " to the inventory again!");
            equipment.Remove(equip.equipTo);
            equip.RemoveModifier(playerData);

            charUI.UpdateCharacterUI();
                
            // Update UI
            if (hasChangedCallback != null)
                hasChangedCallback.Invoke();
        }
    }

}
