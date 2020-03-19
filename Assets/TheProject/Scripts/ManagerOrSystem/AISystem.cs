using System.Collections;
using System.Collections.Generic;
using MainSpace.Activities;
using MainSpace.Grid;
using UnityEngine;

namespace MainSpace
{
    public enum AIBehaviourType
    {
        整队,
    }

    public class AISystem : MonoBehaviour
    {
        private GameManager gameManager;
        private ActivitiesManager activitiesManager;
        private SceneTileMapManager sceneTileMapManager;
        private SceneTileMapManager tileMapManager;

        void Start()
        {
            gameManager = LoadInfo.Instance.gameManager;
            activitiesManager = LoadInfo.Instance.activitiesManager;
            sceneTileMapManager = LoadInfo.Instance.sceneTileMapManager;
            tileMapManager = LoadInfo.Instance.sceneTileMapManager;
        }

        public void SetAIData(CommanderUnit[] _commanderArray)
        {
            StartCoroutine(AIAction(_commanderArray));
        }

        private IEnumerator AIAction(CommanderUnit[] _commanderArray)
        {
            foreach (var v in _commanderArray)
            {
                TileSaveData[] commanderMovingData = sceneTileMapManager.CalculateMovingRange(v);
                activitiesManager.EnterCommanderOrSoliderUnit(v);
                yield return new WaitForSeconds(2);
                activitiesManager.UnitMoveTo(commanderMovingData[0].widthHeighValue + new Vector3Int(0, 0, -1),v);
                activitiesManager.ExitCommanderOrSoliderUnit();
                yield return WaitMoveOn(v);
                activitiesManager.EnterCommanderOrSoliderUnit(v);

                foreach (var vv in v.GetSoliderUnitArray())
                {
                    TileSaveData[] soliderMovingData = sceneTileMapManager.CalculateMovingRange(vv);
                    activitiesManager.UnitMoveTo(soliderMovingData[0].widthHeighValue + new Vector3Int(0, 0, -1), vv);

                    yield return WaitMoveOn(vv);
                }
                activitiesManager.ExitCommanderOrSoliderUnit();

            }

            gameManager.FinishCurrentRoundTurn();
        }

        private IEnumerator WaitMoveOn(ActivitiesUnit _unit)
        {
            WaitForSeconds wait = new WaitForSeconds(0.1f);

            while (true)
            {
                if (_unit.isActionOver)
                    break;
                yield return wait;
            }
        }

    }
}
