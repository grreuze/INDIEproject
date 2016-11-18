using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Chroma))]
public class ChromaDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        label = EditorGUI.BeginProperty(position, label, property);

        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        contentPosition.width *= 0.333f;
        EditorGUI.indentLevel = 0;

        EditorGUIUtility.labelWidth = 14f;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("r"), new GUIContent("R"));

        contentPosition.x += contentPosition.width;
        EditorGUIUtility.labelWidth = 14f;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("g"), new GUIContent("G"));

        contentPosition.x += contentPosition.width;
        EditorGUIUtility.labelWidth = 14f;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("b"), new GUIContent("B"));

        EditorGUI.EndProperty();
    }
}
