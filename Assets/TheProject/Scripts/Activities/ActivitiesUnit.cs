using System.Linq;
using MainSpace.ScriptableObject;
using UnityEngine;


namespace MainSpace.Activities
{
    public enum SoliderType
    {
        步兵, 狂战士,
        骑兵, 重装骑兵,
        枪兵, 重装枪兵,
    }

    public enum TerrainActionType
    {
        步行,
        骑行,
        飞行,
        水行,
        幽灵,       // 无视地形但不能无视敌方士兵 可进入建筑物构建内
        瞬行,         // 无视地形与敌方士兵 但不能停留在建筑物构建内或小山内

    }

    public enum RoleType
    {
        战士, 兵长, 领主, 步兵统帅, 将军,
        骑士, 骑士长, 骑士统帅, 封号骑士,
        法师,
        牧师
    }

    public class ActivitiesUnit : MonoBehaviour
    {
        [Header("Wait Init Base Data")]
        public Vector3Int currentPos ;
        public int[] healthValue, magicValue, moveRangeValue,attackRangeValue,skillRangeValue, attackValue, defenseValue;        // 索引0为当前，索引1为总量
        public ActivityConfig activityConfig;
        public TerrainActionType movingType;
        public Sprite affiliationSprite;
        public string affiliationName, managerKeyName;

        public bool isActionOver = false,isPlayingAnim = false;                       // 是否结束行动 , 是否播放动画
        public ActivitiesManager manager;
        public Color campColor;
        public TroopsType troopsType;
        public ActiveSkillsFlag activeSkillsMastery;
        public PassiveSkillsFlag passiveSkillsMastery;

        [Header("Local Link")]
        public SpriteRenderer mRendererComponent;
        public TextMesh hpText;
        public SpriteRenderer professionSprite , playerColorSprite;

        private Material mMaterial;
        private GameManager gameManager;
        private void Start()
        {
            mMaterial = GetComponentInChildren<SpriteRenderer>().material;
            gameManager = LoadInfo.Instance.gameManager;
            isPlayingAnim = false;
        }

        public virtual void NodeInitData()
        {
            healthValue = magicValue = moveRangeValue = attackRangeValue = attackValue =
                defenseValue = new int[2];
        }

        public virtual void ManagerInitData()
        {
            playerColorSprite.color = campColor;
        }

        private void Update()
        {
            playerColorSprite.transform.localScale = gameManager.lerpVector3Value * 0.7f;

            if (isPlayingAnim && !isActionOver)
            {
                mRendererComponent.sprite = gameManager.lerpIntValue <= 50 ? activityConfig.normalSprite : activityConfig.showOffSprite;
            }
            else
            {
                mRendererComponent.sprite = activityConfig.normalSprite;
            }
        }


        public void SetIntArrayData(ref int[] _array, int _value)
        {
            _array = new int[2];
            _array[0] = _value;
            _array[1] = _value;
        }

        public void UnitColorChange(bool _isGray)
        {
            mMaterial.SetFloat("GrayLone", _isGray ? 1 : 0);
        }

        public void PlayActivityAnim(bool _enabled)
        {
            isPlayingAnim = _enabled;
        }

        ///// <summary>
        ///// 获取iCon是否激活
        ///// </summary>
        ///// <param name="_iconName"></param>
        ///// <returns></returns>
        //public bool GetActivitiesUnitIcon(string _iconName)
        //{
        //    foreach (var v in iconArray)
        //    {
        //        if (v.name.Equals(_iconName) && v.activeInHierarchy)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 设置Icon , 输入null 使单位所有Icon未激活
        ///// </summary>
        ///// <param name="_iconName"></param>
        //public void SetActivitiesUnitIcon(string _iconName)
        //{
        //    foreach (var v in iconArray)
        //    {
        //        v.SetActive(false);
        //    }

        //    foreach (var v in iconArray.Where(v => v.name.Equals(_iconName)))
        //    {
        //        v.SetActive(true);
        //        break;
        //    }
        //}

    }

}
