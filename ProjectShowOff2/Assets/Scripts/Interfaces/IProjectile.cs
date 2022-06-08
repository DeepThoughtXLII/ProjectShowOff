using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile 
{

    public int OwnerId { get; set; }

    public int Damage { get; set; }


    void FlyTowardTarget();

    void HitTarget();

    void ReceiveTarget(Transform target, int dmg, int pOwnerId = -1);

    void HitObstacle();


    public IEnumerator lifeTime();

}
