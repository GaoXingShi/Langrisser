using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using UnityEngine;


namespace MainSpace.Activities
{
    public class CommanderUnit : ActivitiesUnit
    {
        public int[] commandRangeValue, correctedAttack, correctedDefense;
        public int levelValue, levelSliderValue, levelSliderUpgradeValue;
        public Sprite unitFaceSprite;
        public string unitName;
        public RoleType roleTpe;
        private List<SoliderUnit> mSoliderUnits = new List<SoliderUnit>();


        public override void InitData()
        {
            base.InitData();
            commandRangeValue = correctedAttack = correctedDefense = new int[2];
        }

        public void AddSoliderUnits(SoliderUnit _unit)
        {
            mSoliderUnits.Add(_unit);
        }

        public SoliderUnit[] GetSoliderUnitArray()
        {
            return mSoliderUnits.ToArray();
        }
    }
}
