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

    public void Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialogue;
            character.LookTowardsTalker(initiator.position);
            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue, () => {
                idleTimer = 0f;
                state = NPCState.Idle;
            }));

        }
    }
  
    private void Update()
    {
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

        var oldPosition = transform.position;

        //if (!DialogueManager.Instance.IsShowingDialogue)
        //{
        //    yield return character.Move(walkPattern[currentWalkPattern]);
        //    currentWalkPattern = (currentWalkPattern + 1) % walkPattern.Count;
        //}
        yield return character.Move(walkPattern[currentWalkPattern]);
        if (transform.position != oldPosition)
            currentWalkPattern = (currentWalkPattern + 1) % walkPattern.Count;

        state = NPCState.Idle;
    }
}

public enum NPCState { Idle, Walk, Dialogue }
