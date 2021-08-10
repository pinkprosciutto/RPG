using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterManager
{
    [SerializeField] CharacterScriptableObject _base;
    [SerializeField] int level;
    public bool HPChanged { get; set; }

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
    public Dictionary <Stat, int> Stats { get; private set; } //calculate and store stat 
    public Dictionary<Stat, int> StatBoost { get; private set; }
    public Status _Status { get; set; }

    public Queue<string> StatusChange { get; private set; } = new Queue<string>(); //store message in a queue for status change

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
        CalculateStats();
        HP = MaxHealth;

        ResetBoost();
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt(Mathf.FloorToInt((CharacterBase.AtkPoint * ((7 + Level) / 5f)))));
        Stats.Add(Stat.SpecialAttack, Mathf.FloorToInt((CharacterBase.SpecialAtkPoint * ((7 + Level) / 5f))));
        Stats.Add(Stat.Defense, Mathf.FloorToInt((CharacterBase.DefPoint * ((7 + Level) / 5f))));
        Stats.Add(Stat.SpecialDefense, Mathf.FloorToInt((CharacterBase.SpecialDefPoint * ((7 + Level) / 5f))));
        Stats.Add(Stat.Speed, Mathf.FloorToInt((CharacterBase.Speed * ((7 + Level) / 5f))));

        MaxHealth = Mathf.FloorToInt((CharacterBase.HealthPoint * ((8 + Level) / 5f)));
    }

    void ResetBoost()
    {
        StatBoost = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.SpecialAttack, 0},
            {Stat.Defense, 0},
            {Stat.SpecialDefense, 0},
            {Stat.Speed, 0}
        };
    }
    int GetStat(Stat stat)
    {
        int statValue = Stats[stat];
        //stat boost
        int boost = StatBoost[stat];
        var boostValue = new float[] { 1f, 2f, 3f, 3.5f, 4f}; //maximum 4 boosts

        if (boost >-0)
        {
            statValue = Mathf.FloorToInt(statValue * boostValue[boost]);
        }
        else
        {
            statValue = Mathf.FloorToInt(statValue / boostValue[-boost]);
        }

        return statValue;
    }

    public void ApplyBoost(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoost[stat] = Mathf.Clamp(StatBoost[stat] + boost, -4, 4);

            if(boost > 0)
            {
                StatusChange.Enqueue($"{CharacterBase.GetName}'s {stat} rose!");
            }
            else
            {
                StatusChange.Enqueue($"{CharacterBase.GetName}'s {stat} fell!");
            }

            Debug.Log($"{stat} boosted to {StatBoost[stat]}");
        }
    }
    public int MaxHealth { get; private set; }
   
    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int SpecialAttack
    {
        get { return GetStat(Stat.SpecialAttack); ; }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); ; }
    }

    public int SpecialDefense
    {
        get { return GetStat(Stat.SpecialDefense); ; }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }
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
        float attack = (ability.Ability.Category == AbilityCategory.Special) ? attacker.SpecialAttack : attacker.Attack;
        float defense = (ability.Ability.Category == AbilityCategory.Special) ? SpecialDefense : Defense;

        float damageModifier = Random.Range(0.8f, 1f) * element * critHit;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * ability.Ability.Strength * ((float)attack / defense) + 2;
        int dmg = Mathf.FloorToInt(d * damageModifier);

        UpdateHP(dmg);

        return damageDetail;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHealth); //don't go below zero
        HPChanged = true; //to update Hp UI
    }

    public void SetStatus(StatusID statusID)
    {
        _Status = StatusData._Status[statusID];
        StatusChange.Enqueue($"{CharacterBase.GetName} {_Status.StatusMessage}");

    }

    public AbilityManager GetAbilityForEnemy()
    {
        int random = Random.Range(0, charAbility.Count);
        return charAbility[random];
    }

    public void OnAfterTurn()
    {
        _Status?.OnAfterTurn?.Invoke(this); //will be called if it's not null
    }

    public void OnBattleOver()
    {
        ResetBoost();
    }
}

public class DamageDetail
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }

    public float Element { get; set; }
}
