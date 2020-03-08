using System;
using System.Linq;
using Sense.BehaviourTree;
using UnityEditor;
using UnityEngine;

namespace Sense.BehaviourTree.Editor
{
    [InitializeOnLoad]
    internal class BehaviourTreeHierarchyEditor
    {
        private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;
        static BehaviourTreeHierarchyEditor()
        {
            //EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
            // 刷新Hierarchy面板。
            EditorApplication.update += EditorApplication.RepaintHierarchyWindow;

            hiearchyItemCallback = HierarchyItemCB;
            EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, hiearchyItemCallback);
        }

        private static void HierarchyItemCB(int instanceid, Rect selectionrect)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceid) as GameObject;
            if (obj != null)
            {
                if (!obj.activeSelf)
                {
                    return;
                }

                if (obj.GetComponent<BehaviourNode>() != null)
                {
                    var targetScript = obj.GetComponent<BehaviourNode>();
                    Rect boxRect = new Rect(selectionrect) { x = 0 };
                    boxRect.width = boxRect.width + 200;

                    string targetState = "Node";
                    Color stateColor = new Color(221 / 255.0f, 255 / 255.0f, 215 / 255.0f, 1);
                    if (Application.isPlaying)
                    {
                        targetState = "Disable";
                        stateColor = Color.yellow;
                        if (targetScript.State == NodeState.Ready)
                        {
                            targetState = "Ready";
                            stateColor = Color.white;
                            stateColor.a = 0.7f;
                        }
                        else if (targetScript.State == NodeState.Running)
                        {
                            targetState = "Run";
                            stateColor = Color.white;
                        }
                        else if (targetScript.State == NodeState.Succeed)
                        {
                            targetState = "Succeed";
                            stateColor = Color.green;
                        }
                        else if (targetScript.State == NodeState.Failed)
                        {
                            targetState = "Failed";
                            stateColor = Color.red;
                        }
                    }

                    var selectionObjs = Selection.gameObjects;
                    Color cjNormalColor = new Color(1, 1, 1, 0.05f);
                    Color cjSelectionColor = new Color(221 / 255.0f, 255 / 255.0f, 215 / 255.0f, 0.2f);
                    EditorGUI.DrawRect(boxRect, selectionObjs.Contains(obj) ? cjSelectionColor : cjNormalColor);

                    Rect labelRect = new Rect(selectionrect);
                    labelRect.x = labelRect.xMax - 30;
                    labelRect.y = labelRect.y + 1.5f;
                    GUIStyle style = new GUIStyle
                    {
                        fontSize = 9,
                        normal = {textColor = stateColor}
                    };
                    
                    GUI.Label(labelRect, targetState, style);
                }
                else if (obj.GetComponent<TriggerBehaviour>() != null)
                {
                    var targetScript = obj.GetComponent<TriggerBehaviour>();
                    Rect boxRect = new Rect(selectionrect) { x = 0 };
                    boxRect.width = boxRect.width + 200;

                    string targetState = "Trigger";
                    Color stateColor = new Color(221 / 255.0f, 255 / 255.0f, 215 / 255.0f, 1);
                    if (Application.isPlaying)
                    {
                        targetState = "Wait";
                        stateColor = Color.white;

                        if (targetScript.IsRunning)
                        {
                            targetState = "Run";
                            stateColor = Color.white;
                        }
                    }

                    var selectionObjs = Selection.gameObjects;
                    Color cjNormalColor = new Color(1, 1, 1, 0.05f);
                    Color cjSelectionColor = new Color(221 / 255.0f, 255 / 255.0f, 215 / 255.0f, 0.2f);
                    EditorGUI.DrawRect(boxRect, selectionObjs.Contains(obj) ? cjSelectionColor : cjNormalColor);
                    //EditorGUI.HelpBox(selectionrect, "Helpaaaaa", MessageType.Error);
                    Rect labelRect = new Rect(selectionrect);
                    labelRect.x = labelRect.xMax - 30;
                    labelRect.y = labelRect.y + 1.5f;

                    GUIStyle style = new GUIStyle
                    {
                        fontSize = 9,
                        normal = { textColor = stateColor }
                    };
                    GUI.Label(labelRect, targetState, style);
                }
            }
        }

    }

}