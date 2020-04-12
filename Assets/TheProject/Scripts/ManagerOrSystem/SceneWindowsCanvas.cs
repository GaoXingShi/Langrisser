using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using UnityEngine;
using UnityEngine.UI;

namespace MainSpace
{
    public class SceneWindowsCanvas : MonoBehaviour
    {
        public CanvasGroup intBtnArray, commanderPlane, soliderPlane;
        [Header("IntBtnArray Link")] public bool intBtnArrayData;
        public Button turnOverBtn, setAsBtn, saveBtn, loadBtn, cancelBtn;
        [Header("CommanderPlane Link")] public bool commanderPlaneData;
        public Image faceImage, commanderAffiliationImage;
        public Text nameText, roleText, commanderAffiliationText, levelText;
        public Slider levelSlider;

        public Text commanderAttackText,
            commanderDefenseText,
            commanderMoveText,
            commanderHealthPointText,
            commanderMagicPointText,
            commandRangeText,
            correctedText;

        public Button magicBtn, cureBtn, instructBtn;

        [Header("SoliderPlane Link")] public bool soliderPlaneData;
        public Image soliderAffiliationImage;

        public Text soliderText,
            soliderAffiliationText,
            soliderCommanderText,
            soliderAttackText,
            soliderDefenseText,
            soliderMoveText,
            soliderHealthPointText,
            soliderMagicPointText;

        [Header("NotClickPlane Link")] public CanvasGroup canNotClickImage;

        private CommanderUnit cacheCommandUnit;
        private SoliderUnit cacheSoliderUnit;

        /// <summary>
        /// 指挥官选中方法
        /// </summary>
        /// <param name="_unit"></param>
        public void SetActivitiesData(CommanderUnit _unit)
        {
            cacheCommandUnit = _unit;

            CanvasGroupAdjust(intBtnArray, false);
            CanvasGroupAdjust(soliderPlane, false);
            CanvasGroupAdjust(commanderPlane, true);

            SetCanNotClickPanelState(false);


            faceImage.sprite = _unit.unitFaceSprite;
            commanderAffiliationImage.sprite = _unit.affiliationSprite;

            nameText.text = _unit.unitName;
            roleText.text = _unit.roleTpe.ToString();
            commanderAffiliationText.text = _unit.affiliationName;
            levelText.text = "LV " + _unit.levelValue.ToString();
            levelSlider.maxValue = _unit.levelSliderUpgradeValue;
            levelSlider.value = _unit.levelSliderValue;

            commanderAttackText.text = string.Concat("攻击力:", _unit.attackValue[0]);
            commanderDefenseText.text = string.Concat("防御力:", _unit.defenseValue[0]);
            commanderMoveText.text = string.Concat("移动:", _unit.moveRangeValue[0]);
            commanderHealthPointText.text = string.Concat("生命值:", _unit.healthValue[0], " / ", _unit.healthValue[1]);
            commanderMagicPointText.text = string.Concat("魔法值:", _unit.magicValue[0], " / ", _unit.magicValue[1]);
            commandRangeText.text = string.Concat("指挥范围:", _unit.commandRangeValue[0]);
            correctedText.text = string.Concat("修正值:", _unit.correctedAttack[0], " / " + _unit.correctedDefense[0]);
        }

        /// <summary>
        /// 士兵选中方法
        /// </summary>
        /// <param name="_unit"></param>
        public void SetActivitiesData(SoliderUnit _unit)
        {
            cacheSoliderUnit = _unit;

            CanvasGroupAdjust(intBtnArray, false);
            CanvasGroupAdjust(commanderPlane, false);
            CanvasGroupAdjust(soliderPlane, true);

            SetCanNotClickPanelState(false);


            soliderAffiliationImage.sprite = _unit.affiliationSprite;
            soliderText.text = _unit.soliderType.ToString();
            soliderAffiliationText.text = _unit.affiliationName;
            // todo 修正有问题,离开范围
            soliderCommanderText.text = string.Concat("指挥官:",_unit.mineCommanderUnit.unitName);
            soliderAttackText.text = string.Concat("攻击力:", _unit.attackValue[0], " + ", 
                _unit.isInMineCommanderRange ? _unit.mineCommanderUnit.correctedAttack[0] : 0);
            soliderDefenseText.text = string.Concat("防御力:", _unit.defenseValue[0], " + ",
                _unit.isInMineCommanderRange ? _unit.mineCommanderUnit.correctedDefense[0] : 0);
            soliderMoveText.text = string.Concat("移动:", _unit.moveRangeValue[0]);
            soliderHealthPointText.text = string.Concat("生命值:", _unit.healthValue[0], " / ", _unit.healthValue[1]);
            soliderMagicPointText.text = string.Concat("魔法值:", _unit.magicValue[0], " / ", _unit.healthValue[1]);
        }

