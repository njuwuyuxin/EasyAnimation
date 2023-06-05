using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class IntValueGraphNode : ValueGraphNode
    {
        public override ENodeType nodeType => ENodeType.IntValueNode;

        public IntValueGraphNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            ColorUtility.TryParseHtmlString("#003366", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            nodeName = "IntValue";
            m_NodeConfig = new IntValueNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.ValuePort, 0);
        }
    }
}
