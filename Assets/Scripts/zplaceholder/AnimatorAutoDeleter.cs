using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorAutoDeleter : MonoBehaviour {

	// Set auto delete
	void Start ()
    {
        Destroy(gameObject, GetComponent<Animation>().clip.length - 0.1f);
	}
}
