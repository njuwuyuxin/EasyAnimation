using System.Collections.Generic;
using UnityEngine;

namespace AnimationGraph
{
    public class StringSelectorNode : SelectorNode<StringSelectorPoseNodeConfig>
    {
        public Dictionary<string, int> string2PortIndex = new Dictionary<string, int>();
        public IValueNodeInterface condition => m_InputValueNodes[0];
        
        private string m_CurrentCondition;

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            id = m_NodeConfig.id;
            for (int i = 0; i < m_NodeConfig.selections.Count; i++)
            {
                string2PortIndex.Add(m_NodeConfig.selections[i], i);
            }

            m_TransitionTime = m_NodeConfig.blendTime;

            SetPoseInputSlotCount(m_NodeConfig.selections.Count);
            SetValueInputSlotCount(1);
            InitializePlayable(animationGraphRuntime);
        }

        public override void OnStart()
        {
            m_CurrentCondition = condition.stringValue;
            if (string2PortIndex.TryGetValue(condition.stringValue, out int portIndex))
            {
                var node = m_InputPoseNodes[portIndex];
                node.OnStart();
                TransitionImmediate(node);
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            if (condition.stringValue != m_CurrentCondition)
            {
                ChangeSourcePlayable();
                m_CurrentCondition = condition.stringValue;
            }

            m_CurrentActiveNode.OnUpdate(deltaTime);
            UpdateTransition(deltaTime);
        }

        private void ChangeSourcePlayable()
        {
            if (string2PortIndex.TryGetValue(condition.stringValue, out int portIndex))
            {
                var node = m_InputPoseNodes[portIndex];
                StartTransition(node);
                node.OnStart();
            }
            else
            {
                Debug.LogError("StringSelectorNode: No String matches name: " + condition.stringValue);
            }
        }
    }
}