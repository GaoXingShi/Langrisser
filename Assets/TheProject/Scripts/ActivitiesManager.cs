using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainSpace.Activities;
using MainSpace.Grid;
using UnityEngine;

namespace MainSpace
{
    /// <summary>
    /// 所有可行动单位的信息类
    /// </summary>
    public class ActivitiesManager : MonoBehaviour
    {
        public readonly List<ActivitiesUnit> UnitList = new List<ActivitiesUnit>();

        private ActivitiesUnit currentSelectionUnit;
        private CommanderUnit cacheRangeUnit;
        private SceneTileMapManager tileMapManager;
        private SceneWindowsCanvas sceneWindowsCanvas;
        private void Start()
        {
            tileMapManager = LoadInfo.Instance.sceneTileMapManager;
            sceneWindowsCanvas = LoadInfo.Instance.sceneWindowsCanvas;
        }
        /// <summary>
        /// 选中了可行动单位.
        /// </summary>
        /// <param name="_unit"></param>
        public void SelectionUnit(ActivitiesUnit _unit)
        {
            if (currentSelectionUnit == null || _unit != currentSelectionUnit)
            {
                tileMapManager.HideCanMoveCorrelationGrid();

                tileMapManager.CalculateMovingRange(_unit);
                tileMapManager.ShowCanMoveCorrelationGrid();
                currentSelectionUnit = _unit;

                if (currentSelectionUnit.GetType() == typeof(CommanderUnit))
                {
                    sceneWindowsCanvas.SetActivitiesData(currentSelectionUnit as CommanderUnit);
                }
                else
                {
                    sceneWindowsCanvas.SetActivitiesData(currentSelectionUnit as SoliderUnit);
                }
            }
            else if (_unit == currentSelectionUnit)
            {
                // 点击了原点这个情况
                var temp = currentSelectionUnit.currentPos;
                temp.z = -1;

                //OnFinishedUnitMove(currentSelectionUnit);
                ClickTilePos(temp);

            }
        }

        public void EnterCommanderOrSoliderUnit(CommanderUnit _unit)
        {
            tileMapManager.ColorValueChange(true);

            if (_unit != cacheRangeUnit || cacheRangeUnit == null)
            {
                tileMapManager.HideCommanderCircleGrid();
            }

            tileMapManager.ShowCommanderCircleGrid(_unit.currentPos, _unit.commandRangeValue[0]);

            cacheRangeUnit = _unit;
        }

        public void NoneActivitiesUnit()
        {
            tileMapManager.HideCommanderCircleGrid();
            if (currentSelectionUnit != null)
            {
                if (currentSelectionUnit.GetType() == typeof(CommanderUnit))
                {
                    EnterCommanderOrSoliderUnit(currentSelectionUnit as CommanderUnit);
                }
                else if(currentSelectionUnit.GetType() == typeof(SoliderUnit))
                {
                    EnterCommanderOrSoliderUnit((currentSelectionUnit as SoliderUnit)?.mineCommanderUnit);
                }
            }
            else
            {
                tileMapManager.ColorValueChange(false);
            }
        }

        /// <summary>
        /// 点击了该瓦片.
        /// </summary>
        /// <param name="_cellPos"></param>
        public void ClickTilePos(Vector3Int _cellPos)
        {
            if (currentSelectionUnit != null)
            {
                if (tileMapManager.GetMoveToUnitAllow(_cellPos))
                {
                    if (!currentSelectionUnit.isActionOver)
                    {
                        _cellPos.z = -1;
                        currentSelectionUnit.MoveTo(_cellPos);
                    }
                    else
                    {
                        // 相当于移动完了的情况。
                        OnFinishedUnitMove(currentSelectionUnit);
                    }

                }
                else
                {
                    OnFinishedUnitMove(currentSelectionUnit);
                }

                currentSelectionUnit = null;
            }
        }

        /// <summary>
        /// 取消可移动单位的移动操作。
        /// </summary>
        public void CancelTileSelection()
        {
            if (currentSelectionUnit != null)
                OnFinishedUnitMove(currentSelectionUnit);

            if (currentSelectionUnit)
            {
                currentSelectionUnit = null;
                tileMapManager.HideCanMoveCorrelationGrid();
            }

        }

        public void OnFinishedUnitMove(ActivitiesUnit _unit)
        {
            // ui 变化
            if (currentSelectionUnit == null || currentSelectionUnit == _unit)
                sceneWindowsCanvas.ClearUnitData();

            tileMapManager.HideCanMoveCorrelationGrid();
            tileMapManager.HideCommanderCircleGrid();
        }

        /// <summary>
        /// 输入中心点以及范围，返回方格内，活动单元的信息。
        /// </summary>
        /// <param name="_centerPos"></param>
        /// <param name="_range"></param>
        /// <returns></returns>
        public ActivitiesUnit[] GetActivitiesUnit(Vector3Int _centerPos, int _range)
        {
            return UnitList.Where(x => _centerPos.Vector3IntRangeValue(x.currentPos) <= _range).ToArray();
        }

        /// <summary>
        /// 添加可行动单位
        /// </summary>
        /// <param name="_unit"></param>
        public void AddActivitiesUnit(ActivitiesUnit _unit)
        {
            if (!UnitList.Contains(_unit))
            {
                UnitList.Add(_unit);
                _unit.transform.SetParent(transform);
            }
            else
            {
                UnitList[UnitList.IndexOf(_unit)] = _unit;
            }
        }


        /// <summary>
        /// 输入位置，返回该位置是否有士兵.
        /// </summary>
        /// <param name="_pos"></param>
        /// <returns></returns>
        public bool GetUnitPosContains(Vector3Int _pos)
        {
            return UnitList.Any(x => x.currentPos.Vector3IntRangeValue(_pos) == 0);
        }
    }

    public static class Vector3IntExtends
    {
        /// <summary>
        /// 参数a到参数b之间的距离，int值。
        /// </summary>
        /// <param name="_a"></param>
        /// <param name="_b"></param>
        /// <returns></returns>
        public static int Vector3IntRangeValue(this Vector3Int _a, Vector3Int _b)
        {
            int x = Mathf.Abs(_a.x - _b.x);
            int y = Mathf.Abs(_a.y - _b.y);
            return x + y;
        }
    }
}
