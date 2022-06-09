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
        setAttackLocation();
        StartCoroutine(arrowAttack());
    }

    public void setAttackLocation()
    {
        direction = (Vector2)_target.position - (Vector2)this.transform.position;
        angleOffset = Random.Range(minusDeviation, maximumDeviation);
        rotateVector(direction, angleOffset);
        Vector2 position = gameObject.transform.position;
        position += direction;
        gameObject.transform.position.Set(position.x, position.y, 0f);
    }
    public IEnumerator arrowAttack()
    {
        highlightArea.SetActive(true);
        yield return new WaitForSeconds(chargeTime);
        highlightArea.SetActive(false);
        attackArea.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        Destroy(gameObject);
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
