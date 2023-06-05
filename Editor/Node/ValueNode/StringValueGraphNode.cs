using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class StringValueGraphNode : ValueGraphNode
    {
        public override ENodeType nodeType => ENodeType.StringValueNode;

        public StringValueGraphNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            ColorUtility.TryParseHtmlString("#336699", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            nodeName = "StringValue";
            m_NodeConfig = new StringValueNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.ValuePort, 0);
        }
    }
}
