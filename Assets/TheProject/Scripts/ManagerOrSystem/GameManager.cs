#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using MainSpace.Activities;
using MainSpace.Grid;
using UnityEngine;
using UnityEngine.UI;

namespace MainSpace
{
    public enum CampType
    {
        雷德卡斯王国,
        商会,
        教廷,
        巴尔迪亚帝国,
        黑风寨
    }
    public enum CtrlType
    {
        Player,
        AI
    }

    public enum TroopsType
    {
        队伍1 = 1,
        队伍2,
        队伍3,
        队伍4,
        队伍5,
        队伍6,
    }

    [System.Serializable]
    public struct CampData
    {
        public string identifyValue;
        public Sprite affiliationSprite;
        public CampType campType;
        public Color campColor;
        public int campColorIndex;
        public TroopsType troopType;
        public CtrlType ctrlType;
    }

    /// <summary>
    /// 游戏信息类 目前掌管回合系统与人物阵营信息
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public int roundValue = 1;
        public CampData[] campData;
        public float lerpValue { get; set; }
        // 人物动画切换速度、指挥权速度
        public int lerpIntValue { get; set; }
        // 人物左下角的阵营小星
        public Vector3 lerpVector3Value => new Vector3(lerpValue, lerpValue, lerpValue);
        private int currentRoundCampDataIndex;
        private AISystem aiSystem;
        private ActivitiesManager activitiesManager;
        private SceneTileMapManager sceneTileMapManager;

        private RectTransform backageImage;
        private CanvasGroup group;
        private Text turnText, playerText;
        private Image turnImage;
        private Sequence doTweenSequence, lerpSequence, lerpIndexSequence;

        private void Start()
        {
            DOTween.Init(true, false, LogBehaviour.ErrorsOnly);
            aiSystem = LoadInfo.Instance.aiSystem;
            activitiesManager = LoadInfo.Instance.activitiesManager;
            sceneTileMapManager = LoadInfo.Instance.sceneTileMapManager;

            backageImage = transform.Find("TurnCanvas/BackAgeImage").GetComponent<RectTransform>();
            group = transform.Find("TurnCanvas/group").GetComponent<CanvasGroup>();
            turnText = group.transform.Find("TurnText").GetComponent<Text>();
            playerText = group.transform.Find("PlayerText").GetComponent<Text>();
            turnImage = group.transform.Find("TurnImage").GetComponent<Image>();

            roundValue = 1;
            currentRoundCampDataIndex = 0;

            lerpValue = 0;
            lerpSequence = DOTween.Sequence();
            lerpSequence.AppendInterval(.25f);
            lerpSequence.Append(DOTween.To(() => lerpValue, x => lerpValue = x, 1, 1));
            lerpSequence.AppendInterval(.25f);
            lerpSequence.SetLoops(-1, LoopType.Yoyo);

            lerpIndexSequence = DOTween.Sequence();
            lerpIndexSequence.Append(DOTween.To(() => lerpIntValue, x => lerpIntValue = x, 100, .75f));
            lerpIndexSequence.SetLoops(-1, LoopType.Yoyo);


            Invoke(nameof(PlayGame), 0.3f);
        }
        
        public void PlayGame()
        {
            LoadInfo.Instance.gameCursor.isExecute = false;
            PlayMovie(roundValue, campData[currentRoundCampDataIndex].campType, campData[currentRoundCampDataIndex].campColor,
                campData[currentRoundCampDataIndex].affiliationSprite, false);
        }

        public void FinishCurrentRoundTurn()
        {
            // 所有的士兵颜色恢复
            activitiesManager.AllUnitColorChange(false);

            // 处理当前
            if (campData[currentRoundCampDataIndex].ctrlType == CtrlType.Player)
            {
                LoadInfo.Instance.gameCursor.isExecute = false;
            }

            if (currentRoundCampDataIndex + 1 == campData.Length)
            {
                currentRoundCampDataIndex = 0;
                roundValue++;
            }
            else
            {
                currentRoundCampDataIndex++;
            }

            // 播放动画
            PlayMovie(roundValue, campData[currentRoundCampDataIndex].campType, campData[currentRoundCampDataIndex].campColor,
                campData[currentRoundCampDataIndex].affiliationSprite, true);

        }

