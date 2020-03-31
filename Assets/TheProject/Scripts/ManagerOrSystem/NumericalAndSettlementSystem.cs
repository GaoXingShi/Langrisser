using System;
using MainSpace.Activities;
using UnityEngine;

namespace MainSpace
{
    public enum ActionScopeType
    {
        OnlyOurSoldiers,                // 仅我方士兵
        OnlyOurCommanders,              // 仅我方指挥官
        OnlyOur,                        // 仅我方
        OnlyFriendSoldiers,             // 仅友方士兵
        OnlyFriendCommanders,           // 仅友方指挥官
        OnlyFriend,                     // 仅友方
        OnlyOurAndFriendSoldiers,       // 仅我方与友军士兵
        OnlyOurAndFriendCommanders,     // 仅我方与友军指挥官
        OnlyOurAndFriend,               // 仅我方与友军
        OnlyEnemySoldiers,              // 仅敌方士兵
        OnlyEnemyCommanders,            // 仅敌方指挥官
        OnlyEnemy,                      // 仅敌方
        AllActivities,                  // 所有单位
        NoActivitiesUnit,               // 无单位方格
        AllUnit,                        // 所有方格
    }

    public enum SkillType
    {
        Attack,             // 只能对准攻击范围内,并选择
        Range,              // 选择自己的施法范围+X,之后出现施法范围并施展
        Selection,          // 选择自己的施法范围+X,选择某个单位后，圈中所在部队，对单位所在部分进行施展
    }

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
