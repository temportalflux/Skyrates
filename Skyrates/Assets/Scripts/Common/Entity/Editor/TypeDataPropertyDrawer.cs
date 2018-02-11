using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Entity;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Entity.TypeData))]
public class TypeDataPropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty typeInt = property.FindPropertyRelative("EntityTypeAsInt");
        SerializedProperty typeIndex = property.FindPropertyRelative("EntityTypeIndex");

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        position.height = 20;

        typeInt.intValue = EditorGUI.Popup(position, typeInt.intValue, Entity.AllTypesString);

        if (typeInt.intValue < Entity.ListableTypes.Length && EntityList.Instance != null)
        {
            position.y += 20 - EditorGUIUtility.standardVerticalSpacing;
            typeIndex.intValue = EditorGUI.Popup(position, typeIndex.intValue, EntityList.Instance.Categories[typeInt.intValue].Names);
        }
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty typeInt = property.FindPropertyRelative("EntityTypeAsInt");
        return base.GetPropertyHeight(property, label) + (typeInt.intValue < Entity.ListableTypes.Length ? 20 : 0);
    }

}
