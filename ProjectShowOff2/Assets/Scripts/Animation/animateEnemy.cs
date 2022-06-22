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

    bool isMoving;

    bool isAttacking;


    Animator anim;

    enemyPathing enemypath;

    enemyShooting _enemyShooting;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    

    void Start()
    {
        anim = GetComponent<Animator>();
        enemypath = GetComponent<enemyPathing>();
        _enemyShooting = gameObject.GetComponent<enemyShooting>();

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
        direction = enemypath.movement;
        isMoving = enemypath.isMoving;
        isAttacking = _enemyShooting.isAttacking;


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
        
        if (isAttacking)
        {
            anim.SetBool("IsAttacking", true);
        }
        else
        {
            anim.SetBool("IsAttacking", false);
        }

    }




}
