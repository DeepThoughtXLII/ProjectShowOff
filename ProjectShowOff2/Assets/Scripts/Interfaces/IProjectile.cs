using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile 
{

    public int OwnerId { get; set; }

    public int Damage { get; set; }


    void FlyTowardTarget();

    void HitTarget(IDamageable target);

    void ReceiveTarget(Transform target, int dmg, int pOwnerId = -1);

    void ReceiveDirection(Vector3 direction, int dmg, int pOwnerId = -1);

    void HitObstacle();




    public IEnumerator lifeTime();

}
