using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainSpace
{
    [Flags]
    public enum SkillFlag
    {
        none = 0,
        魔法箭 = 1,
        火球 = 2,
        冰刺 = 4,
    }

    public class SkillSystem : MonoBehaviour
    {
        public SkillFlag skillFlag;
        void Start()
        {
        }

        void OnGUI()
        {
            GUI.Label(new Rect(Vector2.one,Vector2.one * 200) ,(skillFlag).ToString() );
            GUI.Label(new Rect(Vector2.up * 50,Vector2.one * 200) ,(IsContain(skillFlag,SkillFlag.冰刺)).ToString() );
        }

        public bool IsContain(SkillFlag _allSkill, SkillFlag _sthSkill)
        {
            // 与运算， 如果sthSkill 与 _allSkill并不对应则会返回0
            return 0 != (_allSkill & _sthSkill);
        }


    }

}

