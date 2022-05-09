using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour, IProjectile
{
    private Transform _target;
    public float speed = 70f;
    public GameObject impactEffect;
    public int damage = 1;




    // Start is called before the first frame update
    void Start()
    {
       
    }

    void Update()
    {
        FlyTowardTarget();
    }

    public void ReceiveTarget(Transform target)
    {
        _target = target;
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
        Destroy(effectIns, 2f);
        Destroy(gameObject);
    }

    


}
