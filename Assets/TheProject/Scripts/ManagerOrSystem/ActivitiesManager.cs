using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
        private readonly List<ActivitiesUnit> UnitList = new List<ActivitiesUnit>();

        private ActivitiesUnit currentSelectionUnit;
        private CommanderUnit cacheRangeUnit;
        private SceneTileMapManager tileMapManager;
        private SceneWindowsCanvas sceneWindowsCanvas;
        private GameManager gameManager;
        private Sequence dotweenSequence;
        private bool isStandByOrOtherMode = false;
        private void Start()
        {
            DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
            tileMapManager = LoadInfo.Instance.sceneTileMapManager;
            sceneWindowsCanvas = LoadInfo.Instance.sceneWindowsCanvas;
            gameManager = LoadInfo.Instance.gameManager;

        }
        #region 移动相关
        /// <summary>
        /// 选中了可行动单位.
        /// </summary>
        /// <param name="_unit"></param>
        public void SelectionUnit(ActivitiesUnit _unit)
        {
            if (isStandByOrOtherMode)
            {
                // 点击原点
                if (_unit.GetInstanceID() == currentSelectionUnit.GetInstanceID())
                {
                    UnitOnFinish(_unit);
                    currentSelectionUnit = null;
                }
                else
                {
                    // 如果单位身上标有可攻击标志，则触发攻击。
                }


                return;
            }

            if (currentSelectionUnit == null || _unit != currentSelectionUnit)
            {
                tileMapManager.HideCanMoveCorrelationGrid();

                tileMapManager.CalculateMovingRange(_unit);
                tileMapManager.ShowCanMoveCorrelationGrid(_unit, true);
                currentSelectionUnit = _unit;
                initPos = currentSelectionUnit.currentPos;

                if (currentSelectionUnit.GetType() == typeof(CommanderUnit))
                {
                    sceneWindowsCanvas.SetActivitiesData(currentSelectionUnit as CommanderUnit);
                }
                else
                {
                    sceneWindowsCanvas.SetActivitiesData(currentSelectionUnit as SoliderUnit);
                }
            }
            else if (_unit.GetInstanceID() == currentSelectionUnit.GetInstanceID())
            {
                // 点击了原点这个情况
                var temp = currentSelectionUnit.currentPos;
                temp.z = -1;
                ClickTilePos(temp);

            }


        }

        /// <summary>
        /// 点击了该瓦片.
        /// </summary>
        /// <param name="_cellPos"></param>
        public void ClickTilePos(Vector3Int _cellPos)
        {
            if (isStandByOrOtherMode)
            {
                return;
            }

            if (currentSelectionUnit != null)
            {
                Vector3Int[] allPos = tileMapManager.GetMoveToUnitAllow(_cellPos);
                if (gameManager.GetIsLocalPlayerAround(currentSelectionUnit.managerKeyName) && allPos != null && allPos.Length != 0)
                {
                    if (!currentSelectionUnit.isActionOver)
                    {
                        tileMapManager.HideCanMoveCorrelationGrid();
                        tileMapManager.ShowCurrentMovingCorrelationGrid(currentSelectionUnit, allPos);
                        UnitMoveTo(allPos.RemoveDuplicates(), currentSelectionUnit, CtrlType.Player);

                    }
                    else
                    {
                        // 相当于移动完了的情况。
                        OverCurrentMoving();
                    }
                }
                else
                {
                    // 不能允许移动到这里，并且取消本次移动。
                    // 播放禁止移动音频
                    //OverCurrentMoving();
                }
            }

        }

        /// <summary>
        /// 控制单位移动
        /// </summary>
        /// <param name="_posArray"></param>
        /// <param name="_unit"></param>
        public void UnitMoveTo(Vector3Int[] _posArray, ActivitiesUnit _unit, CtrlType _ctrlType)
        {
            //StopAllCoroutines();
            UnitMoveLerp(_posArray, _unit, _ctrlType);
        }

        /// <summary>
        /// 控制单位移动
        /// </summary>
        /// <param name="_pos"></param>
        /// <param name="_unit"></param>
        public void UnitMoveTo(Vector3Int _pos, ActivitiesUnit _unit, CtrlType _ctrlType)
        {
            Vector3Int[] temp = new Vector3Int[] { _pos };
            UnitMoveTo(temp, _unit, _ctrlType);
        }

        /// <summary>
        /// 取消可移动单位的移动操作。
        /// </summary>
        public bool CancelTileSelection()
        {
            if (isStandByOrOtherMode)
            {
                isStandByOrOtherMode = !isStandByOrOtherMode;
                tileMapManager.ShowCanMoveCorrelationGrid(currentSelectionUnit, false);
                currentSelectionUnit.currentPos = initPos;
                currentSelectionUnit.transform.position = initPos;
                // 刷新指挥圈
                tileMapManager.RefreshCommanderCircleGird(currentSelectionUnit);
                return true;
            }
            if (currentSelectionUnit)
            {
                OnFinishedUnitMove(currentSelectionUnit);
                tileMapManager.HideCommanderCircleGrid();
                currentSelectionUnit = null;
                tileMapManager.HideCanMoveCorrelationGrid();
                tileMapManager.ClearCacheSaveData();
                SetAllActivityAnim(false);
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// ActivitiesUnit完成移动时的回调
        /// </summary>
        /// <param name="_unit"></param>
        public void OnFinishedUnitMove(ActivitiesUnit _unit)
        {
            // ui 变化
            if (currentSelectionUnit == null || currentSelectionUnit == _unit)
                sceneWindowsCanvas.ClearUIInfo();

            tileMapManager.HideCanMoveCorrelationGrid();
            tileMapManager.ClearCacheSaveData();
            isStandByOrOtherMode = false;

            if (_unit.GetType() == typeof(CommanderUnit))
            {
                //tileMapManager.HideCommanderCircleGrid();

                //CommanderUnit temp = (CommanderUnit)_unit;
                tileMapManager.RefreshCommanderCircleGird(_unit);
                //tileMapManager.ShowCommanderCircleGrid(temp.currentPos, temp.commandRangeValue[0], temp.campColor);
            }
            LoadInfo.Instance.gameCursor.clickActivitiesUnit = null;
        }


        #endregion

        #region 指挥圈相关
        /// <summary>
        /// 触发指挥圈范围
        /// </summary>
        /// <param name="_unit"></param>
        public void EnterCommanderOrSoliderUnit(CommanderUnit _unit)
        {
            //if (cacheRangeUnit != null)
            //{
            //    if (_unit != cacheRangeUnit)
            //    {
            //        tileMapManager.RefreshCommanderCircleGird(_unit);
            //        SetActivityAnim(cacheRangeUnit, false);
            //    }
            //}
            //else
            //{
            //    tileMapManager.SetColorValueState(true);
            //    tileMapManager.ShowCommanderCircleGrid(_unit.currentPos, _unit.commandRangeValue[0], _unit.campColor);
            //    SetActivityAnim(_unit, true);
            //}


            // 当两次触发非同一个指挥阵营时
            if (cacheRangeUnit != null && _unit != cacheRangeUnit)
            {
                tileMapManager.HideCommanderCircleGrid();
                SetActivityAnim(cacheRangeUnit, false);
            }

            tileMapManager.SetColorValueState(true);
            tileMapManager.ShowCommanderCircleGrid(_unit.currentPos, _unit.commandRangeValue[0], _unit.campColor);
            SetActivityAnim(_unit, true);

            if (currentSelectionUnit == null)
                cacheRangeUnit = _unit;
        }

        /// <summary>
        /// 退出指挥圈范围
        /// </summary>
        public void ExitCommanderOrSoliderUnit()
        {
            tileMapManager.HideCommanderCircleGrid();

            if (currentSelectionUnit != null)
            {
                if (currentSelectionUnit.GetType() == typeof(CommanderUnit))
                {
                    EnterCommanderOrSoliderUnit(currentSelectionUnit as CommanderUnit);
                }
                else if (currentSelectionUnit.GetType() == typeof(SoliderUnit))
                {
                    EnterCommanderOrSoliderUnit((currentSelectionUnit as SoliderUnit)?.mineCommanderUnit);
                }

                SetAllActivityAnim(false);
                if (cacheRangeUnit != null)
                {
                    SetActivityAnim(cacheRangeUnit, true);
                }
            }
            else
            {
                if (cacheRangeUnit != null)
                {
                    SetActivityAnim(cacheRangeUnit, false);
                    cacheRangeUnit = null;
                }

                tileMapManager.SetColorValueState(false);
            }

        }

        #endregion

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
                _unit.ManagerInitData();
            }
            else
            {
                UnitList[UnitList.IndexOf(_unit)] = _unit;
            }
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
        /// 输入位置，返回该位置是否有士兵.
        /// </summary>
        /// <param name="_pos"></param>
        /// <returns></returns>
        public bool GetUnitPosContains(Vector3Int _pos)
        {
            return UnitList.Any(x => x.currentPos.Vector3IntRangeValue(_pos) == 0);
        }

        /// <summary>
        /// 输入位置与玩家自身Key，返回该位置是否是相同阵营士兵,相同阵营为true,不同阵营为false.
        /// </summary>
        /// <param name="_pos"></param>
        /// <param name="_mineKeyName"></param>
        /// <returns></returns>
        public bool GetUnitPosContainsOtherTroop(Vector3Int _pos, string _mineKeyName)
        {
            var unit = UnitList.FirstOrDefault(x => x.currentPos.Vector3IntRangeValue(_pos) == 0);
            var campData = gameManager.GetCampData(_mineKeyName);
            return unit.troopsType == campData.troopType;
        }

        /// <summary>
        /// 设置所有士兵的颜色以及行动
        /// </summary>
        /// <param name="_isGray"></param>
        public void AllUnitColorChange(bool _isGray)
        {
            foreach (var v in UnitList)
            {
                v.UnitColorChange(false);
                v.isActionOver = false;
            }
        }

        /// <summary>
        /// 设置所有人物动画
        /// </summary>
        /// <param name="_enabled"></param>
        public void SetAllActivityAnim(bool _enabled)
        {
            foreach (var v in UnitList.Where(x => x.isPlayingAnim))
            {
                v.PlayActivityAnim(_enabled);
            }
        }

        /// <summary>
        /// 获取该_keyName 阵营中所有的指挥官(通过指挥官可以获得所有士兵)
        /// </summary>
        /// <param name="_keyName"></param>
        /// <returns></returns>
        public CommanderUnit[] GetCampCommanderArray(string _keyName)
        {
            return UnitList.Where(x => x.managerKeyName.Equals(_keyName)
                                       && x.GetType() == typeof(CommanderUnit)).OfType<CommanderUnit>().ToArray();
        }

        private void SetActivityAnim(CommanderUnit _unit, bool _enabled)
        {
            _unit.PlayActivityAnim(_enabled);
            foreach (var v in _unit.GetSoliderUnitArray())
            {
                v.PlayActivityAnim(_enabled);
            }
        }

        private Vector3Int initPos;
        private void UnitMoveLerp(Vector3Int[] _posArray, ActivitiesUnit _unit, CtrlType _ctrlType)
        {
            if (_posArray.Length == 1 && _posArray[0].Vector3IntRangeValue(_unit.currentPos) == 0)
            {
                _unit.currentPos = _posArray[0];
                if (_ctrlType == CtrlType.Player)
                {
                    tileMapManager.ShowStandByOrOtherActionGrid(_unit);
                    isStandByOrOtherMode = true;
                }
                else if (_ctrlType == CtrlType.AI)
                {
                    UnitOnFinish(_unit);
                }
                return;
            }
            dotweenSequence = DOTween.Sequence();
            Vector3Int cacheMoving = _unit.currentPos;
            for (int i = 0; i < _posArray.Length; i++)
            {
                var forValue = _posArray[i];
                dotweenSequence.Append(DOTween.To(() => _unit.transform.position, x => _unit.transform.position = x,
                    forValue, cacheMoving.Vector3IntRangeValue(forValue) * 0.1f));
                cacheMoving = forValue;
            }

            dotweenSequence.AppendCallback(() =>
            {
                _unit.currentPos = _posArray[_posArray.Length - 1];
                tileMapManager.RefreshCommanderCircleGird(_unit);
                if (_ctrlType == CtrlType.Player)
                {
                    tileMapManager.ShowStandByOrOtherActionGrid(_unit);
                    isStandByOrOtherMode = true;
                }
                else if (_ctrlType == CtrlType.AI)
                {
                    UnitOnFinish(_unit);
                }
            });

        }

        private void UnitOnFinish(ActivitiesUnit _unit)
        {
            // 玩家才会变灰.
            _unit.UnitColorChange(LoadInfo.Instance.gameManager.GetCampData(_unit.managerKeyName).ctrlType == CtrlType.Player);
            _unit.isActionOver = true;

            // 单位完成了移动。
            if (LoadInfo.Instance.gameManager.GetCampData(_unit.managerKeyName).ctrlType == CtrlType.Player)
            {
                OnFinishedUnitMove(_unit);
            }
        }
        private void OverCurrentMoving()
        {
            sceneWindowsCanvas.ClearUIInfo();
            tileMapManager.HideCanMoveCorrelationGrid();
            tileMapManager.ClearCacheSaveData();
            tileMapManager.HideCommanderCircleGrid();
            SetAllActivityAnim(false);
            LoadInfo.Instance.gameCursor.clickActivitiesUnit = null;
            currentSelectionUnit = null;
        }
    }

    public static class Vector3IntExtends
    {
        /// <summary>
        /// 参数a到参数b之间的距离，返回int值。 只对xy值
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

        public static Vector3Int RemoveZValuie(this Vector3Int _pos, int _zValue)
        {
            return new Vector3Int(_pos.x, _pos.y, _zValue);
        }

        /// <summary>
        /// 输入一个路径组，返回去重复的路径组。
        /// </summary>
        /// <param name="_posArray"></param>
        /// <returns></returns>
        public static Vector3Int[] RemoveDuplicates(this Vector3Int[] _posArray)
        {

            List<Vector3Int> returnValue = new List<Vector3Int>();
            if (_posArray.Length > 1)
            {
                Vector3Int temp = _posArray[1] - _posArray[0];
                returnValue.Add(_posArray[0]);
                for (int i = 1; i < _posArray.Length; i++)
                {
                    if (temp == _posArray[i] - _posArray[i - 1])
                    {
                        if (i == _posArray.Length - 1)
                        {
                            returnValue.Add(_posArray[i]);
                        }
                        continue;
                    }
                    else
                    {

                        returnValue.Add(_posArray[i - 1]);
                        temp = _posArray[i] - _posArray[i - 1];

                        if (i == _posArray.Length - 1)
                        {
                            returnValue.Add(_posArray[i]);
                        }
                    }
                }
                // T0不要
                returnValue.RemoveAt(0);
                return returnValue.ToArray();
            }

            return _posArray;
        }

        public static int DistanceValue(this Vector3Int _currentPos, Vector3Int _targetPos)
        {
            return Mathf.Abs(_currentPos.x - _targetPos.x) + Mathf.Abs(_currentPos.y - _targetPos.y);
        }
    }
}
