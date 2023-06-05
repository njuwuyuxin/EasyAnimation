using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class StateMachineNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.StateMachineNode;

        //The index = portIndex
        private List<StatePoseNodeConfig> m_StateConfigs = new List<StatePoseNodeConfig>();
        public List<StatePoseNodeConfig> stateConfigs => m_StateConfigs;

        private List<TransitionConfig> m_TransitionConfigs = new List<TransitionConfig>();
        public List<TransitionConfig> transitionConfigs => m_TransitionConfigs;
        
        public int defaultStateId { get; set; }

        public StateMachineNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "StateMachine";
            ColorUtility.TryParseHtmlString("#663366", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new StateMachinePoseNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.PosePort, 0);
        }
        
        public override void LoadNodeData(NodeData data)
        {
            base.LoadNodeData(data);
            var config = m_NodeConfig as StateMachinePoseNodeConfig;
            m_StateConfigs = new List<StatePoseNodeConfig>();
            if (config.states != null)
            {
                m_StateConfigs.AddRange(config.states);
            }

            m_TransitionConfigs = new List<TransitionConfig>();
            if (config.transitions != null)
            {
                m_TransitionConfigs.AddRange(config.transitions);
            }

            defaultStateId = config.defaultStateId;
        }

        public override void OnSave()
        {
            var stateMachineNodeConfig = m_NodeConfig as StateMachinePoseNodeConfig;
            stateMachineNodeConfig.states = new List<StatePoseNodeConfig>();
            stateMachineNodeConfig.states.AddRange(m_StateConfigs);
            stateMachineNodeConfig.transitions = new List<TransitionConfig>();
            stateMachineNodeConfig.transitions.AddRange(m_TransitionConfigs);
            stateMachineNodeConfig.defaultStateId = defaultStateId;
        }

        public int GetStateIndex(StateNode state)
        {
            return m_StateConfigs.IndexOf(state.nodeConfig as StatePoseNodeConfig);
        }

        public void OnAddState(StatePoseNodeConfig stateConfig)
        {
            CreatePort(Direction.Input, Port.Capacity.Multi, stateConfig.stateName, NodePort.EPortType.PosePort, m_StateConfigs.Count);
            m_StateConfigs.Add(stateConfig);
        }

        public void OnRemoveState(StatePoseNodeConfig stateConfig)
        {
            var portIndex = m_StateConfigs.IndexOf(stateConfig);
            var statePort = GetInputPort(NodePort.EPortType.PosePort, portIndex);
            foreach (var connection in statePort.connections)
            {
                m_AnimationGraphView.RemoveElement(connection);
            }
            
            DeletePort(statePort);

            for (int i = portIndex + 1; i < m_StateConfigs.Count; i++)
            {
                GetInputPort(NodePort.EPortType.PosePort, i).portIndex = i - 1;
            }

            m_StateConfigs.Remove(stateConfig);
        }

        public void OnAddTransition(TransitionConfig transitionConfig)
        {
            m_TransitionConfigs.Add(transitionConfig);
        }

        public void OnRemoveTransition(TransitionConfig transitionConfig)
        {
            m_TransitionConfigs.Remove(transitionConfig);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.clickCount == 2)
            {
                OpenStateMachineGraphView();
            }
        }

        private void OpenStateMachineGraphView()
        {
            Debug.Log("Open State Machine Graph View");
            m_AnimationGraphView.OpenStateMachineGraphView(this);
        }
    }
}
