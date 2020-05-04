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
    [Flags]
    public enum ActiveSkillsFlag
    {
        none = 0,
        魔法箭 = 1,
        火球 = 2,
        冰刺 = 4,
        龙卷 = 8,
        闪电链 = 16,
    }

    [Flags]
    public enum PassiveSkillsFlag
    {
        none = 0,
        疗伤指导 = 1,           // 周边范围1的己方士兵可以得到相等的治疗
        TheBoss = 2,
        战争狂 = 4,
    }

    public class SkillSystem : MonoBehaviour
    {
        public ActiveSkillsFlag activeSkillsFlag;
        void Start()
        {
        }


        public bool IsContain(ActiveSkillsFlag _allActiveSkills, ActiveSkillsFlag _sthActiveSkills)
        {
            // 与运算， 如果sthSkill 与 _allSkill并不对应则会返回0
            return 0 != (_allActiveSkills & _sthActiveSkills);
        }


    }
#if UNITY_EDITOR
    public class SkillSelectionEditorWindows : EditorWindow
    {
        private static SkillSelectionEditorWindows windows;

        private static Sense.BehaviourTree.Apply.CreateCommanderUnitNodeInspectorEditor createCommanderEditor;
        private static MainSpace.ScriptableObject.SoliderConfigInspectorEditor soliderConfig;
        private static int soliderConfigIndex;
        private static bool[] activeSkillsToggleInitArray, activeSkillsToggleRuntimeArray;
        private static bool[] passiveSkillsToggleInitArray, passiveSkillsToggleRuntimeArray;

        public static void OpenWindow(Sense.BehaviourTree.Apply.CreateCommanderUnitNodeInspectorEditor _createCommanderEditor)
        {
            if (windows == null)
            {
                Rect rect = new Rect(Screen.width / 2, Screen.height / 2, 600, 420);
                windows = EditorWindow.GetWindowWithRect<SkillSelectionEditorWindows>(rect, true, "技能编辑面板");
            }

            createCommanderEditor = _createCommanderEditor;
            activeSkillsToggleInitArray = new bool[Enum.GetValues(typeof(ActiveSkillsFlag)).Length];
            passiveSkillsToggleInitArray = new bool[Enum.GetValues(typeof(PassiveSkillsFlag)).Length];
            activeSkillsToggleRuntimeArray = new bool[Enum.GetValues(typeof(ActiveSkillsFlag)).Length];
            passiveSkillsToggleRuntimeArray = new bool[Enum.GetValues(typeof(PassiveSkillsFlag)).Length];

            ToggleInit((int) createCommanderEditor.editorTarget.activeSkillsMastery, activeSkillsToggleInitArray,
                activeSkillsToggleRuntimeArray, Enum.GetValues(typeof(ActiveSkillsFlag)));

            ToggleInit((int)createCommanderEditor.editorTarget.passiveSkillsMastery, passiveSkillsToggleInitArray,
                passiveSkillsToggleRuntimeArray, Enum.GetValues(typeof(PassiveSkillsFlag)));

            windows.Show();
        }

        /// <summary>
        /// toggle初始化
        /// </summary>
        /// <param name="skillValue"></param>
        /// <param name="activeSkillsToggleInitArray"></param>
        /// <param name="activeSkillsToggleRuntimeArray"></param>
        /// <param name="skillsArray"></param>
        private static void ToggleInit(int skillValue,bool[] activeSkillsToggleInitArray,bool[] activeSkillsToggleRuntimeArray,Array skillsArray)
        {
            if (skillValue == -1)
            {
                for (int i = 1; i < activeSkillsToggleInitArray.Length; i++)
                {
                    activeSkillsToggleInitArray[i] = activeSkillsToggleRuntimeArray[i] = true;
                }

            }
            else if (skillValue == 0)
            {
                activeSkillsToggleInitArray[0] = activeSkillsToggleRuntimeArray[0] = true;
            }
            else
            {
                for (int i = 1; i < activeSkillsToggleInitArray.Length; i++)
                {
                    activeSkillsToggleInitArray[i] = activeSkillsToggleRuntimeArray[i] = (skillValue & (int)skillsArray.GetValue(i)) != 0;
                }
            }
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
            activeSkillsToggleInitArray = new bool[Enum.GetValues(typeof(ActiveSkillsFlag)).Length];
            passiveSkillsToggleInitArray = new bool[Enum.GetValues(typeof(PassiveSkillsFlag)).Length];
            activeSkillsToggleRuntimeArray = new bool[Enum.GetValues(typeof(ActiveSkillsFlag)).Length];
            passiveSkillsToggleRuntimeArray = new bool[Enum.GetValues(typeof(PassiveSkillsFlag)).Length];

            ToggleInit((int)_soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].activeSkillsMastery,
                activeSkillsToggleInitArray, activeSkillsToggleRuntimeArray, Enum.GetValues(typeof(ActiveSkillsFlag)));

            ToggleInit((int)_soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].passiveSkillsMastery, 
                passiveSkillsToggleInitArray, passiveSkillsToggleRuntimeArray, Enum.GetValues(typeof(PassiveSkillsFlag)));

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

            // 主动技能
            {
                GUILayout.Label(new GUIContent("主动技能"));

                int skill1 = 0;
                int skill2 = 0;
                if (createCommanderEditor != null)
                {
                    skill1 = (int) createCommanderEditor.editorTarget.activeSkillsMastery;
                }

                if (soliderConfig != null)
                {
                    skill2 = (int) soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].activeSkillsMastery;
                }

                ToggleValueUpdate<ActiveSkillsFlag>(activeSkillsToggleInitArray, activeSkillsToggleRuntimeArray, ref skill1,
                    Enum.GetValues(typeof(ActiveSkillsFlag)),
                    ref skill2, Enum.GetValues(typeof(ActiveSkillsFlag)));

                if (createCommanderEditor != null)
                    createCommanderEditor.editorTarget.activeSkillsMastery = (ActiveSkillsFlag) skill1;
                if (soliderConfig != null)
                    soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].activeSkillsMastery =
                        (ActiveSkillsFlag) skill2;
            }

            // 被动技能
            {
                GUILayout.Label(new GUIContent("被动技能"));

                int skill1 = 0;
                int skill2 = 0;
                if (createCommanderEditor != null)
                {
                    skill1 = (int)createCommanderEditor.editorTarget.passiveSkillsMastery;
                }

                if (soliderConfig != null)
                {
                    skill2 = (int)soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].passiveSkillsMastery;
                }

                ToggleValueUpdate<PassiveSkillsFlag>(passiveSkillsToggleInitArray, passiveSkillsToggleRuntimeArray, ref skill1,
                    Enum.GetValues(typeof(PassiveSkillsFlag)),
                    ref skill2, Enum.GetValues(typeof(PassiveSkillsFlag)));

                if (createCommanderEditor != null)
                    createCommanderEditor.editorTarget.passiveSkillsMastery = (PassiveSkillsFlag)skill1;
                if (soliderConfig != null)
                    soliderConfig.editorTarget.soliderDataArray[soliderConfigIndex].passiveSkillsMastery =
                        (PassiveSkillsFlag)skill2;
            }

            EditorGUILayout.EndVertical();


        }

        /// <summary>
        /// Toggle的绘制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_toggleInitArray"></param>
        /// <param name="_toggleRuntimeArray"></param>
        /// <param name="_createCommanderActiveSkills"></param>
        /// <param name="_createCommanderArray"></param>
        /// <param name="_soliderConfigActiveSkills"></param>
        /// <param name="_soliderConfigArray"></param>
        private void ToggleValueUpdate<T>(bool[] _toggleInitArray, bool[] _toggleRuntimeArray, ref int _createCommanderActiveSkills,Array _createCommanderArray, ref int _soliderConfigActiveSkills, Array _soliderConfigArray) where T:Enum
        {
            int index = 0;

            for (int i = 0; i < _createCommanderArray.Length; i++)
            {
                T currentFlag = (T)_createCommanderArray.GetValue(i);

                if (index == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                index++;
                _toggleRuntimeArray[i] = EditorGUILayout.ToggleLeft(currentFlag.ToString(),
                    _toggleRuntimeArray[i]);

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

            if (_toggleInitArray[0] == _toggleRuntimeArray[0])
            {
                for (int i = 1; i < _toggleRuntimeArray.Length; i++)
                {
                    if (_toggleInitArray[i] != _toggleRuntimeArray[i])
                    {
                        _toggleInitArray[i] = _toggleRuntimeArray[i];
                        if (_toggleRuntimeArray[i])
                        {
                            _toggleInitArray[0] = _toggleRuntimeArray[0] = false;

                            if (createCommanderEditor != null)
                            {
                                _createCommanderActiveSkills =
                                    _createCommanderActiveSkills | (int)_createCommanderArray.GetValue(i);
                            }
                            else if (soliderConfig != null)
                            {
                                _soliderConfigActiveSkills =
                                    _soliderConfigActiveSkills | (int)_soliderConfigArray.GetValue(i);
                            }
                        }
                        else
                        {
                            if (createCommanderEditor != null)
                            {
                                _createCommanderActiveSkills =  _createCommanderActiveSkills & ~(int)_createCommanderArray.GetValue(i);
                            }
                            else if (soliderConfig != null)
                            {
                                _soliderConfigActiveSkills = _soliderConfigActiveSkills & ~(int)_soliderConfigArray.GetValue(i);
                            }

                            if (!_toggleRuntimeArray.Any(x => x))
                            {
                                if (createCommanderEditor != null)
                                {
                                    _createCommanderActiveSkills = 0;
                                }
                                else if (soliderConfig != null)
                                {
                                    _soliderConfigActiveSkills = 0;
                                }

                                _toggleRuntimeArray[0] = _toggleInitArray[0] = true;
                            }
                        }
                    }
                }
            }
            else
            {
                _toggleInitArray[0] = _toggleRuntimeArray[0];
                if (_toggleRuntimeArray[0])
                {
                    if (createCommanderEditor != null)
                    {
                        _createCommanderActiveSkills = 0;
                    }
                    else if (soliderConfig != null)
                    {
                        _soliderConfigActiveSkills = 0;
                    }

                    for (int i = 1; i < _toggleInitArray.Length; i++)
                    {
                        _toggleInitArray[i] = _toggleRuntimeArray[i] = false;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            windows = null;
            createCommanderEditor = null;
            soliderConfig = null;
            soliderConfigIndex = 0;
            activeSkillsToggleInitArray = null;
        }
    }
#endif

}

