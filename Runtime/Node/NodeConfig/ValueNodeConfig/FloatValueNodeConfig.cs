using System;

namespace AnimationGraph
{
    [Serializable]
    public class FloatValueNodeConfig : ValueNodeConfig
    {
        public float value;

        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            FloatValueNode floatValueNode = new FloatValueNode();
            floatValueNode.m_NodeConfig = this;
            floatValueNode.InitializeGraphNode(graphRuntime);
            return floatValueNode;
        }
    }
}
