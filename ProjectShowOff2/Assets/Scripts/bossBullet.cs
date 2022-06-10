using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossBullet : MonoBehaviour, IProjectile
{
    private Transform _target;
    private Vector3 targetDirection;



    public float speed = 70f;
    public GameObject impactEffect;
    public int damage = 1;

    bool isFlying = false;

    public LayerMask obstacles;

    private int ownerId = 0;

    public float aliveForSeconds = 7;

    Rigidbody2D rb;

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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        targetDirection.Normalize();
        Vector2 direc = new Vector2(targetDirection.x, targetDirection.y);
        Vector2 move = rb.position +  direc * speed * Time.fixedDeltaTime;
        //  transform.Translate(targetDirection.normalized * speed * Time.deltaTime, Space.World);
        rb.MovePosition(move);
    }


    public void ReceiveTarget(Transform target, int dmg, int pOwnerId = -1)
    {
        ownerId = pOwnerId;
        _target = target;
        damage = dmg;
        StartCoroutine(lifeTime());
    }

    public void ReceiveDirection(Vector3 direction, int dmg, int pOwnerId = -1)
    {
        ownerId = pOwnerId;
        targetDirection = direction;
        damage = dmg;
        StartCoroutine(lifeTime());
        isFlying = true;
    }

    public void FlyTowardTarget()
    {
        targetDirection = _target.position - transform.position;
        isFlying = true;
    }


    public void HitTarget(IDamageable target)
    {
        Debug.Log("take damage for fucks sake i beg you boss version");
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        //IDamageable target = _target.GetComponent<IDamageable>();
        target.takeDamage(damage);
        if (target.Health <= damage)
        {
            if (_target.TryGetComponent<XpCarrier>(out XpCarrier toBeDead))
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


    public IEnumerator lifeTime()
    {
        yield return new WaitForSeconds(aliveForSeconds);
        HitObstacle();
    }
}
