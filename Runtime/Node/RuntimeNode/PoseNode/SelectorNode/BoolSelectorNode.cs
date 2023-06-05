namespace AnimationGraph
{
    public class BoolSelectorNode : SelectorNode<BoolSelectorPoseNodeConfig>
    {
        public IPoseNodeInterface trueNode => m_InputPoseNodes[0];
        public IPoseNodeInterface falseNode => m_InputPoseNodes[1];
        public IValueNodeInterface condition => m_InputValueNodes[0];
        
        private bool m_CurrentCondition;

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            id = m_NodeConfig.id;
            m_TransitionTime = m_NodeConfig.blendTime;
            SetPoseInputSlotCount(2);
            SetValueInputSlotCount(1);
            InitializePlayable(animationGraphRuntime);
        }

        public override void OnStart()
        {
            m_CurrentCondition = condition.boolValue;
            if (condition.boolValue)
            {
                trueNode.OnStart();
                TransitionImmediate(trueNode);
            }
            else
            {
                falseNode.OnStart();
                TransitionImmediate(falseNode);
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            if (condition.boolValue != m_CurrentCondition)
            {
                ChangeSourcePlayable();
                m_CurrentCondition = condition.boolValue;
            }

            m_CurrentActiveNode.OnUpdate(deltaTime);
            UpdateTransition(deltaTime);
        }
        
        private void ChangeSourcePlayable()
        {
            if (condition.boolValue)
            {
                //Need to call StartTransition() before Node.OnStart()
                StartTransition(trueNode);
                trueNode.OnStart();
            }
            else
            {
                StartTransition(falseNode);
                falseNode.OnStart();
            }
        }
    }
}