using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour, IProjectile
{
    //this script is the regular bullet script. this bullet recives a target and from this target it derives a direction and keeps moving into that direction until it hits something or expires (eg lifetime())
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private Transform _target;
    private Vector3 targetDirection;

    

    public float speed = 70f;
    public GameObject impactEffect;
    public int damage = 1;

    bool isFlying = false;

    public LayerMask obstacles;

    private int ownerId = 0;

    public float aliveForSeconds = 7f;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET() AND SET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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


    Rigidbody2D rb;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIXED UPDATE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void FixedUpdate()
    {
        if (isFlying)
        {
            targetDirection.Normalize();
            Vector2 direc = new Vector2(targetDirection.x, targetDirection.y);
            Vector2 move = rb.position + direc * speed * Time.fixedDeltaTime;
            //  transform.Translate(targetDirection.normalized * speed * Time.deltaTime, Space.World);
            rb.MovePosition(move);
           // transform.Translate(targetDirection.normalized * speed * Time.deltaTime, Space.World);
        } else
        {
            FlyTowardTarget();
        }
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     RECEIVE TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ReceiveTarget(Transform target, int dmg, int pOwnerId = -1)
    {
        ownerId = pOwnerId;
        _target = target;
        damage = dmg;
        StartCoroutine(lifeTime());
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FLY TOWARD TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void FlyTowardTarget()
    {
        targetDirection = _target.position - transform.position;

        isFlying = true;
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     HIT TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void HitTarget(IDamageable target)
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        //IDamageable target = _target.GetComponent<IDamageable>();
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


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     HIT OBSTACLE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void HitObstacle()
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);
        Destroy(gameObject);
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     RCEIVE DIRECTION()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ReceiveDirection(Vector3 direction, int dmg, int pOwnerId = -1)
    {
        ownerId = pOwnerId;
        targetDirection = direction;
        damage = dmg;
        StartCoroutine(lifeTime());
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     COLLISION CHECKS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "obstacle")
        {
            HitObstacle();
        }
        else if (collision.gameObject.tag == "Player")
        {
            HitTarget(collision.gameObject.GetComponent<IDamageable>());
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
            HitTarget(collision.gameObject.GetComponent<IDamageable>());
        }
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     LIFETIME
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public IEnumerator lifeTime()
    {
        yield return new WaitForSeconds(aliveForSeconds);
        HitObstacle();
    }
}
