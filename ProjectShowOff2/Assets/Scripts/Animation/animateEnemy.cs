using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animateEnemy : MonoBehaviour
{
    //script to read what direction the player is facing and play the correct animation

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public Vector2 direction;
    public bool isMoving;

    Player p;
    Animator anim;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    

    void Start()
    {
        p = GetComponent<Player>();
        anim = GetComponent<Animator>();
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Update()
    {
        checkAnimStates();
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     checkAnimStates()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///bool notMoving decides if player should be idle or play a moving animation at all
    ///direction decides if the player walks left,right,up or down
    ///depending on which one (or which combination of directions) are set to true, the animator plays the correct walking animation
    void checkAnimStates()
    {
        direction = p.GetPlayerMovement().direction;
        isMoving = p.GetPlayerMovement().isMoving;
        if(direction.x > 0.26)
        {
            anim.SetBool("walkingRight", true);

        }
        else
        {
            anim.SetBool("walkingRight", false);
        }

        if (direction.x < -0.26)
        {
            anim.SetBool("walkingLeft", true);

        }
        else
        {
            anim.SetBool("walkingLeft", false);
        }

        if (direction.y < -0.26)
        {
            anim.SetBool("walkingDown", true);

        }
        else
        {
            anim.SetBool("walkingDown", false);
        }

        if (direction.y > 0.26)
        {
            anim.SetBool("walkingUp", true);

        }
        else
        {
            anim.SetBool("walkingUp", false);
        }


        if (isMoving)
        {
            anim.SetBool("notMoving", false);
        }
        else
        {
            anim.SetBool("notMoving", true);
        }
    }




}
