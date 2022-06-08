using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class honingBullet : MonoBehaviour, IProjectile
{
    private Transform _target;
    public float speed = 70f;
    public GameObject impactEffect;
    public int damage = 1;

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

    // Start is called before the first frame update
    void Start()
    {
       
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        FlyTowardTarget();
    }

    public void ReceiveTarget(Transform target, int dmg, int pOwnerId = -1)
    {
        ownerId = pOwnerId;
        _target = target;
        damage = dmg;
        StartCoroutine(lifeTime());

    }

    public void FlyTowardTarget()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 dir = _target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }


    public void HitTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        IDamageable target = _target.GetComponent<IDamageable>();
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
        if (collision.gameObject.name == "obstacle" || collision.gameObject.name == "obstacleCircle")
        {
            HitObstacle();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "obstacle" || collision.gameObject.name == "obstacleCircle")
        {
            HitObstacle();
        }
    }

    public IEnumerator lifeTime()
    {
        yield return new WaitForSeconds(aliveForSeconds);
        HitObstacle();
    }
}
