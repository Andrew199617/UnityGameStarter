using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

namespace Managers
{
    /// <summary>
    /// Singleton class Audio Manager. There should only ever be one instance of this object. 
    /// Allows for most things you'll need from audio.
    /// </summary>
    public class AudioManager : MonoBehaviour {

        #region Private Variables

        private const int MaxChildren = 4;

        private readonly List<AudioSource> backgroundMusicSources = new List<AudioSource>();
        private readonly List<AudioSource> soundEffectSources = new List<AudioSource>();
        private AudioSource onClickSource;
        private AudioSource mainMenuMusicSource;

        /// <summary>
        /// Used to know if we should delete this AudioManager on scene load.
        /// </summary>
        private string sceneName;

        /// <summary>
        /// This is found in the Assets when creating the Audio Files.
        /// </summary>
        private AudioMixer master;

        /// <summary>
        /// AudioMixerGroup names.
        /// </summary>
        private const string
            MusicGroup = "Music",
            SfxGroup = "SoundEffects";

        private const string
            MainMenuMusic = "MainMenuMusic",
            SoundEffects = "SoundEffects",
            BackgroundMusic = "BackgroundMusic",
            OnClick = "OnClick";

        #endregion

        #region Public Variables

        public AudioClip MainMenuMusicClip;
#if UNITY_EDITOR
        public List<AudioClip> BackgroundMusicClips = new List<AudioClip>();
        public List<AudioClip> SoundEffectClips = new List<AudioClip>();
        public AudioClip OnClickClip;
#endif

        [NonSerialized]
        public float ClickLength;

        /// <summary>
        /// Useful for having diffrent audios for diffrent maps.false merges the two audiomanagers. 
        /// </summary>
        public bool ReplaceLastAudioManager;

        /// <summary>
        /// Singleton Inst
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static AudioManager audioManager;

        #endregion

        #region Usable Methods

        /// <summary>
        /// Infinetly play Menu music in a random order. 
        /// </summary>
        /// <returns>Couroutine</returns>
        public IEnumerator PlayRandomMenuMusic()
        {
            //if (mainMenuMusicSources.Count == 0)
            //{
            //    yield return null;
            //}
            StopMusic();
            mainMenuMusicSource.Play();
            yield return null;
            //while (true)
            //{
            //    //int randomChildIndex = UnityEngine.Random.Range(0, backgroundMusicSources.Count);
            //    yield return new WaitUntil(() => mainMenuMusicSource.time > mainMenuMusicSource.clip.length - .5f);
            //}
        }

        /// <summary>
        /// Looks thorough a list called soundEffectSources to find the sound effect.
        /// </summary>
        /// <param name="soundEffect">The name of the soundeffect. Would be what it's called in Unity.</param>
        public void PlaySoundEffect(string soundEffect)
        {
            var soundEffectAusioSource = GetSoundEffect(soundEffect);
            if (soundEffectAusioSource)
            {
                soundEffectAusioSource.Play();
            }
        }

        /// <summary>
        /// Gets the Audios source for the sound effect and returns the audio source.
        /// Null if could not find an audio source.
        /// </summary>
        /// <param name="soundEffect"></param>
        /// <returns>The Sound Effects Audio Source.</returns>
        public AudioSource GetAndPlaySoundEffect(string soundEffect)
        {
            var soundEffectAusioSource = GetSoundEffect(soundEffect);
            if (soundEffectAusioSource)
            {
                soundEffectAusioSource.Play();
            }
            return soundEffectAusioSource;
        }

        /// <summary>
        /// Plays OnClickClip Sound.
        /// </summary>
        public void ButtonPressed()
        {
            if (audioManager && audioManager.onClickSource)
            {
                audioManager.onClickSource.Play();
            }
        }

        /// <summary>
        /// Infinetly play background music in a random order.
        /// </summary>
        /// <returns>Couroutine</returns>
        public IEnumerator PlayRandomBackgroundMusic()
        {
            StopMusic();
            if (backgroundMusicSources.Count == 0)
            {
                yield return null;
            }
            while (true)
            {
                int randomChildIndex = UnityEngine.Random.Range(0, backgroundMusicSources.Count);
                AudioSource audioSource = backgroundMusicSources[randomChildIndex];
                audioSource.Play();
                yield return new WaitUntil(() => audioSource.time > audioSource.clip.length - .5f);
            }
        }

