using System;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class EdgeConfig
    {
        [SerializeField]
        public int id;

        public void SetId(int id)
        {
            this.id = id;
        }
    }
}