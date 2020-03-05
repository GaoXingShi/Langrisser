using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using UnityEngine;
using UnityEngine.UI;

namespace MainSpace
{
    public class SceneWindowsCanvas : MonoBehaviour
    {
        public CanvasGroup intBtnArray, particularPlane;
        [Header("IntBtnArray Link")]
        public bool intBtnArrayData;
        public Button turnOverBtn,setAsBtn, saveBtn, loadBtn;
        [Header("ParticularPlane Link")]
        public bool particularPlaneData;
        public Image faceImage, affiliationImage;
        public Text nameText, roleText, affiliationText , levelText;
        public Slider levelSlider;
        public Text attackText, defenseText, moveText, healthPointText, magicPointText, commandRangeText, correctedText;
        public Button magicBtn, cureBtn, instructBtn;


        public void SetData(ActivitiesUnit _unit)
        {
            CanvasGroupAdjust(intBtnArray, false);
            CanvasGroupAdjust(particularPlane, true);
        }

        public void ClearUnitData()
        {
            CanvasGroupAdjust(intBtnArray, true);
            CanvasGroupAdjust(particularPlane, false);
        }

        private void CanvasGroupAdjust(CanvasGroup _group,bool _isAlpha)
        {
            _group.alpha = _isAlpha ? 1 : 0;
            _group.interactable = _isAlpha;
            _group.blocksRaycasts = _isAlpha;
        }
    }
}