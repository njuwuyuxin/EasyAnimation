using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class Blend1DNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.Blend1DNode;

        public Blend1DNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "Blend1D";
            ColorUtility.TryParseHtmlString("#663366", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new Blend1DPoseNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.PosePort, 0);
            CreatePort(Direction.Input, Port.Capacity.Multi, "Slot0", NodePort.EPortType.PosePort, 0);
            CreatePort(Direction.Input, Port.Capacity.Multi, "Slot1", NodePort.EPortType.PosePort, 1);
            CreatePort(Direction.Input, Port.Capacity.Multi, "Parameter", NodePort.EPortType.ValuePort, 0);
        }
    }
}
