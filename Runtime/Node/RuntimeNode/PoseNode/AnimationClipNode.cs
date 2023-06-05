using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class AnimationClipNode : PoseNode<AnimationClipPoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;
        private AnimationClipPlayable m_AnimationClipPlayable;
        
        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            id = m_NodeConfig.id;
            m_AnimationGraphRuntime = animationGraphRuntime;
            m_AnimationClipPlayable = AnimationClipPlayable.Create(m_AnimationGraphRuntime.m_PlayableGraph, m_NodeConfig.clip);
        }

        public override void OnStart()
        {
            m_AnimationClipPlayable.SetTime(0);
            m_AnimationClipPlayable.SetTime(0);
        }

        public override Playable GetPlayable()
        {
            return m_AnimationClipPlayable;
        }
    }
}