        /// <summary>
        /// 如果是该回合允许单位，则返回真
        /// </summary>
        /// <param name="_keyName"></param>
        /// <returns></returns>
        public bool GetIsLocalPlayerAround(string _keyName)
        {
            return campData[currentRoundCampDataIndex].identifyValue.Equals(_keyName);
        }

        /// <summary>
        /// 两个Unit是否是相同队伍
        /// </summary>
        /// <param name="_unit1"></param>
        /// <param name="_unit2"></param>
        /// <returns></returns>
        public bool VerifySameTroop(ActivitiesUnit _unit1, ActivitiesUnit _unit2)
        {
            return _unit1.troopsType == _unit2.troopsType;
        }

        /// <summary>
        /// 输入keyName,返回玩家游戏信息条目
        /// </summary>
        /// <param name="_keyName"></param>
        /// <returns></returns>
        public CampData GetCampData(string _keyName)
        {
            return campData.FirstOrDefault(x => x.identifyValue.Equals(_keyName));
        }
        private void StartNextRoundTurn()
        {
            if (campData[currentRoundCampDataIndex].ctrlType == CtrlType.Player)
            {
                // 手动操作
                LoadInfo.Instance.sceneWindowsCanvas.SetCanNotClickPanelState(false);

                LoadInfo.Instance.gameCursor.isExecute = true;
            }
            else if (campData[currentRoundCampDataIndex].ctrlType == CtrlType.AI)
            {
                // AI操作
                //LoadInfo.Instance.sceneWindowsCanvas.SetCanNotClickPanelState(true);

                aiSystem.SetAIData(
                    activitiesManager.GetCampCommanderArray(campData[currentRoundCampDataIndex].identifyValue));
            }
        }

