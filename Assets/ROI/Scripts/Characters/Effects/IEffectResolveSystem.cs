using System.Collections.Generic;

namespace ROI
{
    public interface IEffectResolveSystem
    {
        List<T> ApplyEffect<T>(ChampionData champion, T data) where T : IEffectData;
        List<T> RemoveEffect<T>(ChampionData champion, T data) where T : IEffectData;
        List<T> GetEffectData<T>(ChampionData champion) where T : IEffectData;
        
        Dictionary<uint, List<T>> GetEffectData<T>() where T : IEffectData;
    }
}