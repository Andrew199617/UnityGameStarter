using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip TitleScreenAudio;
    public AudioClip[] InGameAudio;

    public static AudioManager AudioManagerInst;

    // Use this for initialization
    void Start ()
    {

        if (!AudioManagerInst)
        {
            AudioManagerInst = this;

            audioSource = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
            SceneManagerOnSceneLoaded(SceneManager.GetActiveScene());

            DontDestroyOnLoad(this);
        }
        else
        {
            //Destroy if Duplicate AudioManager
            Destroy(this);
        }
    }

    private void SceneManagerOnSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        if (loadedScene.name == "Game1")
        {
            var randomIndex = Random.Range(0, InGameAudio.Length);
            audioSource.clip = InGameAudio[randomIndex];
            audioSource.volume = 1.0f;
            audioSource.Play();
        }
        else if (loadedScene.name == "TitleScreen")
        {
            audioSource.clip = TitleScreenAudio;
            audioSource.volume = .1f;
            audioSource.Play();
        }
    }
    
}
