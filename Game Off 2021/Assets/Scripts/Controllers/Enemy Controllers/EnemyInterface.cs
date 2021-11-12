using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyInterface
{
    public int health { get; set; }

    public void OnHit(int damage);
}
