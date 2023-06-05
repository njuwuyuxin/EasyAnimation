using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace AnimationGraph.Editor
{
    public class BatchDeleteCommand : ICommand
    {
        private AnimationGraphView m_AnimationGraphView;
        private List<GraphNode> m_DeletedNodes;
        private List<Edge> m_DeletedEdges;

        public BatchDeleteCommand(AnimationGraphView animationGraphView)
        {
            m_AnimationGraphView = animationGraphView;
        }
        
        public void Do()
        {
            m_DeletedNodes = new List<GraphNode>();
            m_DeletedEdges = new List<Edge>();
            var selection = new List<ISelectable>();
            selection.AddRange(m_AnimationGraphView.selection);
            foreach (var element in selection)
            {
                if (element is GraphNode graphNode)
                {
                    DeleteNode(graphNode);
                }

                if (element is Edge edge)
                {
                    DeleteEdge(edge);
                }
                ValidateEdges();
            }

        }

        public void Undo()
        {
            foreach (var node in m_DeletedNodes)
            {
                m_AnimationGraphView.AddElement(node);
                if (node is ValueGraphNode valueNode)
                {
                    valueNode.ReCombineWithParameter();
                }
            }
            
            foreach (var edge in m_DeletedEdges)
            {
                m_AnimationGraphView.AddElement(edge);
                edge.input.Connect(edge);
                edge.output.Connect(edge);
            }
        }

        public void Redo()
        {
            foreach (var node in m_DeletedNodes)
            {
                m_AnimationGraphView.RemoveElement(node);
                if (node is ValueGraphNode valueNode)
                {
                    valueNode.DeCombineWithParameter();
                }
            }
            
            foreach (var edge in m_DeletedEdges)
            {
                m_AnimationGraphView.RemoveElement(edge);
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
            }
        }

        private void DeleteNode(GraphNode graphNode)
        {
            if (graphNode is ValueGraphNode valueNode)
            {
                valueNode.DeCombineWithParameter();
            }

            m_AnimationGraphView.RemoveElement(graphNode);
            m_DeletedNodes.Add(graphNode);
        }

        private void DeleteEdge(Edge edge)
        {
            m_AnimationGraphView.RemoveElement(edge);
            m_DeletedEdges.Add(edge);
            edge.input.Disconnect(edge);
            edge.output.Disconnect(edge);
        }

        private void ValidateEdges()
        {
            List<Edge> edgeToDelete = new List<Edge>();
            foreach (var edge in m_AnimationGraphView.edges)
            {
                if (m_DeletedNodes.Contains(edge.input.node as GraphNode) ||
                    m_DeletedNodes.Contains(edge.output.node as GraphNode))
                {
                    edgeToDelete.Add(edge);
                }
            }

            foreach (var edge in edgeToDelete)
            {
                DeleteEdge(edge);
            }
        }
    }
}