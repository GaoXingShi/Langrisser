using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainSpace.Activities;
using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

#endif

namespace MainSpace.ScriptableObject
{
    public enum TerrainType
    {
        荒地,
        草地,
        森林,
        山地,
        深山,
        水域,
        宫殿,
        桥梁,

    }

    [System.Serializable]
    public struct TerrainCalculateData
    {
        public TerrainActionType movingType;
        public int[] terrainMovingValue;
    }

    [System.Serializable]
    public struct TerrainSpriteData
    {
        public TerrainType allTerrainType;
        public List<Sprite> terrainTypeSpriteList;
    }


    [CreateAssetMenu]
    public class EnvironmentConfig : UnityEngine.ScriptableObject
    {
        public TerrainCalculateData[] data;
        public TerrainSpriteData[] allTerrainType;

        public int GetConsumeValue(TerrainActionType _actionType,Sprite _sprite)
        {
            int index = (int)(allTerrainType.FirstOrDefault(x => x.terrainTypeSpriteList.Contains(_sprite)).allTerrainType);

            return data.FirstOrDefault(x => x.movingType == _actionType).terrainMovingValue[index];
        }


    }
#if UNITY_EDITOR
    [CustomEditor(typeof(EnvironmentConfig))]
    public class EnvironmentConfigEditor : Editor
    {
        private SerializedObject serialized;
        private EnvironmentConfig envirconmentConfig;
        private static int terrainActionTypeCount, terrainTypeCount;
        private static Dictionary<TerrainType, List<Sprite>> cacheDictionary;
        void OnEnable()
        {
            serialized = new SerializedObject(target);
            envirconmentConfig = target as EnvironmentConfig;
            TerrainCalculateData[] temp = new TerrainCalculateData[] { };

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
                        if (j == envirconmentConfig.data[i].terrainMovingValue.Length)
                        {
                            break;
                        }
                        envirconmentConfig.data[i].terrainMovingValue[j] = temp[i].terrainMovingValue[j];
                    }
                }

            }
             
            if (envirconmentConfig.allTerrainType == null)
            {
                envirconmentConfig.allTerrainType = new TerrainSpriteData[Enum.GetNames(typeof(TerrainType)).Length];

                for (int i = 0; i < envirconmentConfig.allTerrainType.Length; i++)
                {
                    envirconmentConfig.allTerrainType[i].allTerrainType = (TerrainType) i;
                    envirconmentConfig.allTerrainType[i].terrainTypeSpriteList = new List<Sprite>();
                }
            }


            terrainActionTypeCount = Enum.GetNames(typeof(TerrainActionType)).Length;
            terrainTypeCount = Enum.GetNames(typeof(TerrainType)).Length;

        }

        public override void OnInspectorGUI()
        {
            serialized.Update();

            if (EditorUtility.IsDirty(envirconmentConfig))
            {
                if (GUILayout.Button("Save"))
                {
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                    AssetDatabase.Refresh();
                    EditorUtility.ClearDirty(envirconmentConfig);
                }

            }

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
                    int cacheValue = EditorGUILayout.IntField(envirconmentConfig.data[i].terrainMovingValue[j]);
                    if (envirconmentConfig.data[i].terrainMovingValue[j] != cacheValue)
                    {
                        EditorUtility.SetDirty(envirconmentConfig);
                        envirconmentConfig.data[i].terrainMovingValue[j] = cacheValue;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }


            foreach (var v in envirconmentConfig.allTerrainType)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent(v.allTerrainType.ToString()));
                if (GUILayout.Button("+"))
                {
                    EditorUtility.SetDirty(envirconmentConfig);
                    v.terrainTypeSpriteList.Add(null);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical();
                for (int i = 0; i < v.terrainTypeSpriteList.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    v.terrainTypeSpriteList[i] = EditorGUILayout.ObjectField(v.terrainTypeSpriteList[i], typeof(Sprite), true) as Sprite;

                    if (GUILayout.Button("-"))
                    {
                        EditorUtility.SetDirty(envirconmentConfig);
                        v.terrainTypeSpriteList.Remove(v.terrainTypeSpriteList[i]);
                    }

                    EditorGUILayout.EndHorizontal();

                }

                EditorGUILayout.EndVertical();


            }




            EditorGUILayout.EndVertical();

            serialized.ApplyModifiedProperties();
        }
    }
#endif

}
