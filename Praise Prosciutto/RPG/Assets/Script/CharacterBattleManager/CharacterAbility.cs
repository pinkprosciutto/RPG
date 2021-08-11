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
    [SerializeField] AbilityCategory category;
    [SerializeField] AbilityEffect effect;
    [SerializeField] List<SecondaryEffect> secondaryEffect;
    [SerializeField] AbilityTarget target;

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

    public AbilityCategory Category
    {
        get { return category; }
    }

    public List<SecondaryEffect> SecondaryEffects
    {
        get { return secondaryEffect; }
    }

    public AbilityEffect Effects
    {
        get { return effect;  }
    }

    public AbilityTarget Target
    {
        get { return target; }
    }
}

[System.Serializable]
public class AbilityEffect
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] StatusID status;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }

    public StatusID StatusEffect
    {
        get { return status; }
    }
}

[System.Serializable]
public class SecondaryEffect : AbilityEffect //inherit
{
    [SerializeField] int chance;
    [SerializeField] AbilityTarget target;

    public int Chance
    {
        get { return chance; }
    }

    public AbilityTarget Target
    {
        get { return target; }
    }
  
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;   
}

public enum AbilityCategory
{
    Physical, Special, Status
}

public enum AbilityTarget
{
    Self, Enemy
}