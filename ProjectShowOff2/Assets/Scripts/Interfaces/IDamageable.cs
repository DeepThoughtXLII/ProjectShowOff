using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{

    public int Health { get; set; }

    void takeDamage(int damage);
}