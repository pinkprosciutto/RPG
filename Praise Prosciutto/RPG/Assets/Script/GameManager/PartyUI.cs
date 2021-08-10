using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text healthText;
    [SerializeField] GameObject healthBar;


    CharacterManager _character;


    public void SetData(CharacterManager character)
    {
        _character = character;

        nameText.text = character.CharacterBase.GetName;
        levelText.text = "Lv: " + character.Level;
        healthText.text = "HP " + character.HP + "/ " + character.MaxHealth;
        healthBar.transform.localScale = new Vector2(((float)character.HP / character.MaxHealth), 1f);
    }

    public void SetSelect(bool select)
    {
        if (select)
        {
            nameText.color = Color.red;
        } else
        {
            Color32 c = new Color32(94, 94, 94, 255);
            nameText.color = c;
        }
    }
    
}
