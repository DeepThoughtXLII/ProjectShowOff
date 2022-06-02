using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class enemyShooting : MonoBehaviour
{
    GameObject[] players;
    List<Transform> targetsInRange = new List<Transform>();
    public Transform target = null;
    public float range = 1.5f;

    public GameObject bulletPrefab;
    public Transform firepoint;

    public float firerate = 0.2f;
    bool readyToShoot = true;

    TargetingManager targetingManager;

    private void Start()
    {
        // players = GameObject.FindGameObjectsWithTag("Player");
        targetingManager = GameObject.FindGameObjectWithTag("targetManager").GetComponent<TargetingManager>();
    }


    private void Update()
    {
        getTarget();
        if (target != null && readyToShoot)
        {
            StartCoroutine(Shoot());
        }
    }

    void updateTarget()
    {
        if(Vector3.Distance(transform.position, target.position) > range)
        {
            target = null;
        }
    }

    void getTarget()
    {
        target = targetingManager.GetTargetInShootingRange(transform, range).transform;
        if(target != null)
        {
            updateTarget();
        }
    }

    IEnumerator Shoot()
    {
        if (target != null)
        {
            GameObject newProjectile = (GameObject)Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
            IProjectile projectile = newProjectile.GetComponent<IProjectile>();
            if (projectile != null)
            {
                projectile.ReceiveTarget(target);
            }
        }
        readyToShoot = false;
        yield return new WaitForSeconds(firerate);
        readyToShoot = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
