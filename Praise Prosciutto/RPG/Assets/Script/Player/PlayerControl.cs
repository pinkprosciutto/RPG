using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float speed;
    public Transform point; //determines where to move
    public bool isMoving;
    public LayerMask stopMovement;
    private Vector2 input;

    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        PlayerMovement();
        
    }

    //tile based movement
    void PlayerMovement()
    {

        //input.x = Input.GetAxisRaw("Horizontal");
        //input.y = Input.GetAxisRaw("Vertical");

        ////player sprite moves towards Transform point
        //transform.position = Vector3.MoveTowards(transform.position, point.position, speed * Time.deltaTime);

        ////if player's sprite is close to Transform point, then it can continue to take movement input
        //if (Vector3.Distance(transform.position, point.position) <= 0.0001f)
        //{
        //    if (Mathf.Abs(input.x) == 1f) //if left or right key is pressed
        //    {
        //        //draw a circle at Transform point, stops movement if it collides with a sprite that doesn't allow movement
        //        if (!Physics2D.OverlapCircle(point.position + new Vector3(input.x, 0f, 0f), 0.2f, stopMovement))
        //        {
        //            StartCoroutine(Move(input.x, 0f));
        //        }

        //    }
        //    else if (Mathf.Abs(input.y) == 1f) //if up or down key is pressed
        //    {
        //        if (!Physics2D.OverlapCircle(point.position + new Vector3(0f, input.y, 0f), 0.2f, stopMovement))
        //        {
        //            StartCoroutine(Move(0f, input.y));
        //        }
        //    }
        //    anim.SetBool("Moving", isMoving);
        //    anim.SetFloat("moveX", input.x);
        //    anim.SetFloat("moveY", input.y);
        //}
        if (!isMoving)
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
                anim.SetFloat("moveX", input.x);
                anim.SetFloat("moveY", input.y);

                //compute new target position  
                var newPosition = transform.position;
                newPosition.x += input.x;
                newPosition.y += input.y;

                if (StopMovement(newPosition))
                {
                    StartCoroutine(Move(newPosition));
                }
            }
        }

        anim.SetBool("Moving", isMoving);
    }

    //IEnumerator Move(float x, float y)
    //{
    //    isMoving = true;

    //    point.position += new Vector3(x, y, 0f); //move up or down

    //    yield return null;

    //    isMoving = false;

    //}

    IEnumerator Move(Vector3 newPosition)
    {
        isMoving = true;
        //move the player from its current position to the newly computed position
        //if the difference with player's current position and the newly computed position is greater than the small 0.001f, move player
        while ((newPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = newPosition;
        isMoving = false;
    }

    private bool StopMovement(Vector3 newPosition)
    {
        //draw a circle at Transform point, stops movement if it collides with a sprite that doesn't allow movement
        if( Physics2D.OverlapCircle(newPosition, 0.1f, stopMovement) != null)
        {
            return false;
        }
        return true;
    }
}
