using BombusApisBee.BeeHelperProj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;

namespace BombusApisBee.BeeDamageClass
{
    public abstract class ApiaryItem : BeeDamageItem
    {
        /// <summary>
        /// Called on bees spawned from the apiary when holding right click
        /// </summary>
        /// <param name="projectile">The projectile whos ai is being changed</param>
        public virtual void HoldAI(Projectile projectile)
        {
            
        }

        // see below
        public virtual void AddStaticDefaults()
        {
            
        }

        public sealed override void SafeSetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            DisplayName.SetDefault("Apiary");
            AddStaticDefaults();
        }

        // im not doing SafeSafeSetDefaults
        public virtual void AddDefaults()
        {
            
        }

        public sealed override void SafeSetDefaults()
        {
            AddDefaults();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool SafeCanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.channel = true;
                Item.noUseGraphic = true;
            }
            else
            {
                Item.channel = false;
                Item.noUseGraphic = false;
            }             

            return true;
        }
    }

    public class ApiaryPlayer : ModPlayer
    {
        public bool apiaryActive => Player.HeldItem.ModItem is ApiaryItem && Player.channel;

    }

    class ApiaryGlobalProjectile : GlobalProjectile
    {
        public bool fromApiary;
        public bool Active(int playerWhoAmI) => Main.player[playerWhoAmI].GetModPlayer<ApiaryPlayer>().apiaryActive;
        public override bool InstancePerEntity => true;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            bool vanillaProjectile = projectile.type == ProjectileID.Bee || projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.Wasp;

            if (projectile.ModProjectile is BaseBeeProjectile || vanillaProjectile)
            {
                if (source is EntitySource_ItemUse_WithAmmo { Entity: Player p, Item: Item i })
                {
                    if (i.ModItem is ApiaryItem)
                        fromApiary = true;
                }
            }        
        }

        public override bool PreAI(Projectile projectile)
        {
            if (Active(projectile.owner))
            {
                Player player = Main.player[projectile.owner];

                (player.HeldItem.ModItem as ApiaryItem).HoldAI(projectile);

                return false;
            }

            return true;
        }
    }
}
