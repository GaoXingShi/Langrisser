using System.Collections;
using System.Collections.Generic;
using MainSpace;
using MainSpace.Activities;
using MainSpace.ScriptableObject;
using UnityEngine;

namespace Sense.BehaviourTree.Apply
{
    public class CreateCommanderUnitNode : BehaviourNode
    {
        public CommanderUnit template;

        public Sprite unitFaceSprite;
        public ActivityConfig activityConfig;
        public string unitName , managerKeyName;
        public RoleType roleType;
        [Range(1, 10)] public int levelValue = 1;
        public int levelSliderValue, levelSliderUpgradeValue, attackValue, defenseValue, moveValue, healthValue, magicValue, commandRangeValue, correctedAttackValue, correctedDefenseValue;
        public Vector3Int showPos;
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

            CommanderUnit temp = Instantiate(template);

            temp.NodeInitData();

            // int[]
            temp.SetIntArrayData(ref temp.healthValue, healthValue);
            temp.SetIntArrayData(ref temp.magicValue, magicValue);
            temp.SetIntArrayData(ref temp.commandRangeValue, commandRangeValue);
            temp.SetIntArrayData(ref temp.correctedAttack, correctedAttackValue);
            temp.SetIntArrayData(ref temp.correctedDefense, correctedDefenseValue);
            temp.SetIntArrayData(ref temp.attackValue, attackValue);
            temp.SetIntArrayData(ref temp.defenseValue, defenseValue);
            temp.SetIntArrayData(ref temp.moveValue, moveValue);

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

            // enum
            temp.roleTpe = roleType;
            temp.troopsType = campData.troopType;

            // pos
            Vector3Int calculateValue = LoadInfo.Instance.sceneTileMapManager.GetUnitSpacePos(showPos);
            temp.transform.position = calculateValue;
            temp.currentPos = calculateValue;

            // other
            temp.campColor = campData.campColor;
            temp.manager = LoadInfo.Instance.activitiesManager;
            temp.manager.AddActivitiesUnit(temp);


            cacheCommanderUnit = temp;

            State = NodeState.Succeed;
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
}
