using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onHailAttackEnter : MonoBehaviour
{
    hailOfArrows hailOfArrows;
    void Start()
    {
        hailOfArrows = gameObject.GetComponent<hailOfArrows>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "player")
        {
            hailOfArrows.HitTarget();
        }
    }
}
