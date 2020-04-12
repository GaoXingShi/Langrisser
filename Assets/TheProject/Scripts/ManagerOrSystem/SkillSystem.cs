using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MainSpace
{
    [Flags]
    public enum SkillFlag
    {
        none = 0,
        魔法箭 = 1,
        火球 = 2,
        冰刺 = 4,
        龙卷 = 8,
        闪电链 = 16
    }

    public class SkillSystem : MonoBehaviour
    {
        public SkillFlag skillFlag;
        void Start()
        {
        }


        public bool IsContain(SkillFlag _allSkill, SkillFlag _sthSkill)
        {
            // 与运算， 如果sthSkill 与 _allSkill并不对应则会返回0
            return 0 != (_allSkill & _sthSkill);
        }


    }
#if UNITY_EDITOR
    public class SkillSelectionEditorWindows : EditorWindow
    {
        private static SkillSelectionEditorWindows windows;

        private static Sense.BehaviourTree.Apply.CreateCommanderUnitNodeInspectorEditor createCommanderEditor;
        private static MainSpace.ScriptableObject.SoliderConfigInspectorEditor soliderConfig;
        private static int soliderConfigIndex;
        private static bool[] toggleInitArray, toggleRuntimeArray;

        public static void OpenWindow(Sense.BehaviourTree.Apply.CreateCommanderUnitNodeInspectorEditor _createCommanderEditor)
        {
            if (windows == null)
            {
                Rect rect = new Rect(Screen.width / 2, Screen.height / 2, 600, 420);
                windows = EditorWindow.GetWindowWithRect<SkillSelectionEditorWindows>(rect, true, "技能编辑面板");
            }

            createCommanderEditor = _createCommanderEditor;
            toggleInitArray = new bool[Enum.GetValues(typeof(SkillFlag)).Length];
            toggleRuntimeArray = new bool[Enum.GetValues(typeof(SkillFlag)).Length];
            //createCommanderEditor.editorTarget.skillMastery

            if ((int)createCommanderEditor.editorTarget.skillMastery == -1)
            {
                for (int i = 1; i < toggleInitArray.Length; i++)
                {
                    toggleInitArray[i] = toggleRuntimeArray[i] = true;
                }
            }
            else if ((int)createCommanderEditor.editorTarget.skillMastery == 0)
            {
                toggleInitArray[0] = toggleRuntimeArray[0] = true;
            }
            else
            {
                for (int i = 1; i < toggleInitArray.Length; i++)
                {
                    toggleInitArray[i] = toggleRuntimeArray[i] = (createCommanderEditor.editorTarget.skillMastery &
                                                                  (SkillFlag)Enum.GetValues(typeof(SkillFlag)).GetValue(i)) != 0;
                }
            }
            windows.Show();
        }

        public static void OpenWindow(ScriptableObject.SoliderConfigInspectorEditor _soliderConfig, int _index)
        {
            if (windows == null)
            {
                Rect rect = new Rect(Screen.width / 2, Screen.height / 2, 600, 420);
                windows = EditorWindow.GetWindowWithRect<SkillSelectionEditorWindows>(rect, true, "技能编辑面板");
            }

            soliderConfig = _soliderConfig;
            soliderConfigIndex = _index;
            toggleInitArray = new bool[Enum.GetValues(typeof(SkillFlag)).Length];
            toggleRuntimeArray = new bool[Enum.GetValues(typeof(SkillFlag)).Length];
            //createCommanderEditor.editorTarget.skillMastery

            if ((int)_soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].skillMastery == -1)
            {
                for (int i = 1; i < toggleInitArray.Length; i++)
                {
                    toggleInitArray[i] = toggleRuntimeArray[i] = true;
                }
            }
            else if ((int)_soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].skillMastery == 0)
            {
                toggleInitArray[0] = toggleRuntimeArray[0] = true;
            }
            else
            {
                for (int i = 1; i < toggleInitArray.Length; i++)
                {
                    toggleInitArray[i] = toggleRuntimeArray[i] = (_soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].skillMastery &
                                                                  (SkillFlag)Enum.GetValues(typeof(SkillFlag)).GetValue(i)) != 0;
                }
            }
            windows.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            if (createCommanderEditor != null)
            {
                GUILayout.Label(createCommanderEditor.editorTarget.unitName);
                GUILayout.Label(new GUIContent(createCommanderEditor.editorTarget.activityConfig.normalSprite.texture));
            }
            else if (soliderConfig != null)
            {
                GUILayout.Label(soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].soliderType.ToString());
                GUILayout.Label(new GUIContent(soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].activityConfig.normalSprite.texture));
            }



            int index = 0;

            for (int i = 0; i < Enum.GetValues(typeof(SkillFlag)).Length; i++)
            {
                SkillFlag currentFlag = (SkillFlag)Enum.GetValues(typeof(SkillFlag)).GetValue(i);

                if (index == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                index++;
                toggleRuntimeArray[i] = EditorGUILayout.ToggleLeft(currentFlag.ToString(),
                    toggleRuntimeArray[i]);

                if (index == 3)
                {
                    index = 0;
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (index != 0)
            {
                EditorGUILayout.EndHorizontal();
            }

            if (toggleInitArray[0] == toggleRuntimeArray[0])
            {
                for (int i = 1; i < toggleRuntimeArray.Length; i++)
                {
                    if (toggleInitArray[i] != toggleRuntimeArray[i])
                    {
                        toggleInitArray[i] = toggleRuntimeArray[i];
                        if (toggleRuntimeArray[i])
                        {
                            toggleInitArray[0] = toggleRuntimeArray[0] = false;

                            if (createCommanderEditor != null)
                            {
                                createCommanderEditor.editorTarget.skillMastery =
                                    createCommanderEditor.editorTarget.skillMastery |
                                    (SkillFlag)Enum.GetValues(typeof(SkillFlag)).GetValue(i);
                            }
                            else if (soliderConfig != null)
                            {
                                soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].skillMastery =
                                    soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].skillMastery |
                                    (SkillFlag)Enum.GetValues(typeof(SkillFlag)).GetValue(i);
                            }
                        }
                        else
                        {
                            if (createCommanderEditor != null)
                            {
                                createCommanderEditor.editorTarget.skillMastery =
                                    createCommanderEditor.editorTarget.skillMastery &
                                    ~(SkillFlag)Enum.GetValues(typeof(SkillFlag)).GetValue(i);
                            }
                            else if (soliderConfig != null)
                            {
                                soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].skillMastery =
                                    soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].skillMastery &
                                    ~(SkillFlag)Enum.GetValues(typeof(SkillFlag)).GetValue(i);
                            }

                            if (!toggleRuntimeArray.Any(x => x))
                            {
                                if (createCommanderEditor != null)
                                {
                                    createCommanderEditor.editorTarget.skillMastery = 0;
                                }
                                else if (soliderConfig != null)
                                {
                                    soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].skillMastery = 0;
                                }

                                toggleRuntimeArray[0] = toggleInitArray[0] = true;
                            }
                        }
                    }
                }
            }
            else
            {
                toggleInitArray[0] = toggleRuntimeArray[0];
                if (toggleRuntimeArray[0])
                {
                    if (createCommanderEditor != null)
                    {
                        createCommanderEditor.editorTarget.skillMastery = 0;
                    }
                    else if (soliderConfig != null)
                    {
                        soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].skillMastery = 0;
                    }

                    for (int i = 1; i < toggleInitArray.Length; i++)
                    {
                        toggleInitArray[i] = toggleRuntimeArray[i] = false;
                    }
                }
            }



            EditorGUILayout.EndVertical();


        }

        private void OnDestroy()
        {
            windows = null;
            createCommanderEditor = null;
            soliderConfig = null;
            soliderConfigIndex = 0;
            toggleInitArray = null;
        }
    }
#endif

}

