using System;
using UnityEngine;

public class SimpleAnimationPlayer : MonoBehaviour
{
    [System.Serializable]
    public class AnimationReference
    {
        public Animator animator;
        public string variableName = "";

        public void SetTrigger()
        {
            if (animator == null) return;

            animator.SetTrigger(variableName);
            Debug.Log(variableName);
        }

        public void SetBool(bool state)
        {
            animator.SetBool(variableName, state);
        }
    }

    [System.Serializable]
    public class Particles
    {
        public ParticleSystem vfx;

        public void PlayParticles()
        {
            if (vfx == null) return;

            vfx.Play(true);
        }
    }

    public AnimationReference[] animatedObjects;
    public Particles[] particles;
    
    public void TriggerAnimator(int id)
    {
        if (animatedObjects == null) return;
        if (animatedObjects.Length <= 0 || animatedObjects.Length <= id) return;

        animatedObjects[id].SetTrigger();
    }

    public void SetBoolAnimatorTrue(int id)
    {
        if (animatedObjects == null) return;
        if (animatedObjects.Length <= 0 || animatedObjects.Length <= id) return;

        animatedObjects[id].SetBool(true);
    }

    public void SetBoolAnimatorFalse(int id)
    {
        if (animatedObjects == null) return;
        if (animatedObjects.Length <= 0 || animatedObjects.Length <= id) return;

        animatedObjects[id].SetBool(false);
    }

    public void PlayParticles(int id)
    {
        if (particles == null) return;
        if (particles.Length <= 0 || particles.Length <= id) return;

        particles[id].PlayParticles();
    }
}