        public void RefreshActivitiesData()
        {
            if (cacheCommandUnit)
            {
                faceImage.sprite = cacheCommandUnit.unitFaceSprite;
                commanderAffiliationImage.sprite = cacheCommandUnit.affiliationSprite;

                nameText.text = cacheCommandUnit.unitName;
                roleText.text = cacheCommandUnit.roleTpe.ToString();
                commanderAffiliationText.text = cacheCommandUnit.affiliationName;
                levelText.text = "LV " + cacheCommandUnit.levelValue.ToString();
                levelSlider.maxValue = cacheCommandUnit.levelSliderUpgradeValue;
                levelSlider.value = cacheCommandUnit.levelSliderValue;

                commanderAttackText.text = string.Concat("攻击力:", cacheCommandUnit.attackValue[0]);
                commanderDefenseText.text = string.Concat("防御力:", cacheCommandUnit.defenseValue[0]);
                commanderMoveText.text = string.Concat("移动:", cacheCommandUnit.moveRangeValue[0]);
                commanderHealthPointText.text = string.Concat("生命值:", cacheCommandUnit.healthValue[0], " / ", cacheCommandUnit.healthValue[1]);
                commanderMagicPointText.text = string.Concat("魔法值:", cacheCommandUnit.magicValue[0], " / ", cacheCommandUnit.magicValue[1]);
                commandRangeText.text = string.Concat("指挥范围:", cacheCommandUnit.commandRangeValue[0]);
                correctedText.text = string.Concat("修正值:", cacheCommandUnit.correctedAttack[0], " / " + cacheCommandUnit.correctedDefense[0]);
            }
            else if (cacheSoliderUnit)
            {
                soliderAffiliationImage.sprite = cacheSoliderUnit.affiliationSprite;
                soliderText.text = cacheSoliderUnit.soliderType.ToString();
                soliderAffiliationText.text = cacheSoliderUnit.affiliationName;
                // todo 修正有问题,离开范围
                soliderCommanderText.text = string.Concat("指挥官:", cacheSoliderUnit.mineCommanderUnit.unitName);
                soliderAttackText.text = string.Concat("攻击力:", cacheSoliderUnit.attackValue[0], " + ",
                    cacheSoliderUnit.isInMineCommanderRange ? cacheSoliderUnit.mineCommanderUnit.correctedAttack[0] : 0);
                soliderDefenseText.text = string.Concat("防御力:", cacheSoliderUnit.defenseValue[0], " + ",
                    cacheSoliderUnit.isInMineCommanderRange ? cacheSoliderUnit.mineCommanderUnit.correctedDefense[0] : 0);
                soliderMoveText.text = string.Concat("移动:", cacheSoliderUnit.moveRangeValue[0]);
                soliderHealthPointText.text = string.Concat("生命值:", cacheSoliderUnit.healthValue[0], " / ", cacheSoliderUnit.healthValue[1]);
                soliderMagicPointText.text = string.Concat("魔法值:", cacheSoliderUnit.magicValue[0], " / ", cacheSoliderUnit.healthValue[1]);
            }
        }

        /// <summary>
        /// 清空选中信息
        /// </summary>
        public void ClearUIInfo()
        {
            cacheSoliderUnit = null;
            cacheCommandUnit = null;

            CanvasGroupAdjust(intBtnArray, false);
            CanvasGroupAdjust(commanderPlane, false);
            CanvasGroupAdjust(soliderPlane, false);

            SetCanNotClickPanelState(false);

        }
        public void SetInitPanel()
        {
            CanvasGroupAdjust(intBtnArray, true);
            CanvasGroupAdjust(commanderPlane, false);
            CanvasGroupAdjust(soliderPlane, false);

            SetCanNotClickPanelState(true);
        }

        /// <summary>
        /// 设置限制点击的Panel
        /// </summary>
        /// <param name="_enabled"></param>
        public void SetCanNotClickPanelState(bool _enabled)
        {
            CanvasGroupAdjust(canNotClickImage, _enabled);
        }

        private void Start()
        {
            turnOverBtn.onClick.AddListener(TurnOverBtnEvent);
            setAsBtn.onClick.AddListener(SetAsBtnEvent);
            saveBtn.onClick.AddListener(SaveBtnEvent);
            loadBtn.onClick.AddListener(LoadBtnEvent);
            cancelBtn.onClick.AddListener(CancelBtnEvent);
        }

        private void TurnOverBtnEvent()
        {
            LoadInfo.Instance.gameManager.FinishCurrentRoundTurn();
            ClearUIInfo();
        }

        private void SetAsBtnEvent()
        {

        }

        private void SaveBtnEvent()
        {

        }

        private void LoadBtnEvent()
        {

        }

        private void CancelBtnEvent()
        {
            ClearUIInfo();
        }

        private void CanvasGroupAdjust(CanvasGroup _group, bool _isAlpha)
        {
            _group.alpha = _isAlpha ? 1 : 0;
            _group.interactable = _isAlpha;
            _group.blocksRaycasts = _isAlpha;
        }
    }
}