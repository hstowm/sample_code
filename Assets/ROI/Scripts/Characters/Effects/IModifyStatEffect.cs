namespace ROI
{
    public interface IModifyStatEffect
    {
        void ApplyModify<T>(ChampionData championData, T data) where T : ModifyStatEffectData;
        void RemoveModify<T>(ChampionData championData, T data) where T : ModifyStatEffectData;

    }

}
