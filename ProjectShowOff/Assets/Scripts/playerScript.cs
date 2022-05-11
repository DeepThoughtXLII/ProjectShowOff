using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerScript : MonoBehaviour, IDamageable
{

    [SerializeField]
    private int health = 0;

    PlayerControls controls;

    Vector2 move;

    public float speed = 5f;

    public int Health
    {
        set { health = value; }
        get { return health; }
    }

    public void takeDamage(int i)
    {

    }

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.move.canceled += ctx => move = Vector2.zero;
    }


    private void Update()
    {
        Vector2 m = new Vector2(move.x, move.y) * speed * Time.deltaTime;
        transform.Translate(m, Space.World);
    }



    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}