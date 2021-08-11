using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusData
{
    public static void Initialize()
    {
        foreach (var kvp in _Status)
        {
            var statusID = kvp.Key;
            var status = kvp.Value;

            status.Id = statusID;
        }
    }

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
                    character.StatusChange.Enqueue($"{character.CharacterBase.GetName} takes bleed damage.");
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
                    character.StatusChange.Enqueue($"{character.CharacterBase.GetName} takes burn damage.");
                }
            }

        },
        {
            StatusID.charmed,
            new Status()
            {
                Name = "Charmed",
                StatusMessage = "is charmed",
                OnBeforeMove = (CharacterManager character) =>
                {
                    if (Random.Range(1, 4) == 1)
                    {
                        character.StatusChange.Enqueue($"{character.CharacterBase.GetName} is immobilized by the power of love.");
                        return false; //character can't perform move, immobilized by love 
                    }
                    return true; 
                }
            }

        },
        {
            StatusID.freeze,
            new Status()
            {
                Name = "Freeze",
                StatusMessage = "is frozen",
                OnBeforeMove = (CharacterManager character) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        character.CureStatus();
                        character.StatusChange.Enqueue($"{character.CharacterBase.GetName} is unthawed.");
                        return true; //character unfreezes 
                    }
                    character.StatusChange.Enqueue($"{character.CharacterBase.GetName} is ice solid.");
                    return false; //character can't perform move
                }
            }

        }
    };
}

public enum StatusID
{
    none, bleed, burn, charmed, freeze
}
