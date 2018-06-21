using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipLocation { None, Hands, Chest, Head, Gloves, Boots };

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item", order = 1)]
public class Item : ScriptableObject
{

    // slot on which this item can be equipped
    public EquipLocation equipTo;

    public Sprite icon;
    public Mesh mesh;

    public string description;

    public List<AttributeModifier> attributeMods = new List<AttributeModifier>();
    public List<DefenseModifier> defenseMods = new List<DefenseModifier>();
    public List<AttackModifier> attackMods = new List<AttackModifier>();

    public bool bIsUnique = false;

	// can be used to make the object automatically interacting within a certain range and certain characters / other objects 
	// (see player controller)
	public bool bAutoInteract = false;


	public void Spawn(Vector3 pos, int level = 1)
    {

        GameObject go = new GameObject(name);
	
        go.transform.position = new Vector3(pos.x, pos.y + .1f, pos.z);
        go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		go.AddComponent<Rotator>();
        go.layer = 8;
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = mesh;
        go.AddComponent<BoxCollider>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.material.color = Color.red;
        Pickup pickup = go.AddComponent<Pickup>();
        pickup.item = this;
		// initialize drop, dependent on the level of the character that dropped the item
		pickup.item.Init(level);
		go.tag = "Pickup";
    }

	public virtual void Init(int level = 1)
	{

	}



    public void AddModifier(CharacterData charData)
    {

        foreach (AttributeModifier mod in attributeMods)
        {
            charData.attributes[mod.target].AddModifier(mod);
        }



        foreach (AttackModifier mod in attackMods)
        {
            // todo
        }


        foreach (DefenseModifier mod in defenseMods)
        {
            // todo
        }
    }

    public void RemoveModifier(CharacterData charData)
    {

        foreach (AttributeModifier mod in attributeMods)
        {
            charData.attributes[mod.target].RemoveModifier(mod);
        }


        foreach (AttackModifier mod in attackMods)
        {
            // todo
        }


        foreach (DefenseModifier mod in defenseMods)
        {
            // todo
        }

    }

    public void MakeUnique()
    {
        if (bIsUnique)
            return;

        List<AttributeModifier> tempAttributeMods = new List<AttributeModifier>();
        List<DefenseModifier> tempDefenseMods = new List<DefenseModifier>();
        List<AttackModifier> tempAttackMods = new List<AttackModifier>();

        foreach (AttributeModifier mod in attributeMods)
        {
            AttributeModifier tmpMod = Instantiate(mod);
            tmpMod.name = mod.name;
            tempAttributeMods.Add(tmpMod);
        }
        attributeMods = tempAttributeMods;

        foreach (AttackModifier mod in attackMods)
        {
            AttackModifier tmpMod = Instantiate(mod);
            tmpMod.name = mod.name;
            tempAttackMods.Add(tmpMod);
        }
        attackMods = tempAttackMods;

        foreach (DefenseModifier mod in defenseMods)
        {
            DefenseModifier tmpMod = Instantiate(mod);
            tmpMod.name = mod.name;
            tempDefenseMods.Add(tmpMod);
        }
        defenseMods = tempDefenseMods;

        bIsUnique = true;

    }
}
