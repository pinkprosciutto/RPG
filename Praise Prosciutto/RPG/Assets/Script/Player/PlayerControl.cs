using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float speed;

    private Vector2 input;

    public event Action OnEncounter;

    private Character character;
    private void Awake()
    {
        character = GetComponent<Character>();

    }

    public void HandleUpdate()
    {
        PlayerMovement();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
        
    }

    //tile based movement
    void PlayerMovement()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //prevent diagonal movement
            if (input.x != 0)
            {
                input.y = 0;
            }

            //if there is an input on horizontal and vertical keys
            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, Encounter));
            }
        }

        character.HandleUpdate();
    }

    void Interact()
    {
        var facingDirection = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPosition = transform.position + facingDirection;

        var collider = Physics2D.OverlapCircle(interactPosition, 0.1f, GameLayer.Instance.InteractableLayer);

        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void Encounter()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.1f, GameLayer.Instance.EnemyLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 50f)
            {
                OnEncounter();
                character.Animator.IsMoving = false;
            }
        }

    }

}
