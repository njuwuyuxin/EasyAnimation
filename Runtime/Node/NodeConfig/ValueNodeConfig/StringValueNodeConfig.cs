using System;

namespace AnimationGraph
{
    [Serializable]
    public class StringValueNodeConfig : ValueNodeConfig
    {
        public string value = String.Empty;

        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            StringValueNode stringValueNode = new StringValueNode();
            stringValueNode.m_NodeConfig = this;
            stringValueNode.InitializeGraphNode(graphRuntime);
            return stringValueNode;
        }
    }
}
