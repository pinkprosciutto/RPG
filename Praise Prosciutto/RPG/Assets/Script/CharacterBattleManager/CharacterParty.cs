using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterParty : MonoBehaviour
{
    [SerializeField] List<CharacterManager> characters;

    public List<CharacterManager> Characters
    {
        get
        {
            return characters;
        }
    }

    void Start()
    {
        foreach (var character in characters)
        {
            character.Initialize(); //initialize each characters in CharacterManager
        }
    }

    public CharacterManager HealthyCharacter()
    {
        return characters.Where(i => i.HP > 0).FirstOrDefault(); //loop through the party, return HP whenever the condition is satisfied
    }
}
