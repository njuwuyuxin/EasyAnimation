using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimationGraph
{
    public class AnimationActor
    {
        public GameObject gameObject { get; set; }
        public Animator animator { get; set; }
        public AnimationActor(GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.animator = gameObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("AnimationActor:" + gameObject + "Create Failed!");
            }
        }
    }
}
