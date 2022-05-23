using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour, IProjectile
{


    private Transform _target;
    private Vector3 targetDirection;

    

    public float speed = 70f;
    public GameObject impactEffect;
    public int damage = 1;

    bool isFlying = false;

    public LayerMask obstacles;

   
    void Update()
    {
        if (isFlying)
        {
            transform.Translate(targetDirection.normalized * speed * Time.deltaTime, Space.World);
        } else
        {
            FlyTowardTarget();
        }
    }


    public void ReceiveTarget(Transform target)
    {
        _target = target;
    }

    public void FlyTowardTarget()
    {
        targetDirection = _target.position - transform.position;
        isFlying = true;
    }


    public void HitTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        IDamageable target = _target.GetComponent<IDamageable>();
        target.takeDamage(damage);
        Destroy(effectIns, 2f);
        Destroy(gameObject);
    }

    public void HitObstacle()
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "obstacle")
        {
            HitObstacle();
        }
        else if (collision.gameObject.tag == "Player")
        {
            HitTarget();
        }
    }
}
