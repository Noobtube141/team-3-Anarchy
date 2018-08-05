using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour {

    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy Weakness")
        {
            other.gameObject.GetComponentInParent<EnemyStatusManager>().EnemyTakeDamage(Mathf.FloorToInt(damage * 1.5f), -1, GameObject.FindGameObjectWithTag("Player").transform.position - other.transform.position);
        }
        else if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyStatusManager>().EnemyTakeDamage(damage, -1, GameObject.FindGameObjectWithTag("Player").transform.position - other.transform.position);
        }
    }
}
