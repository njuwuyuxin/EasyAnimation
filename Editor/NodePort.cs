using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class NodePort : Port
    {
        public enum EPortType
        {
            PosePort = 0,
            ValuePort = 1,
        }

        public int id { get; set; }
        public EPortType portType { get; set; }
        
        //Input 中 PosePort 和 ValuePort分别从0开始索引
        //Output Port 也从0开始索引
        public int portIndex { get; set; }
        public GraphNode GraphNode => m_GraphNode;
        private GraphNode m_GraphNode;

        public NodePort(GraphNode node, Orientation portOrientation, Direction portDirection,
            Port.Capacity portCapacity, System.Type type, EPortType portType, int portIndex) : base(
            portOrientation, portDirection, portCapacity, type)
        {
            EdgeConnectorListener connectorListener = new EdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<Edge>(connectorListener);
            this.AddManipulator(edgeConnector);
            
            m_GraphNode = node;
            this.portType = portType;
            this.portIndex = portIndex;
            id = Animator.StringToHash(Guid.NewGuid().ToString());
        }

        public NodePort(GraphNode node, Orientation portOrientation, Direction portDirection,
            Port.Capacity portCapacity, System.Type type, EPortType portType, int portIndex, int id) : base(
            portOrientation, portDirection, portCapacity, type)
        {
            EdgeConnectorListener connectorListener = new EdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<Edge>(connectorListener);
            this.AddManipulator(edgeConnector);

            m_GraphNode = node;
            this.portType = portType;
            this.portIndex = portIndex;
            this.id = id;
        }
    }
}