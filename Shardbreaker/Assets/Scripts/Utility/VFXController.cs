using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
    [System.Serializable]
    public class VFX
    {
        public ParticleSystem pfx;
        public VisualEffect vfx;
        public AudioSource sfx;

        public void PlayFX()
        {
            if (pfx != null) pfx.Play();
            if (sfx != null) sfx.Play();
            if (vfx != null) vfx.Play();
        }
        public void StopFX()
        {
            if (pfx != null) pfx.Stop();
            if (sfx != null) sfx.Stop();
            if (vfx != null) vfx.Stop();
        }
    }

    public VFX[] effects;

    public void PlayVFX(int index)
    {
        if (effects == null) return;
        if (effects.Length <= 0 || effects.Length <= index) return;

        effects[index].PlayFX();
    }

    public void StopVFX(int index)
    {
        if (effects == null) return;
        if (effects.Length <= 0 || effects.Length <= index) return;

        effects[index].StopFX();
    }


}
