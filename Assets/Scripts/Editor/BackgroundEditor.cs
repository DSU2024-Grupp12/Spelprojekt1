using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Background))]
public class BackgroundEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        Background bg = target as Background;
        if (GUILayout.Button("Generate Stars")) bg!.GenerateStars();
        if (GUILayout.Button("Clear Stars")) bg!.ClearStars();
    }
}