
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CharacterUI : MonoBehaviour
{

	// reference to the UI for toggling visibility
	public GameObject characterUI;

	// reference to the player object
	public GameObject player;

	// Text boxes for stats
	public TextMeshProUGUI textStrength;
	public TextMeshProUGUI textDexterity;
	public TextMeshProUGUI textIntelligence;
	public TextMeshProUGUI textVitality;

	// Text box and progress bar for experience
	public TextMeshProUGUI textExperience;
	public Slider sliderExperience;

	// Textbox for Level
	public TextMeshProUGUI textLevel;

    // Textbox for remaining attribute points
    public TextMeshProUGUI textRemainingAttributePoints;

    // Buttons
    public Button[] attributeIncreaseButtons;

	// reference to the info panel to display
	[SerializeField]
	GameObject infoCanvas;

	[SerializeField]
	GameObject textAttributeName;

	[SerializeField]
	GameObject textDescription;

	[SerializeField]
	GameObject textEffect;

	CharacterData charData;
	Character character;

	void Start()
	{
		// safe character data for quick access
		charData = player.GetComponent<Character>().data;
		character = player.GetComponent<Character>();

		UpdateCharacterUI();
	}


	// Update is called once per frame
	void Update()
	{
		// toggle visibility of inventory
		if (Input.GetButtonDown("Character"))
		{
			if (characterUI.activeSelf)
			{
				characterUI.GetComponent<Canvas>().sortingOrder = 0;
				characterUI.SetActive(false);
			}
			else
			{
				characterUI.SetActive(true);
				characterUI.GetComponent<Canvas>().sortingOrder = UICanvasSorting.sorting++;
			}

			// hide info canvas if window was closed
			if (!characterUI.activeSelf)
			{
				infoCanvas.transform.position = new Vector2(-200f, -200f);
				infoCanvas.SetActive(false);
			}
		}

		// update character values while window is opened
		if (characterUI.activeSelf)
		{
			UpdateCharacterUI();
		}

	}

    public void Increase(string attribute)
    {
        charData.SpendAttributePoint(attribute);
        UpdateCharacterUI();

		// update info canvas
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
	}

	public void UpdateCharacterUI()
	{
		// fill in stats
		// Strength

		textStrength.text = charData.attributes["strength"].GetBaseValue().ToString();
		if (charData.attributes["strength"].GetModifierValue() > 0)
		{
			textStrength.text += "<color=blue> + " + charData.attributes["strength"].GetModifierValue().ToString();
		}

		// Dexterity
		textDexterity.text = charData.attributes["dexterity"].GetBaseValue().ToString();
		if (charData.attributes["dexterity"].GetModifierValue() > 0)
		{
			textDexterity.text += "<color=blue> + " + charData.attributes["dexterity"].GetModifierValue().ToString();
		}

		// Intelligence
		textIntelligence.text = charData.attributes["intelligence"].GetBaseValue().ToString();
		if (charData.attributes["intelligence"].GetModifierValue() > 0)
		{
			textIntelligence.text += "<color=blue> + " + charData.attributes["intelligence"].GetModifierValue().ToString();

		}

		// Vitality
		textVitality.text = charData.attributes["vitality"].GetBaseValue().ToString();
		if (charData.attributes["vitality"].GetModifierValue() > 0)
		{
			textVitality.text += "<color=blue> + " + charData.attributes["vitality"].GetModifierValue().ToString();
		}


		// fill in level
		int level = charData.level;
		textLevel.text = "Level " + level.ToString();

		// fill in experience
		int currentXP = charData.GetCurrentExperiencePoints();
		int neededXP = charData.CalculateExperienceForLevel(level + 1);
		int lastLevelXP = charData.CalculateExperienceForLevel(level);
		textExperience.text = lastLevelXP.ToString() + " / " + "<color=blue><b>" + currentXP.ToString() + "</b> / " + "<color=white>" + neededXP.ToString();
        float xpRatio = ((float)currentXP - (float)lastLevelXP) / ((float)neededXP - (float)lastLevelXP);
        sliderExperience.value = xpRatio;


        // fill in remaining attribute points
        int remainingAttributePoints = charData.GetRemainingAttributePoints();
        textRemainingAttributePoints.text = remainingAttributePoints.ToString();

        // if there are attribute points left to spend, we activate the buttons for doing so
        if (remainingAttributePoints > 0)
        {
            foreach (Button btn in attributeIncreaseButtons)
            {
                btn.interactable = true;
            }
        }
        else
        {
            foreach (Button btn in attributeIncreaseButtons)
            {
                btn.interactable = false;
            }
        }

	}
}
