using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakable : MonoBehaviour, IDamageable, ITargetable
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public int health;
    public Color targetColor;
    Color defColor;

    SpriteRenderer rend;

    bool isTarget = false;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET() AND SET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public int Health
    {
        set { health = value; }
        get { return health; }
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        rend = this.GetComponent<SpriteRenderer>();
        defColor = rend.color;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     TAKE DAMAGE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void takeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     BECOME TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void becomeTarget()
    {
        if(isTarget == false)
        {
            rend.color = targetColor;
            isTarget = true;
        }
        
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     LOSE TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void loseTarget()
    {
        rend.color = defColor;
        isTarget = false;
    }



}
