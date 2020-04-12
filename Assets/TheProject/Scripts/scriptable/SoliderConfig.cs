using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using UnityEngine;

namespace MainSpace.ScriptableObject
{
    [CreateAssetMenu]
    public class SoliderConfig : UnityEngine.ScriptableObject
    {
        //public bool 我在这里留白了;    // 类型将是地形类型(因为Behaviour中的英雄也需要引用)
        [System.Serializable]
        public struct SoliderData
        {
            public string elementName;
            public SoliderType soliderType;
            public ActivityConfig activityConfig;
            public int attackValue, attackRangeValue, defenseValue, moveRangeValue, healthValue, magicValue;
            public SkillFlag skillMastery;
            public int skillRangeValue;
        }

        public SoliderData[] soliderDataArray;
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SoliderConfig), true)]
    public class SoliderConfigInspectorEditor : UnityEditor.Editor
    {
        public SoliderConfig editorTarget;
        private int selectionIndex = 0;
        private void OnEnable()
        {
            editorTarget = target as SoliderConfig;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            
            if (editorTarget.soliderDataArray.Length >= 1)
            {
                selectionIndex = UnityEditor.EditorGUILayout.IntSlider(selectionIndex, 0, editorTarget.soliderDataArray.Length - 1);

                if (GUILayout.Button("编辑技能"))
                {
                    SkillSelectionEditorWindows.OpenWindow(this, selectionIndex);
                }
            }


        }
    }
#endif

}

