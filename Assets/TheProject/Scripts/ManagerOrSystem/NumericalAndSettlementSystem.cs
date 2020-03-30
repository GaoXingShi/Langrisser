using System;
using MainSpace.Activities;
using UnityEngine;

namespace MainSpace
{
    /// <summary>
    /// 数值结算系统
    /// </summary>
    public class NumericalAndSettlementSystem : MonoBehaviour
    {

        void Start()
        {

        }

        public void AttackFighting(ActivitiesUnit _leftUnit,ActivitiesUnit _rightUnit,Action _callBack)
        {
            Debug.Log("left Attack firm to 30 damage");
        }
    }
}
