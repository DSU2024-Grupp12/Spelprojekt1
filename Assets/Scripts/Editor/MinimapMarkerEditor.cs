using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MinimapMarker))]
public class MinimapMarkerEditor : Editor
{
    private MinimapMarker marker;

    private SerializedProperty proxyPrefab;
    private SerializedProperty seperateProxySprite;
    private SerializedProperty proxySprite;
    private SerializedProperty proxyColor;

    private void OnEnable() {
        marker = target as MinimapMarker;

        proxyPrefab = serializedObject.FindProperty("proxyPrefab");
        seperateProxySprite = serializedObject.FindProperty("seperateProxySprite");
        proxySprite = serializedObject.FindProperty("proxySprite");
        proxyColor = serializedObject.FindProperty("proxyColor");
    }

    /// <inheritdoc />
    public override void OnInspectorGUI() {
        EditorGUILayout.PropertyField(proxyPrefab);
        EditorGUILayout.PropertyField(seperateProxySprite);

        bool seperate = seperateProxySprite.boolValue;

        if (seperate) {
            EditorGUILayout.PropertyField(proxySprite);
            EditorGUILayout.PropertyField(proxyColor);
        }
        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}