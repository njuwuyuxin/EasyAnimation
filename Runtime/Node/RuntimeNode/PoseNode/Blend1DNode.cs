using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class Blend1DNode : PoseNode<Blend1DPoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;
        private AnimationMixerPlayable m_AnimationMixerPlayable;

        private IValueNodeInterface parameter => m_InputValueNodes[0];
        
        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            id = m_NodeConfig.id;
            SetPoseInputSlotCount(2);
            SetValueInputSlotCount(1);
            m_AnimationGraphRuntime = animationGraphRuntime;
            m_AnimationMixerPlayable = AnimationMixerPlayable.Create(m_AnimationGraphRuntime.m_PlayableGraph,2);
        }

        public override void OnStart()
        {
            m_AnimationMixerPlayable.DisconnectInput(0);
            m_AnimationMixerPlayable.DisconnectInput(1);
            m_AnimationMixerPlayable.ConnectInput(0, m_InputPoseNodes[0].GetPlayable(), 0);
            m_AnimationMixerPlayable.ConnectInput(1, m_InputPoseNodes[1].GetPlayable(), 0);
        }

        public override void OnUpdate(float deltaTime)
        {
            m_InputPoseNodes[0].OnUpdate(deltaTime);
            m_InputPoseNodes[1].OnUpdate(deltaTime);
            float weight = Mathf.Clamp01((parameter.floatValue - m_NodeConfig.slot0) / (m_NodeConfig.slot1 - m_NodeConfig.slot0));
            m_AnimationMixerPlayable.SetInputWeight(0, 1 - weight);
            m_AnimationMixerPlayable.SetInputWeight(1, weight);
        }

        public override void OnDisconnected()
        {
            m_AnimationMixerPlayable.DisconnectInput(0);
            m_AnimationMixerPlayable.DisconnectInput(1);
            m_InputPoseNodes[0].OnDisconnected();
            m_InputPoseNodes[1].OnDisconnected();
        }

        public override Playable GetPlayable()
        {
            return m_AnimationMixerPlayable;
        }
    }
}