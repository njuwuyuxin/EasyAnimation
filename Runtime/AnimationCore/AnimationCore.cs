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
        public CompiledAnimationGraph animationGraph;
        
        private SkinnedMeshRenderer m_SkinnedMeshRenderer;

        private AnimationGraphRuntime m_AnimationGraphRuntime;

        private AnimationClip m_TestAnimationClip;
        private float StartTime;
        

        private BoneContainer m_BoneContainer;

        public void Initialize(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            m_SkinnedMeshRenderer = skinnedMeshRenderer;

            m_BoneContainer = new BoneContainer();
            m_BoneContainer.InitializeBoneContainer(m_SkinnedMeshRenderer);
            
            StartTime = Time.time;
        }
        

        public void SetTestAnimationClip(AnimationClip animationClip)
        {
            m_TestAnimationClip = animationClip;
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
            
            EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(m_TestAnimationClip);
            foreach (EditorCurveBinding curveBinding in curveBindings)
            {
                AnimationCurve animationCurve = AnimationUtility.GetEditorCurve(m_TestAnimationClip, curveBinding);
                float AnimationTime = (Time.time - StartTime) % m_TestAnimationClip.length;
                float curveValue = animationCurve.Evaluate(AnimationTime);
                int boneIndex = m_BoneContainer.GetBoneIndexFromBonePath(curveBinding.path);
                
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
                        compactPose[boneIndex].scale.x = curveValue;
                        break;
                    case "m_LocalScale.z":
                        compactPose[boneIndex].scale.x = curveValue;
                        break;
                }
            }
            
            RefreshBoneTransforms(compactPose);
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
