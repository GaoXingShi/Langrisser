using System;
using System.Collections.Generic;
using MainSpace.Activities;
using UnityEngine;

namespace MainSpace
{
    public enum ActionScopeType
    {
        None,                           // 哪都不能点
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
        MeAndEnemy,                     // 仅我和敌方(待机与攻击)
        AllActivities,                  // 所有单位
        NoActivitiesUnit,               // 无单位方格
        AllUnit,                        // 所有方格
    }

    /// <summary>
    /// 数值结算系统
    /// </summary>
    public class NumericalAndSettlementSystem
    {
        public struct RefrainStruct
        {
            public FightType fightType1, fightType2;
            public int attackValue, defenseValue;
        }

        public List<RefrainStruct> refrainArray;
        public NumericalAndSettlementSystem()
        {
            refrainArray = new List<RefrainStruct>();
            AddElement(FightType.步兵, FightType.枪兵, 5, 1);
            AddElement(FightType.枪兵, FightType.骑兵, 8, 8);
            AddElement(FightType.骑兵, FightType.步兵, 3, 5);
            AddElement(FightType.弓兵, FightType.飞兵, 8, 0);
        }

        /// <summary>
        /// fightType1 克制 fightType2
        /// </summary>
        /// <param name="_fightType1"></param>
        /// <param name="_fightType2"></param>
        /// <param name="_attackValue"></param>
        /// <param name="_defenseValue"></param>
        private void AddElement(FightType _fightType1, FightType _fightType2,int _attackValue,int _defenseValue)
        {
            RefrainStruct temp = new RefrainStruct();
            temp.fightType1 = _fightType1;
            temp.fightType2 = _fightType2;
            temp.attackValue = _attackValue;
            temp.defenseValue = _defenseValue;

            refrainArray.Add(temp);
        }

        public void AttackFighting(ActivitiesUnit _initiativeUnit, ActivitiesUnit _passivityUnit, Action _callBack)
        {
            // 主动、被动、回调
           // Vector2Int AtDp = Vector2Int.zero;
            int refrainValue = RefrainValue(_initiativeUnit, _passivityUnit, out Vector2Int AtDp);

            int player1AT = _initiativeUnit.attackValue[0];
            int player2AT = _passivityUnit.attackValue[0];
            int player1DF = _initiativeUnit.defenseValue[0];
            int player2DF = _passivityUnit.defenseValue[0];

            if (refrainValue == 1)
            {
                player1AT += AtDp.x;
                player1DF += AtDp.y;
            }
            else if (refrainValue == 2)
            {
                player2AT += AtDp.x;
                player2DF += AtDp.y;
            }


        }

        public void SkillFighting(ActivitiesUnit _caster, ActivitiesUnit[] _affected)
        {

        }

        /// <summary>
        /// 克制数值增长
        /// </summary>
        /// <param name="_unit1">左Unit</param>
        /// <param name="_unit2">右Unit</param>
        /// <param name="_value1">具体攻防</param>
        /// <returns>
        /// 0：不克制
        /// 1：左克右
        /// 2：右克左
        /// </returns>
        private int RefrainValue(ActivitiesUnit _unit1, ActivitiesUnit _unit2, out Vector2Int _value1)
        {
            FightType fightType1 = _unit1.activityConfig.fightType;
            FightType fightType2 = _unit2.activityConfig.fightType;
            _value1 = Vector2Int.zero;

            if (fightType1 == fightType2)
            {
                return 0;
            }

            for (int i = 0; i < refrainArray.Count; i++)
            {
                RefrainStruct temp = refrainArray[i];
                if (temp.fightType1 == fightType1 && temp.fightType2 == fightType2)
                {
                    _value1 = new Vector2Int(temp.attackValue, temp.defenseValue);
                    return 1;
                }
                else if (temp.fightType2 == fightType1 && temp.fightType1 == fightType2)
                {
                    _value1 = new Vector2Int(temp.attackValue, temp.defenseValue);
                    return 2;
                }
            }

            return 0;

        }
        
    }
}
