using BombusApisBee.Core.BeekeeperClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Core.Common.HoneycombShard
{
    public class HoneycombShardCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeycomb Shard Cooldown");
            Description.SetDefault("You cannot use Honeycomb Shard Effects\nBees still get strengthened, though!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }

    public abstract class HoneycombShardItem : BeekeeperAccessory
    {
        internal int _maxCooldown;
        protected HoneycombShardItem(string name, string tooltip, int maxCooldown) : base(name, tooltip + "\nOnly one honeycomb shard can be equipped at a time\n") { _maxCooldown = maxCooldown; }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            // if the incoming item is a honeycomb shard item, return false (can't be equipped with)

            if (equippedItem.ModItem is HoneycombShardItem && incomingItem.ModItem is HoneycombShardItem)
                return false;


            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }

        /// <summary>
        /// if the accessory is equipped on the player and off cooldown
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns></returns>
        public bool CanActivateEffect(Player player) => IsEquipped(player) && !player.HasBuff<HoneycombShardCooldown>();

        /// <summary>
        /// Handles cooldown and on proc effects specific to the player
        /// </summary>
        /// <param name="player"></param>
        public void ActivateEffect(Player player)
        {
            player.AddBuff<HoneycombShardCooldown>(_maxCooldown);
            DoEffects(player);
        }

        /// <summary>
        /// Effects on the player that happen when an effect is procced
        /// </summary>
        public virtual void DoEffects(Player player) { }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (_maxCooldown > 0)
            {
                int index = tooltips.FindIndex(tt => tt.Mod.Equals("Terraria") && tt.Name.Equals("Equipable"));

                if (index != -1)
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "Effect Cooldown", "This accessory has a " + (_maxCooldown / 60) + " second cooldown between uses"));
            }
        }
    }
}
