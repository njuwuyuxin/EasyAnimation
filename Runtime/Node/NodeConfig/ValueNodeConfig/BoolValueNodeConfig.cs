using System;

namespace AnimationGraph
{
    [Serializable]
    public class BoolValueNodeConfig : ValueNodeConfig
    {
        public bool value;

        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            BoolValueNode boolValueNode = new BoolValueNode();
            boolValueNode.m_NodeConfig = this;
            boolValueNode.InitializeGraphNode(graphRuntime);
            return boolValueNode;
        }
    }
}
