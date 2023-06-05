using UnityEngine;

namespace AnimationGraph.Editor
{
    public partial class AnimationGraphView
    {
        private GraphNode CreateNodeInternal(ENodeType nodeType, Vector2 position)
        {
            GraphNode node = null;
            switch (nodeType)
            {
                case ENodeType.FinalPoseNode: node = new FinalPoseNode(this, position);
                    break;
                case ENodeType.AnimationClipNode: node = new AnimationClipNode(this, position);
                    break;
                case ENodeType.BoolSelectorNode: node = new BoolSelectorNode(this, position);
                    break;
                case ENodeType.StringSelectorNode: node = new StringSelectorNode(this, position);
                    break;
                case ENodeType.BoolValueNode: node = new BoolValueGraphNode(this, position);
                    break;
                case ENodeType.IntValueNode: node = new IntValueGraphNode(this, position);
                    break;
                case ENodeType.FloatValueNode: node = new FloatValueGraphNode(this, position);
                    break;
                case ENodeType.StringValueNode: node = new StringValueGraphNode(this, position);
                    break;
                case ENodeType.Blend1DNode: node = new Blend1DNode(this, position);
                    break;
                case ENodeType.StateMachineNode: node = new StateMachineNode(this, position);
                    break;
                default: node = new GraphNode(this, position);
                    break;
            }

            if (node != null)
            {
                AddElement(node);
            }
            else
            {
                Debug.LogError("[AnimationGraph][GraphView]: Create Node failed, nodeType:" + nodeType);
            }

            return node;
        }

        private GraphNode CreateDefaultNode(ENodeType nodeType, Vector2 position)
        {
            CreateNodeCommand createNodeCommand = new CreateNodeCommand(this, nodeType, position);
            PushNewCommand(createNodeCommand);
            return createNodeCommand.GetCreatedNode();
        }

        private void CreateParameterNode(ParameterCard parameterCard, Vector2 position)
        {
            ENodeType nodeType = ENodeType.BaseNode;
            switch (parameterCard)
            {
                case BoolParameterCard: nodeType = ENodeType.BoolValueNode;
                    break;
                case IntParameterCard: nodeType = ENodeType.IntValueNode;
                    break;
                case FloatParameterCard: nodeType = ENodeType.FloatValueNode;
                    break;
                case StringParameterCard: nodeType = ENodeType.StringValueNode;
                    break;
            }
            
            CreateParameterNodeCommand createParameterNodeCommand =
                new CreateParameterNodeCommand(this, nodeType, position, parameterCard);
            PushNewCommand(createParameterNodeCommand);
        }

        private GraphNode CreateNodeFromAsset(NodeData data)
        {
            var node = CreateNodeInternal(data.nodeType, new Vector2(data.positionX, data.positionY));
            node.LoadNodeData(data);
            return node;
        }
    }
}
