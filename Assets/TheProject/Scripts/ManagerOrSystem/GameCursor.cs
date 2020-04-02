﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using MainSpace.Activities;
using MainSpace.Grid;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace MainSpace
{
    public class StepInfo
    {
        public ActivitiesUnit unit;
        public Vector3Int unitCurrentPos;
        public TileSaveData[] tile;
        public ActionScopeType actionScopeType;
        public Action<ActivitiesUnit> activitiesAction;
        public Action<Vector3Int> clickTilePosAction;
        public Action cancelAction;
        public bool[] moveComponentEnable;
    }


    /// <summary>
    /// 游戏光标类 只负责光标产生的信息传递，不处理逻辑
    /// </summary>
    public class GameCursor : MonoBehaviour
    {
        public CinemachineVirtualCamera cinemachine;
        public UnityEngine.Grid grid;
        public Texture2D cursorTexture, attackTexture;
        [HideInInspector] public bool isExecute = true;
        [HideInInspector] public ActivitiesUnit clickActivitiesUnit;
        private CinemachineFramingTransposer cine;
        private ActivitiesManager activitiesManager;
        private ActivitiesUnit cacheHitRaycastUnit;
        void Start()
        {
            cine = ((CinemachineFramingTransposer)cinemachine.GetComponentPipeline()[0]);
            activitiesManager = LoadInfo.Instance.activitiesManager;
            isExecute = true;

            Cursor.SetCursor(cursorTexture, Vector2.one * 8, CursorMode.Auto);
        }

        void Update()
        {
            // 不懂为什么，将Enable关闭了之后，会出现鼠标经过两次的问题
            if (!isExecute)
            {
                return;
            }

            var worldPointV3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = grid.WorldToCell(worldPointV3);

            // 非UI层。
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                var hit2D = Physics2D.Raycast(worldPointV3, Vector2.zero, 10);
                if (hit2D.transform != null)
                {
                    bool gridPlayerLayer = hit2D.transform.gameObject.layer == LayerMask.NameToLayer("GridPlayer");
                    ActivitiesUnit unit = hit2D.transform.GetComponent<ActivitiesUnit>();
                    if (gridPlayerLayer)
                    {
                        // Touch Unit 
                        if (cacheHitRaycastUnit != null && cacheHitRaycastUnit != unit)
                        {
                            CommanderRangeUnit(null);
                        }

                        if (cacheHitRaycastUnit == null || cacheHitRaycastUnit != unit)
                        {
                            CommanderRangeUnit(unit);
                        }
                    }
                    else
                    {
                        // Exit Unit
                        if (cacheHitRaycastUnit != null)
                        {
                            CommanderRangeUnit(null);
                        }

                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        ExecuteClickStepEvent(unit, cellPos, gridPlayerLayer);
                    }

                }
                else
                {
                    // 通知现在没点到士兵
                    if (cacheHitRaycastUnit != null)
                    {
                        CommanderRangeUnit(null);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                CancelStepEvent();
            }





            //// 非UI层。
            //if (!EventSystem.current.IsPointerOverGameObject())
            //{
            //    var hit2D = Physics2D.Raycast(worldPointV3, Vector2.zero, 10);
            //    if (hit2D.transform != null)
            //    {
            //        bool gridPlayerLayer = hit2D.transform.gameObject.layer == LayerMask.NameToLayer("GridPlayer");
            //        ActivitiesUnit unit = hit2D.transform.GetComponent<ActivitiesUnit>();
            //        if (gridPlayerLayer)
            //        {
            //            if (Input.GetMouseButtonDown(0))
            //            {
            //                // 告诉士兵管理系统
            //                activitiesManager.SelectionUnit(unit);

            //                if (clickActivitiesUnit == null /*|| (clickActivitiesUnit != null && clickActivitiesUnit == unit)*/)
            //                {
            //                    clickActivitiesUnit = unit;
            //                }
            //            }
            //            else
            //            {
            //                // Touch Unit 
            //                if (cacheHitRaycastUnit != null && cacheHitRaycastUnit != unit)
            //                {
            //                    CommanderRangeUnit(null);
            //                }

            //                if (cacheHitRaycastUnit == null || cacheHitRaycastUnit != unit)
            //                {
            //                    CommanderRangeUnit(unit);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (Input.GetMouseButtonDown(0))
            //            {
            //                // todo 如果通知的脚本过多不如弄成事件。
            //                activitiesManager.ClickTilePos(cellPos);
            //            }
            //            else
            //            {
            //                // Exit Unit
            //                if (cacheHitRaycastUnit != null)
            //                {
            //                    CommanderRangeUnit(null);
            //                }
            //            }
            //        }

            //    }
            //    else
            //    {
            //        // 通知现在没点到士兵
            //        if (cacheHitRaycastUnit != null)
            //        {
            //            CommanderRangeUnit(null);
            //        }
            //    }


            //    if (Input.GetMouseButtonDown(1))
            //    {
            //        if (!activitiesManager.CancelTileSelection())
            //        {
            //            // 则ui进入初始化界面
            //            LoadInfo.Instance.sceneWindowsCanvas.SetInitPanel();
            //            //CommanderRangeUnit(null);
            //        }
            //        else
            //        {
            //            clickActivitiesUnit = null;
            //        }
            //    }
            //}
        }

        private Stack<StepInfo> stepInfoStack = new Stack<StepInfo>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_unit"></param>
        /// <param name="_unitCurrentPos"></param>
        /// <param name="_tile"></param>
        /// <param name="_actionScopeType"></param>
        /// <param name="_activitiesAction"> 鼠标左键触发的事件 </param>
        /// <param name="_clickTilePosAction"> 鼠标左键触发的事件 </param>
        /// <param name="_cancelAction"> 鼠标右键触发的事件 </param>
        public void AddStepEvent(ActivitiesUnit _unit, Vector3Int _unitCurrentPos, TileSaveData[] _tile, ActionScopeType _actionScopeType, Action<ActivitiesUnit> _activitiesAction, Action<Vector3Int> _clickTilePosAction, Action _cancelAction)
        {

            TileSaveData[] tmpTile = null;
            bool[] moveComponentValue = null;
            if (_tile != null)
            {
                tmpTile = new TileSaveData[_tile.Length];

                moveComponentValue = new bool[_tile.Length];

                Debug.Log(_tile.Any(x=>x.activitiesAllowUnit.moveSpriteRenderer.enabled));
                for (int i = 0; i < tmpTile.Length; i++)
                {
                    tmpTile[i] = new TileSaveData();
                    tmpTile[i].aliasName = _tile[i].aliasName;
                    tmpTile[i].isChange = _tile[i].isChange;
                    tmpTile[i].path = _tile[i].path;
                    moveComponentValue[i] = _tile[i].activitiesAllowUnit.moveSpriteRenderer.enabled;
                }
            }

            // none 类型在弹出时直接跳过 因为none是角色移动过程
            StepInfo temp = new StepInfo()
            {
                unit = _unit,
                unitCurrentPos = _unitCurrentPos,
                tile = _tile == null ? null : tmpTile,
                actionScopeType = _actionScopeType,
                activitiesAction = _activitiesAction,
                clickTilePosAction = _clickTilePosAction,
                cancelAction = _cancelAction,
                moveComponentEnable = _tile == null ? null : moveComponentValue
            };
            stepInfoStack.Push(temp);
        }

        private void ExecuteClickStepEvent(ActivitiesUnit _unit, Vector3Int _cellPos, bool _gridPlayerLayer)
        {
            if (stepInfoStack.Count == 0)
            {
                if (_unit)
                {
                    // 告诉士兵管理系统
                    activitiesManager.SelectionUnit(_unit);

                    if (clickActivitiesUnit == null /*|| (clickActivitiesUnit != null && clickActivitiesUnit == unit)*/)
                    {
                        clickActivitiesUnit = _unit;
                    }
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

        private void CancelStepEvent()
        {
            if (stepInfoStack.Count == 0)
            {
                if (!activitiesManager.CancelTileSelection())
                {
                    // 则ui进入初始化界面
                    LoadInfo.Instance.sceneWindowsCanvas.SetInitPanel();
                    //CommanderRangeUnit(null);
                }
                else
                {
                    clickActivitiesUnit = null;
                }
            }
            else
            {
                Debug.Log(stepInfoStack.Count);
                Debug.Log("CancelStepEvent:" + stepInfoStack.Peek().actionScopeType);
                StepInfo temp = stepInfoStack.Pop();
                temp.cancelAction();
                while (stepInfoStack.Count != 0)
                {
                    if (stepInfoStack.Peek().actionScopeType == ActionScopeType.none)
                    {
                        var whileValue = stepInfoStack.Pop();
                        whileValue.cancelAction();
                    }
                    else
                    {
                        break;
                    }
                }
                ResetStepEvent();
            }
        }

        private void ResetStepEvent()
        {
            if (stepInfoStack.Count != 0)
            {
                Debug.Log("ResetStepEvent:" + stepInfoStack.Peek().actionScopeType);
                StepInfo temp = stepInfoStack.Peek();
                //LoadInfo.Instance.sceneTileMapManager.HideCanMoveCorrelationGrid();
                LoadInfo.Instance.sceneTileMapManager.LoadCorrelationGrid(temp);
            }
        }


        /// <summary>
        /// 指挥圈变更通知
        /// </summary>
        /// <param name="_unit"></param>
        private void CommanderRangeUnit(ActivitiesUnit _unit)
        {
            cacheHitRaycastUnit = _unit;

            if (_unit == null)
            {
                activitiesManager.ExitCommanderOrSoliderUnit();
                return;
            }

            if (_unit.GetType() == typeof(CommanderUnit))
            {
                activitiesManager.EnterCommanderOrSoliderUnit(_unit as CommanderUnit);
            }
            else if (_unit.GetType() == typeof(SoliderUnit))
            {
                activitiesManager.EnterCommanderOrSoliderUnit((_unit as SoliderUnit)?.mineCommanderUnit);
            }
        }

        private bool isMouseBtn;
        private float cacheDeadZone = 0.95f;
        private Vector3 centerViewportToWorldPoint;
        private Vector3 cacheMousePosition;

        void LateUpdate()
        {
            if (!isExecute)
            {
                return;
            }

            var worldPointV3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPointV3.z = 0;
            if (Input.GetMouseButtonDown(2))
            {
                cacheDeadZone = 0.03f;

                if (!isMouseBtn)
                {
                    cacheMousePosition = worldPointV3;
                    centerViewportToWorldPoint = Camera.main.ViewportToWorldPoint(Vector2.one / 2.0f);
                    centerViewportToWorldPoint.z = 0;
                    isMouseBtn = !isMouseBtn;
                }
            }
            else if (Input.GetMouseButtonUp(2))
            {
                isMouseBtn = false;
                cacheDeadZone = 0.95f;
            }

            transform.position = isMouseBtn
                ? centerViewportToWorldPoint - (worldPointV3 - cacheMousePosition) * 2
                : worldPointV3;
            cine.m_DeadZoneWidth = cacheDeadZone;
            cine.m_DeadZoneHeight = cacheDeadZone;

        }

    }
}