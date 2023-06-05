using System;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class AnimationClipPoseNodeConfig : PoseNodeConfig
    {
        public AnimationClip clip;
        public float playSpeed;
        
        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            AnimationClipNode animationClipNode = new AnimationClipNode();
            animationClipNode.m_NodeConfig = this;
            animationClipNode.InitializeGraphNode(graphRuntime);
            return animationClipNode;
        }
    }
}
