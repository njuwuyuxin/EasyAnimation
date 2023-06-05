using System;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class IntParameter : GraphParameter
    {
        [Serializable]
        public class IntValue : Value
        {
            [SerializeField]
            private int m_Value;
            public override int intValue
            {
                get => m_Value;
                set => m_Value = value;
            }
            
            public override bool IsEqual(Value rhs)
            {
                return m_Value == rhs.intValue;
            }
            
            public override bool IsLessThan(Value rhs)
            {
                return m_Value < rhs.intValue;
            }
            
            public override bool IsLessEqualThan(Value rhs)
            {
                return m_Value <= rhs.intValue;
            }
            
            public override bool IsGreaterThan(Value rhs)
            {
                return m_Value > rhs.intValue;
            }
            
            public override bool IsGreaterEqualThan(Value rhs)
            {
                return m_Value >= rhs.intValue;
            }
        }

        public IntParameter()
        {
            value = new IntValue();
        }
    }
}