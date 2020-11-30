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
        public GameObject intBtnArray;
        [Header("IntBtnArray Link")] public bool intBtnArrayData;
        public Button turnOverBtn, setAsBtn, saveBtn, loadBtn, cancelBtn;

        [System.Serializable]
        public struct UIPropertyInspector
        {
            public GameObject infoPanel;
            public Image activitiesUnitImage , roleUnitImage;
            public Text nameText, roleText, levelText;
            public Slider levelSlider;

            public Text attackText,
                defenseText,
                healthPointText,
                magicPointText;
        }

        public UIPropertyInspector[] uiPropertyInspectors;

        [Header("UnitOtherActionPlane Link")] public CanvasGroup unitOtherActionPlane;
        public Button roundOverBtn, skillBtn, treatBtn, detailPanelBtn, setUpBtn;
        public SkillPlaneModule skillPlaneModule;

        [Header("NotClickPlane Link")] public CanvasGroup canNotClickImage;

        /// <summary>
        /// 显示ActivitiesUnit信息(目前是鼠标经过Unit单位时，显示信息)
        /// </summary>
        /// <param name="_unit"></param>
        /// <param name="_index"></param>
        public void ShowActivitiesUIData(ActivitiesUnit _unit, int _index)
        {
            ActivitiesUnit localUnit = _unit;

            if (_index >= uiPropertyInspectors.Length)
            {
                Debug.LogError("array index exceed Range");
                return;
            }
            if (localUnit == null) { return; }

            uiPropertyInspectors[_index].infoPanel.SetActive(true);

            // 通用
            {
                uiPropertyInspectors[_index].activitiesUnitImage.sprite = localUnit.activityConfig.normalSprite;
                //uiPropertyInspectors[_index].roleUnitImage.sprite = localUnit.activityConfig.normalSprite;
            }

            if (localUnit.GetType() == typeof(CommanderUnit))
            {
                CommanderUnit commanderUnit = localUnit as CommanderUnit;
                if (commanderUnit == null)
                {
                    Debug.LogError("commanderUnit is Null", gameObject);
                    return;
                }

                uiPropertyInspectors[_index].nameText.text = commanderUnit.unitName;
                uiPropertyInspectors[_index].roleText.text = commanderUnit.role;
                uiPropertyInspectors[_index].levelText.text = "LV " + commanderUnit.levelValue;
                uiPropertyInspectors[_index].levelSlider.maxValue = commanderUnit.levelSliderUpgradeValue;
                uiPropertyInspectors[_index].levelSlider.value = commanderUnit.levelSliderValue;

                uiPropertyInspectors[_index].attackText.text = string.Concat("AT:", commanderUnit.attackValue[0]);
                uiPropertyInspectors[_index].defenseText.text = string.Concat("DF:", commanderUnit.defenseValue[0]);
                uiPropertyInspectors[_index].healthPointText.text = string.Concat("HP:", commanderUnit.healthValue[0], " / ", commanderUnit.healthValue[1]);
                uiPropertyInspectors[_index].magicPointText.text = string.Concat("MP:", commanderUnit.magicValue[0], " / ", commanderUnit.magicValue[1]);
            }
            else if (localUnit.GetType() == typeof(SoliderUnit))
            {
                SoliderUnit soliderUnit = localUnit as SoliderUnit;
                if (soliderUnit == null)
                {
                    Debug.LogError("soliderUnit is Null", gameObject);
                    return;
                }

                uiPropertyInspectors[_index].nameText.text = soliderUnit.mineCommanderUnit.unitName;
                uiPropertyInspectors[_index].roleText.text = soliderUnit.FightType.ToString();
                uiPropertyInspectors[_index].levelText.text = "LV " + (soliderUnit.isInMineCommanderRange ? soliderUnit.mineCommanderUnit.levelValue : 0);
                uiPropertyInspectors[_index].levelSlider.maxValue = (soliderUnit.isInMineCommanderRange ? soliderUnit.mineCommanderUnit.levelSliderUpgradeValue : 0);
                uiPropertyInspectors[_index].levelSlider.value = (soliderUnit.isInMineCommanderRange ? soliderUnit.mineCommanderUnit.levelSliderValue : 0);

                uiPropertyInspectors[_index].attackText.text = string.Concat("AT:", soliderUnit.attackValue[0], "\t+",
                    soliderUnit.isInMineCommanderRange ? soliderUnit.mineCommanderUnit.correctedAttack[0] : 0);
                uiPropertyInspectors[_index].defenseText.text = string.Concat("DF:", soliderUnit.defenseValue[0], "\t+",
                    soliderUnit.isInMineCommanderRange ? soliderUnit.mineCommanderUnit.correctedDefense[0] : 0);
                uiPropertyInspectors[_index].healthPointText.text = string.Concat("HP:", soliderUnit.healthValue[0], " / ", soliderUnit.healthValue[1]);
                uiPropertyInspectors[_index].magicPointText.text = string.Concat("MP:", soliderUnit.magicValue[0], " / ", soliderUnit.healthValue[1]);
            }


        }

        /// <summary>
        /// 清空指定UI面板
        /// </summary>
        /// <param name="_index"></param>
        public void ClearActivitiesUIInfo(int _index)
        {
            intBtnArray.SetActive(false);
            uiPropertyInspectors[_index].infoPanel.SetActive(false);
        }

        /// <summary>
        /// 清空UI面板
        /// </summary>
        public void ClearActivitiesUIInfo()
        {
            intBtnArray.SetActive(false);
            for (int i = 0; i < uiPropertyInspectors.Length; i++)
            {
                ClearActivitiesUIInfo(i);
            }
        }

        /// <summary>
        /// 人物选中方法
        /// </summary>
        /// <param name="_unit"></param>
        public void SetActivitiesData(ActivitiesUnit _unit,int _index)
        {
            //cacheActivitiesUnit = _unit;
            ShowActivitiesUIData(_unit, _index);
            SetCanNotClickPanelState(false);
            OnSetMenuData(_unit);
        }

        /// <summary>
        /// 刷新单位的状态
        /// </summary>
        public void RefreshActivitiesData(ActivitiesUnit _unit,int _index)
        {
            if (_unit != null)
                ShowActivitiesUIData(_unit, _index);
        }

        /// <summary>
        /// 清空选中信息
        /// </summary>
        public void ClearUIInfo()
        {
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


        /// <summary>
        /// 右下角UI,角色选择期间不允许结束回合。
        /// </summary>
        /// <param name="_unit"></param>
        private void OnSetMenuData(ActivitiesUnit _unit)
        {
            // UnitOtherActionPlane
            roundOverBtn.gameObject.SetActive(false);
            treatBtn.gameObject.SetActive(false);
            detailPanelBtn.gameObject.SetActive(true);
        }
    }
}