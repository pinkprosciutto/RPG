using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCControl : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] List<Vector2> walkPattern;
    [SerializeField] float timeBetweenPattern;

    Character character;

    NPCState state;

    float idleTimer = 0f;
    int currentWalkPattern = 0;

    void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact()
    {
        if (state == NPCState.Idle)
            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
    }
  
    private void Update()
    {
        if (DialogueManager.Instance.IsShowingDialogue) return;

        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (walkPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }

        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walk;

        //if (!DialogueManager.Instance.IsShowingDialogue)
        //{
        //    yield return character.Move(walkPattern[currentWalkPattern]);
        //    currentWalkPattern = (currentWalkPattern + 1) % walkPattern.Count;
        //}

        yield return character.Move(walkPattern[currentWalkPattern]);
        currentWalkPattern = (currentWalkPattern + 1) % walkPattern.Count;

        state = NPCState.Idle;
    }
}

public enum NPCState { Idle, Walk }
