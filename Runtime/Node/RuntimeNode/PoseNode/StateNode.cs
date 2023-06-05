using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class StateNode : PoseNode<StatePoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            id = m_NodeConfig.id;
            // SetPoseInputSlotCount(2);
            // SetValueInputSlotCount(1);
            m_AnimationGraphRuntime = animationGraphRuntime;
        }

        public override void OnStart()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnDisconnected()
        {
        }

        // public override Playable GetPlayable()
        // {
        //     return m_AnimationMixerPlayable;
        // }
    }
}