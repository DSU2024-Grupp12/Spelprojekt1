using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

// https://docs.unity3d.com/6000.0/Documentation/ScriptReference/PropertyDrawer.html
[CustomPropertyDrawer(typeof(Upgradeable))]
public class UpgradeableDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        VisualElement container = new VisualElement();
    
        // Create property fields.
        PropertyField baseValueField = new(property.FindPropertyRelative("baseValue"), "Base Value");
        PropertyField matrixField = new(property.FindPropertyRelative("matrix"), "Upgrade Matrix");
        PropertyField attributeIDField = new(property.FindPropertyRelative(""), "Fancy Name");
    
        // Add fields to the container.
        container.Add(baseValueField);
        container.Add(matrixField);
        container.Add(attributeIDField);
    
        return container;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
    }
}

// IngredientDrawerUIE
// [CustomPropertyDrawer(typeof(Ingredient))]
public class IngredientDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var amountField = new PropertyField(property.FindPropertyRelative("amount"));
        var unitField = new PropertyField(property.FindPropertyRelative("unit"));
        var nameField = new PropertyField(property.FindPropertyRelative("name"), "Fancy Name");

        // Add fields to the container.
        container.Add(amountField);
        container.Add(unitField);
        container.Add(nameField);

        return container;
    }
}
