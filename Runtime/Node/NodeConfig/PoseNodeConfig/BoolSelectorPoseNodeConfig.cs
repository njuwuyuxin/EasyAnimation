using System;

namespace AnimationGraph
{
    [Serializable]
    public class BoolSelectorPoseNodeConfig : PoseNodeConfig
    {
        public float blendTime = 0.25f;
        
        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            BoolSelectorNode boolSelectorNode = new BoolSelectorNode();
            boolSelectorNode.m_NodeConfig = this;
            boolSelectorNode.InitializeGraphNode(graphRuntime);
            return boolSelectorNode;
        }
    }
}