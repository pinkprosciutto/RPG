using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//create new abilities in unity with scriptable object
[CreateAssetMenu(fileName = "Ability", menuName = "Character/Add a new ability")]
public class CharacterAbility : ScriptableObject
{
    [TextArea]
    [SerializeField] string abilityName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] CharacterElement element;
    [SerializeField] int abilityAccuracy;
    [SerializeField] int abilityStrength;
    [SerializeField] int abilityAmount;

    public string GetName
    {
        get { return abilityName; }
    }

    public string Description
    {
        get { return description; }
    }
   
    public CharacterElement Element
    {
        get { return element; }
    }

    public int Accuracy
    {
        get { return abilityAccuracy; }
    }

    public int Strength
    {
        get { return abilityStrength; }
    }

    public int Amount
    {
        get { return abilityAmount; }
    }


}
