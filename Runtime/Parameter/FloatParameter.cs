using System;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class FloatParameter : GraphParameter
    {
        [Serializable]
        public class FloatValue : Value
        {
            [SerializeField]
            private float m_Value;
            public override float floatValue
            {
                get => m_Value;
                set => m_Value = value;
            }
            
            public override bool IsEqual(Value rhs)
            {
                return Mathf.Approximately(m_Value, rhs.floatValue);
            }
            
            public override bool IsLessThan(Value rhs)
            {
                return m_Value < rhs.floatValue;
            }
            
            public override bool IsLessEqualThan(Value rhs)
            {
                return m_Value <= rhs.floatValue;
            }
            
            public override bool IsGreaterThan(Value rhs)
            {
                return m_Value > rhs.floatValue;
            }
            
            public override bool IsGreaterEqualThan(Value rhs)
            {
                return m_Value >= rhs.floatValue;
            }
        }

        public FloatParameter()
        {
            value = new FloatValue();
        }
    }
}