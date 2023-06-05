using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class BoolValueGraphNode : ValueGraphNode
    {
        public override ENodeType nodeType => ENodeType.BoolValueNode;

        public BoolValueGraphNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            ColorUtility.TryParseHtmlString("#336666", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            nodeName = "BoolValue";
            m_NodeConfig = new BoolValueNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.ValuePort, 0);
        }
    }
}
