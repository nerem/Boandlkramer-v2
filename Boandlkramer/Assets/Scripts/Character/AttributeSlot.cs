using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;


public class AttributeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    // specify the attribute of this slot ["strength", "vitality", "dexterity", "intelligence"]
    public string attribute;

    [SerializeField]
    string description;

    // reference to the info panel to display
    [SerializeField]
    GameObject infoCanvas;

    [SerializeField]
    GameObject textAttributeName;

    [SerializeField]
    GameObject textDescription;

	[SerializeField]
	GameObject textEffect;


    // reference to the player object
    [SerializeField]
    GameObject player;

    Character character;


    void Start()
    {
        // safe character for quick access
        character = player.GetComponent<Character>();

    }

    // On Mouse over event for this slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show description of item if there is any
        if (attribute != "")
        {
            // show info box
            infoCanvas.SetActive(true);

			// fill item data
			textAttributeName.GetComponent<TextMeshProUGUI>().text = attribute.Remove(1).ToUpper() + attribute.Substring(1);
			textDescription.GetComponent<TextMeshProUGUI>().text = description;

			// fill in information on how the current value of the attribute influences stats of the player
			switch (attribute)
			{
				case "dexterity":
					float critChance = Mathf.Max(character.CalculateCrit(character.data.level), 0f);
					float attackSpeed = Mathf.Max(character.GetAttackSpeed(), 0f);
					textEffect.GetComponent<TextMeshProUGUI>().text = "Critical hit chance: " + critChance + "% \n"
						+ "Melee attack speed: " + attackSpeed.ToString().Remove(3);
					break;

				case "strength":
					float damage = character.GetDamage(character.data.level);
					textEffect.GetComponent<TextMeshProUGUI>().text = "Base melee damage: " + damage;
					break;

				case "vitality":
					textEffect.GetComponent<TextMeshProUGUI>().text = "";
					break;

				case "intelligence":
					textEffect.GetComponent<TextMeshProUGUI>().text = "";
					break;
			}

			// adjust info box position
			Vector3 pos = transform.position;
            pos.x += GetComponent<RectTransform>().rect.width / 2;
            pos.y += GetComponent<RectTransform>().rect.height / 2 + 1.1f * infoCanvas.GetComponent<RectTransform>().rect.height;

            infoCanvas.transform.position = pos;

        }
    }

    // Mouse has left the slot
    public void OnPointerExit(PointerEventData eventData)
    {
        // hide description of item
        infoCanvas.SetActive(false);
    }



}