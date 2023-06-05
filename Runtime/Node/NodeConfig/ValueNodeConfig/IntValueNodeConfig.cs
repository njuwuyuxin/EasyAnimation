using System;

namespace AnimationGraph
{
    [Serializable]
    public class IntValueNodeConfig : ValueNodeConfig
    {
        public int value;

        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            IntValueNode intValueNode = new IntValueNode();
            intValueNode.m_NodeConfig = this;
            intValueNode.InitializeGraphNode(graphRuntime);
            return intValueNode;
        }
    }
}
