using System;
using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using UnityEngine;
using UnityEngine.UI;

namespace MainSpace
{
    public class SceneWindowsCanvas : MonoBehaviour
    {
        // 显示属性的root面板
        public GameObject intBtnArray, commanderPlane, soliderPlane;
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

        [Header("UnitOtherActionPlane Link")] public CanvasGroup unitOtherActionPlane;
        public Button roundOverBtn, skillBtn, treatBtn, detailPanelBtn, setUpBtn;
        public SkillPlaneModule skillPlaneModule;

        [Header("NotClickPlane Link")] public CanvasGroup canNotClickImage;

        private ActivitiesUnit cacheActivitiesUnit;

        /// <summary>
        /// 显示ActivitiesUnit信息(目前是鼠标经过Unit单位时，显示信息)
        /// </summary>
        /// <param name="_unit"></param>
        public void ShowActivitiesUIData(ActivitiesUnit _unit)
        {
            ActivitiesUnit localUnit = _unit;

            // 鼠标离开Unit信息
            if (cacheActivitiesUnit == null && localUnit == null)
            {
                ClearActivitiesUIInfo();
                return;
            } // 已经选中Unit时，鼠标离开其他Unit
            else if (cacheActivitiesUnit != null && localUnit == null)
            {
                localUnit = cacheActivitiesUnit;
            }

            if (localUnit != null)
            {
                OnSetActivitiesUnitUIData(localUnit);
            }

            if (localUnit.GetType() == typeof(CommanderUnit))
            {
                CommanderUnit commanderUnit = localUnit as CommanderUnit;
                if (commanderUnit == null)
                {
                    Debug.LogError("commanderUnit is Null", gameObject);
                    return;
                }
                faceImage.sprite = commanderUnit.unitFaceSprite;
                commanderAffiliationImage.sprite = commanderUnit.affiliationSprite;

                nameText.text = commanderUnit.unitName;
                roleText.text = commanderUnit.roleTpe.ToString();
                commanderAffiliationText.text = commanderUnit.affiliationName;
                levelText.text = "LV " + commanderUnit.levelValue;
                levelSlider.maxValue = commanderUnit.levelSliderUpgradeValue;
                levelSlider.value = commanderUnit.levelSliderValue;

                commanderAttackText.text = string.Concat("攻击力:", commanderUnit.attackValue[0]);
                commanderDefenseText.text = string.Concat("防御力:", commanderUnit.defenseValue[0]);
                commanderMoveText.text = string.Concat("移动:", commanderUnit.moveRangeValue[0]);
                commanderHealthPointText.text = string.Concat("生命值:", commanderUnit.healthValue[0], " / ", commanderUnit.healthValue[1]);
                commanderMagicPointText.text = string.Concat("魔法值:", commanderUnit.magicValue[0], " / ", commanderUnit.magicValue[1]);
                commandRangeText.text = string.Concat("指挥范围:", commanderUnit.commandRangeValue[0]);
                correctedText.text = string.Concat("修正值:", commanderUnit.correctedAttack[0], " / " + commanderUnit.correctedDefense[0]);
            }
            else if (localUnit.GetType() == typeof(SoliderUnit))
            {
                SoliderUnit soliderUnit = localUnit as SoliderUnit;
                if (soliderUnit == null)
                {
                    Debug.LogError("soliderUnit is Null", gameObject);
                    return;
                }
                soliderAffiliationImage.sprite = soliderUnit.affiliationSprite;
                soliderText.text = soliderUnit.soliderType.ToString();
                soliderAffiliationText.text = soliderUnit.affiliationName;

                soliderCommanderText.text = string.Concat("指挥官:", soliderUnit.mineCommanderUnit.unitName);
                soliderAttackText.text = string.Concat("攻击力:", soliderUnit.attackValue[0], " + ",
                    soliderUnit.isInMineCommanderRange ? soliderUnit.mineCommanderUnit.correctedAttack[0] : 0);
                soliderDefenseText.text = string.Concat("防御力:", soliderUnit.defenseValue[0], " + ",
                    soliderUnit.isInMineCommanderRange ? soliderUnit.mineCommanderUnit.correctedDefense[0] : 0);
                soliderMoveText.text = string.Concat("移动:", soliderUnit.moveRangeValue[0]);
                soliderHealthPointText.text = string.Concat("生命值:", soliderUnit.healthValue[0], " / ", soliderUnit.healthValue[1]);
                soliderMagicPointText.text = string.Concat("魔法值:", soliderUnit.magicValue[0], " / ", soliderUnit.healthValue[1]);
            }


        }

        /// <summary>
        /// 清空人物的UI面板
        /// </summary>
        public void ClearActivitiesUIInfo()
        {
            intBtnArray.SetActive(false);
            soliderPlane.SetActive(false);
            commanderPlane.SetActive(false);
        }

        /// <summary>
        /// 人物选中方法
        /// </summary>
        /// <param name="_unit"></param>
        public void SetActivitiesData(ActivitiesUnit _unit)
        {
            cacheActivitiesUnit = _unit;

            SetCanNotClickPanelState(false);
            OnSetMenuData(_unit);
        }

        /// <summary>
        /// 刷新单位的状态
        /// </summary>
        public void RefreshActivitiesData()
        {
            if (cacheActivitiesUnit != null)
                ShowActivitiesUIData(cacheActivitiesUnit);
        }

        /// <summary>
        /// 清空选中信息
        /// </summary>
        public void ClearUIInfo(bool _isMouseNotExit = false)
        {
            cacheActivitiesUnit = null;
            if (_isMouseNotExit)
            {
                return;
            }

            ClearActivitiesUIInfo();

            // UnitOtherActionPlane
            roundOverBtn.gameObject.SetActive(true);
            skillBtn.gameObject.SetActive(false);
            treatBtn.gameObject.SetActive(false);
            detailPanelBtn.gameObject.SetActive(false);


            SetCanNotClickPanelState(false);
        }


        public void SetUpPanel()
        {
            ClearActivitiesUIInfo();
            intBtnArray.SetActive(true);

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

            roundOverBtn.onClick.AddListener(TurnOverBtnEvent);

            skillBtn.gameObject.SetActive(false);
            treatBtn.gameObject.SetActive(false);
            detailPanelBtn.gameObject.SetActive(false);

            ClearActivitiesUIInfo();
        }

        private void TurnOverBtnEvent()
        {
            LoadInfo.Instance.gameManager.FinishCurrentRoundTurn();
            ClearUIInfo();
            SetCanNotClickPanelState(true);
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

        private void OnSetActivitiesUnitUIData(ActivitiesUnit _unit)
        {
            commanderPlane.SetActive(_unit.GetType() == typeof(CommanderUnit));
            soliderPlane.SetActive(_unit.GetType() == typeof(SoliderUnit));
        }

        /// <summary>
        /// 右下角UI
        /// </summary>
        /// <param name="_unit"></param>
        private void OnSetMenuData(ActivitiesUnit _unit)
        {
            // UnitOtherActionPlane
            roundOverBtn.gameObject.SetActive(false);
            treatBtn.gameObject.SetActive(_unit.GetType() == typeof(CommanderUnit));
            detailPanelBtn.gameObject.SetActive(true);
        }
    }
}