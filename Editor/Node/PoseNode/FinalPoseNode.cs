using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class FinalPoseNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.FinalPoseNode;

        public FinalPoseNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "FinalPose";
            var divider = topContainer.Q("divider");
            topContainer.Remove(divider);
            outputContainer.style.flexGrow = 0;
            ColorUtility.TryParseHtmlString("#993333", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new FinalPosePoseNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Input, Port.Capacity.Single, "Input", NodePort.EPortType.PosePort, 0);
        }
    }
}
