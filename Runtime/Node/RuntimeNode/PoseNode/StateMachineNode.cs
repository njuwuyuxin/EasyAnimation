using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class StateMachineNode : PoseNode<StateMachinePoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;
        private AnimationMixerPlayable m_MixerPlayable;
        
        private class State
        {
            public int portIndex;
            public StatePoseNodeConfig config;
            public List<TransitionConfig> transitions = new List<TransitionConfig>();
        }

        private Dictionary<int, State> m_States;
        private State m_CurrentState;
        private State m_LastState;
        
        private float m_TransitionTimer = 0f;
        private float m_TransitionTime;
        private bool m_IsTransitioning;

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            m_AnimationGraphRuntime = animationGraphRuntime;
            id = m_NodeConfig.id;
            SetPoseInputSlotCount(m_NodeConfig.states.Count);
            SetValueInputSlotCount(0);

            m_MixerPlayable = AnimationMixerPlayable.Create(m_AnimationGraphRuntime.m_PlayableGraph, 2);

            m_States = new Dictionary<int, State>();
            for(int i=0;i<m_NodeConfig.states.Count;i++)
            {
                var state = m_NodeConfig.states[i];
                m_States.Add(state.id, new State() { portIndex = i, config = state });
            }

            foreach (var transition in m_NodeConfig.transitions)
            {
                if (m_States.TryGetValue(transition.sourceStateId, out var state))
                {
                    state.transitions.Add(transition);
                }
            }
        }

        public override void OnStart()
        {
            m_CurrentState = m_States[m_NodeConfig.defaultStateId];
            var playable = GetStateNode(m_CurrentState).GetPlayable();
            m_MixerPlayable.ConnectInput(0, playable, 0);
            m_MixerPlayable.SetInputWeight(0, 1);
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var transition in m_CurrentState.transitions)
            {
                if (ValidateConditions(transition.conditions))
                {
                    StartTransition(m_CurrentState, m_States[transition.targetStateId], transition);
                }
            }

            UpdateTransition(deltaTime);
        }
        
        protected void UpdateTransition(float deltaTime)
        {
            if (m_IsTransitioning)
            {
                m_TransitionTimer += deltaTime;
                if (m_TransitionTimer >= m_TransitionTime)
                {
                    TransitionFinish();
                    return;
                }

                float transitionPercentage = m_TransitionTimer / m_TransitionTime;
                m_MixerPlayable.SetInputWeight(0, 1 - transitionPercentage);
                m_MixerPlayable.SetInputWeight(1, transitionPercentage);
            }
        }

        private bool ValidateConditions(List<TransitionCondition> conditions)
        {
            foreach (var condition in conditions)
            {
                GraphParameter parameter = m_AnimationGraphRuntime.GetParameterById(condition.parameterId);
                switch (condition.conditionType)
                {
                    case EConditionType.NotEqual:
                        if (parameter.value.IsEqual(condition.value))
                        {
                            return false;
                        }
                        break;
                    case EConditionType.Equal:
                        if (!parameter.value.IsEqual(condition.value))
                        {
                            return false;
                        }
                        break;
                    case EConditionType.Greater:
                        if (!parameter.value.IsGreaterThan(condition.value))
                        {
                            return false;
                        }
                        break;
                    case EConditionType.GreaterEqual:
                        if (!parameter.value.IsGreaterEqualThan(condition.value))
                        {
                            return false;
                        }
                        break;
                    case EConditionType.Less:
                        if (!parameter.value.IsLessThan(condition.value))
                        {
                            return false;
                        }
                        break;
                    case EConditionType.LessEqual:
                        if (!parameter.value.IsLessEqualThan(condition.value))
                        {
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        // private void StartTransition(State oldState, State newState, TransitionConfig transitionConfig)
        // {
        //     m_IsTransitioning = true;
        //     m_MixerPlayable.DisconnectInput(0);
        //     m_MixerPlayable.ConnectInput(0, GetStateNode(newState).GetPlayable(), 0);
        //     m_MixerPlayable.SetInputWeight(0, 1);
        //     m_CurrentState = newState;
        // }
        
        private void StartTransition(State oldState, State newState, TransitionConfig transitionConfig)
        {
            //Finish last transition before new transition start
            if (m_IsTransitioning)
            {
                TransitionFinish();
            }
            
            m_MixerPlayable.DisconnectInput(0);
            m_MixerPlayable.DisconnectInput(1);
            
            var targetPlayable = GetStateNode(newState).GetPlayable();
            if (m_CurrentState == null)
            {
                m_MixerPlayable.ConnectInput(1, targetPlayable, 0);
                m_MixerPlayable.SetInputWeight(1, 1);
            }
            else
            {
                var currentActivePlayable = GetStateNode(m_CurrentState).GetPlayable();
                m_MixerPlayable.ConnectInput(0, currentActivePlayable, 0);
                m_MixerPlayable.ConnectInput(1, targetPlayable, 0);
                m_MixerPlayable.SetInputWeight(0, 1);
                m_MixerPlayable.SetInputWeight(1, 0);
                m_IsTransitioning = true;
                m_TransitionTimer = 0f;
            }

            m_TransitionTime = transitionConfig.blendTime;
            m_LastState = m_CurrentState;
            m_CurrentState = newState;
        }
        
        private void TransitionFinish()
        {
            m_IsTransitioning = false;
            m_MixerPlayable.SetInputWeight(0, 0);
            m_MixerPlayable.SetInputWeight(1, 1);
            m_MixerPlayable.DisconnectInput(0);

            if (m_LastState != null)
            {
                GetStateNode(m_LastState).OnDisconnected();
            }
        }

        private IPoseNodeInterface GetStateNode(State state)
        {
            return m_InputPoseNodes[state.portIndex];
        }

        public override Playable GetPlayable()
        {
            return m_MixerPlayable;
        }
        
        public override void OnDisconnected()
        {
            if (m_CurrentState != null)
            {
                GetStateNode(m_CurrentState).OnDisconnected();
            }
            m_MixerPlayable.DisconnectInput(0);
            m_MixerPlayable.DisconnectInput(1);
        }
    }
}