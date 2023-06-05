using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace AnimationGraph.Editor
{
    public partial class AnimationGraphView
    {
        public void Save()
        {
            m_AnimationGraphAsset.nodes = new List<NodeData>();
            m_AnimationGraphAsset.ports = new List<PortData>();
            m_AnimationGraphAsset.edges = new List<EdgeData>();
            
            nodes.ForEach(node =>
            {
                GraphNode graphNode = node as GraphNode;
                if (graphNode != null)
                {
                    graphNode.OnSave();
                    NodeData nodeData = new NodeData();
                    nodeData.id = graphNode.id;
                    nodeData.nodeType = graphNode.nodeType;
                    nodeData.positionX = graphNode.GetPosition().x;
                    nodeData.positionY = graphNode.GetPosition().y;
                    nodeData.nodeConfig = graphNode.nodeConfig;
                    nodeData.customData = graphNode.customData;
                    m_AnimationGraphAsset.nodes.Add(nodeData);
                }
            });

            ports.ForEach(port =>
            {
                NodePort nodePort = port as NodePort;
                if (nodePort != null)
                {
                    PortData portData = new PortData();
                    switch (port.direction)
                    {
                        case Direction.Input:
                            portData.direction = EPortDirection.Input;
                            break;
                        case Direction.Output:
                            portData.direction = EPortDirection.Output;
                            break;
                    }
                    
                    switch (port.capacity)
                    {
                        case Port.Capacity.Single:
                            portData.capacity = EPortCapacity.Single;
                            break;
                        case Port.Capacity.Multi:
                            portData.capacity = EPortCapacity.Multi;
                            break;
                    }

                    portData.nodeId = nodePort.GraphNode.id;
                    portData.portId = nodePort.id;
                    portData.portName = nodePort.portName;
                    portData.portType = nodePort.portType;
                    portData.portIndex = nodePort.portIndex;
                    m_AnimationGraphAsset.ports.Add(portData);
                }
            });

            edges.ForEach(edge =>
            {
                EdgeData edgeData = new EdgeData();
                var inputPort = edge.input as NodePort;
                var outputPort = edge.output as NodePort;
                if (inputPort != null && outputPort != null)
                {
                    edgeData.inputPort = inputPort.id;
                    edgeData.outputPort = outputPort.id;
                    m_AnimationGraphAsset.edges.Add(edgeData);
                }
            });

            EditorUtility.SetDirty(m_AnimationGraphAsset);
            AssetDatabase.SaveAssets();
        }

        public void LoadAnimGraphAsset(AnimationGraphAsset graphAsset)
        {
            m_AnimationGraphAsset = graphAsset;
            LoadNodes();
            LoadPorts();
            LoadEdges();
        }

        private void LoadNodes()
        {
            foreach (var nodeData in m_AnimationGraphAsset.nodes)
            {
                var graphNode = CreateNodeFromAsset(nodeData);
            }
        }

        private void LoadPorts()
        {
            foreach (var portData in m_AnimationGraphAsset.ports)
            {
                var node = GetNodeById(portData.nodeId);
                
                Direction direction = Direction.Input;
                Port.Capacity capacity = Port.Capacity.Single;
                switch (portData.direction)
                {
                    case EPortDirection.Input:
                        direction = Direction.Input;
                        break;
                    case EPortDirection.Output:
                        direction = Direction.Output;
                        break;
                }
                    
                switch (portData.capacity)
                {
                    case EPortCapacity.Single:
                        capacity = Port.Capacity.Single;
                        break;
                    case EPortCapacity.Multi:
                        capacity = Port.Capacity.Multi;
                        break;
                }
                node.CreatePort(direction, capacity, portData.portName, portData.portType, portData.portIndex, portData.portId);
            }
        }

        private void LoadEdges()
        {
            foreach (var edgeData in m_AnimationGraphAsset.edges)
            {
                NodePort inputPort = GetPortById(edgeData.inputPort);
                NodePort outputPort = GetPortById(edgeData.outputPort);
                Edge edge = new Edge();
                edge.input = inputPort;
                edge.output = outputPort;
                inputPort.Connect(edge);
                outputPort.Connect(edge);
                AddElement(edge);
            }
        }
        
        public void Compile(CompiledAnimationGraph compiledGraph)
        {
            compiledGraph.nodes = new List<NodeConfig>();
            compiledGraph.nodeConnections = new List<Connection>();

            nodes.ForEach(node =>
            {
                GraphNode graphNode = node as GraphNode;
                if (graphNode != null)
                {
                    compiledGraph.nodes.Add(graphNode.nodeConfig);
                    if (graphNode.nodeType == ENodeType.FinalPoseNode)
                    {
                        compiledGraph.finalPosePoseNode = graphNode.nodeConfig as FinalPosePoseNodeConfig;
                    }
                }
            });
            
            edges.ForEach(edge =>
            {
                var inputPort = edge.input as NodePort;
                var outputPort = edge.output as NodePort;
                Connection connection = new Connection();
                connection.sourceNodeId = outputPort.GraphNode.nodeConfig.id;
                connection.targetNodeId = inputPort.GraphNode.nodeConfig.id;
                connection.targetPortIndex = inputPort.portIndex;
                compiledGraph.nodeConnections.Add(connection);
            });
            
            
        }
    }
}
