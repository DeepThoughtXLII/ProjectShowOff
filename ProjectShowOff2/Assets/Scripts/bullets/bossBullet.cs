using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class bossBullet : MonoBehaviour, IProjectile
{
    //Script for boss bullets. basically a modified version of the regular bullet script. the main difference is that this bullet doesnt rceieve a target but really only a direction

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

    public float aliveForSeconds = 7;

    Rigidbody2D rb;

    [SerializeField]string opponentTag = "Player";


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



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIXED UPDATE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void FixedUpdate()
    {
        targetDirection.Normalize();
        Vector2 direc = new Vector2(targetDirection.x, targetDirection.y);
        Vector2 move = rb.position +  direc * speed * Time.fixedDeltaTime;
        //  transform.Translate(targetDirection.normalized * speed * Time.deltaTime, Space.World);
        rb.MovePosition(move);
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     RECEIVE TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ReceiveTarget(Transform target, int dmg, int pOwnerId = -1)
    {
        _target = target;
        ownerId = pOwnerId;
        targetDirection = target.position - transform.position;
        damage = dmg;
        StartCoroutine(lifeTime());
        isFlying = true;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     RECEIVE DIRECTION()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ReceiveDirection(Vector3 direction, int dmg, int pOwnerId = -1)
    {
        ownerId = pOwnerId;
        targetDirection = direction;
        damage = dmg;
        StartCoroutine(lifeTime());
        isFlying = true;
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
        Debug.Log("take damage for fucks sake i beg you boss version");
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        //IDamageable target = _target.GetComponent<IDamageable>();
        target.takeDamage(damage);
        if (target.Health <= damage)
        {
            if (_target != null)
            {
                if (_target.TryGetComponent<XpCarrier>(out XpCarrier toBeDead))
                {
                    Debug.Log("found xp carrier. added xp");
                    toBeDead.SetKiller(ownerId);
                }
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
    ///                                                                     COLLISION CHECKS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "obstacle")
        {
            HitObstacle();
        }
        else if (collision.gameObject.tag == opponentTag)
        {
            Debug.Log($"hit enemy {collision.gameObject} with tag {collision.gameObject.tag}");
            HitTarget(collision.gameObject.GetComponent<IDamageable>());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "obstacle")
        {
            HitObstacle();
        }
        else if (collision.gameObject.tag == opponentTag)
        {
            Debug.Log($"hit enemy {collision.gameObject} with tag {collision.gameObject.tag}");
            HitTarget(collision.gameObject.GetComponent<IDamageable>());
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     LIFETIME()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public IEnumerator lifeTime()
    {
        yield return new WaitForSeconds(aliveForSeconds);
        HitObstacle();
    }
}
