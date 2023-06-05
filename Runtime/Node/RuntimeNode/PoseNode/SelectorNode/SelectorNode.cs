using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public abstract class SelectorNode<TNodeConfig> : PoseNode<TNodeConfig> where TNodeConfig :PoseNodeConfig
    {
        protected AnimationMixerPlayable m_MixerPlayable;
        protected IPoseNodeInterface m_CurrentActiveNode;
        private IPoseNodeInterface m_LastActiveNode;
        protected float m_TransitionTimer = 0f;
        protected float m_TransitionTime = 0.25f;
        protected PlayableGraph m_PlayableGraph;
        
        private bool m_IsTransitioning;

        protected void InitializePlayable(AnimationGraphRuntime animationGraphRuntime)
        {
            m_PlayableGraph = animationGraphRuntime.m_PlayableGraph;
            //Input 0 = old playable, Input 1 = new playable
            m_MixerPlayable = AnimationMixerPlayable.Create(animationGraphRuntime.m_PlayableGraph, 2);
        }
        
        protected void StartTransition(IPoseNodeInterface targetNode)
        {
            //Finish last transition before new transition start
            if (m_IsTransitioning)
            {
                TransitionFinish();
            }
            
            m_MixerPlayable.DisconnectInput(0);
            m_MixerPlayable.DisconnectInput(1);
            
            var targetPlayable = targetNode.GetPlayable();
            if (m_CurrentActiveNode == null)
            {
                m_MixerPlayable.ConnectInput(1, targetPlayable, 0);
                m_MixerPlayable.SetInputWeight(1, 1);
            }
            else
            {
                var currentActivePlayable = m_CurrentActiveNode.GetPlayable();
                m_MixerPlayable.ConnectInput(0, currentActivePlayable, 0);
                m_MixerPlayable.ConnectInput(1, targetPlayable, 0);
                m_MixerPlayable.SetInputWeight(0, 1);
                m_MixerPlayable.SetInputWeight(1, 0);
                m_IsTransitioning = true;
                m_TransitionTimer = 0f;
            }

            m_LastActiveNode = m_CurrentActiveNode;
            m_CurrentActiveNode = targetNode;
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

        private void TransitionFinish()
        {
            m_IsTransitioning = false;
            m_MixerPlayable.SetInputWeight(0, 0);
            m_MixerPlayable.SetInputWeight(1, 1);
            m_MixerPlayable.DisconnectInput(0);

            if (m_LastActiveNode != null)
            {
                m_LastActiveNode.OnDisconnected();
            }
        }

        protected void TransitionImmediate(IPoseNodeInterface targetNode)
        {
            m_MixerPlayable.DisconnectInput(0);
            m_MixerPlayable.DisconnectInput(1);
            m_MixerPlayable.ConnectInput(1, targetNode.GetPlayable(), 0);
            m_MixerPlayable.SetInputWeight(1, 1);
            m_LastActiveNode = m_CurrentActiveNode;
            m_CurrentActiveNode = targetNode;
        }
        
        public override void OnDisconnected()
        {
            if (m_CurrentActiveNode != null)
            {
                m_CurrentActiveNode.OnDisconnected();
            }
            m_MixerPlayable.DisconnectInput(0);
            m_MixerPlayable.DisconnectInput(1);
        }
        
        public override Playable GetPlayable()
        {
            return m_MixerPlayable;
        }
    }
}
