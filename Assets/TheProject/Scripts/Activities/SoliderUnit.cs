using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainSpace.Activities
{
    public class SoliderUnit : ActivitiesUnit
    {
        public SoliderType soliderType { set; get; }

        public CommanderUnit mineCommanderUnit { set; get; }

        public bool isInMineCommanderRange =>
            currentPos.Vector3IntRangeValue(mineCommanderUnit.currentPos) <=
            mineCommanderUnit.commandRangeValue[0];
    }
}

