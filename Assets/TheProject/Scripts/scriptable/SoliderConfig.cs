using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using UnityEngine;

namespace MainSpace.ScriptableObject
{
    [CreateAssetMenu]
    public class SoliderConfig : UnityEngine.ScriptableObject
    {
        //public bool 我在这里留白了;    // 类型将是地形类型(因为Behaviour中的英雄也需要引用)
        [System.Serializable]
        public struct SoliderData
        {
            public SoliderType soliderType;
            public ActivityConfig activityConfig;
            public int attackValue, defenseValue, moveValue, healthValue, magicValue;
        }

        public SoliderData[] soliderDataArray;
    }

}

