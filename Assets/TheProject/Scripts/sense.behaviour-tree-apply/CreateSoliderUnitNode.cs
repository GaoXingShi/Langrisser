using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainSpace;
using MainSpace.Activities;
using MainSpace.ScriptableObject;
using UnityEngine;

namespace Sense.BehaviourTree.Apply
{
    public class CreateSoliderUnitNode : BehaviourNode
    {
        public SoliderConfig soliderConfig;
        // 跟随指挥官
        public CreateCommanderUnitNode followCommander;

        private SoliderUnit template;
        public override void ResetNode(int _depth, int _nodeNumber, BehaviourNode _parentNode)
        {
            base.ResetNode(_depth, _nodeNumber, _parentNode);
        }

        public override void Execute(bool _isLinear)
        {
            template = Resources.Load<SoliderUnit>("Prefabs/SoliderUnitTemplate");
            SoliderUnit temp = Instantiate(template);

            temp.NodeInitData();
            {
                temp.curProperty.healthValue = temp.originProperty.healthValue = soliderConfig.healthValue;
                temp.curProperty.magicValue = temp.originProperty.magicValue = soliderConfig.magicValue;
                //                temp.curProperty.armorValue = temp.originProperty.armorValue = soliderConfig.armorValue;
                temp.curProperty.moveRangeValue = temp.originProperty.moveRangeValue = soliderConfig.moveValue;
                temp.curProperty.attackRangeValue = temp.originProperty.attackRangeValue = soliderConfig.attackDistanceValue;
                temp.curProperty.skillRangeValue = temp.originProperty.skillRangeValue = soliderConfig.skillRangeValue;
                temp.curProperty.attackPowerValue = temp.originProperty.attackPowerValue = soliderConfig.attackValue;
                temp.curProperty.defensePowerValue = temp.originProperty.defensePowerValue = soliderConfig.defenseValue;
                temp.curProperty.skillPowerValue = temp.originProperty.skillPowerValue = soliderConfig.skillPowerValue;
            }

            SoliderConfig data = soliderConfig;

            temp.affiliationName = followCommander.GetCampData().campType.ToString();
            temp.managerKeyName = followCommander.managerKeyName;

            // sprite
            temp.affiliationSprite = followCommander.GetCampData().affiliationSprite;

            // component
            temp.PlayActivityAnim(false);
            temp.mRendererComponent.sprite = data.normalSprite;
            temp.hpText.text = data.healthValue.ToString();
            temp.professionSprite.sprite = followCommander.GetCampData().affiliationSprite;

            // enum
            temp.FightType = data.fightType;
            temp.movingType = data.movingType;
            temp.troopsType = followCommander.GetCampData().troopType;

            // pos
            Vector3Int calculateValue = LoadInfo.Instance.sceneTileMapManager.GetUnitSpacePos(followCommander.showPos);
            calculateValue.z = -1;
            temp.currentPos = calculateValue;

            temp.mineCommanderUnit = followCommander.GetCacheCommanderUnit();
            temp.mineCommanderUnit.AddSoliderUnits(temp);

            temp.campColor = followCommander.GetCampData().campColor;
            temp.manager = LoadInfo.Instance.activitiesManager;
            temp.manager.AddActivitiesUnit(temp);

            State = NodeState.Succeed;
        }

        public override void Abort(NodeState _state)
        {
            base.Abort(_state);
        }
    }
}
