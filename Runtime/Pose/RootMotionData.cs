using UnityEngine;

namespace AnimationGraph
{
    public struct RootMotionData
    {
        public Vector3 currentPosition;
        public Quaternion currentRotation;
        public Vector3 deltaPosition;
        public Quaternion deltaRotation;
    }
}
