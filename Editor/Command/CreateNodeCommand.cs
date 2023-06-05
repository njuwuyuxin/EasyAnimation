using UnityEngine;

namespace AnimationGraph.Editor
{
    public class CreateNodeCommand : ICommand
    {
        protected AnimationGraphView m_AnimationGraphView;
        protected ENodeType m_NodeType;
        protected Vector2 m_Position;
        protected GraphNode m_Node;
        
        public CreateNodeCommand(AnimationGraphView animationGraphView, ENodeType nodeType, Vector2 position)
        {
            m_AnimationGraphView = animationGraphView;
            m_NodeType = nodeType;
            m_Position = position;
        }
        
        public virtual void Do()
        {
            m_Node = CreateNodeInternal();
            m_Node.InitializeDefault();
        }

        public virtual void Undo()
        {
            m_AnimationGraphView.RemoveElement(m_Node);
        }

        public virtual void Redo()
        {
            m_AnimationGraphView.AddElement(m_Node);
        }
        
        private GraphNode CreateNodeInternal()
        {
            GraphNode node = null;
            switch (m_NodeType)
            {
                case ENodeType.FinalPoseNode: node = new FinalPoseNode(m_AnimationGraphView, m_Position);
                    break;
                case ENodeType.AnimationClipNode: node = new AnimationClipNode(m_AnimationGraphView, m_Position);
                    break;
                case ENodeType.BoolSelectorNode: node = new BoolSelectorNode(m_AnimationGraphView, m_Position);
                    break;
                case ENodeType.StringSelectorNode: node = new StringSelectorNode(m_AnimationGraphView, m_Position);
                    break;
                case ENodeType.Blend1DNode: node = new Blend1DNode(m_AnimationGraphView, m_Position);
                    break;
                case ENodeType.StateMachineNode: node = new StateMachineNode(m_AnimationGraphView, m_Position);
                    break;
                case ENodeType.BoolValueNode: node = new BoolValueGraphNode(m_AnimationGraphView, m_Position);
                    break;
                case ENodeType.IntValueNode: node = new IntValueGraphNode(m_AnimationGraphView, m_Position);
                    break;
                case ENodeType.FloatValueNode: node = new FloatValueGraphNode(m_AnimationGraphView, m_Position);
                    break;
                case ENodeType.StringValueNode: node = new StringValueGraphNode(m_AnimationGraphView, m_Position);
                    break;
                default: Debug.LogError("No Matching NodeType " + m_NodeType);
                    break;
            }

            if (node != null)
            {
                m_AnimationGraphView.AddElement(node);
            }
            else
            {
                Debug.LogError("[AnimationGraph][GraphView]: Create Node failed, nodeType:" + m_NodeType);
            }

            return node;
        }

        public GraphNode GetCreatedNode()
        {
            return m_Node;
        }
    }
}