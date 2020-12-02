using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MainSpace.ScriptableObject
{
    public class ActivityConfig : UnityEngine.ScriptableObject
    {
        public Sprite normalSprite, showOffSprite;
        public string roleName;
        public FightType fightType;
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
            OnInit();
        }

        protected virtual void OnInit()
        {
            serialized = new SerializedObject(target);
            activityConfig = target as ActivityConfig;
        }

        public override void OnInspectorGUI()
        {
            serialized.Update();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("单位在地图显示的小图案：");
            if (activityConfig.normalSprite != null)
            {
                GUILayout.Label(new GUIContent(activityConfig.normalSprite.texture));
            }
            activityConfig.normalSprite =
                EditorGUILayout.ObjectField(activityConfig.normalSprite, typeof(Sprite), true) as Sprite;
            if (activityConfig.showOffSprite != null)
            {
                GUILayout.Label(new GUIContent(activityConfig.showOffSprite.texture));
            }
            activityConfig.showOffSprite =
                EditorGUILayout.ObjectField(activityConfig.showOffSprite, typeof(Sprite), true) as Sprite;
            EditorGUILayout.EndHorizontal();

            activityConfig.roleName = EditorGUILayout.TextField("职业描述", activityConfig.roleName);
            activityConfig.fightType = (FightType)EditorGUILayout.EnumPopup("战斗克制", activityConfig.fightType);
            activityConfig.movingType = (TerrainActionType)EditorGUILayout.EnumPopup("行动方式", activityConfig.movingType);

            EditorGUILayout.EndVertical();

            serialized.ApplyModifiedProperties();
        }


    }

#endif
}