using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animatePlayer : MonoBehaviour
{

    public Vector2 direction;
    public bool isMoving;

    Player p;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        p = GetComponent<Player>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        checkAnimStates();
    }



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
