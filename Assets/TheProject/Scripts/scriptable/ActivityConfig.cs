using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MainSpace.ScriptableObject
{
    [CreateAssetMenu]
    public class ActivityConfig : UnityEngine.ScriptableObject
    {
        public Sprite normalSprite, showOffSprite;
        public TerrainActionType movingType;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(ActivityConfig))]
    public class ActivityConfigEditor : Editor
    {
        private SerializedObject serialized;
        private ActivityConfig activityConfig;

        void OnEnable()
        {
            serialized = new SerializedObject(target);
            activityConfig = target as ActivityConfig;
        }

        public override void OnInspectorGUI()
        {
            serialized.Update();

            EditorGUILayout.BeginVertical();

            activityConfig.normalSprite =
                EditorGUILayout.ObjectField(new GUIContent("NormalSprite"),activityConfig.normalSprite, typeof(Sprite), true) as Sprite;
            activityConfig.showOffSprite =
                EditorGUILayout.ObjectField(new GUIContent("ShowOffSprite"),activityConfig.showOffSprite, typeof(Sprite), true) as Sprite;

            activityConfig.movingType = (TerrainActionType)EditorGUILayout.EnumPopup("行动方式", activityConfig.movingType);

            EditorGUILayout.EndVertical();


            serialized.ApplyModifiedProperties();
        }
    }
#endif
}