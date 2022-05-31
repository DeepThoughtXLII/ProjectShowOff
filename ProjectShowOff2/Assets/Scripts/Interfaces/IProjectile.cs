using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile 
{

    public int OwnerId { get; set; }

    void FlyTowardTarget();

    void HitTarget();

    void ReceiveTarget(Transform target, int pOwnerId = -1);

    void HitObstacle();

}
