using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainSpace.Activities
{
    public class SoliderUnit : ActivitiesUnit
    {
        public FightType FightType { set; get; }

        public CommanderUnit mineCommanderUnit { set; get; }

        public bool isInMineCommanderRange =>
            currentPos.Vector3IntRangeValue(mineCommanderUnit.currentPos) <=
            mineCommanderUnit.curProperty.commandRangeValue;
    }
}

