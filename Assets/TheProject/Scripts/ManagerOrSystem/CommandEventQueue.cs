using System;
using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using MainSpace.Grid;
using UnityEngine;

namespace MainSpace
{

    public class StepInfo
    {
        public ActivitiesUnit unit;
        public Vector3Int unitCurrentPos;
        public TileSaveDataCommand[] tileCommand;
        public ActionScopeType actionScopeType;
        public Action<ActivitiesUnit> activitiesAction;
        public Action<Vector3Int> clickTilePosAction;
        public Action cancelAction;
    }

    public struct TileSaveDataCommand
    {
        public string aliasName, path; // 别名
        public bool isChange, moveEnable;
        public Vector3Int pos;
    }


    public class CommandEventQueue
    {
        private ActivitiesManager activitiesManager;
        private GameCursor cursor;

        public CommandEventQueue()
        {
            activitiesManager = LoadInfo.Instance.activitiesManager;
            cursor = LoadInfo.Instance.gameCursor;
        }

        private Stack<StepInfo> stepInfoStack = new Stack<StepInfo>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_unit">角色信息</param>
        /// <param name="_tile">瓦片存储信息,比如角色移动后又撤销了，需要把之前存储过可移动的瓦片信息再次显示出来</param>
        /// <param name="_actionScopeType">可操作类型</param>
        /// <param name="_activitiesAction"> 鼠标左键触发的事件 </param>
        /// <param name="_clickTilePosAction"> 鼠标左键触发的事件 </param>
        /// <param name="_cancelAction"> 鼠标右键触发的事件 </param>
        public void AddStepEvent(ActivitiesUnit _unit, TileSaveData[] _tile, ActionScopeType _actionScopeType, Action<ActivitiesUnit> _activitiesAction, Action<Vector3Int> _clickTilePosAction, Action _cancelAction)
        {
            bool valueNull = _tile == null;

            TileSaveDataCommand[] addStepValue = new TileSaveDataCommand[valueNull ? 1 : _tile.Length];
            if (!valueNull)
            {
                for (int i = 0; i < _tile.Length; i++)
                {
                    addStepValue[i] = new TileSaveDataCommand();
                    addStepValue[i].aliasName = _tile[i].aliasName;
                    addStepValue[i].path = _tile[i].path;
                    addStepValue[i].isChange = _tile[i].isChange;
                    addStepValue[i].moveEnable = false;
                    addStepValue[i].pos = _tile[i].widthHeighValue;
                }
            }

            // None 类型在弹出时直接跳过 因为none是角色移动过程
            StepInfo temp = new StepInfo()
            {
                unit = _unit,
                unitCurrentPos = _unit == null ? Vector3Int.one : _unit.currentPos,
                tileCommand = valueNull ? null : addStepValue,
                actionScopeType = _actionScopeType,
                activitiesAction = _activitiesAction,
                clickTilePosAction = _clickTilePosAction,
                cancelAction = _cancelAction,
            };
            stepInfoStack.Push(temp);
        }


        /// <summary>
        /// 完成该步骤，清理状态
        /// </summary>
        public void FinishStepEvent(bool _isCancel)
        {
            // 回到初始状态
            activitiesManager.OverSelection(_isCancel);
            cursor.ClearCursorState();
            stepInfoStack.Clear();
        }

        public void ExecuteStepEvent(ActivitiesUnit _unit, Vector3Int _cellPos, bool _gridPlayerLayer)
        {

            if (stepInfoStack.Count == 0)
            {
                if (_unit)
                {
                    // 告诉士兵管理系统
                    activitiesManager.SelectionUnit(_unit);
                }
                else
                {
                    // todo 如果通知的脚本过多不如弄成事件。
                    activitiesManager.ClickTilePos(_cellPos);
                }
            }
            else
            {

                StepInfo temp = stepInfoStack.Peek();
                switch (temp.actionScopeType)
                {
                    case ActionScopeType.OnlyOurSoldiers:
                    case ActionScopeType.OnlyOurCommanders:
                    case ActionScopeType.OnlyOur:
                    case ActionScopeType.OnlyFriendSoldiers:
                    case ActionScopeType.OnlyFriendCommanders:
                    case ActionScopeType.OnlyFriend:
                    case ActionScopeType.OnlyOurAndFriendSoldiers:
                    case ActionScopeType.OnlyOurAndFriendCommanders:
                    case ActionScopeType.OnlyOurAndFriend:
                    case ActionScopeType.OnlyEnemySoldiers:
                    case ActionScopeType.OnlyEnemyCommanders:
                    case ActionScopeType.OnlyEnemy:
                    case ActionScopeType.MeAndEnemy:
                    case ActionScopeType.AllActivities:
                        if (_unit != null)
                            temp.activitiesAction(_unit);
                        break;

                    case ActionScopeType.NoActivitiesUnit:
                        temp.clickTilePosAction(_cellPos);
                        break;
                    case ActionScopeType.AllUnit:
                        if (_unit != null)
                            temp.activitiesAction(_unit);
                        temp.clickTilePosAction(_cellPos);
                        break;
                }
            }

        }

        public void RemoveStepEvent()
        {
            // 如果，栈里有数据，则使用栈的取消方法。
            // 否则，则使用系统内部的完成方法。
            if (stepInfoStack.Count == 0)
            {
                // 则ui进入初始化界面
                // LoadInfo.Instance.sceneWindowsCanvas.SetUpPanel();
                // 打算做一个魔法或者召唤或者指令用的选项
            }
            else
            {
                StepInfo temp = stepInfoStack.Pop();
                temp.cancelAction();
                while (stepInfoStack.Count != 0 && stepInfoStack.Peek().actionScopeType == ActionScopeType.None)
                {
                    var whileValue = stepInfoStack.Pop();
                    whileValue.cancelAction();
                }
                CancelStepEvent();
            }
        }

        /// <summary>
        /// 取消当前步骤
        /// </summary>
        private void CancelStepEvent()
        {
            if (stepInfoStack.Count != 0)
            {
                StepInfo temp = stepInfoStack.Peek();
                // 加载栈顶的信息，回到上一步时tileMap的状态
                LoadInfo.Instance.sceneTileMapManager.LoadCorrelationGrid(temp);
                LoadInfo.Instance.sceneWindowsCanvas.RefreshActivitiesData(temp.unit, 0);
            }
            else
            {
                // 回到初始状态
                FinishStepEvent(true);
            }

        }
    }
}
