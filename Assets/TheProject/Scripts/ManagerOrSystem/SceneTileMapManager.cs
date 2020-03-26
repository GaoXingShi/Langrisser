﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainSpace.Activities;
using MainSpace.ScriptableObject;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityScript.Scripting.Pipeline;

namespace MainSpace.Grid
{
    [System.Serializable]
    public class TileSaveData
    {
        public TileBase tile;                           // 瓦片信息
        public Sprite sprite;                           // sprite信息
        public Vector3Int widthHeighValue;              // 瓦片坐标
        public AllowUnitInfo activitiesAllowUnit;      // 坐标位置的可移动方块
        //public int aSont;                             // A值是 阶数
        public int[] pixelValue;    // 0 是更改数 1是不变数
        public bool isChange = false;
        public void InfoEntry(TileBase _tile, Sprite _sprite, Vector3Int _widthHeigh, AllowUnitInfo _activitiesAllowUnit)
        {
            tile = _tile;
            sprite = _sprite;
            widthHeighValue = _widthHeigh;
            activitiesAllowUnit = _activitiesAllowUnit;
            SetPixelValue(-1);
        }

        public void SetPixelValue(int _value)
        {
            pixelValue = new int[] { _value, _value };
            isChange = false;
        }
    }

    /// <summary>
    /// 地图地形信息类 , 也管理单位的移动权限
    /// </summary>
    public class SceneTileMapManager : MonoBehaviour
    {
        [Header("Prefab")]
        public GameObject moveSprite;

        [Header("Link")]
        public Tilemap ground;
        public Tilemap supplement;
        public Transform activitiesAllowUnitRoot;
        public EnvironmentConfig environmentConfig;

        [Header("Data")]
        public float colorAValue = 0;
        // 边界数目
        private int width, height;

        private TileSaveData[,] tileArray;
        private /*readonly*/ List<TileSaveData> tileList = new List<TileSaveData>();
        private TileSaveData[] cacheSaveData;
        private ActivitiesManager activitiesManager;
        private bool lerpStart;
        void Start()
        {
            InitCalculateValue();
            activitiesManager = LoadInfo.Instance.activitiesManager;

            HideCommanderCircleGrid();
        }

        void Update()
        {
            if (!lerpStart)
            {
                return;
            }
            colorAValue = LoadInfo.Instance.gameManager.lerpIntValue * 1.2f;

        }

        /// <summary>
        /// 输入一个活动单位，返回该活动单位可以活动的区域。
        /// </summary>
        /// <param name="_unit">单位</param>
        /// <returns></returns>
        public TileSaveData[] CalculateMovingRange(ActivitiesUnit _unit)
        {
            foreach (var v in tileList.Where(x => x.pixelValue[0] != -1))
            {
                v.SetPixelValue(-1);
            }

            for (int i = 1; i <= _unit.moveValue[0]; i++)
            {
                foreach (var v in tileList.Where(x => x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) == i))
                {
                    //v.SetASont(i);
                    if (activitiesManager.GetUnitPosContains(v.widthHeighValue) && !activitiesManager.GetUnitPosContainsOtherTroop(v.widthHeighValue, _unit.managerKeyName))
                    {
                        v.SetPixelValue(99);
                    }
                    else
                    {
                        v.SetPixelValue(environmentConfig.GetConsumeValue(_unit.movingType, GetSprite(v.widthHeighValue)));
                    }
                }
            }
            var currentSaveData = tileList.FirstOrDefault(x => x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) == 0);
            currentSaveData.SetPixelValue(0);
            currentSaveData.isChange = true;
            PressureAlgorithm(currentSaveData.widthHeighValue, _unit.moveValue[0]);

