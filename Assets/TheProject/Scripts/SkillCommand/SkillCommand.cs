using System;
using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using MainSpace.Grid;
using UnityEngine;

namespace MainSpace.SkillCommandSpace
{
    public class SkillBaseCommand
    {
        public virtual void StartCommand(ActivitiesUnit _unit, GameCursor _cursor, SceneTileMapManager _tileMapManager)
        { }

    }

    public class SkillRangeCommand : SkillBaseCommand
    {
        protected int skillIncrement;

        protected ActivitiesUnit cacheUnit;
        protected GameCursor cursor;
        protected SceneTileMapManager tileMapManager;
        protected Vector3Int skillPos;

        protected virtual void InitValue()
        {
            skillIncrement = 0;

        }

        // 第一步 技能施法范围显示
        public override void StartCommand(ActivitiesUnit _unit, GameCursor _cursor, SceneTileMapManager _tileMapManager)
        {
            cacheUnit = _unit;
            cursor = _cursor;
            tileMapManager = _tileMapManager;

            TileSaveData[] tileData = _tileMapManager.GetRoundTileSaveData(_unit.currentPos, _unit.skillRangeValue[0] + skillIncrement);
            _tileMapManager.ShowCustomActionGrid(tileData);
            _cursor.AddStepEvent(_unit, tileData, ActionScopeType.AllUnit, null, SkillTriggerClick, () =>
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
                cursor.AddStepEvent(cacheUnit, tileData, ActionScopeType.AllUnit, null, SkillTriggerSureClick, () =>
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
                cursor.FinishStepEvent(false);
            }
        }
    }

    public class FireSkillCommand : SkillRangeCommand
    {
    }
}
