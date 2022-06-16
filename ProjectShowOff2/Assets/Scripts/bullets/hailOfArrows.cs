using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hailOfArrows : MonoBehaviour
{
    public GameObject highlightArea;
    public GameObject attackArea;

    public float maximumDeviation = 10;
    private float minusDeviation = 0;

    private Transform _target;

    private Vector2 direction;

    private float angleOffset;

    public float chargeTime = 3f;
    public float attackDuration = 1f;
    public int damage = 1;

    private int ownerId = 0;

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


    void Start()
    {
        minusDeviation -= maximumDeviation;
    }

    void Update()
    {
       
    }

    public void HitTarget()
    {
        IDamageable target = _target.GetComponent<IDamageable>();
        target.takeDamage(damage);
        if(target.Health <= damage)
        {
            if (_target.TryGetComponent<XpCarrier>(out XpCarrier toBeDead))
            {
                toBeDead.SetKiller(ownerId);
            }
        }
    }

    public void ReceiveTarget(Transform target, int pOwnerId = -1)
    {
        ownerId = pOwnerId;
        _target = target;
    }

    public void setAttackLocation(Transform enemyTransform)
    {
        direction = (Vector2)_target.position - (Vector2)gameObject.transform.position;
        angleOffset = Random.Range(minusDeviation, maximumDeviation);
        gameObject.transform.Translate(direction.x, direction.y, 0f);
        float rotation = Vector2.Angle((Vector2)_target.position, (Vector2)enemyTransform.position) + angleOffset;
        gameObject.transform.Rotate(0, 0, rotation);
    }

    public Vector2 rotateVector(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

}
