using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    public string Name { get; set; }

    public string Description { get; set; }
    
    public string StatusMessage { get; set; }

    public Action<CharacterManager> OnAfterTurn { get; set; }

}
