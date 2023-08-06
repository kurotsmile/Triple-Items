using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effec_explosie : MonoBehaviour
{
    public ParticleSystem particle;

    public void set_sp()
    {
        this.particle.GetComponent<RenderTexture>();
    }
}
