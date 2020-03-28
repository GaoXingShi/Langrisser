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
                activitiesManager.UnitMoveTo(sceneTileMapManager.GetMoveToUnitAllow(commanderMovingData[0].widthHeighValue), v, CtrlType.AI);
                activitiesManager.ExitCommanderOrSoliderUnit();
                yield return WaitMoveOn(v);
                activitiesManager.EnterCommanderOrSoliderUnit(v);

                foreach (var vv in v.GetSoliderUnitArray())
                {
                    TileSaveData[] soliderMovingData = sceneTileMapManager.CalculateMovingRange(vv);

                    activitiesManager.UnitMoveTo(sceneTileMapManager.GetMoveToUnitAllow(soliderMovingData[0].widthHeighValue), vv, CtrlType.AI);

                    yield return WaitMoveOn(vv);
                }
                activitiesManager.ExitCommanderOrSoliderUnit();

                // 主要为了去掉cacheSaveData数据
                sceneTileMapManager.ClearCacheSaveData();
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
