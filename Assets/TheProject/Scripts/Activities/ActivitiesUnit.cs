using System.Collections.Generic;
using System.Linq;
using MainSpace.ScriptableObject;
using UnityEngine;


namespace MainSpace.Activities
{
    // todo https://wenku.baidu.com/view/859b8cf3fab069dc502201fb.html 这种重甲无甲的概念怎么样呢
    /// <summary>
    /// 战斗类型 (用于计算兵种克制)
    /// </summary>
    public enum FightType
    {
        步兵,
        骑兵,
        枪兵,
        僧侣,
        弓兵,
        飞兵,
        水兵,
        灵体,
        魔鬼,
        无           // 不参与克制关系
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

    public class ActivitiesProperty
    {
        public int
            healthValue, // hp值
            armorValue, // 护甲值
            magicValue, // mp值

            moveRangeValue, // 移动范围
            attackRangeValue, // 攻击范围
            skillRangeValue, // 技能范围

            attackPowerValue, // 攻击数值
            defensePowerValue, // 防御数值
            skillPowerValue;    // 技能伤害数值

        public int
            commandRangeValue, // 指挥官指挥范围
            correctedAttack, // 范围圈内附加攻击值
            correctedDefense; // 范围圈内附加防御值

        public ActivitiesProperty()
        {
            healthValue = 100;
            armorValue = 0;
            magicValue = 0;
            moveRangeValue = 3;
            attackRangeValue = 1;
            skillRangeValue = 1;
            attackPowerValue = 10;
            defensePowerValue = 10;
            skillPowerValue = 0;

            commandRangeValue = 1;
            correctedAttack = 0;
            correctedDefense = 0;
        }

    }


    public class ActivitiesUnit : MonoBehaviour
    {
        public Vector3Int currentPos
        {
            set => transform.position = value;
            get => Vector3Int.RoundToInt(transform.position);
        }

        [Header("Wait Init Base Data")]
        // cur为当前的属性、origin为初始的属性
        public ActivitiesProperty curProperty;
        public ActivitiesProperty originProperty;
        public TerrainActionType movingType;
        public ActivityConfig activityConfig;
        public TroopsType troopsType;
        public List<MainSpace.SkillCommandSpace.SkillType> skillTypes;

        public Sprite affiliationSprite;
        public string affiliationName, managerKeyName;

        public bool isActionOver = false,isPlayingAnim = false;                       // 是否结束行动 , 是否播放动画
        public ActivitiesManager manager;
        public Color campColor;
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
            curProperty = new ActivitiesProperty();
            originProperty = new ActivitiesProperty();
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

        public void UnitColorChange(bool _isGray)
        {
            mMaterial.SetFloat("GrayLone", _isGray ? 1 : 0);
        }

        public void PlayActivityAnim(bool _enabled)
        {
            isPlayingAnim = _enabled;
        }

        public void SetPropertyChange(int _hpChange)
        {
            if (_hpChange < 0)
            {
                _hpChange = Mathf.Abs(_hpChange);
                if (_hpChange >= curProperty.armorValue)
                {
                    curProperty.armorValue = 0;
                    if (_hpChange - curProperty.armorValue >= curProperty.healthValue)
                    {
                        // dead
                        curProperty.healthValue = 0;
                    }
                    else
                    {
                        curProperty.healthValue -= _hpChange - curProperty.armorValue;
                    }
                }
                else
                {
                    curProperty.armorValue -= _hpChange;
                }
            }

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
