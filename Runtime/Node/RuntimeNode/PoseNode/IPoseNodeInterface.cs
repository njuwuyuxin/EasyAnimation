using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public abstract class IPoseNodeInterface : INode
    {
        public override ENodeType nodeType => ENodeType.PoseNode;

        protected IPoseNodeInterface[] m_InputPoseNodes;
        protected List<IPoseNodeInterface> m_OutputPoseNodes = new List<IPoseNodeInterface>();
        protected IValueNodeInterface[] m_InputValueNodes;


        public virtual Playable GetPlayable()
        {
            throw new NotImplementedException();
        }

        public override void AddInputNode(INode inputNode, int slotIndex)
        {
            switch (inputNode.nodeType)
            {
                case ENodeType.PoseNode:
                    AddInputPoseNode(inputNode as IPoseNodeInterface, slotIndex);
                    break;
                case ENodeType.ValueNode:
                    AddInputValueNode(inputNode as IValueNodeInterface, slotIndex);
                    break;
            }
        }

        public override void AddOutputNode(INode outputNode)
        {
            AddOutputPoseNode(outputNode as IPoseNodeInterface);
        }
        
        protected virtual void AddInputPoseNode(IPoseNodeInterface inputNode, int slotIndex)
        {
            if (m_InputPoseNodes == null)
            {
                Debug.LogError(this + " doesn't have pose input slot");
                return;
            }
            m_InputPoseNodes[slotIndex] = inputNode;
        }

        protected virtual void AddOutputPoseNode(IPoseNodeInterface outputNode)
        {
            m_OutputPoseNodes.Add(outputNode);
        }

        protected virtual void AddInputValueNode(IValueNodeInterface inputNode, int slotIndex)
        {
            if (m_InputPoseNodes == null)
            {
                Debug.LogError(this + " doesn't have pose input slot");
                return;
            }
            m_InputValueNodes[slotIndex] = inputNode;
        }

        protected void SetPoseInputSlotCount(int count)
        {
            m_InputPoseNodes = new IPoseNodeInterface[count];
        }
        
        protected void SetValueInputSlotCount(int count)
        {
            m_InputValueNodes = new IValueNodeInterface[count];
        }

        public virtual void OnDisconnected()
        {
            
        }
    }
}
