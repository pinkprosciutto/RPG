using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, InBattle, InDialogue }

public class GameControl : MonoBehaviour
{
    GameState state;
    [SerializeField] PlayerControl playerControl;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    void Awake()
    {
        StatusData.Initialize();
    }


    private void Start()
    {
        playerControl.OnEncounter += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        DialogueManager.Instance.OnShowDialouge += () =>
        {
            state = GameState.InDialogue;
        };

        DialogueManager.Instance.OnCloseDialouge += () =>
        {
            if(state == GameState.InDialogue)
                state = GameState.FreeRoam;
        };


    }

    void StartBattle()
    {
        state = GameState.InBattle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var characterParty = playerControl.GetComponent<CharacterParty>();
        var enemy = FindObjectOfType<Map>().GetComponent<Map>().GetRandomEnemy(); //take script from Map, call GetRandomEnemy

        battleSystem.StartBattle(characterParty, enemy);
    }

    void EndBattle(bool victory)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

    }

    //Gives control to player depending on the game's state
    void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerControl.HandleUpdate();

        } else if (state == GameState.InBattle)
        {
            battleSystem.HandleUpdate();
        } else if (state == GameState.InDialogue)
        {
            DialogueManager.Instance.HandleUpdate();
        }
    }
}
