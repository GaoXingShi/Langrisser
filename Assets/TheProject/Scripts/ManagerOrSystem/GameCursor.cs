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


    /// <summary>
    /// 游戏光标类 只负责光标产生的信息传递，不处理逻辑
    /// </summary>
    public class GameCursor : MonoBehaviour
    {
        public CinemachineVirtualCamera cinemachine;
        public UnityEngine.Grid grid;
        public Texture2D cursorTexture, attackTexture;
        [HideInInspector] public bool isExecute = true;
        private CinemachineFramingTransposer cine;
        private ActivitiesManager activitiesManager;
        private ActivitiesUnit cacheHitRaycastUnit;
        private CommandEventQueue commandEventQueue;
        void Start()
        {
            cine = ((CinemachineFramingTransposer)cinemachine.GetComponentPipeline()[0]);
            activitiesManager = LoadInfo.Instance.activitiesManager;
            commandEventQueue = LoadInfo.Instance.commandEventQueue;
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
                            //MouseHoverUnit(null);
                        }

                        if (cacheHitRaycastUnit == null || cacheHitRaycastUnit != unit)
                        {
                            MouseHoverUnit(unit);
                        }
                    }
                    else
                    {
                        // Exit Unit
                        if (cacheHitRaycastUnit != null)
                        {
                            MouseHoverUnit(null);
                        }

                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        MouseLeftClickUnit(unit, cellPos, gridPlayerLayer);
                    }

                }
                else
                {
                    // 通知现在没点到士兵
                    if (cacheHitRaycastUnit != null)
                    {
                        Debug.Log("??");
                        MouseHoverUnit(null);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                MouseRightClick();
            }

        }

        public void ClearCursorState()
        {
            cacheHitRaycastUnit = null;
        }

        private void MouseLeftClickUnit(ActivitiesUnit _unit, Vector3Int _cellPos, bool _gridPlayerLayer)
        {
            commandEventQueue.ExecuteStepEvent(_unit, _cellPos, _gridPlayerLayer);
        }

        /// <summary>
        /// 鼠标调用取消时的回调
        /// </summary>
        private void MouseRightClick()
        {
            commandEventQueue.RemoveStepEvent();
        }


        /// <summary>
        /// 鼠标Enter Exit 角色通知
        /// </summary>
        /// <param name="_unit"></param>
        private void MouseHoverUnit(ActivitiesUnit _unit)
        {
            activitiesManager.ActivitiesUnitCursorEvent(_unit);
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