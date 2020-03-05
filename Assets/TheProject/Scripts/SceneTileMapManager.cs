using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainSpace.Activities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MainSpace
{
    [System.Serializable]
    public struct TileSaveData
    {
        public TileBase tile;                           // 瓦片信息
        public Vector3Int widthHeighValue;              // 瓦片坐标
        public SpriteRenderer activitiesAllowUnit;      // 坐标位置的可移动方块

        public void InfoEntry(TileBase _tile, Vector3Int _widthHeigh, SpriteRenderer _activitiesAllowUnit)
        {
            tile = _tile;
            widthHeighValue = _widthHeigh;
            activitiesAllowUnit = _activitiesAllowUnit;
            _activitiesAllowUnit.enabled = false;
        }
    }

    /// <summary>
    /// 地图地形信息类
    /// </summary>
    public class SceneTileMapManager : MonoBehaviour
    {
        [Header("Prefab")]
        public GameObject moveSprite;

        [Header("Link")]
        public Tilemap ground;
        public Tilemap supplement;
        public Transform activitiesAllowUnitRoot;
        // 边界数目
        private int width, height;

        private TileSaveData[,] tileArray;
        private readonly List<TileSaveData> tileList = new List<TileSaveData>();
        private TileSaveData[] cacheSaveData;
        void Start()
        {
            InitCalculateValue();
        }

        /// <summary>
        /// 输入一个活动单位，返回该活动单位可以活动的区域。
        /// </summary>
        /// <param name="_unit">单位</param>
        /// <returns></returns>
        public TileSaveData[] CalculateMovingRange(ActivitiesUnit _unit)
        {
            // 目前不计算障碍。
            // ActivitiesManager.GetActivitiesUnit 需要计算
            cacheSaveData = tileList
                .Where(x => x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) <= _unit.grossMv)
                .ToArray();
            return cacheSaveData;
        }

        //public TileSaveData[] CalculateAttackRange(ActivitiesUnit _unit)
        //{

        //}

        public bool MoveToUnit(Vector3Int _pos)
        {
            if (cacheSaveData == null || cacheSaveData.Length == 0) return false;

            foreach (var v in cacheSaveData)
            {
                if (v.widthHeighValue == _pos)
                {
                    return true;
                }
            }

            return false;
        }

        public void ShowCorrelationGrid()
        {
            foreach (var v in cacheSaveData)
            {
                v.activitiesAllowUnit.enabled = true;
                v.activitiesAllowUnit.color = new Color(0,118,255,160);
            }
        }

        public void HideAllGrid()
        {
            if (cacheSaveData == null) return;

            foreach (var v in cacheSaveData)
            {
                v.activitiesAllowUnit.enabled = false;
                v.activitiesAllowUnit.color = new Color(255, 255, 255, 255);
            }

            cacheSaveData = null;
        }

        private void InitCalculateValue()
        {
            // 长宽估测
            Vector3Int _vector3 = Vector3Int.zero;
            for (int i = 0; ; i++)
            {
                _vector3.x = i;
                if (ground.GetTile(_vector3) == null && supplement.GetTile(_vector3) == null)
                {
                    width = i;
                    break;
                }
            }

            _vector3 = Vector3Int.zero;
            _vector3.x = width - 1;
            for (int i = 0; ; i++)
            {
                _vector3.y = i;
                if (ground.GetTile(_vector3) == null && supplement.GetTile(_vector3) == null)
                {
                    height = i;
                    break;
                }
            }

            tileArray = new TileSaveData[height, width];
            for (int i = 0; i < height; i++)
            {
                _vector3.y = i;
                for (int j = 0; j < width; j++)
                {
                    _vector3.x = j;
                    tileArray[i, j].InfoEntry(this.GetTile(_vector3), _vector3, Instantiate(moveSprite, _vector3, Quaternion.identity, activitiesAllowUnitRoot).GetComponent<SpriteRenderer>());
                    tileList.Add(tileArray[i, j]);

                    if (tileArray[i, j].tile == null)
                    {
                        Debug.LogError("TileMap error");
                    }
                }
            }

            Debug.Log(tileArray[height - 1, width - 1].tile);
        }

        private TileBase GetTile(Vector3Int _vector3)
        {
            return supplement.GetTile(_vector3) == null ? ground.GetTile(_vector3) : supplement.GetTile(_vector3);
        }

    }

}

