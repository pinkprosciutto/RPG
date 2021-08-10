using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    public string Name { get; set; }

    public StatusID Id { get; set; }

    public string Description { get; set; }
    
    public string StatusMessage { get; set; }

    public Action<CharacterManager> OnStart { get; set; }

    public Func<CharacterManager, bool> OnBeforeMove { get; set; }

    public Action<CharacterManager> OnAfterTurn { get; set; }

}
