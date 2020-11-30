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

            SoliderConfig data = soliderConfig;
            
            // int[]
            temp.SetIntArrayData(ref temp.healthValue, data.healthValue);
            temp.SetIntArrayData(ref temp.magicValue, data.magicValue);
            temp.SetIntArrayData(ref temp.attackValue, data.attackValue);
            temp.SetIntArrayData(ref temp.attackRangeValue, data.attackDistanceValue);
            temp.SetIntArrayData(ref temp.defenseValue, data.defenseValue);
            temp.SetIntArrayData(ref temp.moveRangeValue, data.moveValue);
            temp.SetIntArrayData(ref temp.skillRangeValue,data.skillRangeValue);

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
            temp.transform.position = calculateValue;
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
