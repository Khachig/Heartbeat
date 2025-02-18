using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EasyRhythmAudioManager))]
public class EasyRhythmAudioManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        EasyRhythmAudioManager audioManager = (EasyRhythmAudioManager)target;

        if (GUILayout.Button("Start My Event"))
        {
            audioManager.myAudioEvent.start();
        }

        if (GUILayout.Button("Stop My Event"))
        {
            audioManager.myAudioEvent.stop();
        }
    }
}
