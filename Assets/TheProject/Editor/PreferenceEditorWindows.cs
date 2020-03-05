using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PreferenceEditorWindows : EditorWindow
{
    public static Color movingColor, attackColor;
    [PreferenceItem("GaoXingShi")]
    public static void OpenWindow()
    {
        movingColor = EditorGUILayout.ColorField(new GUIContent("movingColor"),movingColor);
        attackColor = EditorGUILayout.ColorField(new GUIContent("attackColor"),attackColor);
    }
}
