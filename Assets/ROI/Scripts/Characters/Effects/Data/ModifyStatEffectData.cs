public enum TypeModify
{
    Speed,
    AtkSpeed,
    Armor,
    Health,
    Damage
}

public class ModifyStatEffectData
{
    public TypeModify typeModify;
    public float timeModify;
    public bool removed;

}
