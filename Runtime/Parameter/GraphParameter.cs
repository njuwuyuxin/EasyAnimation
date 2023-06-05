using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class GraphParameter
    {
        [Serializable]
        public abstract class Value
        {
            public virtual bool boolValue { get; set; }
            public virtual int intValue { get; set; }
            public virtual float floatValue { get; set; }
            public virtual string stringValue { get; set; }

            public virtual bool IsEqual(Value rhs)
            {
                return false;
            }
            
            public virtual bool IsLessThan(Value rhs)
            {
                return false;
            }
            
            public virtual bool IsLessEqualThan(Value rhs)
            {
                return false;
            }
            
            public virtual bool IsGreaterThan(Value rhs)
            {
                return false;
            }
            
            public virtual bool IsGreaterEqualThan(Value rhs)
            {
                return false;
            }
        }
        
        public int id;
        public string name;
        
        [SerializeReference]
        public Value value;
        
        public List<int> associatedNodes;
    }
}
