using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager
{
    public CharacterScriptableObject CharacterBase { get; set; }
    public int Level { get; set; }

    public int HP { get; set; }

    public List<AbilityManager> charAbility { get; set; }

    public CharacterManager(CharacterScriptableObject baseChar, int baseLevel)
    {
        CharacterBase = baseChar;
        Level = baseLevel;
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
        get { return Mathf.FloorToInt((CharacterBase.AtkPoint * ((15 + Level) / 5f))); }
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

        float damageModifier = Random.Range(0.8f, 1f) * element * critHit;
        float attack = (2 * attacker.Level + 10) / 250f;
        float d = attack * ability.Ability.Strength * ((float)attacker.Attack / Defense) + 2;
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
