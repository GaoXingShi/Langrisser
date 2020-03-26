using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainSpace.Activities;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.SceneManagement;
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
        墙壁,
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

        public int GetConsumeValue(TerrainActionType _actionType, Sprite _sprite)
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

        private int scrollValue;
        private AnimBool[] animBoolArray;

        void OnEnable()
        {
            serialized = new SerializedObject(target);
            envirconmentConfig = target as EnvironmentConfig;
            TerrainCalculateData[] temp = new TerrainCalculateData[] { };

            // 动画初始化
            animBoolArray = new AnimBool[Enum.GetNames(typeof(TerrainType)).Length];
            for (int i = 0; i < animBoolArray.Length; i++)
            {
                animBoolArray[i] = new AnimBool(false);
                // 注册动画监听
                animBoolArray[i].valueChanged.AddListener(this.Repaint);
            }


            if (envirconmentConfig.data.Length != Enum.GetNames(typeof(TerrainActionType)).Length || envirconmentConfig.allTerrainType.Length != Enum.GetNames(typeof(TerrainType)).Length)
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
                    envirconmentConfig.allTerrainType[i].allTerrainType = (TerrainType)i;
                    envirconmentConfig.allTerrainType[i].terrainTypeSpriteList = new List<Sprite>();
                }
            }


        }


        public override void OnInspectorGUI()
        {
            serialized.Update();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10);

            {
                int length = (scrollValue * 5 + 5) >= Enum.GetNames(typeof(TerrainType)).Length ? Enum.GetNames(typeof(TerrainType)).Length : (scrollValue * 5 + 5);
                for (int i = scrollValue * 5; i < length; i++)
                {
                    GUILayout.Label(((TerrainType)i).ToString());
                }
            }

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginVertical();

            for (int i = 0; i < Enum.GetNames(typeof(TerrainActionType)).Length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(((TerrainActionType)i).ToString());
                int length = (scrollValue * 5 + 5) >= Enum.GetNames(typeof(TerrainType)).Length ? Enum.GetNames(typeof(TerrainType)).Length : (scrollValue * 5 + 5);
                for (int j = scrollValue * 5; j < length; j++)
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

            {
                int count = Enum.GetNames(typeof(TerrainType)).Length / 5 +
                            (Enum.GetNames(typeof(TerrainType)).Length % 5 != 0 ? 1 : 0);
                scrollValue = Convert.ToInt32(GUILayout.HorizontalScrollbar(scrollValue, 1, 0,
                    count));
            }

            //animBool.target = EditorGUILayout.Foldout(animBool.target, "BeginFadeGroup", true);
            //// 系统使用tween渐变faded数值
            //if (EditorGUILayout.BeginFadeGroup(animBool.faded))
            //{
            //    EditorGUILayout.BoundsField("BoundsField", new Bounds());
            //    EditorGUILayout.BoundsIntField("BoundsIntField", new BoundsInt());
            //}
            //// begin - end 之间元素会进行动画
            //EditorGUILayout.EndFadeGroup();

            {
                int index = 0;
                foreach (var v in envirconmentConfig.allTerrainType)
                {
                    EditorGUILayout.BeginHorizontal();

                    animBoolArray[index].target = EditorGUILayout.Foldout(animBoolArray[index].target, v.allTerrainType.ToString(), true);

                    //GUILayout.Label(new GUIContent(v.allTerrainType.ToString()));

                    if (GUILayout.Button("+"))
                    {
                        EditorUtility.SetDirty(envirconmentConfig);
                        v.terrainTypeSpriteList.Add(null);
                        animBoolArray[index].target = true;
                    }

                    EditorGUILayout.EndHorizontal();

                    if (EditorGUILayout.BeginFadeGroup(animBoolArray[index].faded))
                    {
                        EditorGUILayout.BeginVertical();
                        for (int i = 0; i < v.terrainTypeSpriteList.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            v.terrainTypeSpriteList[i] =
                                EditorGUILayout.ObjectField(v.terrainTypeSpriteList[i], typeof(Sprite), true) as Sprite;

                            if (GUILayout.Button("-"))
                            {
                                EditorUtility.SetDirty(envirconmentConfig);
                                v.terrainTypeSpriteList.Remove(v.terrainTypeSpriteList[i]);
                            }

                            EditorGUILayout.EndHorizontal();

                        }

                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndFadeGroup();
                    index++;
                }
            }


            if (EditorUtility.IsDirty(envirconmentConfig))
            {
                if (GUILayout.Button("Save"))
                {
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                    AssetDatabase.Refresh();
                    EditorUtility.ClearDirty(envirconmentConfig);
                }

            }

            EditorGUILayout.EndVertical();

            serialized.ApplyModifiedProperties();
        }
    }
#endif

}
