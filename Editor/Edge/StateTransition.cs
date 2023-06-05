using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public partial class StateTransition : GraphElement
    {
        public int id { get; set; }

        private StateNode m_SourceState;
        private StateNode m_TargetState;
        private StateMachineGraphView m_StateMachineGraphView;

        private EdgeConfig m_EdgeConfig;
        public EdgeConfig edgeConfig => m_EdgeConfig;
        
        
        public StateTransition(StateMachineGraphView stateMachineGraphView)
        {
            m_StateMachineGraphView = stateMachineGraphView;

            styleSheets.Add(Resources.Load<StyleSheet>(k_StyleSheetPrefix + "StateTransition"));
            AddToClassList("state-transition");
            style.position = Position.Absolute;
            capabilities |= Capabilities.Selectable | Capabilities.Deletable | Capabilities.Copiable;
            
            Add(transitionControl);
            
            RegisterCallback<AttachToPanelEvent>(OnTransitionAttach);
        }

        public void InitializeDefault(StateNode source, StateNode target)
        {
            id = Animator.StringToHash(Guid.NewGuid().ToString());
            m_SourceState = source;
            m_TargetState = target;
            if (source != null)
            {
                source.AddOutputTransition(this);
                from = m_SourceState.GetPosition().center;
            }

            if (target != null)
            {
                target.AddInputTransition(this);
                to = m_TargetState.GetPosition().center;
            }
            UpdateTransitionControl();

            //Init Default TransitionConfig
            if (source != null && target != null)
            {
                m_EdgeConfig = new TransitionConfig();
                m_EdgeConfig.SetId(id);
                var transitionConfig = m_EdgeConfig as TransitionConfig;
                transitionConfig.sourceStateId = m_SourceState.id;
                transitionConfig.targetStateId = m_TargetState.id;
                transitionConfig.blendTime = 0.25f;
            }
        }

        public void LoadFromConfig(TransitionConfig transitionConfig)
        {
            m_EdgeConfig = transitionConfig;
            id = transitionConfig.id;
            m_SourceState = m_StateMachineGraphView.GetNodeById(transitionConfig.sourceStateId) as StateNode;
            m_TargetState = m_StateMachineGraphView.GetNodeById(transitionConfig.targetStateId) as StateNode;
                
            m_SourceState.AddOutputTransition(this);
            m_TargetState.AddInputTransition(this);
            from = m_SourceState.GetPosition().center;
            to = m_TargetState.GetPosition().center;
            UpdateTransitionControl();
        }

        public void OnEdgeInspectorGUI(SerializedObject serializedObject)
        {
            
        }

        public void OnEdgeConfigUpdate()
        {
            
        }

        public void OnDestroy()
        {
            if (m_SourceState != null)
            {
                m_SourceState.outputTransitions.Remove(this);
            }

            if (m_TargetState != null)
            {
                m_TargetState.inputTransitions.Remove(this);
            }

            m_StateMachineGraphView.stateMachineNode.OnRemoveTransition(m_EdgeConfig as TransitionConfig);
        }

        public void OnDestroyByStateDestroy()
        {
            m_StateMachineGraphView.stateMachineNode.OnRemoveTransition(m_EdgeConfig as TransitionConfig);
        }
    }
}