using UnityEngine;

namespace Sense.BehaviourTree
{
    [System.Serializable]
    public enum NodeState
    {
        Disable,                                    // 未初始化
        Ready,                                      // 初始化
        Running,                                    // 运行中
        Succeed,                                    // 成功结果
        Failed,                                     // 失败结果
    }

    public enum ThroughEnum
    {
        Breadth,                                    // 广度遍历
        Depth                                       // 深度遍历
    }

    /// <summary>
    /// 广度遍历时需要用到的结构体
    /// </summary>
    public struct NodeReseterData
    {
        public BehaviourNode reseterNode;           // Reset的节点
        public int depth;                           // 深度
        public BehaviourNode parentNode;            // Reset的父节点
    }


    [DisallowMultipleComponent]
    public class BehaviourNode : MonoBehaviour
    {
        /// <summary>
        /// 任务是否正在执行中
        /// </summary>
        private NodeState state;
        private BehaviourNode parentNode;
        private int depth;
        private int nodeNumber;

        /// <summary>
        /// 节点状态
        /// </summary>
        public NodeState State
        {
            get => state;
            protected set
            {
                NodeState temp = state;
                state = value;
                StateChanged(temp, state);
            }
        }

        /// <summary>
        /// 节点深度
        /// </summary>
        public int Depth
        {
            get => depth;
            protected set => depth = value;
        }

        /// <summary>
        /// 节点编号
        /// </summary>
        public int NodeNumber
        {
            get => nodeNumber;
            set => nodeNumber = value;
        }

        /// <summary>
        /// 父节点
        /// </summary>
        public BehaviourNode ParentNode
        {
            get => parentNode;
            protected set => parentNode = value;
        }

        // 当节点状态发生改变时
        public delegate void StateChange(NodeState _beforeState, NodeState _afterState);
        public event StateChange StateChanged = (_beforeState, _afterState) => { };
        /// <summary>
        /// 执行方法
        /// </summary>
        public virtual void Execute(bool _isLinear)
        {
            state = NodeState.Running;
        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <param name="_depth">深度</param>
        /// <param name="_nodeNumber">节点编号</param>
        /// <param name="_parentNode">父节点</param>
        /// <param name="_through">遍历方式</param>
        public virtual void ResetNode(int _depth, int _nodeNumber, BehaviourNode _parentNode, ThroughEnum _through = ThroughEnum.Depth)
        {
            ResetNode(_depth, _nodeNumber, _parentNode);
        }

        public virtual void ResetNode(int _depth, int _nodeNumber, BehaviourNode _parentNode)
        {
            depth = _depth;
            nodeNumber = _nodeNumber;
            parentNode = _parentNode;
            state = NodeState.Ready;
        }

        public virtual void ResetNode()
        {
            ResetNode(0, 0, null, ThroughEnum.Depth);
        }

        /// <summary>
        /// 中止方法
        /// </summary>
        public virtual void Abort(NodeState _state)
        {
            state = _state;
        }

        /// <summary>
        /// 返回当前这棵树最后一个叶子节点的节点编号，如果是叶子节点则默认返回自己。
        /// </summary>
        /// <returns></returns>
        public virtual int GetTreeLastLeafNumber()
        {
            return nodeNumber;
        }
    }
}