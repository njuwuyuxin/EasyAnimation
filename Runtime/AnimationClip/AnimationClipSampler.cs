using UnityEditor;
using UnityEngine;

namespace AnimationGraph
{
    public class AnimationClipSampler
    {
        private AnimationClip m_AnimationClip;
        private BoneContainer m_BoneContainer;

        private EditorCurveBinding[] m_CurveBindings;

        public AnimationClipSampler( AnimationClip animationClip,BoneContainer boneContainer)
        {
            m_AnimationClip = animationClip;
            m_BoneContainer = boneContainer;
            
            m_CurveBindings = AnimationUtility.GetCurveBindings(m_AnimationClip);
        }
        
        private AnimationClipSampler(){}
        
        public void EvaluatePose(float time, ref CompactPose outPose)
        {
            outPose.ResetToRefPose();

            if (m_AnimationClip.isLooping)
            {
                time %= m_AnimationClip.length;
            }
            
            foreach (EditorCurveBinding curveBinding in m_CurveBindings)
            {
                AnimationCurve animationCurve = AnimationUtility.GetEditorCurve(m_AnimationClip, curveBinding);
                
                float curveValue = animationCurve.Evaluate(time);
                int boneIndex = m_BoneContainer.GetBoneIndexFromBonePath(curveBinding.path);
                
                //Sample Pose, inluding root bone
                switch (curveBinding.propertyName)
                {
                    case "m_LocalPosition.x":
                        outPose[boneIndex].position.x = curveValue;
                        break;
                    case "m_LocalPosition.y":
                        outPose[boneIndex].position.y = curveValue;
                        break;
                    case "m_LocalPosition.z":
                        outPose[boneIndex].position.z = curveValue;
                        break;
                    case "m_LocalRotation.x":
                        outPose[boneIndex].rotation.x = curveValue;
                        break;
                    case "m_LocalRotation.y":
                        outPose[boneIndex].rotation.y = curveValue;
                        break;
                    case "m_LocalRotation.z":
                        outPose[boneIndex].rotation.z = curveValue;
                        break;
                    case "m_LocalRotation.w":
                        outPose[boneIndex].rotation.w = curveValue;
                        break;
                    case "m_LocalScale.x":
                        outPose[boneIndex].scale.x = curveValue;
                        break;
                    case "m_LocalScale.y":
                        outPose[boneIndex].scale.y = curveValue;
                        break;
                    case "m_LocalScale.z":
                        outPose[boneIndex].scale.z = curveValue;
                        break;
                }
            }
        }

        public BoneTransformCurve ExtractRootMotionCurve()
        {
            BoneTransformCurve rootBoneTransformCurve = new BoneTransformCurve();
            foreach (EditorCurveBinding curveBinding in m_CurveBindings)
            {
                //Extract Root Bone Curve
                if (m_BoneContainer.GetBoneByBonePath(curveBinding.path) == m_BoneContainer.rootBone)
                {
                    AnimationCurve animationCurve = AnimationUtility.GetEditorCurve(m_AnimationClip, curveBinding);
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
            }

            return rootBoneTransformCurve;
        }
    }
}