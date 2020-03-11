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
    /// 游戏光标类
    /// </summary>
    public class GameCursor : MonoBehaviour
    {
        public CinemachineVirtualCamera cinemachine;
        public UnityEngine.Grid grid;
        public Tilemap map1, map2;

        private CinemachineFramingTransposer cine;
        private ActivitiesManager activitiesManager;

        void Start()
        {
            cine = ((CinemachineFramingTransposer)cinemachine.GetComponentPipeline()[0]);
            activitiesManager = LoadInfo.Instance.activitiesManager;
        }

        void Update()
        {
            var worldPointV3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = grid.WorldToCell(worldPointV3);

            // 非UI层。
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // 按下了鼠标左键
                if (Input.GetMouseButtonDown(0))
                {
                    var hit2D = Physics2D.Raycast(worldPointV3, Vector2.zero, 10);
                    if (hit2D.transform != null)
                    {

                        if (hit2D.transform.gameObject.layer == LayerMask.NameToLayer("GridPlayer"))
                        {
                            // 告诉士兵管理系统
                            activitiesManager.SelectionUnit(hit2D.transform
                                .GetComponent<ActivitiesUnit>());
                        }
                        else
                        {
                            // todo 如果通知的脚本过多不如弄成事件。
                            activitiesManager.ClickTilePos(cellPos);
                        }
                    }
                }
                else if(Input.GetMouseButtonDown(1))
                {
                    activitiesManager.CancelTileSelection();
                }
            }



        }

        private bool isMouseBtn;
        private float cacheDeadZone = 0.95f;
        private Vector3 centerViewportToWorldPoint;
        private Vector3 cacheMousePosition;

        void LateUpdate()
        {
            var worldPointV3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPointV3.z = 0;
            //var screenV3 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
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