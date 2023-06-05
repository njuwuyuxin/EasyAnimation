using System;
using UnityEditor;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public abstract class PoseNodeConfig : NodeConfig
    {
        public override ENodeType nodeType => ENodeType.PoseNode;
    }
}
