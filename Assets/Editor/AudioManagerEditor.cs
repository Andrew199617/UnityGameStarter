using System.Linq;
using Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Editor
{
    /// <summary>
    /// Adds a Button to create all the audio sources.
    /// </summary>
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            AudioManager audioManager = (AudioManager)target;
            if (GUILayout.Button("Create Audio Sources"))
            {
                var assets = AssetDatabase.FindAssets("t:audioMixer", new []{"Assets/Audio"}).ToList();
                var asset = assets.FirstOrDefault();
                var mixer = (AudioMixer)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(asset), typeof(AudioMixer));
                
                audioManager.CreateAudioSources(mixer);
            }
        }

    }
}