
namespace AnimationGraph
{
    public class StringValueNode : ValueNode<StringValueNodeConfig>
    {
        private string m_Value;

        public override string stringValue
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
