using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using UnityEngine;


namespace MainSpace.Activities
{
    public class CommanderUnit : ActivitiesUnit
    {
        [Header("Wait Init Command Data")] public bool commandData;
        public int[] commandRangeValue, correctedAttack, correctedDefense;
        public int levelValue, levelSliderValue, levelSliderUpgradeValue;
        public Sprite unitFaceSprite;
        public string unitName;
        public string role;
        private List<SoliderUnit> mSoliderUnits = new List<SoliderUnit>();

        public override void NodeInitData()
        {
            base.NodeInitData();
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
