using System.Collections.Generic;

namespace AnimationGraph
{
    public abstract class IValueNodeInterface : INode
    {
        public override ENodeType nodeType => ENodeType.ValueNode;

        public virtual bool boolValue { get; set; }
        public virtual int intValue { get; set; }
        public virtual float floatValue { get; set; }
        public virtual string stringValue { get; set; }
        
        protected List<IPoseNodeInterface> m_OutputPoseNodes = new List<IPoseNodeInterface>();
        
        public override void AddOutputNode(INode outputNode)
        {
            AddOutputPoseNode(outputNode as IPoseNodeInterface);
        }
        
        protected virtual void AddOutputPoseNode(IPoseNodeInterface outputNode)
        {
            m_OutputPoseNodes.Add(outputNode);
        }
    }
}
