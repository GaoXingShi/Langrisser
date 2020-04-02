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

        private void Update()
        {
            if (commandSpriteRenderer.enabled)
            {
                Color temp = commandSpriteRenderer.color;
                temp.a = LoadInfo.Instance.sceneTileMapManager.colorAValue / 255.0f;
                commandSpriteRenderer.color = temp;
            }
        }

        public void SetMoveGrid(bool _enabled)
        {
            moveSpriteRenderer.enabled = _enabled;
        }

        public void SetCommanderCircleGrid(bool _enabled,Color _color)
        {
            if (_enabled == commandSpriteRenderer.enabled)
            {
                return;
            }


            // 需要有动画
            commandSpriteRenderer.enabled = _enabled;
            commandSpriteRenderer.color = _color;
        }

    }
}
