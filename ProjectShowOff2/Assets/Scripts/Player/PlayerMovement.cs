using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{



    Vector2 move;
    public PlayerInput pi;
    public float speed = 5f;
    public Controls isUsingInput = Controls.KEYBOARD;

    public bool isMoving = false;
    public Vector2 direction;
    private Vector2 lastDir;

    private Rigidbody2D rb;

    PlayerHealth playerHealth;

    public Controls IsUsingInput
    {
        set { isUsingInput = value; }
        get { return isUsingInput; }
    }

    private void Start()
    {
        playerHealth = transform.GetComponent<Player>().GetPlayerHealth();

      
            if (isUsingInput != Controls.ONLINE)
            {
                pi = GetComponentInChildren<PlayerInput>();
                if (pi != null)
                {
                    Debug.Log("subscribed");
                    if (isUsingInput == Controls.GAMEPAD)
                    {
                        pi.onActionTriggered += OnAction2;
                    }
                    else
                    {
                        pi.onActionTriggered += OnAction2;

                    }
                }
            }


            rb = GetComponent<Rigidbody2D>();
        }


    void FixedUpdate()
    {
        if (playerHealth.State != PlayerHealth.PlayerState.REVIVING)
        {
            Move(direction);
            rb.MovePosition(move);
        }
    }


    public void ResetMovement()
    {
        move = Vector2.zero;
        direction = Vector2.zero;
    }


    private void OnAction(InputAction.CallbackContext ctx)
    {
        if (playerHealth.State != PlayerHealth.PlayerState.REVIVING)
        {
            if (ctx.action.name == "move")
            {
                if (ctx.action.phase == InputActionPhase.Performed)
                {
                    direction = ctx.ReadValue<Vector2>();
                }
                else if (ctx.action.phase == InputActionPhase.Canceled)
                {
                    //move = Vector2.zero;
                }
            }
        }
        //Debug.Log(ctx);
    }

    private void OnAction2(InputAction.CallbackContext ctx)
    {

        if (playerHealth.State != PlayerHealth.PlayerState.REVIVING)
        {
            if (ctx.action.name == "move")
            {
                if (ctx.action.phase == InputActionPhase.Performed)
                {
                    if (direction != ctx.ReadValue<Vector2>())
                    {
                        direction = ctx.ReadValue<Vector2>();
                    }

                    isMoving = true;
                }
                else if (ctx.action.phase == InputActionPhase.Canceled)
                {
                    isMoving = false;
                    direction = Vector2.zero;
                }
                //  Move(ctx.ReadValue<Vector2>());
            }
        }
        //Debug.Log(ctx);
    }




    public void Move(Vector2 moveDirection)
    {
        if (playerHealth.State != PlayerHealth.PlayerState.REVIVING)
        {
            lastDir = direction;
            direction -= lastDir;
            direction += (moveDirection);
            direction.Normalize();

            

        }
        if (direction != Vector2.zero)
        {
           
            isMoving = true;
        }
        else
        {
           
            isMoving = false;
        }
        move = rb.position + direction * speed * Time.fixedDeltaTime;

    }

    public void Move2(InputAction.CallbackContext context)
    {
        Vector2 move = Vector2.zero;
        if (context.performed)
        {
            move = context.ReadValue<Vector2>() * speed * Time.fixedDeltaTime;
            
        }
        transform.Translate(move, Space.World);

    }





























}
