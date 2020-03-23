using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class gaoxingshiEditor : Editor
{
    [MenuItem("Tools/GaoXingShiEditor/将选中的AnimTile添加选中的roleTile中")]
    public static void EditorMethod1()
    {
        var selections = Selection.objects;
        var ruleTile = selections.FirstOrDefault(x => x.GetType() == typeof(RuleTile)) as RuleTile;
        var otherTile = selections.Where(x => x.GetType() == typeof(AnimatedTile));

        ruleTile.m_TilingRules = new List<RuleTile.TilingRule>();
        foreach (AnimatedTile v in otherTile)
        {
            var temp = new RuleTile.TilingRule();
            temp.m_Output = RuleTile.TilingRule.OutputSprite.Animation;
            temp.m_Sprites = v.m_AnimatedSprites;
            ruleTile.m_TilingRules.Add(temp);
        }

    }

    [MenuItem("Tools/GaoXingShiEditor/将选中的AnimTile的所有Sprite选中")]
    public static void EditorMethod2()
    {
        var selection = UnityEditor.Selection.activeObject as AnimatedTile;
        Selection.objects = selection.m_AnimatedSprites;

    }

}
