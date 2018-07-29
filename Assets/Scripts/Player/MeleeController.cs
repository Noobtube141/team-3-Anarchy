using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour {

    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<EnemyStatusManager>().EnemyTakeDamage(damage);
    }
}
