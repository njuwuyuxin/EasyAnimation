using UnityEngine;

namespace AnimationGraph
{
    public struct BoneTransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        
        public BoneTransform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        public static BoneTransform identity => new BoneTransform(Vector3.zero, Quaternion.identity, Vector3.zero);
    }
    
    public struct BoneTransformCurve
    {
        public AnimationCurve positionX;
        public AnimationCurve positionY;
        public AnimationCurve positionZ;
        public AnimationCurve rotationX;
        public AnimationCurve rotationY;
        public AnimationCurve rotationZ;
        public AnimationCurve rotationW;
        public AnimationCurve scaleX;
        public AnimationCurve scaleY;
        public AnimationCurve scaleZ;

        public BoneTransform Evaluate(float time)
        {
            BoneTransform boneTransform;
            boneTransform.position.x = positionX?.Evaluate(time) ?? 1;
            boneTransform.position.y = positionY?.Evaluate(time) ?? 1;
            boneTransform.position.z = positionZ?.Evaluate(time) ?? 1;
            boneTransform.rotation.x = rotationX?.Evaluate(time) ?? 0;
            boneTransform.rotation.y = rotationY?.Evaluate(time) ?? 0;
            boneTransform.rotation.z = rotationZ?.Evaluate(time) ?? 0;
            boneTransform.rotation.w = rotationW?.Evaluate(time) ?? 1;
            boneTransform.scale.x = scaleX?.Evaluate(time) ?? 1;
            boneTransform.scale.y = scaleY?.Evaluate(time) ?? 1;
            boneTransform.scale.z = scaleZ?.Evaluate(time) ?? 1;
            return boneTransform;
        }
    }
}
