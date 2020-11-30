using System.Collections;
using System.Collections.Generic;
using MainSpace;
using MainSpace.Activities;
using MainSpace.ScriptableObject;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Sense.BehaviourTree.Apply
{
    public class CreateCommanderUnitNode : BehaviourNode
    {
        // 指挥官属性
        public Sprite unitFaceSprite;
        public ActivityConfig activityConfig;
        public string unitName , managerKeyName;
        //public RoleType roleType;
        [Range(1, 10)] public int levelValue = 1;
        public int levelSliderValue, levelSliderUpgradeValue, attackValue,attackRangeValue = 1,skillRangeValue = 1,skillPowerValue = 1, defenseValue, moveValue, healthValue, magicValue, commandRangeValue, correctedAttackValue, correctedDefenseValue;
        public Vector3Int showPos;

        // 携带佣兵与数量
        public Dictionary<SoliderConfig,int> troops;

        private CommanderUnit template;
        private CommanderUnit cacheCommanderUnit = null;
        private CampData campData;

        public override void ResetNode(int _depth, int _nodeNumber, BehaviourNode _parentNode)
        {
            base.ResetNode(_depth, _nodeNumber, _parentNode);
        }

        public CampData GetCampData()
        {
            return campData;
        }

        public override void Execute(bool _isLinear)
        {
            campData = LoadInfo.Instance.gameManager.GetCampData(managerKeyName);
            CommanderSpawn();
            SoliderSpawn();

            State = NodeState.Succeed;
        }

        private void CommanderSpawn()
        {
            template = Resources.Load<CommanderUnit>("Prefabs/CommanderUnitTemplate");
            CommanderUnit temp = Instantiate(template);
            temp.NodeInitData();
            // int[]
            temp.SetIntArrayData(ref temp.healthValue, healthValue);
            temp.SetIntArrayData(ref temp.magicValue, magicValue);
            temp.SetIntArrayData(ref temp.commandRangeValue, commandRangeValue);
            temp.SetIntArrayData(ref temp.correctedAttack, correctedAttackValue);
            temp.SetIntArrayData(ref temp.correctedDefense, correctedDefenseValue);
            temp.SetIntArrayData(ref temp.attackValue, attackValue);
            temp.SetIntArrayData(ref temp.attackRangeValue, attackRangeValue);
            temp.SetIntArrayData(ref temp.defenseValue, defenseValue);
            temp.SetIntArrayData(ref temp.moveRangeValue, moveValue);
            temp.SetIntArrayData(ref temp.skillRangeValue, skillRangeValue);

            // int
            temp.levelValue = levelValue;
            temp.levelSliderValue = levelSliderValue;
            temp.levelSliderUpgradeValue = levelSliderUpgradeValue;

            // string
            temp.unitName = unitName;
            temp.affiliationName = campData.campType.ToString();
            temp.managerKeyName = managerKeyName;

            // scriptable
            temp.activityConfig = activityConfig;

            // sprite
            temp.unitFaceSprite = unitFaceSprite;
            temp.affiliationSprite = campData.affiliationSprite;

            // component
            temp.PlayActivityAnim(false);
            temp.mRendererComponent.sprite = activityConfig.normalSprite;
            temp.hpText.text = healthValue.ToString();
            temp.professionSprite.sprite = campData.affiliationSprite;
            temp.role = activityConfig.roleName;

            // enum
            temp.troopsType = campData.troopType;
            temp.movingType = activityConfig.movingType;


            // pos
            Vector3Int calculateValue = LoadInfo.Instance.sceneTileMapManager.GetUnitSpacePos(showPos);
            temp.transform.position = calculateValue;
            temp.currentPos = calculateValue;

            // other
            temp.campColor = campData.campColor;
            temp.manager = LoadInfo.Instance.activitiesManager;
            temp.manager.AddActivitiesUnit(temp);


            cacheCommanderUnit = temp;
        }

        private void SoliderSpawn()
        {

        }

        public CommanderUnit GetCacheCommanderUnit()
        {
            return cacheCommanderUnit;
        }

        public override void Abort(NodeState _state)
        {
            base.Abort(_state);
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(CreateCommanderUnitNode), true)]
    public class CreateCommanderUnitNodeInspectorEditor : UnityEditor.Editor
    {
        public CreateCommanderUnitNode editorTarget;
        private void OnEnable()
        {
            editorTarget = target as CreateCommanderUnitNode;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            GUILayout.BeginVertical();
            editorTarget.unitFaceSprite = EditorGUILayout.ObjectField(new GUIContent("指挥官头像"), editorTarget.unitFaceSprite, typeof(Sprite),true) as Sprite;
            editorTarget.activityConfig = EditorGUILayout.ObjectField(new GUIContent("单位Config"), editorTarget.activityConfig, typeof(ActivityConfig),true) as ActivityConfig;
            editorTarget.unitName = EditorGUILayout.TextField("指挥官名称", editorTarget.unitName);
            editorTarget.managerKeyName = EditorGUILayout.TextField("玩家阵营", editorTarget.managerKeyName);

            GUIStyle style = new GUIStyle();
            style.fontSize = 12;
            style.normal.textColor = Color.red;

            GUILayout.Label(new GUIContent("等级数值"), style);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("经验条最大值"));
            editorTarget.levelSliderUpgradeValue = EditorGUILayout.IntField(editorTarget.levelSliderUpgradeValue);
            //editorTarget.levelSliderValue = 0;
            GUILayout.Label(new GUIContent("等级"));
            editorTarget.levelValue = EditorGUILayout.IntSlider(editorTarget.levelValue, 1, 10);
            EditorGUILayout.EndHorizontal();

            GUILayout.Label(new GUIContent("攻防移数值"), style);
            GUILayout.BeginHorizontal();
            GUILayout.Label("攻击");
            editorTarget.attackValue = EditorGUILayout.IntField( editorTarget.attackValue);
            GUILayout.Label("防御");
            editorTarget.defenseValue = EditorGUILayout.IntField(editorTarget.defenseValue);
            GUILayout.Label("移动");
            editorTarget.moveValue = EditorGUILayout.IntField( editorTarget.moveValue);
            GUILayout.EndHorizontal();

            GUILayout.Label(new GUIContent("技能相关"), style);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("魔法范围"));
            editorTarget.skillRangeValue = EditorGUILayout.IntField( editorTarget.skillRangeValue);
            GUILayout.Label(new GUIContent("魔法伤害"));
            editorTarget.skillPowerValue = EditorGUILayout.IntField( editorTarget.skillPowerValue);
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

            GUILayout.Label(new GUIContent("指挥圈数值"), style);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("指挥范围"));
            editorTarget.commandRangeValue = EditorGUILayout.IntField(editorTarget.commandRangeValue);
            GUILayout.Label(new GUIContent("攻击加成"));
            editorTarget.correctedAttackValue = EditorGUILayout.IntField(editorTarget.correctedAttackValue);
            GUILayout.Label(new GUIContent("防御加成"));
            editorTarget.correctedDefenseValue = EditorGUILayout.IntField( editorTarget.correctedDefenseValue);
            GUILayout.EndHorizontal();

            GUILayout.Label(new GUIContent("佣兵数值"), style);
            GUILayout.BeginHorizontal();

            if (editorTarget.troops == null)
            {
                editorTarget.troops = new Dictionary<SoliderConfig, int>();
            }

            List<SoliderConfig> removeArray = new List<SoliderConfig>();
            foreach (SoliderConfig v in editorTarget.troops.Keys)
            {
                if (v == null)
                {
                    //editorTarget.troops[v] = EditorGUILayout.ObjectField( editorTarget.unitFaceSprite, typeof(SoliderConfig), true) as SoliderConfig;
                    continue;
                }
                SoliderConfig key = v;
                GUILayout.BeginVertical();
                GUILayout.Label(key.roleName);
                GUILayout.Box(key.normalSprite.texture);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("-"))
                {
                    editorTarget.troops[key] -= 1;
                    if (editorTarget.troops[key] == 0)
                    {
                        removeArray.Add(key);
                    }
                }
                GUILayout.Label(editorTarget.troops[key].ToString());
                if (GUILayout.Button("+"))
                {
                    editorTarget.troops[key] += 1;
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
            for (int i = 0; i < removeArray.Count; i++)
            {
                editorTarget.troops.Remove(removeArray[i]);
            }

            if (GUILayout.Button("+"))
            {

            }


            GUILayout.EndHorizontal();

            Vector2Int tempShowPos = EditorGUILayout.Vector2IntField("出现位置", new Vector2Int(editorTarget.showPos.x, editorTarget.showPos.y));
            editorTarget.showPos = new Vector3Int(tempShowPos.x, tempShowPos.y, -1);

            GUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("编辑技能"))
            {
                SkillSelectionEditorWindows.OpenWindow();
            }
        }
    }
#endif
}
