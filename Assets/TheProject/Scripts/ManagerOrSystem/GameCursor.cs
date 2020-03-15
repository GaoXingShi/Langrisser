using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using MainSpace.Activities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace MainSpace
{
    /// <summary>
    /// 游戏光标类 只负责光标产生的信息传递，不处理逻辑
    /// </summary>
    public class GameCursor : MonoBehaviour
    {
        public CinemachineVirtualCamera cinemachine;
        public UnityEngine.Grid grid;

        [HideInInspector]
        public bool isExecute = true;
        private CinemachineFramingTransposer cine;
        private ActivitiesManager activitiesManager;
        void Start()
        {
            cine = ((CinemachineFramingTransposer)cinemachine.GetComponentPipeline()[0]);
            activitiesManager = LoadInfo.Instance.activitiesManager;
            isExecute = true;
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
                        if (Input.GetMouseButtonDown(0))
                        {
                            // 告诉士兵管理系统
                            activitiesManager.SelectionUnit(unit);
                        }
                        else
                        {
                            CommanderRangeUnit(unit);
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            // todo 如果通知的脚本过多不如弄成事件。
                            activitiesManager.ClickTilePos(cellPos);
                        }
                        else
                        {
                            // 通知现在没点到士兵
                            CommanderRangeUnit(null);
                        }
                    }
                }
                else
                {
                    // 通知现在没点到士兵
                    CommanderRangeUnit(null);
                }


                if (Input.GetMouseButtonDown(1))
                {
                    activitiesManager.CancelTileSelection();
                }
            }
        }

        /// <summary>
        /// 指挥圈变更通知
        /// </summary>
        /// <param name="_unit"></param>
        private void CommanderRangeUnit(ActivitiesUnit _unit)
        {
            if (_unit == null)
            {
                activitiesManager.NoneActivitiesUnit();
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