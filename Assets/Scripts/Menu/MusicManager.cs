using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    // EGO music manager to be set ingame
    public GameObject musicPlayer;

    // EGOs storing audio sources
    public GameObject[] trackSources;

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
        if (type == "EnemiesEliminated")
        {
            trackSources[1].GetComponent<AudioSource>().Play();

            trackSources[1].GetComponent<AudioSource>().volume = 0.0f;

            for (int i = 0; i < 30; i++)
            {
                trackSources[0].GetComponent<AudioSource>().volume = (30 - i) / 30.0f;

                trackSources[1].GetComponent<AudioSource>().volume = i / 30.0f;

                yield return new WaitForSeconds(0.01f);
            }

            trackSources[0].GetComponent<AudioSource>().Pause();
        }
    }
}
