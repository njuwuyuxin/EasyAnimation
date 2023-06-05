using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class FloatValueGraphNode : ValueGraphNode
    {
        public override ENodeType nodeType => ENodeType.FloatValueNode;

        public FloatValueGraphNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            ColorUtility.TryParseHtmlString("#333366", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            nodeName = "FloatValue";
            m_NodeConfig = new FloatValueNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.ValuePort, 0);
        }
    }
}
