using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;
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
        public string aliasName, path;    // 别名
        public bool isChange = false;
        public void InfoEntry(TileBase _tile, Sprite _sprite, Vector3Int _widthHeigh, AllowUnitInfo _activitiesAllowUnit)
        {
            tile = _tile;
            sprite = _sprite;
            widthHeighValue = _widthHeigh;
            activitiesAllowUnit = _activitiesAllowUnit;
            SetPixelValue(-1);
            InitAliasName();
        }

        public void SetPixelValue(int _value)
        {
            pixelValue = new int[] { _value, _value };
            isChange = false;
        }

        public void InitAliasName()
        {
            aliasName = path = string.Empty;
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
        [HideInInspector]
        public int width, height;

        private TileSaveData[,] tileArray;
        private List<TileSaveData> tileList = new List<TileSaveData>();
        private TileSaveData[] cacheSaveData;
        private ActivitiesManager activitiesManager;
        private GameManager gameManager;
        private GameCursor cursor;
        private bool lerpStart;
        void Start()
        {
            InitCalculateValue();
            activitiesManager = LoadInfo.Instance.activitiesManager;
            gameManager = LoadInfo.Instance.gameManager;
            cursor = LoadInfo.Instance.gameCursor;

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

        #region 移动区域计算相关

        /// <summary>
        /// 输入一个活动单位，返回该活动单位可以活动的区域。
        /// </summary>
        /// <param name="_unit">单位</param>
        /// <returns></returns>
        public TileSaveData[] CalculateMovingRange(ActivitiesUnit _unit)
        {
            // 初始化
            foreach (var v in tileList.Where(x => x.pixelValue[0] != -1))
            {
                v.SetPixelValue(-1);
                v.InitAliasName();
            }
            // 抓取部分数据
            List<TileSaveData> cacheData = tileList.Where(x => x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) <= _unit.moveRangeValue[0]).ToList();

            int tempIndex = 1;
            // 地形移动力消耗值
            foreach (var v in cacheData)
            {
                if (v.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) != 0)
                    v.aliasName = string.Concat("T", tempIndex++);
                if (activitiesManager.GetUnitPosContains(v.widthHeighValue) &&
                    !activitiesManager.GetUnitPosContainsOtherTroop(v.widthHeighValue, _unit.managerKeyName))
                {
                    v.SetPixelValue(99);
                }
                else
                {
                    v.SetPixelValue(environmentConfig.GetConsumeValue(_unit.movingType,
                        GetSprite(v.widthHeighValue)));
                }
            }
            // 单位当前位置
            var currentSaveData =
                tileList.FirstOrDefault(x => x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) == 0);
            currentSaveData.SetPixelValue(0);
            currentSaveData.aliasName = "T0";
            currentSaveData.path = "T0";
            currentSaveData.isChange = true;

            if (_unit.movingType != TerrainActionType.瞬行)
            {
                PressureAlgorithm(currentSaveData.widthHeighValue, _unit.moveRangeValue[0]);

                while (cacheData.Any(x => x.pixelValue[0] < _unit.moveRangeValue[0] && x.pixelValue[0] > 0 && x.isChange))
                {
                    foreach (var v in cacheData.Where(x =>
                        x.pixelValue[0] < _unit.moveRangeValue[0] && x.pixelValue[0] > 0 && x.isChange))
                    {
                        PressureAlgorithm(v.widthHeighValue, _unit.moveRangeValue[0]);
                    }

                }

                // 去除所有数值低于移动力的方格 与没计算过的方格.
                cacheSaveData = cacheData.Where(x =>
                    x.pixelValue[0] <= _unit.moveRangeValue[0] && x.isChange).ToArray();


            }
            else
            {
                cacheSaveData = cacheData.Where(x => x.pixelValue[0] <= _unit.moveRangeValue[0]).ToArray();
            }

            // 去除所有 已有单位的方格
            cacheSaveData = cacheSaveData.Where(x =>
                (!activitiesManager.GetUnitPosContains(x.widthHeighValue) ||
                 x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) == 0)).ToArray();

            return cacheSaveData;
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
        /// 输入_pos，获得从原点到_pos的移动路径
        /// </summary>
        /// <param name="_pos"></param>
        /// <returns></returns>
        public Vector3Int[] GetMoveToUnitAllow(Vector3Int _pos)
        {
            if (cacheSaveData == null || cacheSaveData.Length == 0) return null;

            var searchData = cacheSaveData.FirstOrDefault(x => x.widthHeighValue.Vector3IntRangeValue(_pos) == 0);
            if (searchData != null)
            {
                string[] pathArray = searchData.path.Split(',');

                List<Vector3Int> cacheValue = new List<Vector3Int>();
                for (int i = 0; i < pathArray.Length; i++)
                {
                    Vector3Int value = (cacheSaveData.FirstOrDefault(x => x.aliasName.Equals(pathArray[i])) != null
                        ? cacheSaveData.FirstOrDefault(x => x.aliasName.Equals(pathArray[i]))
                        : tileList.FirstOrDefault(x => x.aliasName.Equals(pathArray[i]))).widthHeighValue;
                    value.z = -1;
                    cacheValue.Add(value);

                }
                return cacheValue.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 清空暂存的数据
        /// </summary>
        public void ClearCacheSaveData()
        {
            cacheSaveData = null;
        }

        #endregion

        #region 移动区域显示相关
        private bool asyncBoolValue = false;
        /// <summary>
        /// 显示可移动相关区域
        /// </summary>
        public async void ShowCanMoveCorrelationGrid(ActivitiesUnit _unit, bool _isAsync)
        {
            cursor.AddEvent(_unit, _unit.currentPos, cacheSaveData.Where(
                x => x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) <= _unit.moveRangeValue[0]).ToArray(),ActionScopeType.NoActivitiesUnit,null, activitiesManager.ClickTilePos,
                () =>
                {
                });

            asyncBoolValue = true;
            foreach (var v in tileList.Where(x => x.activitiesAllowUnit.moveSpriteRenderer.enabled == false))
            {
                v.activitiesAllowUnit.SetMoveGrid(true);
            }

            int ms = 30;
            int moveValue = _unit.moveRangeValue[0];
            for (int i = 0; i <= moveValue; i++)
            {
                if (cacheSaveData == null || !asyncBoolValue)
                {
                    return;
                }

                foreach (var v in cacheSaveData.Where(
                    x => x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) == i))
                {
                    v.activitiesAllowUnit.SetMoveGrid(false);
                }
                if (_isAsync)
                    await Task.Delay(ms);
            }

        }
        /// <summary>
        /// 显示移动轨迹
        /// </summary>
        /// <param name="_unit"></param>
        /// <param name="_posArray"></param>
        public void ShowCurrentMovingCorrelationGrid(ActivitiesUnit _unit, Vector3Int[] _posArray)
        {
            asyncBoolValue = false;
            foreach (var v in tileList.Where(x => x.activitiesAllowUnit.moveSpriteRenderer.enabled == false))
            {
                v.activitiesAllowUnit.SetMoveGrid(true);
            }

            cacheSaveData.FirstOrDefault(x => x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) == 0).activitiesAllowUnit.SetMoveGrid(false);
            for (int i = 0; i < _posArray.Length; i++)
            {
                tileList.FirstOrDefault(x => x.widthHeighValue.Vector3IntRangeValue(_posArray[i]) == 0).activitiesAllowUnit.SetMoveGrid(false);
            }
        }

        /// <summary>
        /// 显示攻击相关区域与待机区域
        /// </summary>
        /// <param name="_unit"></param>
        public void ShowStandByOrOtherActionGrid(ActivitiesUnit _unit)
        {
            foreach (var v in tileList.Where(x => x.activitiesAllowUnit.moveSpriteRenderer.enabled == false))
            {
                v.activitiesAllowUnit.SetMoveGrid(true);
            }

            ActivitiesUnit[] aroundUnitArray = activitiesManager.GetActivitiesUnit(_unit.currentPos, _unit.attackRangeValue[0]);

            ActivitiesUnit[] enemyArray = aroundUnitArray.Where(x =>
                gameManager.GetCampData(x.managerKeyName).troopType !=
                gameManager.GetCampData(_unit.managerKeyName).troopType).ToArray();

            var stackValue = new List<TileSaveData>();
            if (enemyArray.Length != 0)
            {
                foreach (var v in tileList.Where(x => x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) <= _unit.attackRangeValue[0]))
                {
                    v.activitiesAllowUnit.SetMoveGrid(false);
                }

                stackValue = tileList.Where(x =>
                    x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) <= _unit.attackRangeValue[0]).ToList();
            }
            else
            {
                tileList.FirstOrDefault(x => x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) == 0).activitiesAllowUnit.SetMoveGrid(false);
                stackValue =
                    tileList.Where(x => x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) == 0).ToList();
            }

            foreach (var v in enemyArray)
            {
                // 设置单位icon
                //activitiesManager.SetActivitiesUnitIconState(v, "sword");
            }

            cursor.AddEvent(_unit, _unit.currentPos, stackValue.ToArray(), ActionScopeType.MeAndEnemy,activitiesManager.SelectionUnit,null,
                () =>
                {
                });


            //tileList.FirstOrDefault(x => x.widthHeighValue.Vector3IntRangeValue(_unit.currentPos) == 0).activitiesAllowUnit.SetMoveGrid(false);
        }

        // 更改框架部分
        /// <summary>
        /// 根据技能类型显示不同效果
        /// </summary>
        /// <param name="_unit"></param>
        /// <param name="_range"></param>
        /// <param name="_actionScopeType"></param>
        /// <param name="_skillType"></param>
        public void ShowCustomActionGrid(ActivitiesUnit _unit, int _range, ActionScopeType _actionScopeType,
            SkillType _skillType)
        {

        }


        /// <summary>
        /// 隐藏可移动相关区域
        /// </summary>
        public void HideCanMoveCorrelationGrid()
        {
            asyncBoolValue = false;
            if (cacheSaveData == null) return;

            foreach (var v in tileList.Where(x => x.activitiesAllowUnit.moveSpriteRenderer.enabled))
            {
                v.activitiesAllowUnit.SetMoveGrid(false);
            }

        }


        #endregion

        #region 指挥圈相关
        /// <summary>
        /// 显示指挥圈相关区域
        /// </summary>
        /// <param name="_pos"></param>
        /// <param name="_range"></param>
        /// <param name="_commanderCircleColor"></param>
        public void ShowCommanderCircleGrid(Vector3Int _pos, int _range, Color _commanderCircleColor)
        {
            TileSaveData[] array = tileList.Where(x =>
                x.widthHeighValue.Vector3IntRangeValue(_pos) <= _range).ToArray();
            foreach (var v in array)
            {
                v.activitiesAllowUnit.SetCommanderCircleGrid(true, _commanderCircleColor);
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
        /// 刷新指挥圈
        /// </summary>
        /// <param name="_unit"></param>
        public void RefreshCommanderCircleGird(ActivitiesUnit _unit)
        {
            HideCommanderCircleGrid();
            if (_unit.GetType() == typeof(CommanderUnit))
            {
                var commandUnit = ((CommanderUnit)_unit);
                ShowCommanderCircleGrid(commandUnit.currentPos, commandUnit.commandRangeValue[0], commandUnit.campColor);
            }
            else if (_unit.GetType() == typeof(SoliderUnit))
            {
                var soliderUnit = ((SoliderUnit)_unit);
                ShowCommanderCircleGrid(soliderUnit.mineCommanderUnit.currentPos, soliderUnit.mineCommanderUnit.commandRangeValue[0], soliderUnit.campColor);
            }
        }

        /// <summary>
        /// 控制指挥圈状态
        /// </summary>
        /// <param name="_enabled"></param>
        public void SetColorValueState(bool _enabled)
        {
            lerpStart = _enabled;
        }

        #endregion

        /// <summary>
        /// 查看四周并改变四周的pixel值
        /// </summary>
        /// <param name="_currentPos"></param>
        /// <param name="_maxMovingValue"></param>
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

        /// <summary>
        /// 搜索该位置的可行性
        /// </summary>
        /// <param name="_currentData"></param>
        /// <param name="_data"></param>
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
                    {
                        _data.pixelValue[0] = value;
                        _data.path = string.Concat(_currentData.path, ",", _data.aliasName);
                    }
                }
                else
                {
                    _data.isChange = true;
                    _data.pixelValue[0] = value;
                    _data.path = string.Concat(_currentData.path, ",", _data.aliasName);

                }

            }

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

        /// <summary>
        /// 中心点外空闲的位置
        /// </summary>
        /// <param name="_pos"></param>
        /// <param name="_range"></param>
        /// <returns></returns>
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

