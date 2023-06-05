using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AnimationGraph
{
    public enum EConditionType
    {
        NotEqual = 0,
        Equal = 1,
        Less = 2,
        LessEqual = 3,
        Greater = 4,
        GreaterEqual = 5,
    }
    
    [Serializable]
    public class TransitionCondition
    {
        public int parameterId;
        [FormerlySerializedAs("conditionType")] public EConditionType conditionType;
        [SerializeReference]
        public GraphParameter.Value value;
    }
    
    [Serializable]
    public class TransitionConfig : EdgeConfig
    {
        public int sourceStateId;
        public int targetStateId;
        public float blendTime;
        public List<TransitionCondition> conditions = new List<TransitionCondition>();
    }
}