        /// <summary>
        /// Waits till click sound is done playing before running your method.
        /// </summary>
        /// <param name="myMethodName">The Method to be called after button clicked or () => { methodbody }</param>
        public IEnumerator EButtonPressed(Action myMethodName)
        {
            ButtonPressed();
            yield return new WaitForSeconds(audioManager.ClickLength);
            myMethodName();
            yield return null;
        }

        public IEnumerator EPlaySoundEffect(Action myMethodName, string soundEffect)
        {
            AudioSource source = audioManager.GetAndPlaySoundEffect(soundEffect);
            yield return new WaitForSeconds(source.clip.length);
            myMethodName();
            yield return null;
        }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Singleton start with merging of previous object.
        /// </summary>
        void Awake()
        {
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (!audioManager)
            {
                audioManager = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (ReplaceLastAudioManager)
            {
                Destroy(audioManager.gameObject);
                audioManager = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (sceneName == audioManager.sceneName)
            {
                Destroy(gameObject);
            }
            else
            {
                Transform oldSoundEffects = audioManager.transform.Find("SoundEffects");
                Transform soundEffects = transform.Find("SoundEffects");
                MigrateAudioSources(oldSoundEffects, soundEffects);
                Transform oldBackgroundMusicClips = audioManager.transform.Find("BackgroundMusicSources");
                Transform backgroundMusicClips = transform.Find("BackgroundMusicSources");
                MigrateAudioSources(oldBackgroundMusicClips, backgroundMusicClips);
                Destroy(audioManager.gameObject);
                audioManager = this;
                DontDestroyOnLoad(gameObject);
            }

        }

        public void Start()
        {
            GetChildrenAudioSources();
        }

        #endregion

        #region Private Methods

        private void StopMusic()
        {
            mainMenuMusicSource.Stop();
            mainMenuMusicSource.time = 0;
            backgroundMusicSources.ForEach(
                source => 
                {
                    source.Stop();
                    source.time = 0;
                });
        }

        /// <summary>
        /// Looks thorough a list called soundEffectSources to find the sound effect.
        /// </summary>
        /// <param name="soundEffect">The name of the soundeffect. Would be what it's called in Unity.</param>
        private AudioSource GetSoundEffect(string soundEffect)
        {
            if (soundEffectSources.Count > 0)
            {
                AudioSource soundEffectObj = soundEffectSources.Find((audioSource) => audioSource.name == soundEffect);
                if (soundEffectObj)
                {
                    return soundEffectObj;
                }
            }
            Debug.LogError("Tried to play nonexistent soundeffect: " + soundEffect);
            return null;
        }

        /// <summary>
        /// moves audio clips(childs) from oldAudioSources to newAudioSources
        /// </summary>
        /// <param name="oldAudioSources">objects whose clips you'll move to newAudioSources</param>
        /// <param name="newAudioSources"></param>
        private void MigrateAudioSources(Transform oldAudioSources, Transform newAudioSources)
        {
            if (!oldAudioSources || !newAudioSources)
            {
                return;
            }
            for (int i = 0; i < oldAudioSources.childCount; ++i)
            {
                bool hasAudioClip = false;
                foreach (Transform newAudioClip in newAudioSources)
                {
                    if (oldAudioSources.GetChild(i).name == newAudioClip.name)
                    {
                        hasAudioClip = true;
                        break;
                    }
                }
                if (!hasAudioClip)
                {
                    oldAudioSources.GetChild(i).transform.SetParent(newAudioSources.transform);
                    i--;
                }
            }
        }

        /// <summary>
        /// Loops through every child to set values.
        /// </summary>
        private void GetChildrenAudioSources()
        {
            backgroundMusicSources.Clear();
            soundEffectSources.Clear();
            foreach (Transform child in transform)
            {
                //child.name
                switch (child.name)
                {
                    case BackgroundMusic:
                        foreach (Transform backgroundMusicChild in child)
                        {
                            backgroundMusicSources.Add(backgroundMusicChild.GetComponent<AudioSource>());
                        }
                        break;
                    case SoundEffects:
                        foreach (Transform soundEffectChild in child)
                        {
                            soundEffectSources.Add(soundEffectChild.GetComponent<AudioSource>());
                        }
                        break;
                    case OnClick:
                        onClickSource = child.GetComponent<AudioSource>();
                        ClickLength = onClickSource.clip.length;
                        break;
                    case MainMenuMusic:
                        mainMenuMusicSource = child.GetComponent<AudioSource>();
                        //ClickLength = onClickSource.clip.length;
                        break;
                }
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Adds an empty object to hold all the audio clips. Then Creates all the Audio Sources as childs of the empty object.
        /// </summary>
        /// <param name="containerName">The name of the empty object that will hold the Audio Sources.</param>
        /// <param name="audioClips">List of audio clips to add.</param>
        /// <param name="groupName">The mixer group to use for Audio.</param>
        private void CreateAudioClipContainers(string containerName, List<AudioClip> audioClips, string groupName)
        {
            //Check for existing container
            Transform oldContainerTransform = transform.Find(containerName);
            if (oldContainerTransform)
            {
                DestroyImmediate(oldContainerTransform.gameObject);
            }

            //create new container
            GameObject containerObject = new GameObject(containerName);
            containerObject.transform.SetParent(transform);

            audioClips.ForEach(
                audioClip =>
                {
                    CreateAudioSource(audioClip.name, audioClip, groupName, containerObject.transform);
                });
        }

        /// <summary>
        /// Destroys the children of this object if there are too many.
        /// </summary>
        private void DestroyChildren()
        {
            if (transform.childCount > MaxChildren)
            {
                foreach (Transform child in transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        public void CreateAudioSources(AudioMixer mixer)
        {
            master = mixer;
            DestroyChildren();
            GetChildrenAudioSources();

            if (SoundEffectClips.Count > 0)
            {
                CreateAudioClipContainers(SoundEffects, SoundEffectClips, "SoundEffects");
            }
            if (BackgroundMusicClips.Count > 0)
            {
                CreateAudioClipContainers(BackgroundMusic, BackgroundMusicClips, "Music");
            }
            ReplaceAudioClip(OnClick, OnClickClip, "SoundEffects");
            ReplaceAudioClip(MainMenuMusic, MainMenuMusicClip, "Music");
        }

        /// <summary>
        /// Used for single audio sources. Creates a new Audio Source if none exist. Otherwise replaces the old audio source.
        /// </summary>
        /// <param name="audioSourceName">Name of audio source object.</param>
        /// <param name="newAudioClip">Audio clip to check for</param>
        /// <param name="groupName">The mixer group to use for Audio.</param>
        private void ReplaceAudioClip(string audioSourceName, AudioClip newAudioClip, string groupName)
        {
            if (newAudioClip == null)
            {
                return;
            }
            Transform oldAudioSourceTransform = transform.Find(audioSourceName);

            if (oldAudioSourceTransform == null)
            {
                CreateAudioSource(audioSourceName, newAudioClip, groupName);
            }
            else
            {
                AudioSource oldAudioSource = oldAudioSourceTransform.GetComponent<AudioSource>();
                oldAudioSource.clip = newAudioClip;
            }
        }

        /// <summary>
        /// Create an Audio Source in an empty child object.
        /// </summary>
        /// <param name="audioSourceName">Name of audio source obj.</param>
        /// <param name="audioClip">Audio clip to use.</param>
        /// <param name="groupName">The mixer group to use for Audio.</param>
        /// <param name="parentTransform">Defaults to this.transform. The parent of the new game object. </param>
        private void CreateAudioSource(string audioSourceName, AudioClip audioClip, string groupName, Transform parentTransform = null)
        {
            GameObject obj = new GameObject(audioSourceName);
            obj.transform.SetParent(parentTransform ? parentTransform : transform);
            AudioSource audioSource = obj.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = GetMixerGroup(master, groupName);
        }

        /// <summary>
        /// Finds a AudioMixerGroup in a give mixer. Useful for setting mixers when creating audio sources.
        /// </summary>
        /// <param name="mixer">The mixer to check for the group.</param>
        /// <param name="groupName">The mixer group to look for.</param>
        private AudioMixerGroup GetMixerGroup(AudioMixer mixer, string groupName)
        {
            var groups = mixer.FindMatchingGroups(groupName);
            foreach (var audioMixerGroup in groups)
            {
                if (audioMixerGroup.ToString() == groupName)
                {
                    return audioMixerGroup;
                }
            }
            return null;
        }
        
#endif

        #endregion

    }
    
}
