using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    CharacterAnimator anim;
    public float speed;


    public bool IsMoving { get; private set; }

    void Awake()
    {
        anim = GetComponent<CharacterAnimator>();
    }


    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver = null)
    {
        anim.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        anim.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        //compute new target position  
        var newPosition = transform.position;
        newPosition.x += moveVector.x;
        newPosition.y += moveVector.y;

        if (!StopMovement(newPosition))
            yield break;

        IsMoving = true;

        while ((newPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = newPosition;
        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        anim.IsMoving = IsMoving;
    }

    private bool StopMovement(Vector3 newPosition)
    {
        //draw a circle at Transform point, stops movement if it collides with a sprite that doesn't allow movement
        if (Physics2D.OverlapCircle(newPosition, 0.1f, GameLayer.Instance.SolidLayer | GameLayer.Instance.InteractableLayer) != null)
        {
            return false;
        }
        return true;
    }

    public CharacterAnimator Animator
    {
        get => anim;
    }
}
