using System;
using System.Collections;
using System.Collections.Generic;
using MainSpace.Grid;
using Sense.BehaviourTree;
using UnityEngine;

namespace MainSpace
{
    public class LoadInfo : MonoBehaviour
    {
        public GameManager gameManager;
        public SceneTileMapManager sceneTileMapManager;
        public ActivitiesManager activitiesManager;
        public AISystem aiSystem;
        public GameCursor gameCursor;
        public SceneWindowsCanvas sceneWindowsCanvas;
        public NumericalAndSettlementSystem numericalAndSettlementSystem;
        public SequenceNode sequenceNode;

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


        void Start()
        {
            sequenceNode.ResetNode(0, 0, null);
            sequenceNode.Execute(true);
        }

    }


}
