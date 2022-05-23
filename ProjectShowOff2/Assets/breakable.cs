using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakable : MonoBehaviour, IDamageable, ITargetable
{

    public int health;
    public Color targetColor;
    Color defColor;

    SpriteRenderer rend;

    bool isTarget = false;

    public int Health
    {
        set { health = value; }
        get { return health; }
    }


    private void Start()
    {
        rend = this.GetComponent<SpriteRenderer>();
        defColor = rend.color;
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void becomeTarget()
    {
        if(isTarget == false)
        {
            rend.color = targetColor;
            isTarget = true;
        }
        
    }

    public void loseTarget()
    {
        rend.color = defColor;
        isTarget = false;
    }



}
