using BombusApisBee.Core.BeekeeperClass;

namespace BombusApisBee.Content.Crossmod.Calamity
{
    /// Items that derive from Calamity Mod can override these classes. Otherwise, make sure to use the attribute and override IsLoadingEnabled.
    [JITWhenModsEnabled("CalamityMod")]
    public abstract class CalamityItem : BeeKeeperItem
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
    }

    [JITWhenModsEnabled("CalamityMod")]
    public abstract class CalamityDamageItem : BeekeeperWeapon
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
    }
}
