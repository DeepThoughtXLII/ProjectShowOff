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

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
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
        foreach (GameObject player in players)
        {
            if (Physics2D.OverlapCircle(new Vector2(player.transform.position.x, player.transform.position.y), range))
            {
                
                    targetsInRange.Add(player.transform);
                
            }
        }
        if (targetsInRange.Count > 0)
        {

            float shortestDistance = range;
            foreach (Transform target in targetsInRange)
            {
                float dist = Vector3.Distance(target.position, transform.position);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    this.target = target;
                }
            }
           
        }
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
