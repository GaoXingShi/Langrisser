using MainSpace.Grid;
using MainSpace.SkillCommandSpace;
using Sense.BehaviourTree;
using UnityEngine;

namespace MainSpace
{
    public class LoadInfo : MonoBehaviour
    {
        public GameManager gameManager;
        public CommandEventQueue commandEventQueue;
        public SceneTileMapManager sceneTileMapManager;
        public ActivitiesManager activitiesManager;
        public AISystem aiSystem;
        public GameCursor gameCursor;
        public SceneWindowsCanvas sceneWindowsCanvas;
        public NumericalAndSettlementSystem numericalAndSettlementSystem;
        public SequenceNode sequenceNode;
        public SkillSystem skillSystem;


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

            numericalAndSettlementSystem = new NumericalAndSettlementSystem();
            commandEventQueue = new CommandEventQueue();
            skillSystem = new SkillSystem();
        }


        void Start()
        {
            sequenceNode.ResetNode(0, 0, null);
            sequenceNode.Execute(true);
        }

    }


}
