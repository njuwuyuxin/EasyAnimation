using System;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class FinalPosePoseNodeConfig : PoseNodeConfig
    {

        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            FinalPoseNode finalPoseNode = new FinalPoseNode();
            finalPoseNode.m_NodeConfig = this;
            finalPoseNode.InitializeGraphNode(graphRuntime);
            return finalPoseNode;
        }
    }
}
