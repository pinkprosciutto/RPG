using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] CharacterScriptableObject characterBase;
    [SerializeField] int level;

    public CharacterManager Character { get; set; }

    public void Setup()
    {
        Character = new CharacterManager(characterBase, level);
        GetComponent<Image>().sprite = Character.CharacterBase.CharacterSprite;
        
    }

}
