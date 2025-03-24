using System.Collections.Generic;
using UnityEngine;

namespace AnimationGraph
{
    public class BoneContainer
    {
        private Dictionary<string, Transform> bonePathCache = new Dictionary<string, Transform>();
        private BoneTransform[] m_RefPose;
        public BoneTransform[] refPose => m_RefPose;

        private SkinnedMeshRenderer m_SkinnedMeshRenderer;
        private Transform[] m_RawBones;
        private Transform m_RootBone;
        public Transform rootBone => m_RootBone;
        public int boneCount => m_RawBones.Length;
        
        private Dictionary<string, int> bonePathToBoneIndex = new Dictionary<string, int>();

        public void InitializeBoneContainer(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            m_SkinnedMeshRenderer = skinnedMeshRenderer;
            m_RawBones = skinnedMeshRenderer.bones;
            m_RootBone = skinnedMeshRenderer.rootBone;
            
            BuildBoneMap();
            InitializeRefPose();
        }
        
        private void BuildBoneMap()
        {
            bonePathToBoneIndex.Clear();
            for (int boneIndex = 0; boneIndex < m_RawBones.Length; boneIndex++)
            {
                string path = GetBonePath(m_RawBones[boneIndex]);
                bonePathToBoneIndex.Add(path, boneIndex);
            }
        }
        
        private void InitializeRefPose()
        {
            m_RefPose = new BoneTransform[m_SkinnedMeshRenderer.bones.Length];
            List<Matrix4x4> bindposes = new List<Matrix4x4>();
            m_SkinnedMeshRenderer.sharedMesh.GetBindposes(bindposes);
            
            for(int boneIndex = 0; boneIndex < m_SkinnedMeshRenderer.bones.Length; boneIndex++)
            {
                Transform bone = m_SkinnedMeshRenderer.bones[boneIndex];
                Matrix4x4 modelSpaceMatrix = bindposes[boneIndex].inverse;
                BoneTransform refPoseTransform = new BoneTransform();

                if (bone != m_SkinnedMeshRenderer.rootBone)
                {
                    Matrix4x4 parentBoneModelMatrix = bone.parent.localToWorldMatrix;
                    Matrix4x4 localMatrix = parentBoneModelMatrix.inverse * modelSpaceMatrix;
                    refPoseTransform.position = localMatrix.GetPosition();
                    refPoseTransform.rotation = localMatrix.rotation;
                    refPoseTransform.scale = localMatrix.lossyScale;
                }
                else
                {
                    refPoseTransform.position = modelSpaceMatrix.GetPosition();
                    refPoseTransform.rotation = modelSpaceMatrix.rotation;
                    refPoseTransform.scale = modelSpaceMatrix.lossyScale;
                }
                
                m_RefPose[boneIndex] = refPoseTransform;
            }
        }

        public int GetBoneIndexFromBonePath(string bonePath)
        {
            if (bonePathToBoneIndex.ContainsKey(bonePath))
            {
                return bonePathToBoneIndex[bonePath];
            }
            else
            {
                return -1;
            }
        }

        public Transform GetBoneByBonePath(string bonePath)
        {
            int boneIndex = GetBoneIndexFromBonePath(bonePath);
            if (boneIndex != -1)
            {
                return m_RawBones[boneIndex];
            }
            else
            {
                return null;
            }
        }
        
        private string GetBonePath(Transform bone)
        {
            List<string> pathParts = new List<string>();
            Transform current = bone;
    
            // 从当前骨骼向上遍历到根骨骼
            while (current != null)
            {
                // 如果到达根骨骼则停止
                if (current == m_RootBone)
                {
                    pathParts.Add(current.name);
                    break;
                }
                pathParts.Add(current.name);
                current = current.parent;
            }
    
            // 反转路径以保证从根开始
            pathParts.Reverse();
    
            return string.Join("/", pathParts);
        }
    }
}