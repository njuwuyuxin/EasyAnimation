
namespace AnimationGraph
{
    public class FloatValueNode : ValueNode<FloatValueNodeConfig>
    {
        private float m_Value;

        public override float floatValue
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
