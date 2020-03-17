using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using UnityEngine;

namespace MainSpace.ScriptableObject
{
    [CreateAssetMenu]
    public class SoliderConfig : UnityEngine.ScriptableObject
    {

        [System.Serializable]
        public struct SoliderData
        {
            public SoliderType mType;
            public ActivityConfig activityConfig;
            public int attackValue, defenseValue, moveValue, healthValue, magicValue;
        }

        public SoliderData[] soliderDataArray;
    }

}

