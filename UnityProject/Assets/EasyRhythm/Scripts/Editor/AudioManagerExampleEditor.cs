using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioManagerExample))]
public class AudioManagerExampleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        AudioManagerExample audioManager = (AudioManagerExample)target;

        if (GUILayout.Button("Start Audio"))
        {
            audioManager.myAudioEvent.start();
        }

        if (GUILayout.Button("Stop Audio"))
        {
            audioManager.myAudioEvent.stop();
        }
    }
}
