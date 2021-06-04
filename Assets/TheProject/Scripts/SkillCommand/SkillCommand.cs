using System;
using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using MainSpace.Grid;
using UnityEngine;

namespace MainSpace.SkillCommandSpace
{
    public interface FightSkill
    {
        // todo 给予敌方破甲持续两回合的样子
        /// <summary>
        /// 战斗开始时触发技能
        /// </summary>
        /// <param name="_enemy"></param>
        /// <param name="_self"></param>
        void FightStartCommand(ActivitiesUnit _enemy, ActivitiesUnit _self);

        /// <summary>
        /// 战斗结束时触发技能
        /// </summary>
        /// <param name="_enemy"></param>
        /// <param name="_self"></param>
        void FightFinishCommand(ActivitiesUnit _enemy, ActivitiesUnit _self);
    }

    public interface CommanderRangeSkill
    {
        // todo 配合己方回合开始时，指挥圈内敌方受到5点伤害
        /// <summary>
        /// 指挥圈内的影响
        /// </summary>
        void InCommandRangeEffect(ActivitiesUnit _self);

        /// <summary>
        /// 指挥圈外的影响
        /// </summary>
        /// <param name="_self"></param>
        void OutCommandRangeEffect(ActivitiesUnit _self);
    }

    public interface ActivityUnitTurnState
    {
        // todo 己方回合开始时，攻击力增加x点
        /// <summary>
        /// 己方回合开始时
        /// </summary>
        void SelfTurnBegin();

        /// <summary>
        /// 己方回合结束时
        /// </summary>
        void SelfTurnFinish();

        void EnemyTurnBegin();
        void EnemyTurnFinish();

    }

    public interface ActivityUnitWholeState
    {
        // todo 比如拥有这个技能就拥有两格攻击范围
        /// <summary>
        /// 全程状态
        /// </summary>
        /// <param name="_selfUnit"></param>
        void WholeState(ActivitiesUnit _selfUnit);
    }

    public interface CastingSkill
    {
        /// <summary>
        /// 施放技能（主动）
        /// </summary>
        void CastingSkill(ActivitiesUnit _selfUnit);

    }

    public class SkillBaseCommand
    {}

    /// <summary>
    /// 选择自己的施法范围+X,之后出现施法范围并施展
    /// </summary>
    public class SkillRangeCommand : SkillBaseCommand, CastingSkill
    {
        protected int skillIncrement1, skillIncrement2;

        protected ActivitiesUnit cacheSelfUnit;
        protected CommandEventQueue commandEventQueue;
        protected SceneTileMapManager tileMapManager;
        protected Vector3Int skillPos;


        public void CastingSkill(ActivitiesUnit _selfUnit)
        {
            cacheSelfUnit = _selfUnit;
            commandEventQueue = LoadInfo.Instance.commandEventQueue;
            tileMapManager = LoadInfo.Instance.sceneTileMapManager;

            InitValue();
            StartCommand();
        }

        /// <summary>
        /// 初始化数值
        /// </summary>
        protected virtual void InitValue()
        {
            // 技能第一二阶段增量
            skillIncrement1 = 0;
            skillIncrement2 = 0;
        }

        protected virtual void PlayMoving()
        {

        }

        // 第一步 技能施法范围显示
        protected void StartCommand()
        {
            TileSaveData[] tileData = tileMapManager.GetRoundTileSaveData(cacheSelfUnit.currentPos, cacheSelfUnit.curProperty.skillRangeValue + skillIncrement1);
            tileMapManager.ShowCustomActionGrid(tileData);
            commandEventQueue.AddStepEvent(cacheSelfUnit, tileData, ActionScopeType.AllUnit, SkillTriggerClick);
        }

        // 第二步 技能计算范围显示
        protected void SkillTriggerClick(Vector3Int _cellPos)
        {
            //if (cacheSelfUnit.currentPos.Vector3IntRangeValue(_cellPos) <= cacheSelfUnit.moveRangeValue[0])

            skillPos = _cellPos;
            TileSaveData[] tileData = tileMapManager.GetRoundTileSaveData(_cellPos, cacheSelfUnit.curProperty.skillRangeValue + skillIncrement1);
            tileMapManager.ShowCustomActionGrid(tileData);
            commandEventQueue.AddStepEvent(cacheSelfUnit, tileData, ActionScopeType.AllUnit, SkillTriggerSureClick);

        }

        // 第三步 确认该范围，只要点击该范围就直接开始播放动画并计算
        protected void SkillTriggerSureClick(Vector3Int _cellPos)
        {
            PlayMoving();
            // 成功了
            commandEventQueue.FinishStepEvent(false);
        }


    }

    /// <summary>
    /// 选择自己的施法范围+X,选择某个单位后，圈中所在部队，对单位所在部分进行施展
    /// </summary>
    public class SkillSelectionCommand : SkillBaseCommand, CastingSkill
    {
        public void CastingSkill(ActivitiesUnit _selfUnit)
        {

        }
    }

    /// <summary>
    /// 焰火冲击术
    /// </summary>
    public class FireBall1SkillCommand : SkillRangeCommand
    {
        protected override void InitValue()
        {
            // 技能第一二阶段增量
            skillIncrement1 = 0;
            skillIncrement2 = -cacheSelfUnit.curProperty.skillRangeValue + 1;
        }

        protected override void PlayMoving()
        {
            base.PlayMoving();
        }
    }
}
