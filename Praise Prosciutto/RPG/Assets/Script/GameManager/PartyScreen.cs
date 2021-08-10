using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;

    PartyUI[] characterSlot;
    List<CharacterManager> characters;

    public void Initialize()
    {
        characterSlot = GetComponentsInChildren<PartyUI>();
    }

    public void SetPartyData(List<CharacterManager> character)
    {
        characters = character;

        for (int x = 0; x < characterSlot.Length; x++)
        {
            if (x < character.Count)
            {
                characterSlot[x].SetData(character[x]);
            }
            else
            {
                characterSlot[x].gameObject.SetActive(false);
            }

            messageText.text = "Choose a character";
        }
    }

    public void UpdateMemberSelect(int select)
    {
        for(int x = 0; x < characters.Count; x++)
        {
            if (x == select)
            {
                characterSlot[x].SetSelect(true);
            }
            else
            {
                characterSlot[x].SetSelect(false);
            }
        }
    }

    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}
