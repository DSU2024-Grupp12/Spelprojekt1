using UnityEditor;

[CustomEditor(typeof(Shield))]
public class ShieldEditor : Editor
{
    private Shield shield;

    private SerializedProperty regenerationRate;
    private SerializedProperty regenerationCooldown;

    void OnEnable() {
        shield = target as Shield;

        regenerationRate = serializedObject.FindProperty("regenerationRate");
        regenerationCooldown = serializedObject.FindProperty("regenerationCooldown");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (shield.autoRegenerate) {
            EditorGUILayout.PropertyField(regenerationRate);
            EditorGUILayout.PropertyField(regenerationCooldown);
        }

        serializedObject.ApplyModifiedProperties();
    }
}