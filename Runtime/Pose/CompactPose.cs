using System;

namespace AnimationGraph
{
    public ref struct CompactPose
    {
        private Span<BoneTransform> m_BoneTransforms;
        private BoneContainer m_BoneContainer;

        public CompactPose(Span<BoneTransform> boneTransformsBuffer, BoneContainer boneContainer)
        {
            m_BoneTransforms = boneTransformsBuffer;
            m_BoneContainer = boneContainer;
        }

        public void ResetToRefPose()
        {
            m_BoneTransforms = m_BoneContainer.refPose;
        }
        
        public ref BoneTransform this[int index]
        {
            get
            {
                if (index >= m_BoneTransforms.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return ref m_BoneTransforms[index];
            }
        }

        public void SetBoneTransform(int index, BoneTransform boneTransform)
        {
            if (index >= m_BoneTransforms.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            m_BoneTransforms[index] = boneTransform;
        }
    }
}
