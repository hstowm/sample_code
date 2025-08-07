using UnityEngine;
using Mirror;
using ROI;

public class DisplayableEffect : NetworkBehaviour
{
    // Start is called before the first frame update
    public GameObject VFX_prefab;
    private VFXEffect VFX_instance;
    [SerializeField] protected AudioClip effectSound;
    [ClientRpc]
    protected virtual void ApplyVFX(ChampionData champion)
    {
        if (effectSound)
        {
            SoundManager.sfx_basicAtk.PlayOneShot(effectSound);
        }
        // Debug.Log("Add new VFX");
        if (!VFX_prefab)
        {
            Logs.Error("Not attack VFX_Prefab object");
            return;
        }


        if (VFX_instance == null)
        {
            //VFX_instance = GameObject.Instantiate(VFX_prefab, champion.gameObject.transform.position, Quaternion.identity).GetComponent<VFXEffect>();
            
            VFX_instance = GameObject.Instantiate(VFX_prefab, champion.transform).GetComponent<VFXEffect>();
            if (VFX_instance == null)
            {
                Logs.Error("Don't have VFXEffect component in VFX_Prefab");
                return;
            }
        }
        else
        {
            VFX_instance.Remove();
            
            VFX_instance = GameObject.Instantiate(VFX_prefab, champion.transform).GetComponent<VFXEffect>();
        }
        VFX_instance.PLayEffectOnChampion(champion);
    }
    [ClientRpc]
    protected virtual void ApplyIcon(ChampionData champion, int _level, ChampionEffects type, float duration_remain, float duration_total)
    {
        if(isServer)
            champion.AddEffect(type);
        ChampionHealthBar.instance.healthBars[champion.netId].AddEffect(type, _level, duration_remain, duration_total);
    }
    [ClientRpc]
    protected virtual void ClearVFX()
    {
        // Debug.Log("Clear VFX");
        if (VFX_instance != null)
        {
            VFX_instance.Remove();
        }
        VFX_instance = null;
    }
    [ClientRpc]
    protected virtual void RemoveIcon(ChampionData champion, ChampionEffects type, int level)
    {
        if(isServer)
            champion.RemoveEffect(type);
        if(ChampionHealthBar.instance.healthBars.TryGetValue(champion.netId, out _))
            ChampionHealthBar.instance.healthBars[champion.netId].RemoveEffect(type, level);
    }
    [ClientRpc]
    protected void RemoveEffect()
    {
        gameObject.Recycle();
    }


}
