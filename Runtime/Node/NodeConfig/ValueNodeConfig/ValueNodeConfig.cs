using System;

namespace AnimationGraph
{
    [Serializable]
    public abstract class ValueNodeConfig : NodeConfig
    {
        public override ENodeType nodeType => ENodeType.ValueNode;
        public int parameterId;
        public string parameterName;
    }
}
