using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPositioner : MonoBehaviour {

    // Offset from position based on player size
    public Vector3 playerOffset = new Vector3(0.0f, 0.9f, 0.0f);

    // Should the player be desrtoyed? (set true for menu, set false for levels)
    public bool shouldPlayerBeDestroyed = false;

    // Is the game transitioning from the menu to levels (set true for first level, else false)
    public bool isEnteringLevels;

    // Is this the third level? Allows the win state to be triggered
    public bool isLastLevel;

    // Next level to load
    public string nextLevel;

	void Start ()
    {
        if (shouldPlayerBeDestroyed)
        {
            GameObject.FindGameObjectWithTag("Music Player").SendMessage("CrossFade", "MusicToMenu");
            
            Destroy(GameObject.FindGameObjectWithTag("Player"));
        }
        else
        {
            if(GameObject.FindObjectOfType<WaypointManager>() != null)
            {
                GameObject.FindObjectOfType<WaypointManager>().gameObject.SetActive(false);
            }

            if (isEnteringLevels)
            {
                GameObject.FindGameObjectWithTag("Music Player").SendMessage("CrossFade", "IntoGame");
            }
            else
            {
                if (isLastLevel)
                {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<CombatController>().isLastLevel = true;
                }

                GameObject.FindGameObjectWithTag("Music Player").SendMessage("IdleToCombat");
            }

            GameObject.FindGameObjectWithTag("Player").transform.position = transform.position + playerOffset;

            GameObject.FindGameObjectWithTag("Player").GetComponent<CombatController>().scene = nextLevel;
        }

        Destroy(gameObject);
	}
}
