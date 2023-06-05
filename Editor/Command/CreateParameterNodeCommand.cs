using UnityEngine;

namespace AnimationGraph.Editor
{
    public class CreateParameterNodeCommand : CreateNodeCommand
    {
        private ParameterCard m_ParameterCard;
        
        public CreateParameterNodeCommand(AnimationGraphView animationGraphView, ENodeType nodeType, Vector2 position,
            ParameterCard parameterCard) : base(animationGraphView, nodeType, position)
        {
            m_ParameterCard = parameterCard;
        }
        
        public override void Do()
        {
            base.Do();
            var valueNode = m_Node as ValueGraphNode;
            valueNode.CombineWithParameter(m_ParameterCard);
        }

        public override void Undo()
        {
            base.Undo();
            var valueNode = m_Node as ValueGraphNode;
            valueNode.DeCombineWithParameter();
        }

        public override void Redo()
        {
            base.Redo();
            var valueNode = m_Node as ValueGraphNode;
            valueNode.ReCombineWithParameter();
        }
    }
}