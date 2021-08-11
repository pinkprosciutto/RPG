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

        if (!IsPathClear(newPosition))
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

    //create a box ahead that detects if path is clear
    private bool IsPathClear(Vector3 targetPosition)
    {
        var difference = targetPosition - transform.position;
        var direction = difference.normalized;

        //path is not clear
        if (Physics2D.BoxCast(transform.position + direction, new Vector2(0.1f, 0.1f), 0f, direction, difference.magnitude - 1,
            GameLayer.Instance.SolidLayer | GameLayer.Instance.InteractableLayer | GameLayer.Instance.PlayerLayer) == true)
        {
            return false;
        }

        return true;
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

    public void LookTowardsTalker(Vector3 targetPosition)
    {
        var xdifference = Mathf.Floor(targetPosition.x) - Mathf.Floor(transform.position.x);
        var ydifference = Mathf.Floor(targetPosition.y) - Mathf.Floor(transform.position.y);

        if (xdifference == 0 || ydifference == 0)
        {
            anim.MoveX = Mathf.Clamp(xdifference, -1f, 1f);
            anim.MoveY = Mathf.Clamp(ydifference, -1f, 1f);
        } 
    }

    public CharacterAnimator Animator
    {
        get => anim;
    }
}
