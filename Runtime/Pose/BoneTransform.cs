using System.Collections;
using System.Collections.Generic;
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
}
