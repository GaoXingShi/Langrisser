using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sense.BehaviourTree
{
    /// <summary>
    /// 创建这个节点时，需要传入一个节点队列。当运行到这个节点时。他的子节点会一个接一个的运行。
    /// 如果他的子节点状态是SUCCESS，那么他会运行下一个；
    /// 如果他的子节点状态是RUNNING，那么他会将自身也标识成RUNNING，并且等待节点返回其他结果；
    /// 如果他的子节点状态是FAILED，那么他会把自己的状态标识为FAILED并且直接返回。
    /// 如果所有节点都返回结尾为SUCCESS，那么他会将自身标识成为SUCCESS并且返回。
    /// </summary>
    public class SequenceNode : CompositesNode
    {
        private int currentNodeNumber = 0;

        #region OverrideMethod
        public override void Execute(bool isLinear)
        {
            if (nodes.Count != 0)
            {
                nodes[currentNodeNumber].Execute(isLinear);
                base.Execute(isLinear);
            }
            else
            {
                State = NodeState.Succeed;
            }
        }

        public override void ResetNode(int _depth, int _nodeNumber, BehaviourNode _parentNode, ThroughEnum _through = ThroughEnum.Depth)
        {
            currentNodeNumber = 0;
            base.ResetNode(_depth, _nodeNumber, _parentNode, _through);
        }

        public override void Abort(NodeState _state)
        {
            currentNodeNumber = 0;
            foreach (var v in nodes.Where(x => x.State == NodeState.Ready || x.State == NodeState.Running))
            {
                v.Abort(_state);
            }

            base.Abort(_state);
        }

        #endregion

        #region TestMethod
        /// <summary>
        /// 初始化加执行方法
        /// </summary>
        [UnityEngine.ContextMenu("Rest And Execute")]
        public void Test01()
        {
            ResetNode(Depth, NodeNumber, this);
            Execute(true);
        }

        /// <summary>
        /// 跳过该复合节点
        /// </summary>
        [UnityEngine.ContextMenu("Abort To Success")]
        public void Test02()
        {
            Abort(NodeState.Succeed);
        }

        /// <summary>
        /// 忽略该复合节点
        /// </summary>
        [UnityEngine.ContextMenu("Abort To Disable")]
        public void Test03()
        {
            Abort(NodeState.Disable);
        }

        /// <summary>
        /// 强制将当前叶子节点Success
        /// </summary>
        public void CurrentNodeForceSuccess()
        {
            if (Depth == 0 && State == NodeState.Ready)
            {
                Execute(true);
            }

            if (nodes[currentNodeNumber].GetType() == typeof(SequenceNode))
            {
                (nodes[currentNodeNumber] as SequenceNode)?.CurrentNodeForceSuccess();
            }
            else
            {
                if (nodes[currentNodeNumber].State != NodeState.Succeed)
                    nodes[currentNodeNumber].Abort(NodeState.Succeed);
            }
        }

        /// <summary>
        /// 强制将当前叶子节点Ready
        /// </summary>
        public void CurrentNodeForceReady()
        {
            if (State == NodeState.Ready)
            {
                return;
            }

            if (currentNodeNumber == 0)
            {
                State = NodeState.Ready;
                if (ParentNode != null)
                    ((SequenceNode)ParentNode).CurrentNodeForceReady();
                return;
            }
            currentNodeNumber--;

            if ((nodes[currentNodeNumber].GetType() == typeof(SequenceNode) && nodes[currentNodeNumber].transform.childCount == 0))
            {
                nodes[currentNodeNumber].Abort(NodeState.Ready);
                CurrentNodeForceReady();
                return;
            }
            nodes[currentNodeNumber].Execute(false);
            State = NodeState.Running;
        }

        #endregion

        protected void Update()
        {
            if (State != NodeState.Running)
            {
                return;
            }

            // Node节点关闭的话就会跳过。
            if (!nodes[currentNodeNumber].isActiveAndEnabled)
            {
                nodes[currentNodeNumber].Abort(NodeState.Disable);
                NodeNumberPlusPlus();
                return;
            }

            // 检测Node的状态
            if (nodes[currentNodeNumber].State == NodeState.Succeed || nodes[currentNodeNumber].State == NodeState.Disable)
            {
                NodeNumberPlusPlus();
            }
            else if (nodes[currentNodeNumber].State == NodeState.Failed)
            {
                FinishNode(NodeState.Failed, NodeState.Disable);
            }

        }

        private void NodeNumberPlusPlus()
        {
            currentNodeNumber++;

            if (currentNodeNumber == nodes.Count)
            {
                FinishNode(NodeState.Succeed, NodeState.Succeed);
                //currentNodeNumber = nodes.Count - 1;
            }
            else
            {
                nodes[currentNodeNumber].Execute(true);
            }
        }


        private void FinishNode(NodeState _state, NodeState _otherNodeState)
        {
            currentNodeNumber = nodes.Count - 1;
            State = _state;

            foreach (var v in nodes.Where(x => x.State == NodeState.Ready || x.State == NodeState.Running))
            {
                v.Abort(_otherNodeState);
            }
        }

    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(SequenceNode))]
    internal class SequenceNodeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Application.isPlaying)
            {
                var targetNode = target as SequenceNode;
                if (targetNode == null)
                {
                    return;
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Rest And Execute"))
                {
                    targetNode.Test01();
                }
                if (GUILayout.Button("Abort To Success"))
                {
                    targetNode.Test02();
                }
                if (GUILayout.Button("Abort To Disable"))
                {
                    targetNode.Test03();
                }
                GUILayout.EndHorizontal();

            }
        }
    }
#endif
}

