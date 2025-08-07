namespace ROI
{
    /// <summary>
    /// Any source types which current changed stat
    /// </summary>
    public enum SourceTypes : byte
    {
        // BaseLevel,              // from base level
        PassiveSkill,               // from passive Skill
        Trait,                      // from trait
        Ability,
        Equipment,                  // from equipment
        AllyBuff,                   // from ally buff (use active skill)
        ZSet,                       // z-set
        Civ                         // Civ
    }
}