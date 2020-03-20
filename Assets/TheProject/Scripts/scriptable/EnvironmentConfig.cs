using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

namespace MainSpace.ScriptableObject
{
    public enum TerrainType
    {
        荒地,
        草地,
        森林,
        山地,
        浅水,
        深水,
        宫殿,
        桥梁,

    }

    [System.Serializable]
    public struct TerrainCalculateData
    {
        public TerrainActionType movingType;
        public int[] terrainMovingValue;
    }

    [CreateAssetMenu]
    public class EnvironmentConfig : UnityEngine.ScriptableObject
    {
        public TerrainCalculateData[] data;

    }
#if UNITY_EDITOR
    [CustomEditor(typeof(EnvironmentConfig))]
    public class EnvironmentConfigEditor : Editor
    {
        private SerializedObject serialized;
        private EnvironmentConfig envirconmentConfig;
        private TerrainCalculateData[] cacheTerrainCalculateData;
        private int terrainActionTypeCount, terrainTypeCount;
        void OnEnable()
        {
            serialized = new SerializedObject(target);
            envirconmentConfig = target as EnvironmentConfig;
            TerrainCalculateData[] temp = new TerrainCalculateData[]{};

            if (terrainActionTypeCount != Enum.GetNames(typeof(TerrainActionType)).Length || terrainTypeCount != Enum.GetNames(typeof(TerrainType)).Length)
            {
                temp = (TerrainCalculateData[])envirconmentConfig.data.Clone();
                envirconmentConfig.data = new TerrainCalculateData[Enum.GetNames(typeof(TerrainActionType)).Length];
                for (int i = 0; i < Enum.GetNames(typeof(TerrainActionType)).Length; i++)
                {
                    envirconmentConfig.data[i].movingType = (TerrainActionType)i;
                    envirconmentConfig.data[i].terrainMovingValue = new int[Enum.GetNames(typeof(TerrainType)).Length];
                }

            }

            if (temp.Length != 0)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    if (i == envirconmentConfig.data.Length)
                    {
                        break;
                    }

                    envirconmentConfig.data[i].movingType = temp[i].movingType;
                    for (int j = 0; j < temp[i].terrainMovingValue.Length; j++)
                    {
                        envirconmentConfig.data[i].terrainMovingValue[j] = temp[i].terrainMovingValue[j];
                    }
                }

            }

            cacheTerrainCalculateData = envirconmentConfig.data;
            terrainActionTypeCount = Enum.GetNames(typeof(TerrainActionType)).Length;
            terrainTypeCount = Enum.GetNames(typeof(TerrainType)).Length;
        }

        public override void OnInspectorGUI()
        {
            serialized.Update();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space(10);
            foreach (string v in Enum.GetNames(typeof(TerrainType)))
            {
                GUILayout.Label(v);
            }

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginVertical();

            for (int i = 0; i < Enum.GetNames(typeof(TerrainActionType)).Length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(((TerrainActionType)i).ToString());
                for (int j = 0; j < Enum.GetNames(typeof(TerrainType)).Length; j++)
                {
                    cacheTerrainCalculateData[i].terrainMovingValue[j] = EditorGUILayout.IntField(cacheTerrainCalculateData[i].terrainMovingValue[j]);
                }

                EditorGUILayout.EndHorizontal();
            }
            foreach (var v in Enum.GetNames(typeof(TerrainActionType)))
            {

            }


            EditorGUILayout.EndVertical();

            serialized.ApplyModifiedProperties();
        }
    }
#endif

}
