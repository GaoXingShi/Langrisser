using System;
using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using MainSpace.Grid;
using UnityEngine;

namespace MainSpace.SkillCommandSpace
{
    // 或者像这么玩呢 ？
    public interface FightCommand
    {
        public void FightStartCommand(ActivitiesUnit _enemy, ActivitiesUnit _self);

        public void FightFinishCommand(ActivitiesUnit _enemy, ActivitiesUnit _self);
    }

    public class SkillBaseCommand
    {
        /// <summary>
        /// 主动使用这个技能
        /// </summary>
        /// <param name="_unit"></param>
        /// <param name="_commandEventQueue"></param>
        /// <param name="_tileMapManager"></param>
        public virtual void StartCommand(ActivitiesUnit _unit, CommandEventQueue _commandEventQueue, SceneTileMapManager _tileMapManager)
        { }
        // todo 还有像破甲或者燃烧这类持续数回合的被动效果

        /// <summary>
        /// 战斗开始时触发技能
        /// </summary>
        /// <param name="_enemy"></param>
        /// <param name="_self"></param>
        public virtual void FightStartCommand(ActivitiesUnit _enemy, ActivitiesUnit _self)
        {
        }

        /// <summary>
        /// 战斗结束时的回收
        /// </summary>
        /// <param name="_enemy"></param>
        /// <param name="_self"></param>
        public virtual void FightFinishCommand(ActivitiesUnit _enemy, ActivitiesUnit _self)
        {
        }

        /// <summary>
        /// 能力直接的影响
        /// </summary>
        public virtual void CapacityEffect(ActivitiesUnit _self)
        {
        }

        /// <summary>
        /// 指挥圈内的影响
        /// </summary>
        public virtual void CommandRangeEffect(ActivitiesUnit _self)
        {
        }
    }

    public class SkillRangeCommand : SkillBaseCommand
    {
        protected int skillIncrement;

        protected ActivitiesUnit cacheUnit;
        protected CommandEventQueue commandEventQueue;
        protected SceneTileMapManager tileMapManager;
        protected Vector3Int skillPos;

        protected virtual void InitValue()
        {
            skillIncrement = 0;

        }

        // 第一步 技能施法范围显示
        public override void StartCommand(ActivitiesUnit _unit, CommandEventQueue _commandEventQueue, SceneTileMapManager _tileMapManager)
        {
            cacheUnit = _unit;
            commandEventQueue = _commandEventQueue;
            tileMapManager = _tileMapManager;

            TileSaveData[] tileData = _tileMapManager.GetRoundTileSaveData(_unit.currentPos, _unit.skillRangeValue[0] + skillIncrement);
            _tileMapManager.ShowCustomActionGrid(tileData);
            _commandEventQueue.AddStepEvent(_unit, tileData, ActionScopeType.AllUnit, null, SkillTriggerClick, () =>
               {

               });
        }

        // 第二步 技能计算范围显示
        protected void SkillTriggerClick(Vector3Int _cellPos)
        {
            if (cacheUnit.currentPos.Vector3IntRangeValue(_cellPos) <= cacheUnit.moveRangeValue[0])
            {
                skillPos = _cellPos;
                TileSaveData[] tileData = tileMapManager.GetRoundTileSaveData(_cellPos, cacheUnit.skillRangeValue[0]);
                tileMapManager.ShowCustomActionGrid(tileData);
                commandEventQueue.AddStepEvent(cacheUnit, tileData, ActionScopeType.AllUnit, null, SkillTriggerSureClick, () =>
                {

                });
            }
        }

        // 第三步 确认该范围，只要点击该范围就直接开始播放动画并计算
        protected void SkillTriggerSureClick(Vector3Int _cellPos)
        {
            if (skillPos.Vector3IntRangeValue(_cellPos) <= cacheUnit.skillRangeValue[0])
            {
                // 成功了
                commandEventQueue.FinishStepEvent(false);
            }
        }
    }

    public class FireSkillCommand : SkillRangeCommand
    {
    }
}
