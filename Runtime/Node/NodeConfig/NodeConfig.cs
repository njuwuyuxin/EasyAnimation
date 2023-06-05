using System;
using UnityEngine;

namespace AnimationGraph
{
    public enum ENodeType
    {
        PoseNode,
        ValueNode,
    }
    
    [Serializable]
    public abstract class NodeConfig
    {
        [SerializeField]
        public int id;
        public virtual ENodeType nodeType { get; }

        public NodeConfig()
        {
            if (id == 0)
            {
                id = Animator.StringToHash(  Guid.NewGuid().ToString());
            }
        }

        public void SetId(int id)
        {
            this.id = id;
        }

        public abstract INode GenerateNode(AnimationGraphRuntime graphRuntime);
    }
}
