using System;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class StringParameter : GraphParameter
    {
        [Serializable]
        public class StringValue : Value
        {
            [SerializeField]
            private string m_Value;
            public override string stringValue
            {
                get => m_Value;
                set => m_Value = value;
            }
            
            public override bool IsEqual(Value rhs)
            {
                return m_Value.Equals(rhs.stringValue);
            }
        }

        public StringParameter()
        {
            value = new StringValue()
            {
                stringValue = string.Empty
            };
        }
    }
}