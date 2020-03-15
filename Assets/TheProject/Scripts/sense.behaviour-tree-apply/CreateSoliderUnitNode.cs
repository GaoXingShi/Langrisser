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
        public SoliderUnit template;
        public SoliderType soliderType;
        public SoliderConfig soliderConfig;
        public CreateCommanderUnitNode followCommander;
        // 王国 位置 跟随指挥官

        public override void ResetNode(int _depth, int _nodeNumber, BehaviourNode _parentNode)
        {
            base.ResetNode(_depth, _nodeNumber, _parentNode);
        }

        public override void Execute(bool _isLinear)
        {
            SoliderUnit temp = Instantiate(template);

            temp.InitData();

            SoliderConfig.SoliderData data = soliderConfig.soliderDataArray.FirstOrDefault(x => x.mType == soliderType);
            
            // int[]
            temp.SetIntArrayData(ref temp.healthValue, data.healthValue);
            temp.SetIntArrayData(ref temp.magicValue, data.magicValue);
            temp.SetIntArrayData(ref temp.attackValue, data.attackValue);
            temp.SetIntArrayData(ref temp.defenseValue, data.defenseValue);
            temp.SetIntArrayData(ref temp.moveValue, data.moveValue);

            temp.affiliationName = followCommander.GetCampData().campType.ToString();
            temp.managerKeyName = followCommander.managerKeyName;

            // sprite
            temp.unitRenderSprite = data.mRenderSprite;
            temp.affiliationSprite = followCommander.GetCampData().affiliationSprite;

            // component
            temp.mRendererComponent.sprite = data.mRenderSprite;
            temp.hpText.text = data.healthValue.ToString();
            temp.professionSprite.sprite = followCommander.GetCampData().affiliationSprite;

            // enum
            temp.soliderType = soliderType;
            // pos
            Vector3Int calculateValue = LoadInfo.Instance.sceneTileMapManager.GetUnitSpacePos(followCommander.showPos);
            calculateValue.z = -1;
            temp.transform.position = calculateValue;
            temp.currentPos = calculateValue;

            temp.mineCommanderUnit = followCommander.GetCacheCommanderUnit();
            temp.mineCommanderUnit.AddSoliderUnits(temp);

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
