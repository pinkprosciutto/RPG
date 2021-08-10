using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterManager
{
    [SerializeField] CharacterScriptableObject _base;
    [SerializeField] int level;

    public CharacterScriptableObject CharacterBase 
    {
        get
        {
            return _base;
        }
    }
    public int Level
    {
        get
        {
            return level;
        }
    }

    public int HP { get; set; }

    public List<AbilityManager> charAbility { get; set; }

    public void Initialize()
    {
        HP = MaxHealth;

        //Learn new abilities depending on its level
        charAbility = new List<AbilityManager>();
        foreach (var ability in CharacterBase.LearnableAbilities)
        {
            if (ability.Level <= Level)
            {
                charAbility.Add(new AbilityManager(ability.Ability));
            }

            if (charAbility.Count >= 6)
            {
                break;
            }
        }
    }

    public int MaxHealth
    {
        get { return Mathf.FloorToInt((CharacterBase.HealthPoint * ((8 + Level) / 5f))); }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((CharacterBase.AtkPoint * ((7 + Level) / 5f))); }
    }

    public int SpecialAttack
    {
        get { return Mathf.FloorToInt((CharacterBase.SpecialAtkPoint * ((7 + Level) / 5f))); }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((CharacterBase.DefPoint * ((7 + Level) / 5f))); }
    }

    public int SpecialDefense
    {
        get { return Mathf.FloorToInt((CharacterBase.SpecialDefPoint * ((7 + Level) / 5f))); }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((CharacterBase.Speed * ((7 + Level) / 5f))); }
    }

    public DamageDetail TakeDamage(AbilityManager ability, CharacterManager attacker)
    {
        //crit chance
        float critHit = 1f;
        if (Random.value * 100f <= 5f)
        {
            critHit = 2f;
        }

        if (ability.Ability.GetName == "Wink")
        {
            critHit = 2f;
        }

        float element = Element.GetEffectivness(ability.Ability.Element, CharacterBase.Element1) * Element.GetEffectivness(ability.Ability.Element, CharacterBase.Element2);

        var damageDetail = new DamageDetail()
        {
            Element = element,
            Critical = critHit,
            Fainted = false
        };

        //check if it's physical or elemental attack
        float attack = (ability.Ability.IsSpecial) ? attacker.SpecialAttack : attacker.Attack;
        float defense = (ability.Ability.IsSpecial) ? SpecialDefense : Defense;

        float damageModifier = Random.Range(0.8f, 1f) * element * critHit;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * ability.Ability.Strength * ((float)attack / defense) + 2;
        int dmg = Mathf.FloorToInt(d * damageModifier);

        HP -= dmg;

        if(HP <= 0)
        {
            HP = 0;
            damageDetail.Fainted = true; //fainted
        }

        return damageDetail;
    }

    public AbilityManager GetAbilityForEnemy()
    {
        int random = Random.Range(0, charAbility.Count);
        return charAbility[random];
    }
}

public class DamageDetail
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }

    public float Element { get; set; }
}
