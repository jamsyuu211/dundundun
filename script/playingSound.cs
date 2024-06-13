using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class playingSound : MonoBehaviour
{
    GameObject player;
    playerMove playerScript;
    AudioSource sound;
    public AudioClip[] soundSource;
    bool isPlaying = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerMove>();   
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!playerScript.isBossZone)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                sound.volume = 0.7f;
                sound.clip = soundSource[0];
                sound.Play();
            }
        }
        else
        {
            if (isPlaying)
            {
                isPlaying = false;
                sound.volume = 0.5f;
                sound.clip = soundSource[1];
                sound.Play();
            }
        }
    }
}
