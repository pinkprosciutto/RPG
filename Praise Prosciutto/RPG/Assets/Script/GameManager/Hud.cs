using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
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

    public IEnumerator UpdateHealth()
    {
        //yield return healthBar.transform.localScale = new Vector2(((float)_character.HP / _character.MaxHealth), 1f);
        if (_character.HPChanged)
        {
            yield return decreaseHPBar((float)_character.HP / _character.MaxHealth);
            yield return healthText.text = "HP " + _character.HP + "/ " + _character.MaxHealth;
            _character.HPChanged = false;
        }
    }

    public IEnumerator decreaseHPBar(float newHP)
    {
        float currentHP = healthBar.transform.localScale.x;
        float reduceHP = currentHP - newHP;

        while(currentHP - newHP > Mathf.Epsilon)
        {
            currentHP -= reduceHP * Time.deltaTime;
            healthBar.transform.localScale = new Vector3(currentHP, 1f);
            yield return null;
        }
        healthBar.transform.localScale = new Vector3(newHP, 1f);
    }

}
