using System;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class Blend1DPoseNodeConfig : PoseNodeConfig
    {
        public float slot0;
        public float slot1;
        
        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            Blend1DNode blend1DNode = new Blend1DNode();
            blend1DNode.m_NodeConfig = this;
            blend1DNode.InitializeGraphNode(graphRuntime);
            return blend1DNode;
        }
    }
}
