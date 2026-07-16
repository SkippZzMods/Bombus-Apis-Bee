using BombusApisBee.Core.BeekeeperClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Core.Common.Smoker
{
    public abstract class SmokerItem : BeekeeperWeapon
    {
        public virtual void AddDefaults()
        {

        }

        public sealed override void SafeSetDefaults()
        {
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            AddDefaults();
        }

        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }
    }
}
