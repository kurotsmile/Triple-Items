using UnityEngine;

public class effect_explosie : MonoBehaviour
{
    public ParticleSystemRenderer particle_render;
    
    public void set_textture(Texture text2d)
    {
        this.particle_render.material.mainTexture = text2d;
    }

}
