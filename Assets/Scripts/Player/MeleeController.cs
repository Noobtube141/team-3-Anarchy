using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour {

    public int damage;

    // Inflict damage on impact
    private void OnTriggerEnter(Collider other)
    {
        // Inflict half damage if hitting the weakness (because it will also hit the body - adds to 1.5* total)
        if (other.gameObject.tag == "Enemy Weakness")
        {
            other.gameObject.GetComponentInParent<EnemyStatusManager>().EnemyTakeDamage(Mathf.FloorToInt(damage * 0.5f), -1, GameObject.FindGameObjectWithTag("Player").transform.position - other.transform.position);
        }
        // Inflict normal damage when hitting the body
        else if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyStatusManager>().EnemyTakeDamage(damage, -1, GameObject.FindGameObjectWithTag("Player").transform.position - other.transform.position);
        }
    }
}
