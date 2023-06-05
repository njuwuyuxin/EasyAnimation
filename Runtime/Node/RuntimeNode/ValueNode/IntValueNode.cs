
namespace AnimationGraph
{
    public class IntValueNode : ValueNode<IntValueNodeConfig>
    {
        private int m_Value;

        public override int intValue
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
