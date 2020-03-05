using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MainSpace.Activities
{
    public enum BehaviorPatternType
    {
        SwordMan,                           // 步兵
        SpearMan,                           // 枪兵
        CavalryMan,                         // 骑兵
        Archers,                            // 弓箭兵
        Other
    }

    public class ActivitiesUnit : MonoBehaviour
    {
        public Vector3Int currentPos;
        public int grossHp, grossMp, grossMv;
        public bool isActionOver = false;
        public ActivitiesManager manager;
        private int currentHp, currentMp, currentMv;
        private Material mMaterial;

        public void Start()
        {
            mMaterial = GetComponentInChildren<SpriteRenderer>().material;

            if (manager == null)
            {
                manager = LoadInfo.Instance.activitiesManager;
            }
        }

        public void MoveTo(Vector3Int[] _posArray)
        {
            StopAllCoroutines();
            StartCoroutine(MoveLerp(_posArray));
        }

        public void MoveTo(Vector3Int _pos)
        {
            Vector3Int[] temp = new Vector3Int[] { _pos };
            MoveTo(temp);
        }

        public void UnitColorChange(bool _isGray)
        {
            mMaterial.SetFloat("GrayLone", _isGray ? 1 : 0);
        }

        private IEnumerator MoveLerp(Vector3Int[] _posArray)
        {
            WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
            for (int i = 0; i < _posArray.Length; i++)
            {
                while (Vector3.Distance(transform.position, _posArray[i]) >= 0.05f)
                {
                    transform.position = Vector3.Lerp(transform.position, _posArray[i], 0.1f);
                    yield return endOfFrame;
                }

                transform.position = _posArray[i];
            }
            Debug.Log("over");

            currentPos = _posArray[_posArray.Length - 1];

            // 玩家才会变灰.
            if (true)
            {
                UnitColorChange(true);
            }
            isActionOver = true;
            // 单位完成了移动。
            manager.OnFinishedUnitMove(this);

        }

    }

}
