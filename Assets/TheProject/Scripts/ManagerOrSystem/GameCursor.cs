using System;
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
        public TileSaveDataCommand[] tileCommand;
        public ActionScopeType actionScopeType;
        public Action<ActivitiesUnit> activitiesAction;
        public Action<Vector3Int> clickTilePosAction;
        public Action cancelAction;
    }

    public struct TileSaveDataCommand
    {
        public string aliasName, path;    // 别名
        public bool isChange, moveEnable;
        public Vector3Int pos;
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

        public bool isHaveCacheHitRayCastUnit => cacheHitRaycastUnit != null;
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
                            Debug.Log("????");
                            //CommanderRangeUnit(null);
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
                            Debug.Log("??");

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
                        Debug.Log("??");
                        CommanderRangeUnit(null);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                CancelStepEvent();
            }

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

            // none 类型在弹出时直接跳过 因为none是角色移动过程
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
            //clickActivitiesUnit = null;
            stepInfoStack.Clear();
        }

        private void ExecuteClickStepEvent(ActivitiesUnit _unit, Vector3Int _cellPos, bool _gridPlayerLayer)
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

        /// <summary>
        /// 鼠标调用取消时的回调
        /// </summary>
        private void CancelStepEvent()
        {
            if (stepInfoStack.Count == 0)
            {
                // 则ui进入初始化界面
               // LoadInfo.Instance.sceneWindowsCanvas.SetUpPanel();
            }
            else
            {
                StepInfo temp = stepInfoStack.Pop();
                temp.cancelAction();
                while (stepInfoStack.Count != 0 && stepInfoStack.Peek().actionScopeType == ActionScopeType.none)
                {
                    var whileValue = stepInfoStack.Pop();
                    whileValue.cancelAction();
                }
                ResetStepEvent();
            }
        }

        private void ResetStepEvent()
        {
            if (stepInfoStack.Count != 0)
            {
                StepInfo temp = stepInfoStack.Peek();
                LoadInfo.Instance.sceneTileMapManager.LoadCorrelationGrid(temp);
            }
            else
            {
                // 回到初始状态
                FinishStepEvent(true);
            }

            LoadInfo.Instance.sceneWindowsCanvas.RefreshActivitiesData();

        }

        /// <summary>
        /// 指挥圈变更通知
        /// </summary>
        /// <param name="_unit"></param>
        private void CommanderRangeUnit(ActivitiesUnit _unit)
        {

            if (_unit == null)
            {
                activitiesManager.ExitCommanderOrSoliderUnit();
                cacheHitRaycastUnit = null;
                return;
            }

            if (!activitiesManager.GetUnitSameCommander(cacheHitRaycastUnit, _unit))
            {
                activitiesManager.ExitCommanderOrSoliderUnit(true);
            }

            if (_unit.GetType() == typeof(CommanderUnit))
            {
                activitiesManager.EnterCommanderOrSoliderUnit(_unit as CommanderUnit);
                LoadInfo.Instance.sceneWindowsCanvas.ShowActivitiesData(_unit as CommanderUnit, false);
            }
            else if (_unit.GetType() == typeof(SoliderUnit))
            {
                activitiesManager.EnterCommanderOrSoliderUnit((_unit as SoliderUnit)?.mineCommanderUnit);
                LoadInfo.Instance.sceneWindowsCanvas.ClearActivitiesData(_unit as SoliderUnit, false);

            }

            cacheHitRaycastUnit = _unit;
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