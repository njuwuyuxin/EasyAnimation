using System;
using UnityEngine;

namespace AnimationGraph
{
    public enum EAnimationMode
    {
        AnimationClip = 0,
        AnimationGraph = 1,
    }
    public class AnimationCore
    {
        public EAnimationMode animationMode { get; set; }
        public bool enableRootMotion { get; set; }
        public CompiledAnimationGraph animationGraph;
        
        private SkinnedMeshRenderer m_SkinnedMeshRenderer;

        private AnimationGraphRuntime m_AnimationGraphRuntime;
        
        private BoneContainer m_BoneContainer;

        public RootMotionData rootMotionData => m_RootMotionData;
        private RootMotionData m_RootMotionData = new RootMotionData();

        //AnimationClip Mode Data
        private float m_StartTime;
        private AnimationClip m_AnimationClip;
        private AnimationClipSampler m_AnimationClipSampler;
        private BoneTransformCurve m_RootMotionCurve;

        public void Initialize(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            m_SkinnedMeshRenderer = skinnedMeshRenderer;

            m_BoneContainer = new BoneContainer();
            m_BoneContainer.InitializeBoneContainer(m_SkinnedMeshRenderer);
            
            if (animationMode == EAnimationMode.AnimationClip)
            {
                m_AnimationClipSampler = new AnimationClipSampler(m_AnimationClip, m_BoneContainer);
                m_RootMotionCurve = m_AnimationClipSampler.ExtractRootMotionCurve();
            }

            m_StartTime = Time.time;
        }
        

        public void SetTestAnimationClip(AnimationClip animationClip)
        {
            m_AnimationClip = animationClip;
        }
        
        public void Update(float deltaTime)
        {
            if (animationMode == EAnimationMode.AnimationClip)
            {
                EvaluateAnimationFromClip();
            }
            else if (animationMode == EAnimationMode.AnimationGraph)
            {
                UpdateAnimation();
                EvaluateAnimation();
            }
        }


        private void UpdateAnimation()
        {
            
        }
        
        private unsafe void EvaluateAnimation()
        {
            // Span<BoneTransform> boneBuffer = stackalloc BoneTransform[m_SkinnedMeshRenderer.bones.Length];
            // PoseContext poseContext = new PoseContext(boneBuffer);
            // animationGraph.EvaluateGraph(ref poseContext);
            EvaluateAnimationFromClip();
            // RefreshBoneTransforms(poseContext.Pose);
        }

        private void EvaluateAnimationFromClip()
        {
            if (m_SkinnedMeshRenderer == null)
            {
                return;
            }

            Span<BoneTransform> boneTransformBuffer = stackalloc BoneTransform[m_BoneContainer.boneCount];
            CompactPose compactPose = new CompactPose(boneTransformBuffer,m_BoneContainer);
            
            //m_StartTime is the time when AnimationClip start to play, should be initialize in AnimationClipNode.Initialize()
            float AnimationTime = Time.time - m_StartTime;
            m_AnimationClipSampler.EvaluatePose(AnimationTime, ref compactPose);

            if (enableRootMotion)
            {
                compactPose[0].position.x = 0;
                compactPose[0].position.z = 0;
                CalculateRootMotion(ref compactPose, in m_RootMotionCurve);
            }
            
            RefreshBoneTransforms(in compactPose);
        }

        private void CalculateRootMotion(ref CompactPose compactPose, in BoneTransformCurve RootMotionCurve)
        {
            float animationTimeThisFrame = (Time.time - m_StartTime);
            if (m_AnimationClip.isLooping)
            {
                animationTimeThisFrame %= m_AnimationClip.length;
            }
            
            float animationTimeLastFrame = animationTimeThisFrame - Time.deltaTime;
            animationTimeLastFrame = Mathf.Clamp(animationTimeLastFrame, 0, animationTimeLastFrame);

            BoneTransform rootBoneTransformLastFrame = RootMotionCurve.Evaluate(animationTimeLastFrame);
            BoneTransform rootBoneTransformThisFrame = RootMotionCurve.Evaluate(animationTimeThisFrame);
            Vector3 deltaPosition = rootBoneTransformThisFrame.position - rootBoneTransformLastFrame.position;
            Vector3 deltaPositionWorldSpace = m_BoneContainer.rootBone.parent.localToWorldMatrix * deltaPosition;
            m_RootMotionData.deltaPosition = deltaPositionWorldSpace;
        }

        private void RefreshBoneTransforms(in CompactPose compactPose)
        {
            for (int boneIndex = 0; boneIndex < m_SkinnedMeshRenderer.bones.Length; boneIndex++)
            {
                Transform[] bones = m_SkinnedMeshRenderer.bones;
                bones[boneIndex].localPosition = compactPose[boneIndex].position;
                bones[boneIndex].localRotation = compactPose[boneIndex].rotation;
                bones[boneIndex].localScale = compactPose[boneIndex].scale;
            }
        }
    }
    
}
