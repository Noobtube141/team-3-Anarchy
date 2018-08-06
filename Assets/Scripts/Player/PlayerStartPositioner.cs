using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPositioner : MonoBehaviour {

    // Offset from position based on player size
    public Vector3 playerOffset = new Vector3(0.0f, 0.9f, 0.0f);

    // Should the player be desrtoyed? (set true for menu, set false for levels)
    public bool shouldPlayerBeDestroyed = false;

	void Start ()
    {
        if (shouldPlayerBeDestroyed)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
        }
        else
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = transform.position + playerOffset;
        }

        Destroy(gameObject);
	}
}
