using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class GraphViewBase : GraphView
    {
        public GraphViewBase()
        {
            AddGridBackground();
            graphViewChanged += OnGraphViewChanged;
        }
        
        private void AddGridBackground()
        {
            GridBackground bg = new GridBackground();
            Insert(0, bg);
            bg.StretchToParentSize();
        }

        protected void AddDefaultManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale,ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new AnimationGraphSelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
        }

        public GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null && graphViewChange.elementsToRemove.Count > 0)
            {
                foreach (var element in graphViewChange.elementsToRemove)
                {
                    GraphNode graphNode = element as GraphNode;
                    if (graphNode != null)
                    {
                        graphNode.OnDestroy();
                    }
                    
                    StateTransition transition = element as StateTransition;
                    if (transition != null)
                    {
                        transition.OnDestroy();
                    }
                }
            }
            
            return graphViewChange;
        }
        
        protected Vector2 MouseToViewPosition(Vector2 mousePosition)
        {
            Vector2 graphViewPosition = VisualElementExtensions.LocalToWorld(this, new Vector2(transform.position.x,transform.position.y));
            return mousePosition - graphViewPosition;
        }

        public GraphNode GetNodeById(int id)
        {
            GraphNode result = null;
            nodes.ForEach(node =>
            {
                var graphNode = node as GraphNode;
                if (graphNode != null && graphNode.id == id)
                {
                    result = graphNode;
                }
            });
            return result;
        }
        
        public NodePort GetPortById(int id)
        {
            NodePort result = null;
            ports.ForEach(port =>
            {
                var nodePort = port as NodePort;
                if (nodePort != null && nodePort.id == id)
                {
                    result = nodePort;
                }
            });
            return result;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList();
        }

        public void ClearAnimationGraphView()
        {
            DeleteElements(nodes.ToList());
            DeleteElements(edges.ToList());
            DeleteElements(ports.ToList());
        }

        public void OnDestory()
        {
            graphViewChanged -= OnGraphViewChanged;
        }
    }
}
