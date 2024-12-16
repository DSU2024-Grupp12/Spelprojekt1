using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

// https://www.youtube.com/watch?v=ur-qy6SjVQw
[CustomPropertyDrawer(typeof(Upgradeable))]
public class UpgradeableDrawer : PropertyDrawer
{
    private SerializedProperty baseValue;
    private SerializedProperty upgradeMatrix;
    private SerializedProperty attributeID;
    private SerializedProperty highGood;

    private bool matrixWasNull;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (baseValue == null) baseValue = property.FindPropertyRelative("baseValue");
        if (upgradeMatrix == null) upgradeMatrix = property.FindPropertyRelative("matrix");
        if (attributeID == null) attributeID = property.FindPropertyRelative("attributeID");
        if (highGood == null) highGood = property.FindPropertyRelative("highGood");

        EditorGUI.BeginProperty(position, label, property);

        #region Rects

        Rect foldoutRect = new(position.x - 3f, position.y, EditorGUIUtility.labelWidth - 1f,
            EditorGUIUtility.singleLineHeight);
        Rect baseValueRect = new(
            position.x + foldoutRect.width,
            position.y,
            (position.width - foldoutRect.width) / 2f,
            EditorGUIUtility.singleLineHeight
        );
        Rect matrixLabelRect = new(
            position.x + foldoutRect.width + baseValueRect.width + 4f,
            position.y,
            44f,
            EditorGUIUtility.singleLineHeight
        );
        Rect matrixObjectRect = new(
            position.x + foldoutRect.width + baseValueRect.width + matrixLabelRect.width + 4f,
            position.y,
            -2f - matrixLabelRect.width + (position.width - foldoutRect.width) / 2f,
            EditorGUIUtility.singleLineHeight
        );

        #endregion

        GUILayout.BeginHorizontal();
        if (!upgradeMatrix.objectReferenceValue) {
            EditorGUI.LabelField(foldoutRect, label);
            matrixWasNull = true;
            property.isExpanded = false;
        }
        else {
            if (matrixWasNull) {
                matrixWasNull = false;
                property.isExpanded = true;
            }
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
        }
        baseValue.floatValue = EditorGUI.FloatField(baseValueRect, baseValue.floatValue);
        EditorGUI.LabelField(matrixLabelRect, "Matrix");
        EditorGUI.ObjectField(matrixObjectRect, upgradeMatrix, new GUIContent());
        GUILayout.EndHorizontal();

        if (property.isExpanded) {
            EditorGUI.indentLevel++;
            GUILayout.BeginHorizontal();

            #region Rects

            Rect attributeIDLabelRect = new(
                position.x,
                position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                EditorGUIUtility.labelWidth - 16f,
                EditorGUIUtility.singleLineHeight
            );
            Rect attributeIDFieldRect = new(
                position.x + attributeIDLabelRect.width,
                position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                8f + (position.width - attributeIDLabelRect.width) / 2f,
                EditorGUIUtility.singleLineHeight
            );
            Rect highIsGoodLabelRect = new(
                position.x + attributeIDLabelRect.width + attributeIDFieldRect.width - 12f,
                position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                90f,
                EditorGUIUtility.singleLineHeight
            );
            Rect highIsGoodToggleRect = new Rect(
                position.x + attributeIDLabelRect.width + attributeIDFieldRect.width - 24f + highIsGoodLabelRect.width,
                position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                30f,
                EditorGUIUtility.singleLineHeight
            );
            float lowIsGoodXPos = Mathf.Max(
                position.x + position.width - 80f,
                position.x + attributeIDLabelRect.width + attributeIDFieldRect.width
                - 36f + highIsGoodLabelRect.width + highIsGoodToggleRect.width);
            Rect lowIsGoodLabelRect = new Rect(
                lowIsGoodXPos,
                position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                40f,
                EditorGUIUtility.singleLineHeight
            );
            Rect lowIsGoodToggleRect = new Rect(
                lowIsGoodXPos + lowIsGoodLabelRect.width - 12f,
                position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                30f,
                EditorGUIUtility.singleLineHeight
            );

            #endregion

            string attrToolTip =
                $"{property.displayName} will be affected by any upgrade modules " +
                $"in the upgrade matrix with the attribute ID: \"{attributeID.stringValue}\"";
            GUIContent attributeIDLabel = new GUIContent("Attribute ID", attrToolTip);
            EditorGUI.LabelField(attributeIDLabelRect, attributeIDLabel);
            attributeID.stringValue = EditorGUI.TextField(attributeIDFieldRect, attributeID.stringValue);
            GUIContent highIsGoodLabel = new GUIContent("Prefer:  High");
            GUIContent lowIsGoodLabel = new GUIContent("Low");

            EditorGUI.LabelField(highIsGoodLabelRect, highIsGoodLabel);
            highGood.boolValue = EditorGUI.Toggle(highIsGoodToggleRect, highGood.boolValue);

            EditorGUI.LabelField(lowIsGoodLabelRect, lowIsGoodLabel);
            highGood.boolValue = !EditorGUI.Toggle(lowIsGoodToggleRect, !highGood.boolValue);

            GUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (property.isExpanded) return EditorGUIUtility.singleLineHeight * 2 + 4f;
        else return EditorGUIUtility.singleLineHeight;
    }
}