
namespace AnimationGraph
{
    public class BoolValueNode : ValueNode<BoolValueNodeConfig>
    {
        private bool m_Value;

        public override bool boolValue
        {
            get => m_Value;
            set => m_Value = value;
        }

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            id = m_NodeConfig.id;
            m_Value = m_NodeConfig.value;
        }
        
    }
}
