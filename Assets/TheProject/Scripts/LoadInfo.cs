using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainSpace
{
    public class LoadInfo : MonoBehaviour
    {
        public GameManager gameManager;
        public SceneTileMapManager sceneTileMapManager;
        public ActivitiesManager activitiesManager;
        public GameCursor gameCursor;
        public SceneWindowsCanvas sceneWindowsCanvas;

        public static LoadInfo Instance;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }

        }

    }


}
