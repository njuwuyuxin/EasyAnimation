using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class FinalPoseNode : PoseNode<FinalPosePoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            id = m_NodeConfig.id;
            SetPoseInputSlotCount(1);
            m_AnimationGraphRuntime = animationGraphRuntime;
        }

        public override Playable GetPlayable()
        {
            if (m_InputPoseNodes.Length == 0)
            {
                Debug.LogError("FinalPose has no input!");
                throw new NotImplementedException();
            }
            return m_InputPoseNodes[0].GetPlayable();
        }

        public override void OnStart()
        {
            m_InputPoseNodes[0].OnStart();
            m_AnimationGraphRuntime.m_FinalPosePlayable.DisconnectInput(0);
            m_AnimationGraphRuntime.m_FinalPosePlayable.ConnectInput(0, GetPlayable(), 0, 1f);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (m_InputPoseNodes.Length == 0)
            {
                Debug.LogError("FinalPose has no input!");
                return;
            }
            m_InputPoseNodes[0].OnUpdate(deltaTime);
            m_AnimationGraphRuntime.m_FinalPosePlayable.DisconnectInput(0);
            m_AnimationGraphRuntime.m_FinalPosePlayable.ConnectInput(0, GetPlayable(), 0, 1f);
        }
    }
}