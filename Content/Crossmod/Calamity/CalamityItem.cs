using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Content.Crossmod.Calamity
{
    /// Items that derive from Calamity Mod can override these classes. Otherwise, make sure to use the attribute and override IsLoadingEnabled.
    [JITWhenModsEnabled("CalamityMod")]
    public abstract class CalamityItem : BeeKeeperItem
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
    }

    [JITWhenModsEnabled("CalamityMod")]
    public abstract class CalamityDamageItem : BeeDamageItem
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
    }
}
