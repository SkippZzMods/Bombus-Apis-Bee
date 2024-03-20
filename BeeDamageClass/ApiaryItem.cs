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
        /// Call base to keep homing toward mouse behavior
        /// </summary>
        /// <param name="projectile">The projectile whos ai is being changed</param>
        /// <param name="controlsPlayer">The controls player used for player input</param>
        public virtual void HoldAI(Projectile Projectile, ControlsPlayer controlsPlayer)
        {
            controlsPlayer.mouseListener = true;

            Player player = Main.player[Projectile.owner];

            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)(Projectile.spriteDirection == 1 ? 0f : Math.PI) +Projectile.spriteDirection * MathHelper.ToRadians(45f);

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            bool modded = Projectile.ModProjectile != null && Projectile.ModProjectile is BaseBeeProjectile;
            bool giant = modded && (Projectile.ModProjectile as BaseBeeProjectile).otherGiant;

            Projectile.velocity = (Projectile.velocity * 35f + Utils.SafeNormalize(controlsPlayer.mouseWorld - Projectile.Center, Vector2.UnitX) * 7f) / 36f;
        }

        // see below
        public virtual void AddStaticDefaults()
        {
            
        }

        public sealed override void SafeSetStaticDefaults()
        {
            //ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            DisplayName.SetDefault("Apiary");
            AddStaticDefaults();
        }

        // im not doing SafeSafeSetDefaults
        public virtual void AddDefaults()
        {
            
        }

        public sealed override void SafeSetDefaults()
        {
            Item.channel = true;
            Item.noUseGraphic = true;
            AddDefaults();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }

    public class ApiaryPlayer : ModPlayer
    {
        public bool apiaryActive => Player.HeldItem.ModItem is ApiaryItem && Main.mouseRight;
    }

    class ApiaryGlobalProjectile : GlobalProjectile
    {
        public bool fromApiary;
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
            if (fromApiary)
            {
                Player player = Main.player[projectile.owner];

                player.TryGetModPlayer(out ControlsPlayer controlsPlayer);
                controlsPlayer.rightClickListener = true;

                if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is ApiaryItem && controlsPlayer.mouseRight)
                {
                    (player.HeldItem.ModItem as ApiaryItem).HoldAI(projectile, controlsPlayer);
                    Main.NewText(projectile.timeLeft);

                    return false;
                }                     
            }

            return true;
        }
    }
}
