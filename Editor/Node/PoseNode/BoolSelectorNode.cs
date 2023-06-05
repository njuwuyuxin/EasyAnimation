using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class BoolSelectorNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.BoolSelectorNode;

        public BoolSelectorNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "BoolSelector";
            ColorUtility.TryParseHtmlString("#663366", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new BoolSelectorPoseNodeConfig();
            m_NodeConfig.SetId(id);
            //规定 true port index = 0, false port index = 1
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.PosePort, 0);
            CreatePort(Direction.Input, Port.Capacity.Multi, "True", NodePort.EPortType.PosePort, 0);
            CreatePort(Direction.Input, Port.Capacity.Multi, "False", NodePort.EPortType.PosePort, 1);
            CreatePort(Direction.Input, Port.Capacity.Multi, "Condition", NodePort.EPortType.ValuePort, 0);
        }
    }
}
