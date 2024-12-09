using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AsteriodSpawner))]
public class AsteroidSpawnerEditor : Editor
{
    private AsteriodSpawner spawner;

    private bool showClusterParameters;
    private SerializedProperty numberOfClusters;
    private SerializedProperty minClusterRadius;
    private SerializedProperty maxClusterRadius;
    private SerializedProperty minClusterChildren;
    private SerializedProperty maxClusterChildren;

    private SerializedProperty seed;
    private SerializedProperty numberOfAsteroids;
    private SerializedProperty radius;
    private SerializedProperty dist;
    private SerializedProperty mean;
    private SerializedProperty standardDevitation;
    private SerializedProperty minMass;
    private SerializedProperty maxMass;

    private void OnEnable() {
        spawner = target as AsteriodSpawner;

        numberOfClusters = serializedObject.FindProperty("numberOfClusters");
        minClusterRadius = serializedObject.FindProperty("minClusterRadius");
        maxClusterRadius = serializedObject.FindProperty("maxClusterRadius");
        minClusterChildren = serializedObject.FindProperty("minClusterChildren");
        maxClusterChildren = serializedObject.FindProperty("maxClusterChildren");

        seed = serializedObject.FindProperty("seed");
        numberOfAsteroids = serializedObject.FindProperty("numberOfAsteroids");
        radius = serializedObject.FindProperty("radius");
        dist = serializedObject.FindProperty("distributionFunction");
        mean = serializedObject.FindProperty("mean");
        standardDevitation = serializedObject.FindProperty("standardDeviation");
        minMass = serializedObject.FindProperty("minMass");
        maxMass = serializedObject.FindProperty("maxMass");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        showClusterParameters = EditorGUILayout.Foldout(showClusterParameters, "Clusters");

        if (showClusterParameters) {
            EditorGUILayout.PropertyField(numberOfClusters);
            EditorGUILayout.PropertyField(minClusterRadius);
            EditorGUILayout.PropertyField(maxClusterRadius);
            EditorGUILayout.PropertyField(minClusterChildren);
            EditorGUILayout.PropertyField(maxClusterChildren);
        }

        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(numberOfAsteroids);
        EditorGUILayout.PropertyField(radius);

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

    public void OnInspectorUpdate() {
        this.Repaint();
    }
}