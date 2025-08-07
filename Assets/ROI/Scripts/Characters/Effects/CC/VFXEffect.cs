using ROI;
using System.Collections.Generic;
using UnityEngine;


public enum VFXPositionType
{
    Mesh,Head,Center
}

public class VFXEffect:MonoBehaviour
{
    // Start is called before the first frame update
    public List<ParticleSystem> main_particle = new List<ParticleSystem>();
    public VFXPositionType display_position;
    Vector3 offset = Vector3.up*1.2f;

    private void Start()
    {
        //main_particle.Add(GetComponent<ParticleSystem>());
    }

    public void PLayEffectOnChampion(ChampionData player)
    {
        transform.parent = player.transform;
        switch (display_position)
        {
            case VFXPositionType.Mesh:        
            foreach (ParticleSystem particle in main_particle)
            {
                var p = particle.shape;

                var reference_vfx = player.GetComponent<ChampionStatusVFX>();

                p.skinnedMeshRenderer = reference_vfx == null ? player.GetComponent<SkinnedMeshRenderer>() : reference_vfx.vfx_renderer;

                particle.Play();
            }
            break;
            case VFXPositionType.Head:
                transform.localPosition = new Vector3(0, player.CenterPosition.y * 2 + 0.1f, 0 );
                break;
            case VFXPositionType.Center:
                transform.localPosition = new Vector3(0, player.CenterPosition.y/2, 0);
                break;
        }


    }

    public void Remove()
    {
        Destroy(this.gameObject);
    }
}
