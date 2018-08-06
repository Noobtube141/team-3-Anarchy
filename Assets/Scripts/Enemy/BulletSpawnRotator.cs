using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawnRotator : MonoBehaviour {

    // Player transform reference
    private Transform playerTransform;

    // Set component reference
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Always face player
    void Update ()
    {
        transform.LookAt(playerTransform);
	}
}
