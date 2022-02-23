using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePause : MonoBehaviour
{
    public ParticleSystem particles;
    [Range(0,5)]
    public float pauseAfterDelay = 1f;
    bool isPaused = false;
    bool wasPaused = false;
    float time = 0;

    void Awake()
    {
        time = 0;
        isPaused = false;
    }

    void Update()
    {
        if (isPaused && !particles.isPaused)
        {
            particles.Pause();
        }
        else if(!isPaused && particles.isPlaying)
        {
            particles.Play();
        }

        if(!wasPaused && time >= pauseAfterDelay)
        {
            isPaused = true;
            wasPaused = true;
        }

        time += Time.deltaTime;
    }

    public void SetPause(bool state)
    {
        isPaused = state;
    }
}
