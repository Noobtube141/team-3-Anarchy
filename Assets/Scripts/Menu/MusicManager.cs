using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    // EGO music manager to be set ingame
    public GameObject musicPlayer;

    // EGOs storing audio sources
    public GameObject[] trackSources;

    // Is the player currently in combat or idle?
    private bool isInCombat = false;

    // Keep game object if there is not already another
    void Awake()
    {
        musicPlayer = GameObject.Find("MUSIC");

        if (musicPlayer == null)
        {
            musicPlayer = this.gameObject;

            musicPlayer.name = "MUSIC";

            DontDestroyOnLoad(musicPlayer);
        }
        else
        {
            if (this.gameObject.name != "MUSIC")
            {
                Destroy(this.gameObject);
            }
        }
    }

    // Fade first track out and fade second track in
    public IEnumerator CrossFade(string type)
    {
        // Menu track fades out, combat track fades in
        if (type == "IntoGame")
        {
            isInCombat = true;

            trackSources[1].GetComponent<AudioSource>().Play();

            trackSources[1].GetComponent<AudioSource>().volume = 0.0f;

            for (int i = 0; i < 31; i++)
            {
                trackSources[0].GetComponent<AudioSource>().volume = (30 - i) / 30.0f;

                trackSources[1].GetComponent<AudioSource>().volume = i / 30.0f;
                
                yield return new WaitForSeconds(0.01f);
            }

            trackSources[0].GetComponent<AudioSource>().volume = 0.0f;

            trackSources[0].GetComponent<AudioSource>().Stop();
        }
        // Combat track fades out, idle track fades in
        else if (type == "EnemiesEliminated")
        {
            isInCombat = false;

            trackSources[2].GetComponent<AudioSource>().Play();

            trackSources[2].GetComponent<AudioSource>().volume = 0.0f;

            for (int i = 0; i < 31; i++)
            {
                trackSources[1].GetComponent<AudioSource>().volume = (30 - i) / 30.0f;

                trackSources[2].GetComponent<AudioSource>().volume = i / 30.0f;

                yield return new WaitForSeconds(0.01f);
            }

            trackSources[1].GetComponent<AudioSource>().volume = 0.0f;

            trackSources[1].GetComponent<AudioSource>().Stop();
        }
        else if (type == "IntoMenu")
        {
            int combatState;

            if (isInCombat)
            {
                combatState = 1;
            }
            else
            {
                combatState = 2;
            }
            
            trackSources[0].GetComponent<AudioSource>().Play();

            trackSources[0].GetComponent<AudioSource>().volume = 0.0f;

            for (int i = 0; i < 31; i++)
            {
                trackSources[combatState].GetComponent<AudioSource>().volume = (30 - i) / 30.0f;

                trackSources[0].GetComponent<AudioSource>().volume = i / 30.0f;
                
                yield return new WaitForSeconds(0.01f);
            }
            
            trackSources[combatState].GetComponent<AudioSource>().volume = 0.0f;

            trackSources[combatState].GetComponent<AudioSource>().Stop();
            
            isInCombat = false;
        }
    }

    // Switch from idle to combat music. Called at the start of a level
    public void IdleToCombat()
    {
        isInCombat = true;

        trackSources[2].GetComponent<AudioSource>().volume = 0.0f;

        trackSources[2].GetComponent<AudioSource>().Stop();

        trackSources[1].GetComponent<AudioSource>().volume = 1.0f;

        trackSources[1].GetComponent<AudioSource>().Play();
    }
}
