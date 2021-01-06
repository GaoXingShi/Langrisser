using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
#if UNITY_EDITOR
using MainSpace.Activities;
using MainSpace.ScriptableObject;
using Sense.BehaviourTree.Apply;
using UnityEditor;
#endif
using UnityEngine;

namespace MainSpace.SkillCommandSpace
{
    public enum SkillType
    {
        焰火冲击术,
        火焰飞弹术,
        火球术,
        炎爆术,

    }

    //public class SkillSystem
    //{
    //    public readonly Dictionary<SkillType, SkillBaseCommand> skillsList;

    //    public SkillSystem()
    //    {
    //        skillsList = new Dictionary<SkillType, SkillBaseCommand>();
    //        skillsList.Add(SkillType.初级火球术,new FireBall1SkillCommand());
    //    }
    //}
#if UNITY_EDITOR
    public class SkillSelectionEditorWindows : EditorWindow
    {
        private static SkillSelectionEditorWindows windows;
        private static CreateCommanderUnitNode selectionActivityUnit;
        private static SoliderConfig soliderUnit;
        private static List<SkillType> skillTypes;
        public static void OpenWindow(CreateCommanderUnitNode _unit)
        {

            if (windows != null)
            {
                windows.Close();
            }
            Rect rect = new Rect(Screen.width / 2, Screen.height / 2, 600, 420);
            windows = EditorWindow.GetWindowWithRect<SkillSelectionEditorWindows>(rect, true, "技能编辑面板");

            selectionActivityUnit = _unit;
            skillTypes = new List<SkillType>();
            skillTypes.AddRange(_unit.skillTypes);
            windows.Show();
        }

        public static void OpenWindow(SoliderConfig _unit)
        {
            if (windows != null)
            {
                windows.Close();
            }
            Rect rect = new Rect(Screen.width / 2, Screen.height / 2, 600, 420);
            windows = EditorWindow.GetWindowWithRect<SkillSelectionEditorWindows>(rect, true, "技能编辑面板");

            soliderUnit = _unit;
            skillTypes = new List<SkillType>();
            skillTypes.AddRange(_unit.skillTypes);
            windows.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            for (int i = 0; i < Enum.GetValues(typeof(SkillType)).Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                //GUILayout.Label(Enum.GetValues(typeof(SkillType)).GetValue(i).ToString());
                var temp = (SkillType)Enum.GetValues(typeof(SkillType)).GetValue(i);
                bool isToggle = skillTypes.Contains(temp);
                isToggle = EditorGUILayout.ToggleLeft(temp.ToString(), isToggle);

                if (skillTypes.Contains(temp) && !isToggle)
                {
                    // 删除操作
                    skillTypes.Remove(temp);
                }
                else if (!skillTypes.Contains(temp) && isToggle)
                {
                    skillTypes.Add(temp);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("确认修改"))
            {
                if (selectionActivityUnit != null)
                {
                    selectionActivityUnit.skillTypes = new List<SkillType>();
                    selectionActivityUnit.skillTypes.AddRange(skillTypes);
                    selectionActivityUnit = null;
                }

                if (soliderUnit != null)
                {
                    soliderUnit.skillTypes = new List<SkillType>();
                    soliderUnit.skillTypes.AddRange(skillTypes);
                    soliderUnit = null;
                }

                skillTypes = null;
                windows.Close();
            }

            EditorGUILayout.EndVertical();

        }

        private void OnDestroy()
        {
            windows = null;
            selectionActivityUnit = null;
            soliderUnit = null;
            skillTypes = null;
        }
    }
#endif

}

