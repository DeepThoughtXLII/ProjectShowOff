using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile 
{



    void FlyTowardTarget();

    void HitTarget();

    void ReceiveTarget(Transform target);



}
