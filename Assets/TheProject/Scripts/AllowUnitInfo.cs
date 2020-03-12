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

        public void SetMoveGrid(bool _enabled)
        {
            moveSpriteRenderer.enabled = _enabled;
        }

        public void SetCommanderCircleGrid(bool _enabled)
        {
            // 需要有动画
            commandSpriteRenderer.enabled = _enabled;
        }
    }
}
