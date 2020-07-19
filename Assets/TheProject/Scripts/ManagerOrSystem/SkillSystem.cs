using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MainSpace
{

    public class SkillSystem : MonoBehaviour
    {

    }
#if UNITY_EDITOR
    public class SkillSelectionEditorWindows : EditorWindow
    {
        private static SkillSelectionEditorWindows windows;

        public static void OpenWindow()
        {
            if (windows == null)
            {
                Rect rect = new Rect(Screen.width / 2, Screen.height / 2, 600, 420);
                windows = EditorWindow.GetWindowWithRect<SkillSelectionEditorWindows>(rect, true, "技能编辑面板");
            }
            windows.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.EndVertical();


        }

        private void OnDestroy()
        {
            windows = null;
        }
    }
#endif

}

