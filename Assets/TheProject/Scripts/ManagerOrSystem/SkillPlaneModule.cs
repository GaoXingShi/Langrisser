using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainSpace
{
    public class SkillPlaneModule : MonoBehaviour
    {
        [Header("Prefabs")] public Button skillBtn;
        // 显示信息的面板
        [Header("Link")]
        public CanvasGroup showInfoPlane;
        public Transform scrollViewContent;

        private CanvasGroup theCanvas;
        private void Start()
        {
            theCanvas = GetComponent<CanvasGroup>();
        }

        public void CanvasActive(bool _isActive)
        {
            theCanvas.alpha = _isActive ? 1 : 0;
            theCanvas.blocksRaycasts = _isActive;
            theCanvas.interactable = _isActive;
        }

    }
}