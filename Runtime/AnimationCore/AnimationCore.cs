using System;
using UnityEditor;
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

        private AnimationClip m_AnimationClip;
        private float m_StartTime;
        private float m_LastFrameTime;
        

        private BoneContainer m_BoneContainer;

        public RootMotionData rootMotionData => m_RootMotionData;
        private RootMotionData m_RootMotionData = new RootMotionData();

        public void Initialize(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            m_SkinnedMeshRenderer = skinnedMeshRenderer;

            m_BoneContainer = new BoneContainer();
            m_BoneContainer.InitializeBoneContainer(m_SkinnedMeshRenderer);
            
            m_StartTime = Time.time;
            m_LastFrameTime = m_StartTime;
        }
        

        public void SetTestAnimationClip(AnimationClip animationClip)
        {
            m_AnimationClip = animationClip;
        }
        // Update is called once per frame
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
            compactPose.ResetToRefPose();

            BoneTransformCurve rootBoneTransformCurve = new BoneTransformCurve();
            
            
            EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(m_AnimationClip);
            foreach (EditorCurveBinding curveBinding in curveBindings)
            {
                AnimationCurve animationCurve = AnimationUtility.GetEditorCurve(m_AnimationClip, curveBinding);
                float AnimationTime = (Time.time - m_StartTime) % m_AnimationClip.length;
                float curveValue = animationCurve.Evaluate(AnimationTime);
                int boneIndex = m_BoneContainer.GetBoneIndexFromBonePath(curveBinding.path);

                //Extract RootMotion Curve
                if (m_BoneContainer.GetBoneByBonePath(curveBinding.path) == m_BoneContainer.rootBone)
                {
                    switch (curveBinding.propertyName)
                    {
                        case "m_LocalPosition.x":
                            rootBoneTransformCurve.positionX = animationCurve;
                            break;
                        case "m_LocalPosition.y":
                            rootBoneTransformCurve.positionY = animationCurve;
                            break;
                        case "m_LocalPosition.z":
                            rootBoneTransformCurve.positionZ = animationCurve;
                            break;
                        case "m_LocalRotation.x":
                            rootBoneTransformCurve.rotationX = animationCurve;
                            break;
                        case "m_LocalRotation.y":
                            rootBoneTransformCurve.rotationY = animationCurve;
                            break;
                        case "m_LocalRotation.z":
                            rootBoneTransformCurve.rotationZ = animationCurve;
                            break;
                        case "m_LocalRotation.w":
                            rootBoneTransformCurve.rotationW = animationCurve;
                            break;
                        case "m_LocalScale.x":
                            rootBoneTransformCurve.scaleX = animationCurve;
                            break;
                        case "m_LocalScale.y":
                            rootBoneTransformCurve.scaleY = animationCurve;
                            break;
                        case "m_LocalScale.z":
                            rootBoneTransformCurve.scaleZ = animationCurve;
                            break;
                    }
                }
                
                //Sample Pose, inluding root bone
                switch (curveBinding.propertyName)
                {
                    case "m_LocalPosition.x":
                        compactPose[boneIndex].position.x = curveValue;
                        break;
                    case "m_LocalPosition.y":
                        compactPose[boneIndex].position.y = curveValue;
                        break;
                    case "m_LocalPosition.z":
                        compactPose[boneIndex].position.z = curveValue;
                        break;
                    case "m_LocalRotation.x":
                        compactPose[boneIndex].rotation.x = curveValue;
                        break;
                    case "m_LocalRotation.y":
                        compactPose[boneIndex].rotation.y = curveValue;
                        break;
                    case "m_LocalRotation.z":
                        compactPose[boneIndex].rotation.z = curveValue;
                        break;
                    case "m_LocalRotation.w":
                        compactPose[boneIndex].rotation.w = curveValue;
                        break;
                    case "m_LocalScale.x":
                        compactPose[boneIndex].scale.x = curveValue;
                        break;
                    case "m_LocalScale.y":
                        compactPose[boneIndex].scale.y = curveValue;
                        break;
                    case "m_LocalScale.z":
                        compactPose[boneIndex].scale.z = curveValue;
                        break;
                }
            }

            if (enableRootMotion)
            {
                compactPose[0].position.x = 0;
                compactPose[0].position.z = 0;
                CalculateRootMotion(ref compactPose, in rootBoneTransformCurve);
            }
            
            RefreshBoneTransforms(in compactPose);

            m_LastFrameTime = Time.time;
        }

        private void CalculateRootMotion(ref CompactPose compactPose, in BoneTransformCurve RootMotionCurve)
        {
            float animationTimeLastFrame = (m_LastFrameTime - m_StartTime);
            float animationTimeThisFrame = (Time.time - m_StartTime);
            if (m_AnimationClip.isLooping)
            {
                animationTimeLastFrame %= m_AnimationClip.length;
                animationTimeThisFrame %= m_AnimationClip.length;
            }
            BoneTransform RootBoneTransformLastFrame = RootMotionCurve.Evaluate(animationTimeLastFrame);
            BoneTransform RootBoneTransformThisFrame = RootMotionCurve.Evaluate(animationTimeThisFrame);
            Vector3 deltaPosition = RootBoneTransformThisFrame.position - RootBoneTransformLastFrame.position;
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
