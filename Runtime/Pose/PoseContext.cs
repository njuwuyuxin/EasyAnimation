using System;


namespace AnimationGraph
{
    public ref struct PoseContext
    {
        public CompactPose Pose;

        public PoseContext(Span<BoneTransform> boneTransforms)
        {
            // Pose = new CompactPose(boneTransforms);
            Pose = new CompactPose();
        }
    }
}
