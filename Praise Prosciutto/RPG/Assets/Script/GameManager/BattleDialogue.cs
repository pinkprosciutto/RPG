using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogue : MonoBehaviour
{
    [SerializeField] Text dialogueText;
    [SerializeField] float textSpeed;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject abilitySelector;
    [SerializeField] GameObject abilityDetail;

    [SerializeField] List<Text> actionText;
    [SerializeField] List<Text> abilityText;

    [SerializeField] Text amountText;
    [SerializeField] Text typeText;
    public void SetDialogue(string dialogue)
    {
        dialogueText.text = dialogue; 
    }


    //type out text
    public IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";
        foreach(var letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / textSpeed);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogue(bool enable)
    {
        dialogueText.enabled = enable;
    }

    public void EnableAbilitySelector(bool enable)
    {
        abilitySelector.SetActive(enable);
        abilityDetail.SetActive(enable);
    }

    public void EnableActionSelect(bool enable)
    {
        actionSelector.SetActive(enable);
    }

    public void SetAbility(List<AbilityManager> abilities)
    {
        for (int x = 0; x < abilityText.Count; ++x)
        {
            if (x < abilities.Count)
            {
                abilityText[x].text = abilities[x].Ability.GetName;
            }
            else
            {
                abilityText[x].text = "-"; // No new moves
            }
        }
    }

    public void UpdateAbilitySelect(int select, AbilityManager ability)
    {
        Color32 c = new Color32(94, 94, 94, 255);
        Color32 d = new Color32(205, 134, 30, 255);
        for (int x = 0; x < abilityText.Count; ++x)
        {
            if (x == select)
            {
                abilityText[x].color = Color.red;
            }
            else
            {
                abilityText[x].color = c;
            }
        }

        amountText.text = $"Amount\n{ability.Amount}/{ability.Ability.Amount}";
        typeText.text = ability.Ability.Element.ToString();

        if (ability.Amount <= 0)
            amountText.color = d;
        else
            amountText.color = c;
    }

    public void UpdateActionSelect(int select)
    {
        for (int x = 0; x < actionText.Count; ++x)
        {
            if (x == select)
            {
                actionText[x].color = Color.red;
            }
            else
            {
                Color32 c = new Color32(94, 94, 94, 255);
                actionText[x].color = c;
            }
        }
    }

}