        private void PlayMovie(int _turnIndex, CampType _campType, Color _textColor, Sprite _campSprite, bool _isNext)
        {
            _textColor = new Color(_textColor.r, _textColor.g, _textColor.b, 200 / 255.0f);
            turnText.text = String.Concat("Around ", _turnIndex.ToString());
            turnImage.sprite = _campSprite;
            playerText.text = _campType.ToString();
            playerText.color = _textColor;

            doTweenSequence = DOTween.Sequence();
            doTweenSequence.Append(DOTween.To(() => backageImage.anchoredPosition,
                x => backageImage.anchoredPosition = x, Vector2.zero, 0.75f));
            doTweenSequence.Append(DOTween.To(() => @group.alpha, x => @group.alpha = x, 1, 0.5f));
            doTweenSequence.AppendCallback(() =>
            {
                @group.interactable = true;
                @group.blocksRaycasts = true;
            });
            doTweenSequence.AppendInterval(2);
            doTweenSequence.Append(DOTween.To(() => backageImage.anchoredPosition,
                x => backageImage.anchoredPosition = x, new Vector2(1929.7f, 0), 0.75f));
            doTweenSequence.Join(DOTween.To(() => @group.alpha, x => @group.alpha = x, 0, 0.2f));
            doTweenSequence.AppendCallback(() =>
            {
                @group.interactable = false;
                @group.blocksRaycasts = false;
            });
            doTweenSequence.AppendInterval(0.35f);
            doTweenSequence.AppendCallback(() =>
            {
                backageImage.anchoredPosition = new Vector2(-1929.7f, 0);
                if (_isNext)
                {
                    StartNextRoundTurn();
                }
                else
                {
                    LoadInfo.Instance.gameCursor.isExecute = true;
                }
            });

        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        // 序列化对象
        private SerializedObject serialized;
        private GameManager editorCampData;
        private CampType campType;
        private int campDataCount;
        private Color[] allColor = new Color[] { Color.blue, Color.red, Color.yellow, Color.cyan, Color.green };
        void OnEnable()
        {
            serialized = new SerializedObject(target);
            editorCampData = target as GameManager;
            if (editorCampData != null) campDataCount = editorCampData.campData.Length;
        }

        public override void OnInspectorGUI()
        {
            serialized.Update();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("阵营相关信息：");
            campDataCount = EditorGUILayout.IntField(campDataCount);
            EditorGUILayout.EndHorizontal();

            if (editorCampData.campData.Length != campDataCount)
            {
                CampData[] cacheData = editorCampData.campData;
                editorCampData.campData = new CampData[campDataCount];
                for (int i = 0; i < editorCampData.campData.Length; i++)
                {
                    if (i == cacheData.Length)
                    {
                        break;
                    }
                    editorCampData.campData[i] = cacheData[i];
                }
            }

            for (int i = 0; i < editorCampData.campData.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();


                editorCampData.campData[i].identifyValue =
                    EditorGUILayout.TextField(editorCampData.campData[i].identifyValue);

                editorCampData.campData[i].affiliationSprite =
                    EditorGUILayout.ObjectField(editorCampData.campData[i].affiliationSprite, typeof(Sprite), true) as
                        Sprite;
                editorCampData.campData[i].campType = (CampType)EditorGUILayout.EnumPopup(editorCampData.campData[i].campType);

                Texture2D tempTexture = new Texture2D(15, 15);
                Color tempColor = allColor[editorCampData.campData[i].campColorIndex];
                for (int j = 0; j < tempTexture.width; j++)
                {
                    for (int k = 0; k < tempTexture.height; k++)
                    {
                        tempTexture.SetPixel(j, k, tempColor);

                    }
                }
                tempTexture.Apply();

                if (GUILayout.Button(tempTexture))
                {
                    editorCampData.campData[i].campColorIndex =
                        editorCampData.campData[i].campColorIndex + 1 == allColor.Length
                            ? 0
                            : editorCampData.campData[i].campColorIndex + 1;
                }

                editorCampData.campData[i].campColor = tempColor;
                editorCampData.campData[i].troopType = (TroopsType)EditorGUILayout.EnumPopup(editorCampData.campData[i].troopType);
                editorCampData.campData[i].ctrlType = (CtrlType)EditorGUILayout.EnumPopup(editorCampData.campData[i].ctrlType);


                EditorGUILayout.EndHorizontal();
            }


            serialized.ApplyModifiedProperties();


        }

        private Vector3[] cubePosArray, labelPosArray;
        private int height, width;
        private void OnSceneGUI()
        {
            // 绘制文本框
            if (Application.isPlaying)
            {
                if (cubePosArray == null || labelPosArray == null)
                {
                    height = LoadInfo.Instance.sceneTileMapManager.height;
                    width = LoadInfo.Instance.sceneTileMapManager.width;
                    cubePosArray = new Vector3[height * width];
                    labelPosArray = new Vector3[cubePosArray.Length];

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            Vector3 pos = new Vector3(j + 0.5f, i + 0.5f, -1);
                            cubePosArray[i * width + j] = pos;
                            labelPosArray[i * width + j] =
                                pos - new Vector3(0.5f, -0.1f);
                        }
                    }
                }

                for (int i = 0; i < height; i++)
                {
                    Color Icolor = i % 2 == 0 ? Color.blue : Color.red;
                    Color Jcolor = i % 2 == 1 ? Color.blue : Color.red;
                    for (int j = 0; j < width; j++)
                    {
                        Handles.color = j % 2 == 0 ? Icolor : Jcolor;
                        Handles.DrawWireCube(cubePosArray[i * width + j], Vector3.one * 0.98f);
                        Handles.color = Color.white;
                        Handles.Label(labelPosArray[i * width + j], $"({i},{j})");
                    }
                }
            }
        }
    }
#endif
}