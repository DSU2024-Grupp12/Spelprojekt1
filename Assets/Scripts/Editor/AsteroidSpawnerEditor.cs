using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AsteriodSpawner))]
public class AsteroidSpawnerEditor : Editor
{
    private AsteriodSpawner spawner;

    private SerializedProperty seed;
    private SerializedProperty numberOfAsteroids;
    private SerializedProperty width;
    private SerializedProperty height;
    private SerializedProperty dist;
    private SerializedProperty mean;
    private SerializedProperty standardDevitation;
    private SerializedProperty minMass;
    private SerializedProperty maxMass;

    private void OnEnable() {
        spawner = target as AsteriodSpawner;

        seed = serializedObject.FindProperty("seed");
        numberOfAsteroids = serializedObject.FindProperty("numberOfAsteroids");
        width = serializedObject.FindProperty("width");
        height = serializedObject.FindProperty("height");
        dist = serializedObject.FindProperty("distributionFunction");
        mean = serializedObject.FindProperty("mean");
        standardDevitation = serializedObject.FindProperty("standardDeviation");
        minMass = serializedObject.FindProperty("minMass");
        maxMass = serializedObject.FindProperty("maxMass");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(numberOfAsteroids);
        EditorGUILayout.PropertyField(width);
        EditorGUILayout.PropertyField(height);

        EditorGUILayout.PropertyField(seed);
        EditorGUILayout.PropertyField(dist);

        AsteriodSpawner.MassDistribution distribution = (AsteriodSpawner.MassDistribution)dist.enumValueIndex;

        EditorGUILayout.PropertyField(minMass);
        EditorGUILayout.PropertyField(maxMass);
        if (distribution == AsteriodSpawner.MassDistribution.Normal) {
            EditorGUILayout.PropertyField(mean);
            EditorGUILayout.PropertyField(standardDevitation);
        }

        if (serializedObject.ApplyModifiedProperties()) {
            spawner!.CreateAsteroids();
        }

        if (GUILayout.Button("Create Asteroids")) spawner!.CreateAsteroids();
        if (GUILayout.Button("Clear Asteroids")) spawner!.ClearAsteroids();
    }
}