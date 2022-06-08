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

    private int ownerId = 0;

    public float aliveForSeconds = 7f;

    public int OwnerId
    {
        set { ownerId = value; }
        get { return ownerId; }
    }

    public int Damage
    {
        set { damage = value; }
        get { return damage; }
    }

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


    public void ReceiveTarget(Transform target, int pOwnerId = -1)
    {
        ownerId = pOwnerId;
        _target = target;
        StartCoroutine(lifeTime());
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
        if(target.Health <= damage)
        {
            if(_target.TryGetComponent<XpCarrier>(out XpCarrier toBeDead))
            {
                toBeDead.SetKiller(ownerId);
            }
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
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


    public IEnumerator lifeTime()
    {
        yield return new WaitForSeconds(aliveForSeconds);
        HitObstacle();
    }
}
