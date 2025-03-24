using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AnimationGraph
{
    [Serializable]
    [CreateAssetMenu(fileName = "AnimationGraph", menuName = "ScriptableObjects/AnimationGraph")]
    public class CompiledAnimationGraph : ScriptableObject
    {
        public FinalPosePoseNodeConfig finalPosePoseNode;

        [SerializeReference]
        public List<NodeConfig> nodes;

        public List<Connection> nodeConnections;

        [SerializeReference]
        public List<GraphParameter> parameters;

        public void UpdateGraph()
        {
            
        }

        public void EvaluateGraph(ref PoseContext poseContext)
        {
            
        }
    }

    [Serializable]
    public class Connection
    {
        public int sourceNodeId;
        public int targetNodeId;
        public int targetPortIndex;
    }
}
