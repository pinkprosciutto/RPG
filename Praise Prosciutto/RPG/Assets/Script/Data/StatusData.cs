using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusData
{
    public static Dictionary<StatusID, Status> _Status { get; set; } = new Dictionary<StatusID, Status>()
    {
        {
            StatusID.bleed,
            new Status()
            {
                Name = "Bleed",
                StatusMessage = "is bleeding",
                OnAfterTurn = (CharacterManager character) =>
                {
                    character.UpdateHP(character.MaxHealth / 7);
                    character.StatusChange.Enqueue($"{character.CharacterBase.GetName} takes bleed damage");
                }
            }

        },
        {
            StatusID.burn,
            new Status()
            {
                Name = "Burn",
                StatusMessage = "is burning",
                OnAfterTurn = (CharacterManager character) =>
                {
                    character.UpdateHP(character.MaxHealth / 5);
                    character.StatusChange.Enqueue($"{character.CharacterBase.GetName} takes burn damage");
                }
            }

        }
    };
}

public enum StatusID
{
    //posion burn sleep
    none, bleed, burn, charmed
}
