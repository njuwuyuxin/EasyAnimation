using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class GraphNode : Node
    {
        public virtual ENodeType nodeType => ENodeType.BaseNode;
        public int id { get; set; }

        protected string nodeName
        {
            get => m_NodeName;
            set
            {
                m_NodeName = value;
                title = m_NodeName;
            }
        }

        protected AnimationGraphView m_AnimationGraphView;
        protected string m_NodeName;
        protected List<NodePort> m_InputPorts;
        protected NodePort m_OutputPort;
        public NodeConfig nodeConfig => m_NodeConfig;
        protected NodeConfig m_NodeConfig;
        public CustomSerializableData customData => m_CustomData;
        protected CustomSerializableData m_CustomData;

        protected virtual bool m_DrawInspectorCustomize => false;

        public GraphNode(AnimationGraphView graphView, Vector2 position)
        {
            m_AnimationGraphView = graphView;
            nodeName = "Base Node";
            m_InputPorts = new List<NodePort>();
            SetPosition(new Rect(position,Vector2.zero));
        }

        public virtual void InitializeDefault()
        {
            id = Animator.StringToHash(Guid.NewGuid().ToString());
        }

        public virtual void OnSave()
        {
            
        }
        
        public virtual void LoadNodeData(NodeData data)
        {
            id = data.id;
            m_NodeConfig = data.nodeConfig;
            if (data.customData == null)
            {
                m_CustomData = new CustomSerializableData();
            }
            m_CustomData = data.customData;
        }
        
        protected void CreatePort(Direction direction, Port.Capacity capacity, string portName, NodePort.EPortType portType, int portIndex)
        {
            var port = new NodePort(this, Orientation.Horizontal, direction, capacity, typeof(Port), portType, portIndex);
            port.portName = portName;
            
            switch (direction)
            {
                case Direction.Input: 
                    m_InputPorts.Add(port);
                    inputContainer.Add(port);
                    break;
                case Direction.Output: 
                    m_OutputPort = port;
                    outputContainer.Add(port);
                    break;
            }
        }

        public void CreatePort(Direction direction, Port.Capacity capacity, string portName, NodePort.EPortType portType, int portIndex, int id)
        {
            var port = new NodePort(this, Orientation.Horizontal, direction, capacity, typeof(Port), portType, portIndex, id);
            port.portName = portName;

            switch (direction)
            {
                case Direction.Input:
                    m_InputPorts.Add(port);
                    inputContainer.Add(port);
                    break;
                case Direction.Output:
                    m_OutputPort = port;
                    outputContainer.Add(port);
                    break;
            }
        }

        public NodePort GetInputPort(NodePort.EPortType portType, int portIndex)
        {
            return m_InputPorts.Find(port => port.portType == portType && port.portIndex == portIndex);
        }
        
        public void DeletePort(NodePort port)
        {
            switch (port.direction)
            {
                case Direction.Input:
                    m_InputPorts.Remove(port);
                    inputContainer.Remove(port);
                    break;
                case Direction.Output:
                    m_OutputPort = null;
                    outputContainer.Remove(port);
                    break;
            }
        }

        protected virtual void Draw()
        {
            title = m_NodeName;
        }

        public virtual void OnNodeInspectorGUI()
        {
            
        }

        public override void OnSelected()
        {
            base.OnSelected();
            m_AnimationGraphView.inspector.SetGraphNodeIMGUI(this, m_DrawInspectorCustomize);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            m_AnimationGraphView.inspector.ClearInspector();
        }

        public virtual void OnNodeConfigUpdate()
        {
            
        }

        public virtual void OnDestroy()
        {
            
        }
        
    }
}
