using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//creating new character in Unity   
[CreateAssetMenu(fileName = "Character", menuName = "Character/Create a new character")]

public class CharacterScriptableObject : ScriptableObject
{
    [SerializeField] string characterName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite charSprite;

    [SerializeField] CharacterElement element1;
    [SerializeField] CharacterElement element2;

    //stats
    [SerializeField] int health;
    [SerializeField] int attackPoint;
    [SerializeField] int defensePoint;
    [SerializeField] int specialAttackPoint;
    [SerializeField] int specialDefensePoint;
    [SerializeField] int speed;

    [SerializeField] List<LearnableAbility> learnableAbility; //list of learnable moves depending on level

    public string GetName
    {
        get { return characterName; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite CharacterSprite
    {
        get { return charSprite; }
    }

    public int HealthPoint
    {
        get { return health; }
    }

    public CharacterElement Element1
    {
        get { return element1; }
    }

    public CharacterElement Element2
    {
        get { return element2; }
    }

    public int AtkPoint
    {
        get { return attackPoint; }
    }

    public int SpecialAtkPoint
    {
        get { return specialAttackPoint; }
    }

    public int DefPoint
    {
        get { return defensePoint; }
    }

    public int SpecialDefPoint
    {
        get { return specialDefensePoint; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableAbility> LearnableAbilities
    {
        get { return learnableAbility; }
    }

}

public enum CharacterElement
{
    Empty,
    Normal,
    Pyro,
    Demon,
    Flying,
    Spooky,
    BunBun,
    Insect,
    ManlyBadassHero,
    Prosciutto

}

public class Element
{
    static float[][] Elementchart =
    {   //                        NOR    PYR   DEM   FLY    SPO     Bun     Ins    Man     Pro
        /* Normal */ new float[] { 1f,   1f,   1f,  .5f,   .5f,     2f,      1f,   .5f,   .5f},
        /* Pyro   */ new float[] { 1f,   1f,   1f,   1f,    2f,     2f,      2f,   .5f,   .5f},
        /* Demon */  new float[] { 2f,   1f,   1f,   2f,    1f,     2f,     .5f,   .5f,   .5f},
        /* Flying */ new float[] { 2f,   1f,  .5f,   1f,    1f,     2f,      2f,   .5f,   .5f},
        /* Spooky */ new float[] { 2f,  .5f,   1f,   1f,    2f,     2f,      2f,    .5f,   .5f}
    };

    public static float GetEffectivness(CharacterElement attackElement, CharacterElement defenseElement)
    {
        if (attackElement == CharacterElement.Empty || defenseElement == CharacterElement.Empty)
        {
            return 1;
        }

        int row = (int)attackElement - 1;
        int column = (int)defenseElement - 1;
        return Elementchart[row][column];
    }
}



[System.Serializable] //show in inspector
public class LearnableAbility
{
    [SerializeField] CharacterAbility characterAbility;
    [SerializeField] int level;

    public CharacterAbility Ability
    {
        get { return characterAbility; }
    }

    public int Level
    {
        get { return level; }
    }
}
