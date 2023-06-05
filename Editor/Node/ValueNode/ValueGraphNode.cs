using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class ValueGraphNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.BoolValueNode;
        protected ParameterCard m_ParameterCard;

        public ValueGraphNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            var divider = topContainer.Q("divider");
            topContainer.Remove(divider);
            inputContainer.style.flexGrow = 0;
        }

        public void CombineWithParameter(ParameterCard parameterCard)
        {
            m_ParameterCard = parameterCard;
            parameterCard.associatedNodes.Add(id);
            var boolValueNodeConfig = (ValueNodeConfig) m_NodeConfig;
            boolValueNodeConfig.parameterId = parameterCard.id;
            boolValueNodeConfig.parameterName = parameterCard.parameterName;
        }

        public void DeCombineWithParameter()
        {
            m_ParameterCard.associatedNodes.Remove(id);
            var boolValueNodeConfig = (ValueNodeConfig) m_NodeConfig;
            boolValueNodeConfig.parameterId = 0;
            boolValueNodeConfig.parameterName = string.Empty;
        }

        public void ReCombineWithParameter()
        {
            if (m_ParameterCard != null)
            {
                m_ParameterCard.associatedNodes.Add(id);
                var boolValueNodeConfig = (ValueNodeConfig) m_NodeConfig;
                boolValueNodeConfig.parameterId = m_ParameterCard.id;
                boolValueNodeConfig.parameterName = m_ParameterCard.parameterName;
            }
        }

        public override void LoadNodeData(NodeData data)
        {
            base.LoadNodeData(data);
            ValueNodeConfig nodeConfig = data.nodeConfig as ValueNodeConfig;
            m_ParameterCard = m_AnimationGraphView.parameterBoard.TryGetParameterById(nodeConfig.parameterId);
            nodeName = nodeConfig.parameterName;
        }
    }
}
