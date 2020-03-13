using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MainSpace.Activities
{
    public enum SoliderType
    {
        步兵,重装步兵,
        骑兵,重装骑兵,
        枪兵,重装枪兵,
    }

    public enum RoleType
    {
        战士,兵长,领主,步兵统帅,将军,
            骑士,骑士长,骑士统帅,封号骑士,
        法师,
        牧师
    }

    public class ActivitiesUnit : MonoBehaviour
    {
        public Vector3Int currentPos;
        public int[] healthValue, magicValue, moveValue, attackValue, defenseValue;        // 索引0为当前，索引1为总量
        public Sprite unitRenderSprite, affiliationSprite;
        public string affiliationName;

        public bool isActionOver = false;                       // 是否结束行动
        public ActivitiesManager manager;

        public SpriteRenderer mRendererComponent;
        public TextMesh hpText;
        public SpriteRenderer professionSprite;
        private Material mMaterial;

        public void Start()
        {
            mMaterial = GetComponentInChildren<SpriteRenderer>().material;
        }

        public virtual void InitData()
        {
            healthValue = magicValue = moveValue = attackValue =
                defenseValue = new int[2];
        }

        public void SetIntArrayData(ref int[] _array, int _value)
        {
            _array = new int[2];
            _array[0] = _value;
            _array[1] = _value;
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
