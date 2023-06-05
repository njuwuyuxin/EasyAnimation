using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class StateNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.StateNode;
        private StateMachineGraphView m_StateMachineView;

        private HashSet<StateTransition> m_InputTransitions = new HashSet<StateTransition>();
        private HashSet<StateTransition> m_OutputTransitions = new HashSet<StateTransition>();

        public HashSet<StateTransition> inputTransitions => m_InputTransitions;
        public HashSet<StateTransition> outputTransitions => m_OutputTransitions;

        public StateNode(AnimationGraphView graphView, StateMachineGraphView stateMachineView, Vector2 position) : base(graphView,position)
        {
            m_StateMachineView = stateMachineView;
            var divider = contentContainer.Q("divider");
            divider.RemoveFromHierarchy();
            inputContainer.style.flexGrow = 0;
            // topContainer.RemoveFromHierarchy();
            ColorUtility.TryParseHtmlString("#006633", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            nodeName = "Default State";
            m_NodeConfig = new StatePoseNodeConfig();
            m_NodeConfig.SetId(id);
            var stateConfig = m_NodeConfig as StatePoseNodeConfig;
            stateConfig.stateName = nodeName;
        }

        public void LoadFromConfig(StatePoseNodeConfig stateConfig)
        {
            m_NodeConfig = stateConfig;
            id = m_NodeConfig.id;
            nodeName = stateConfig.stateName;
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            foreach (var inputTransition in m_InputTransitions)
            {
                inputTransition.to = evt.newRect.center;
                inputTransition.UpdateTransitionControl();
            }

            foreach (var outputTransition in m_OutputTransitions)
            {
                outputTransition.from = evt.newRect.center;
                outputTransition.UpdateTransitionControl();
            }

            (m_NodeConfig as StatePoseNodeConfig).position = GetPosition().position;
        }

        private VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            TextField stateNameField = InspectorUitils.CreateTextField("State Name", evt =>
            {
                nodeName = evt.newValue;
                (m_NodeConfig as StatePoseNodeConfig).stateName = evt.newValue;
                var stateIndex = m_StateMachineView.stateMachineNode.GetStateIndex(this);
                m_StateMachineView.stateMachineNode.GetInputPort(NodePort.EPortType.PosePort, stateIndex).portName =
                    nodeName;
            });
            stateNameField.value = nodeName;
            root.Add(stateNameField);
            
            return root;
        }
        
        public override void OnSelected()
        {
            base.OnSelected();
            m_AnimationGraphView.inspector.SetCustomContent(CreateInspectorGUI());

            if (m_StateMachineView.isMakingTransition && m_StateMachineView.lastSelectedNode != null)
            {
                m_StateMachineView.transitionToAdd.source = m_StateMachineView.lastSelectedNode;
                m_StateMachineView.transitionToAdd.target = this;
                m_StateMachineView.TryAddTransition();
                m_StateMachineView.isMakingTransition = false;
            }
            m_StateMachineView.currentSelectedNode = this;
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            m_AnimationGraphView.inspector.ClearInspector();
            m_StateMachineView.lastSelectedNode = m_StateMachineView.currentSelectedNode;
            m_StateMachineView.currentSelectedNode = null;
        }

        public override void OnDestroy()
        {
            foreach (var inputTransition in m_InputTransitions )
            {
                m_StateMachineView.RemoveElement(inputTransition);
                //Call RemoveElement will not trigger OnGraphChange Event, need to call transition.OnDestroyByStateDestroy manually.
                inputTransition.OnDestroyByStateDestroy();
            }
            foreach (var outputTransition in m_OutputTransitions )
            {
                m_StateMachineView.RemoveElement(outputTransition);
                outputTransition.OnDestroyByStateDestroy();
            }
            m_StateMachineView.stateMachineNode.OnRemoveState(m_NodeConfig as StatePoseNodeConfig);

            if (m_StateMachineView.defaultNode == this)
            {
                m_StateMachineView.SetRandomStateAsDefault();
            }
        }

        public void AddInputTransition(StateTransition transition)
        {
            m_InputTransitions.Add(transition);
        }
        
        public void AddOutputTransition(StateTransition transition)
        {
            m_OutputTransitions.Add(transition);
        }

        public void SetDefault()
        {
            ColorUtility.TryParseHtmlString("#8B6914", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
        }

        public void CancelDefault()
        {
            ColorUtility.TryParseHtmlString("#006633", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
        }
    }
}
