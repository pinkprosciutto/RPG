using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager
{
    public CharacterAbility Ability { get; set; }
    public int Amount { get; set; }

    public AbilityManager(CharacterAbility _ability)
    {
        Ability = _ability;
        Amount = _ability.Amount;
    }


}
