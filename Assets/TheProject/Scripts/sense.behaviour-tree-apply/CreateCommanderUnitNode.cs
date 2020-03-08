using System.Collections;
using System.Collections.Generic;
using MainSpace;
using MainSpace.Activities;
using UnityEngine;

namespace Sense.BehaviourTree.Apply
{
    public class CreateCommanderUnitNode : BehaviourNode
    {
        public CommanderUnit template;

        public Sprite unitFaceSprite, unitRenderSprite, affiliationSprite;

        public string unitName, affiliationName;
        public RoleType roleType;
        [Range(1, 10)] public int levelValue = 1;
        public int levelSliderValue, levelSliderUpgradeValue, attackValue, defenseValue, moveValue, healthValue, magicValue, commandRangeValue, correctedAttackValue, correctedDefenseValue;
        public Vector3Int showPos;
        private CommanderUnit cacheCommanderUnit = null;


        public override void ResetNode(int _depth, int _nodeNumber, BehaviourNode _parentNode)
        {
            base.ResetNode(_depth, _nodeNumber, _parentNode);
        }

        public override void Execute(bool _isLinear)
        {
            CommanderUnit temp = Instantiate(template);

            temp.InitData();

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
            temp.affiliationName = affiliationName;

            // sprite
            temp.unitFaceSprite = unitFaceSprite;
            temp.unitRenderSprite = unitRenderSprite;
            temp.affiliationSprite = affiliationSprite;

            // component
            temp.mRendererComponent.sprite = unitRenderSprite;
            temp.hpImage.text = healthValue.ToString();
            temp.professionImage.sprite = affiliationSprite;

            // enum
            temp.roleTpe = roleType;

            // pos
            Vector3Int calculateValue = LoadInfo.Instance.sceneTileMapManager.GetUnitSpacePos(showPos);
            temp.transform.position = calculateValue;
            temp.currentPos = calculateValue;

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
