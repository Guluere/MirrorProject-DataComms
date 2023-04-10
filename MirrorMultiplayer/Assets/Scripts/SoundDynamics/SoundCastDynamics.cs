using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCastDynamics : MonoBehaviour
{
    public SoundCast[] soundCasts;

    private void Start()
    {
        foreach (SoundCast soundCast in soundCasts)
        {
            soundCast.soundDynamic = this;
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach (SoundCast SoundCast in soundCasts)
        {
            if (SoundCast == null)
            {
                return;
            }
            Gizmos.DrawWireSphere(gameObject.transform.position, SoundCast.soundRange);
        }
    }
}

[System.Serializable]
public class SoundCast
{
    [HideInInspector] public SoundCastDynamics soundDynamic;
    public string castName;
    //public string earsTag;
    //public LayerMask earsLayer;
    [Min(0f)] public float soundRange;
}