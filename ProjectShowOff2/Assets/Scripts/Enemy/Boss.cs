using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Boss : MonoBehaviour, ITargetable
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public bool isTarget = false;

    PlayerInput pi;

    SpriteRenderer rend;

    Color defColor;
    public Color targetColor = Color.white;

    Player player;

    public GameObject bulletPrefab;



    public float range = 1.5f;


    public Transform firepoint;

    public float firerate = 0.2f;

    bool readyToShoot = true;

    public int bulletDamage = 1;

    public int circularCount = 8;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Start()
    {
        player = GetComponent<Player>();

        rend = GetComponent<SpriteRenderer>();
        defColor = rend.color;

        pi = GetComponentInChildren<PlayerInput>();
        if (pi != null)
        {
            pi.onActionTriggered += OnAction;
        }

        gameObject.layer = LayerMask.NameToLayer("targetable");
        firepoint = transform.GetChild(0);

        firepoint.position = transform.position;
        
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ON ACTION()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnAction(InputAction.CallbackContext ctx)
    {
        if (player.GetPlayerHealth().State != PlayerHealth.PlayerState.REVIVING)
        {
            if (ctx.action.name == "shoot" && ctx.action.phase == InputActionPhase.Performed)
            {
                CanShoot();
                FindObjectOfType<SoundManager>().Play("playerShoot");
            }
        }

    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     BECOME TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void becomeTarget()
    {
        if (isTarget == false)
        {
            rend.color = targetColor;
            isTarget = true;
        }

    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     LOSE TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void loseTarget()
    {
        rend.color = defColor;
        isTarget = false;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     CAN SHOOT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void CanShoot()
    {
        if (readyToShoot)
        {
            StartCoroutine(Shoot());
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     SHOOT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator Shoot()
    {
       


        Vector3 beginDir = new Vector3(1,0,0);
        float angle = 360 / circularCount;
        for (int i = 0; i < circularCount; i++)
        {
            Quaternion rotate = Quaternion.AngleAxis(angle * i, Vector3.forward);
            Vector3 rotateDir = rotate * beginDir;
            GameObject newProjectile = (GameObject)Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
            IProjectile projectile = newProjectile.GetComponent<IProjectile>();
            projectile.ReceiveDirection(rotateDir, bulletDamage);
            newProjectile.transform.position = firepoint.position + rotateDir * 2;            
        }

        readyToShoot = false;
        yield return new WaitForSeconds(firerate);
        readyToShoot = true;
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GIZMOS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}

