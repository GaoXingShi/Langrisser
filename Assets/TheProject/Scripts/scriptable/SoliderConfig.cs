﻿using MainSpace.Activities;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MainSpace.ScriptableObject
{

    [CreateAssetMenu]
    public class SoliderConfig : ActivityConfig
    {
        public int attackValue, attackDistanceValue, defenseValue, moveValue, healthValue, magicValue;
        public int skillRangeValue, skillPowerValue;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SoliderConfig), true)]
    public class SoliderConfigInspectorEditor : ActivityConfigEditor
    {
        public SoliderConfig editorTarget;
        private int selectionIndex = 0;
        private void OnEnable()
        {
            OnInit();
            editorTarget = target as SoliderConfig;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUIStyle style = new GUIStyle();
            style.fontSize = 12;
            style.normal.textColor = Color.red;

            GUILayout.Label(new GUIContent("攻防移数值"), style);
            GUILayout.BeginHorizontal();
            GUILayout.Label("攻击");
            editorTarget.attackValue = EditorGUILayout.IntField(editorTarget.attackValue);
            GUILayout.Label("防御");
            editorTarget.defenseValue = EditorGUILayout.IntField(editorTarget.defenseValue);
            GUILayout.Label("移动");
            editorTarget.moveValue = EditorGUILayout.IntField(editorTarget.moveValue);
            GUILayout.EndHorizontal();

            GUILayout.Label(new GUIContent("技能相关"), style);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("魔法范围"));
            editorTarget.skillRangeValue = EditorGUILayout.IntField(editorTarget.skillRangeValue);
            GUILayout.Label(new GUIContent("魔法伤害"));
            editorTarget.skillPowerValue = EditorGUILayout.IntField(editorTarget.skillPowerValue);
            GUILayout.EndHorizontal();

            // 这个位置采用添加图片那个样式来添加可使用法术
            //GUILayout.BeginHorizontal();
            //GUILayout.EndHorizontal();

            GUILayout.Label(new GUIContent("HPMP"), style);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("HP"));
            editorTarget.healthValue = EditorGUILayout.IntField(editorTarget.healthValue);
            GUILayout.Label(new GUIContent("MP"));
            editorTarget.magicValue = EditorGUILayout.IntField(editorTarget.magicValue);
            GUILayout.EndHorizontal();
        }
    }
#endif


}