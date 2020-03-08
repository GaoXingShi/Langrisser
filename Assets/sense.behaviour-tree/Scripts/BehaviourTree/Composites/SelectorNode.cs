using System.Linq;
using UnityEngine;

namespace Sense.BehaviourTree
{
    /// <summary>
    /// 与顺序节点类似，创建时需要传入一个节点列表，当运行到这个节点时，他的节点会一个接一个的运行。
    /// 如果他的子节点是SUCCESS，那么他会将自身标识成为SUCCESS并且直接返回；
    /// 如果他的子节点状态是RUNNING，那么他会将自身也标识成RUNNING，并且等待节点返回其他结果；
    /// 任何一个节点都没有返回SUCCESS的情况下，他将会将自身标识成为FAILED并且返回
    /// 如果他的子节点状态是FAILED，那么他会等待其他节点。
    /// 所有子节点都是FAILED的情况下，他将会将自身标识成为FAILED并且返回。
    /// </summary>
    public class SelectorNode : CompositesNode
    {

        #region OverrideMethod

        public override void Execute(bool _isLinear)
        {
            foreach (var v in nodes)
            {
                if (v.isActiveAndEnabled)
                {
                    v.Execute(_isLinear);
                }
                else
                {
                    v.Abort(NodeState.Disable);
                }
            }

            if (nodes.Count == 0)
            {
                State = NodeState.Succeed;
            }
            else
            {
                base.Execute(_isLinear);
            }
        }

        public override void Abort(NodeState _state)
        {
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
            ResetNode(Depth, NodeNumber, ParentNode);
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

        #endregion

        private void FinishNode(NodeState _state, NodeState _otherNodeState)
        {
            State = _state;

            foreach (var v in nodes.Where(x => x.State == NodeState.Ready || x.State == NodeState.Running))
            {
                v.Abort(_otherNodeState);
            }
        }
        protected void Update()
        {
            if (State != NodeState.Running)
            {
                return;
            }

            if (nodes.Count(x => x.State == NodeState.Succeed) > 0)
            {
                FinishNode(NodeState.Succeed, NodeState.Disable);
            }

            if (nodes.Count(x => x.State == NodeState.Failed) == nodes.Count)
            {
                FinishNode(NodeState.Failed, NodeState.Failed);
            }
        }
    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(SelectorNode))]
    public class SelectorNodeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Application.isPlaying)
            {
                var targetNode = target as SelectorNode;
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
