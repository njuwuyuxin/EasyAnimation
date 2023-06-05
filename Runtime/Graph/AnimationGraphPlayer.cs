using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class AnimationGraphPlayer : MonoBehaviour
    {
        public CompiledAnimationGraph animationGraph;

        private AnimationGraphRuntime m_AnimationGraphRuntime;
        private AnimationActor m_Actor;

        void Start()
        {
            m_Actor = new AnimationActor(gameObject);
            m_AnimationGraphRuntime = new AnimationGraphRuntime(m_Actor, animationGraph);
            m_AnimationGraphRuntime.Run();
        }

        void Update()
        {
            m_AnimationGraphRuntime.OnUpdate(Time.deltaTime);
        }

        // private void OnAnimatorMove()
        // {
        //     var deltaPosition = m_Actor.animator.deltaPosition;
        //     var deltaRotation = m_Actor.animator.deltaRotation;
        //     transform.position += deltaPosition;
        //     transform.rotation *= deltaRotation;
        // }

        private void OnDestroy()
        {
            m_AnimationGraphRuntime.Destroy();
        }
        
        public void SetBoolParameter(string parameterName, bool value)
        {
            m_AnimationGraphRuntime.SetBoolParameter(parameterName, value);
        }
        
        public void SetFloatParameter(string parameterName, float value)
        {
            m_AnimationGraphRuntime.SetFloatParameter(parameterName, value);
        }
        
        public void SetStringParameter(string parameterName, string value)
        {
            m_AnimationGraphRuntime.SetStringParameter(parameterName, value);
        }
    }
}
