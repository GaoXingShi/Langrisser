using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainSpace.Grid
{
    public class AllowUnitInfo : MonoBehaviour
    {
        public SpriteRenderer commandSpriteRenderer,moveSpriteRenderer;
        private void Start() 
        { 

        }

        public void HideMoveGrid()
        {
            moveSpriteRenderer.enabled = false;
        }

        public void ShowMoveGrid()
        {
            moveSpriteRenderer.enabled = true;
        }

        public void ShowCommanderCircleGrid()
        {
            // 需要有动画
            commandSpriteRenderer.enabled = true;
        }
    }
}