            List<TileSaveData> cacheData = tileList.Where(x => x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) <= _unit.moveValue[0]).ToList();

            while (cacheData.Any(x => x.pixelValue[0] < _unit.moveValue[0] && x.pixelValue[0] > 0 && x.isChange))
            {
                foreach (var v in cacheData.Where(x => x.pixelValue[0] < _unit.moveValue[0] && x.pixelValue[0] > 0 && x.isChange))
                {
                    PressureAlgorithm(v.widthHeighValue, _unit.moveValue[0]);
                }

            }

            cacheSaveData = cacheData.Where(x => x.pixelValue[0] <= _unit.moveValue[0] && x.isChange && (!activitiesManager.GetUnitPosContains(x.widthHeighValue) || x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) == 0)).ToArray();

            return cacheSaveData;
        }

        private void PressureAlgorithm(Vector3Int _currentPos, int _maxMovingValue)
        {
            var currentData = GetTileSaveData(_currentPos);
            if (currentData.pixelValue[0] > _maxMovingValue || currentData.isChange == false)
            {
                return;
            }

            SearchAround(currentData, GetTileSaveData(_currentPos + Vector3Int.left));
            SearchAround(currentData, GetTileSaveData(_currentPos + Vector3Int.right));
            SearchAround(currentData, GetTileSaveData(_currentPos + Vector3Int.up));
            SearchAround(currentData, GetTileSaveData(_currentPos + Vector3Int.down));

            currentData.pixelValue[0] = 0;
        }

        private void SearchAround(TileSaveData _currentData, TileSaveData _data)
        {
            if (_data == null)
            {
                return;
            }

            int value = _data.pixelValue[1] + _currentData.pixelValue[0];
            if (_data.pixelValue[0] != 0)
            {
                if (_data.isChange)
                {
                    if (value < _data.pixelValue[0])
                        _data.pixelValue[0] = value;
                }
                else
                {
                    _data.isChange = true;
                    _data.pixelValue[0] = value;
                }
            }

        }

        /// <summary>
        /// 输入单位的预计放置点，返回真正的放置点(因为很可能出现单位用了同一方格的情况)。
        /// </summary>
        /// <param name="_pos"></param>
        /// <returns></returns>
        public Vector3Int GetUnitSpacePos(Vector3Int _pos)
        {
            // 后期生成时还要注意，单位不能生成的位置。与地形有关。
            if (activitiesManager.GetUnitPosContains(_pos))
            {
                for (int i = 1; i < 10; i++)
                {
                    Vector3Int? temp = GetUnitRangeSpacePos(_pos, i);
                    if (temp != null)
                    {
                        return temp.Value;
                    }
                }
                Debug.LogError("返回的值不对", gameObject);
                return Vector3Int.zero;
            }
            else
            {
                return _pos;
            }
        }

        /// <summary>
        /// 获取该Vector3 Pos 点是否允许移动
        /// </summary>
        /// <param name="_pos"></param>
        /// <returns></returns>
        public bool GetMoveToUnitAllow(Vector3Int _pos)
        {
            if (cacheSaveData == null || cacheSaveData.Length == 0) return false;

            foreach (var v in cacheSaveData)
            {
                if (v.widthHeighValue.Vector3IntRangeValue(_pos) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 显示可移动相关区域
        /// </summary>
        public void ShowCanMoveCorrelationGrid()
        {
            foreach (var v in tileList)
            {
                v.activitiesAllowUnit.SetMoveGrid(true);
            }

            foreach (var v in cacheSaveData)
            {
                v.activitiesAllowUnit.SetMoveGrid(false);
            }
        }

        /// <summary>
        /// 隐藏可移动相关区域
        /// </summary>
        public void HideCanMoveCorrelationGrid()
        {
            if (cacheSaveData == null) return;

            foreach (var v in tileList)
            {
                v.activitiesAllowUnit.SetMoveGrid(false);
            }

            cacheSaveData = null;
        }

        /// <summary>
        /// 显示指挥圈相关区域
        /// </summary>
        /// <param name="_pos"></param>
        /// <param name="_range"></param>
        /// <param name="_commanderCirclecolor"></param>
        public void ShowCommanderCircleGrid(Vector3Int _pos, int _range, Color _commanderCirclecolor)
        {
            TileSaveData[] array = tileList.Where(x =>
                x.widthHeighValue.Vector3IntRangeValue(_pos) <= _range).ToArray();
            foreach (var v in array)
            {
                v.activitiesAllowUnit.SetCommanderCircleGrid(true, _commanderCirclecolor);
            }
        }

        /// <summary>
        /// 隐藏指挥圈相关区域
        /// </summary>
        public void HideCommanderCircleGrid()
        {
            if (!tileList.Any(x => x.activitiesAllowUnit.commandSpriteRenderer.enabled))
            {
                return;
            }

            foreach (var v in tileList.Where(x => x.activitiesAllowUnit.commandSpriteRenderer.enabled))
            {
                v.activitiesAllowUnit.SetCommanderCircleGrid(false, Color.clear);
            }
        }

        /// <summary>
        /// 控制指挥圈状态
        /// </summary>
        /// <param name="_enabled"></param>
        public void SetColorValueState(bool _enabled)
        {
            //if (_enabled.Equals(lerpStart))
            //{
            //    return;
            //}

            lerpStart = _enabled;
            //isLerpUp = true;
            //colorAValue = MINAVALUE;
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
                    tileArray[i, j] = new TileSaveData();
                    tileArray[i, j].InfoEntry(this.GetTile(_vector3), GetSprite(_vector3), _vector3, Instantiate(moveSprite, _vector3, Quaternion.identity, activitiesAllowUnitRoot).GetComponent<AllowUnitInfo>());
                    tileList.Add(tileArray[i, j]);

                    if (tileArray[i, j].tile == null)
                    {
                        Debug.LogError("TileMap error");
                    }
                }
            }
        }

        private Vector3Int? GetUnitRangeSpacePos(Vector3Int _pos, int _range)
        {
            var array = tileList
                .Where(x => x.widthHeighValue.Vector3IntRangeValue(_pos) == _range).ToArray();

            // 优先左 右 上 下 右上 左下
            if (_range == 2)
            {
                Vector3Int temp = _pos;
                TileSaveData a = GetTileSaveData(temp + Vector3Int.up + Vector3Int.right);
                TileSaveData b = GetTileSaveData(temp + Vector3Int.down + Vector3Int.left);
                if (a != null && !activitiesManager.GetUnitPosContains(a.widthHeighValue))
                {
                    return a?.widthHeighValue;
                }
                else if (b != null && !activitiesManager.GetUnitPosContains(b.widthHeighValue))
                {
                    return b?.widthHeighValue;
                }
            }

            foreach (var v in array)
            {
                if (!activitiesManager.GetUnitPosContains(v.widthHeighValue))
                {
                    return v.widthHeighValue;
                }
            }

            return null;
        }

        private TileSaveData GetTileSaveData(Vector3Int _pos)
        {
            int x = _pos.x, y = _pos.y;
            if (y < height && x < width && y > 0 && x > 0)
            {
                return tileArray[y, x];
            }

            return tileList.FirstOrDefault(z => z.widthHeighValue.Vector3IntRangeValue(_pos) == 0);
        }
        private TileBase GetTile(Vector3Int _pos)
        {
            return supplement.GetTile(_pos) == null ? ground.GetTile(_pos) : supplement.GetTile(_pos);
        }

        private Sprite GetSprite(Vector3Int _pos)
        {
            return supplement.GetSprite(_pos.RemoveZValuie(0)) == null ? ground.GetSprite(_pos.RemoveZValuie(0)) : supplement.GetSprite(_pos.RemoveZValuie(0));
        }

    }

}

