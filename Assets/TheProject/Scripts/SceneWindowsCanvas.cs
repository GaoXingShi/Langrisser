﻿using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using UnityEngine;
using UnityEngine.UI;

namespace MainSpace
{
    public class SceneWindowsCanvas : MonoBehaviour
    {
        public CanvasGroup intBtnArray, commanderPlane, soliderPlane;
        [Header("IntBtnArray Link")]
        public bool intBtnArrayData;
        public Button turnOverBtn, setAsBtn, saveBtn, loadBtn;
        [Header("CommanderPlane Link")]
        public bool commanderPlaneData;
        public Image faceImage, commanderAffiliationImage;
        public Text nameText, roleText, commanderAffiliationText, levelText;
        public Slider levelSlider;
        public Text commanderAttackText, commanderDefenseText, commanderMoveText, commanderHealthPointText, commanderMagicPointText, commandRangeText, correctedText;
        public Button magicBtn, cureBtn, instructBtn;

        [Header("SoliderPlane Link")]
        public bool soliderPlaneData;
        public Image soliderAffiliationImage;
        public Text soliderText, soliderAffiliationText, soliderAttackText, soliderDefenseText, soliderMoveText, soliderHealthPointText, soliderMagicPointText;

        public void SetActivitiesData(CommanderUnit _unit)
        {
            CanvasGroupAdjust(intBtnArray, false);
            CanvasGroupAdjust(soliderPlane, false);
            CanvasGroupAdjust(commanderPlane, true);

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
            commanderMoveText.text = string.Concat("移动:", _unit.moveValue[0]);
            commanderHealthPointText.text = string.Concat("生命值:", _unit.healthValue[0]);
            commanderMagicPointText.text = string.Concat("魔法值:", _unit.magicValue[0]);
            commandRangeText.text = string.Concat("指挥范围:", _unit.commandRangeValue[0]);
            correctedText.text = string.Concat("修正值:", _unit.correctedAttack[0], " / " + _unit.correctedDefense[0]);
        }

        public void SetActivitiesData(SoliderUnit _unit)
        {
            CanvasGroupAdjust(intBtnArray, false);
            CanvasGroupAdjust(commanderPlane, false);
            CanvasGroupAdjust(soliderPlane, true);

            soliderAffiliationImage.sprite = _unit.affiliationSprite;
            soliderText.text = _unit.soliderType.ToString();
            soliderAffiliationText.text = _unit.affiliationName;
            // todo 修正有问题,离开范围
            soliderAttackText.text = string.Concat("攻击力:", _unit.attackValue[0]," + ",_unit.mineCommanderUnit.correctedAttack[0]);
            soliderDefenseText.text = string.Concat("防御力:", _unit.defenseValue[0], " + ", _unit.mineCommanderUnit.correctedDefense[0]);
            soliderMoveText.text = string.Concat("移动:", _unit.moveValue[0]);
            soliderHealthPointText.text = string.Concat("生命值:", _unit.healthValue[0]);
            soliderMagicPointText.text = string.Concat("魔法值:", _unit.magicValue[0]);
        }

        public void ClearUnitData()
        {
            CanvasGroupAdjust(intBtnArray, true);
            CanvasGroupAdjust(commanderPlane, false);
            CanvasGroupAdjust(soliderPlane, false);
        }

        private void CanvasGroupAdjust(CanvasGroup _group, bool _isAlpha)
        {
            _group.alpha = _isAlpha ? 1 : 0;
            _group.interactable = _isAlpha;
            _group.blocksRaycasts = _isAlpha;
        }
    }
}