using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
    
namespace Sense.BehaviourTree
{
    public class CompositesNode : BehaviourNode
    {
        protected List<BehaviourNode> nodes = new List<BehaviourNode>();
        public List<BehaviourNode> Nodes => nodes;

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var childNode = transform.GetChild(i).GetComponent<BehaviourNode>();
                if (childNode && childNode.gameObject.activeInHierarchy)
                {
                    nodes.Add(childNode);
                }
            }
        }

        public override void ResetNode(int _depth, int _nodeNumber, BehaviourNode _parentNode, ThroughEnum _through = ThroughEnum.Depth)
        {
            base.ResetNode(_depth, _nodeNumber, _parentNode);

            if (_through == ThroughEnum.Depth)
            {

                int nodeNumberToChildNode = -1;
                for (int i = 0; i < nodes.Count; i++)
                {
                    var childNode = nodes[i];
                    nodeNumberToChildNode = nodeNumberToChildNode == -1 ? _nodeNumber + (i + 1) : nodes[i - 1].GetTreeLastLeafNumber() + 1;
                    childNode.ResetNode(_depth + 1, nodeNumberToChildNode, this, _through);
                }
            }
            else if (_through == ThroughEnum.Breadth)
            {
                if (_parentNode == null)
                {
                    Queue<NodeReseterData> behaviourQueue = new Queue<NodeReseterData>();
                    behaviourQueue.Enqueue(new NodeReseterData { depth = _depth, parentNode = null, reseterNode = this });

                    while (behaviourQueue.Count != 0)
                    {
                        var cache = behaviourQueue.Dequeue();
                        cache.reseterNode.ResetNode(cache.depth, _nodeNumber++, cache.parentNode);

                        if (cache.reseterNode.GetType().BaseType == typeof(CompositesNode))
                        {
                            foreach (var v in ((CompositesNode)cache.reseterNode).Nodes)
                            {
                                behaviourQueue.Enqueue(new NodeReseterData
                                    { reseterNode = v, depth = cache.depth + 1, parentNode = cache.reseterNode });
                            }
                        }
                    }
                }
            }
        }

        public override int GetTreeLastLeafNumber()
        {
            if (nodes.Count == 0)
            {
                return base.GetTreeLastLeafNumber();
            }
            return nodes[nodes.Count - 1].GetTreeLastLeafNumber();
        }

    }
}