using System;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class BoolParameter : GraphParameter
    {
        [Serializable]
        public class BoolValue : Value
        {
            [SerializeField]
            private bool m_Value;
            public override bool boolValue
            {
                get => m_Value;
                set => m_Value = value;
            }
            
            public override bool IsEqual(Value rhs)
            {
                return m_Value == rhs.boolValue;
            }
        }

        public BoolParameter()
        {
            value = new BoolValue();
        }
    }
}