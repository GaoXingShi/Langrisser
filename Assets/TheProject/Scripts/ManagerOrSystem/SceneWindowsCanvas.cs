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
            public GameObject commanderPlane, soliderPlane;
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

            OnSetActivitiesUnitUIData(localUnit, _index);
            if (localUnit.GetType() == typeof(CommanderUnit))
            {
                CommanderUnit commanderUnit = localUnit as CommanderUnit;
                uiPropertyInspectors[_index].faceImage.sprite = commanderUnit.unitFaceSprite;
                uiPropertyInspectors[_index].commanderAffiliationImage.sprite = commanderUnit.affiliationSprite;

                uiPropertyInspectors[_index].nameText.text = commanderUnit.unitName;
                uiPropertyInspectors[_index].roleText.text = commanderUnit.roleTpe.ToString();
                uiPropertyInspectors[_index].commanderAffiliationText.text = commanderUnit.affiliationName;
                uiPropertyInspectors[_index].levelText.text = "LV " + commanderUnit.levelValue;
                uiPropertyInspectors[_index].levelSlider.maxValue = commanderUnit.levelSliderUpgradeValue;
                uiPropertyInspectors[_index].levelSlider.value = commanderUnit.levelSliderValue;

                uiPropertyInspectors[_index].commanderAttackText.text = string.Concat("攻击力:", commanderUnit.attackValue[0]);
                uiPropertyInspectors[_index].commanderDefenseText.text = string.Concat("防御力:", commanderUnit.defenseValue[0]);
                uiPropertyInspectors[_index].commanderMoveText.text = string.Concat("移动:", commanderUnit.moveRangeValue[0]);
                uiPropertyInspectors[_index].commanderHealthPointText.text = string.Concat("生命值:", commanderUnit.healthValue[0], " / ", commanderUnit.healthValue[1]);
                uiPropertyInspectors[_index].commanderMagicPointText.text = string.Concat("魔法值:", commanderUnit.magicValue[0], " / ", commanderUnit.magicValue[1]);
                uiPropertyInspectors[_index].commandRangeText.text = string.Concat("指挥范围:", commanderUnit.commandRangeValue[0]);
                uiPropertyInspectors[_index].correctedText.text = string.Concat("修正值:", commanderUnit.correctedAttack[0], " / " + commanderUnit.correctedDefense[0]);
            }
            else if (localUnit.GetType() == typeof(SoliderUnit))
            {
                SoliderUnit soliderUnit = localUnit as SoliderUnit;
                if (soliderUnit == null)
                {
                    Debug.LogError("soliderUnit is Null", gameObject);
                    return;
                }
                uiPropertyInspectors[_index].soliderAffiliationImage.sprite = soliderUnit.affiliationSprite;
                uiPropertyInspectors[_index].soliderText.text = soliderUnit.soliderType.ToString();
                uiPropertyInspectors[_index].soliderAffiliationText.text = soliderUnit.affiliationName;

                uiPropertyInspectors[_index].soliderCommanderText.text = string.Concat("指挥官:", soliderUnit.mineCommanderUnit.unitName);
                uiPropertyInspectors[_index].soliderAttackText.text = string.Concat("攻击力:", soliderUnit.attackValue[0], " + ",
                    soliderUnit.isInMineCommanderRange ? soliderUnit.mineCommanderUnit.correctedAttack[0] : 0);
                uiPropertyInspectors[_index].soliderDefenseText.text = string.Concat("防御力:", soliderUnit.defenseValue[0], " + ",
                    soliderUnit.isInMineCommanderRange ? soliderUnit.mineCommanderUnit.correctedDefense[0] : 0);
                uiPropertyInspectors[_index].soliderMoveText.text = string.Concat("移动:", soliderUnit.moveRangeValue[0]);
                uiPropertyInspectors[_index].soliderHealthPointText.text = string.Concat("生命值:", soliderUnit.healthValue[0], " / ", soliderUnit.healthValue[1]);
                uiPropertyInspectors[_index].soliderMagicPointText.text = string.Concat("魔法值:", soliderUnit.magicValue[0], " / ", soliderUnit.healthValue[1]);
            }


        }

        /// <summary>
        /// 清空指定UI面板
        /// </summary>
        /// <param name="_index"></param>
        public void ClearActivitiesUIInfo(int _index)
        {
            intBtnArray.SetActive(false);
            uiPropertyInspectors[_index].soliderPlane.SetActive(false);
            uiPropertyInspectors[_index].commanderPlane.SetActive(false);
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

        private void OnSetActivitiesUnitUIData(ActivitiesUnit _unit, int _index)
        {
            uiPropertyInspectors[_index].commanderPlane.SetActive(_unit.GetType() == typeof(CommanderUnit));
            uiPropertyInspectors[_index].soliderPlane.SetActive(_unit.GetType() == typeof(SoliderUnit));
        }

        /// <summary>
        /// 右下角UI,角色选择期间不允许结束回合。
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