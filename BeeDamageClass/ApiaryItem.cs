using BombusApisBee.BeeHelperProj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace BombusApisBee.BeeDamageClass
{
    public abstract class ApiaryItem : BeeDamageItem
    {
        /// <summary>
        /// PreDraw method for bees spawned from an apiary
        /// </summary>
        /// <param name="projectile">The projectile being controlled</param>
        /// <param name="lightColor">The projectile's light color</param>
        /// <param name="active">Whether or not the player is currently controlling the bees</param>
        public virtual void PreDrawApiaryBees(Projectile projectile, ref Color lightColor, bool active)
        {

        }

        /// <summary>
        /// PostDraw method for bees spawned from an apiary
        /// </summary>
        /// <param name="projectile">The projectile being controlled</param>
        /// <param name="lightColor">The projectile's light color</param>
        /// <param name="active">Whether or not the player is currently controlling the bees</param>
        public virtual void PostDrawApiaryBees(Projectile projectile, Color lightColor, bool active)
        {

        }

        /// <summary>
        /// Called on bees spawned from the apiary when holding right click
        /// Call base to keep homing toward mouse behavior
        /// </summary>
        /// <param name="projectile">The projectile whos ai is being changed</param>
        /// <param name="controlsPlayer">The controls player used for player input</param>
        public virtual void HoldAI(Projectile Projectile)
        {
            Player player = Main.player[Projectile.owner];

            player.TryGetModPlayer(out ControlsPlayer controlsPlayer);
            controlsPlayer.mouseListener = true;
           
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

        /// <summary>
        /// Modifies the hit of a bee being controlled by an apiary. Mirrors Projectile.ModifyHitNPC
        /// </summary>
        /// <param name="projectile">The projectile that is having its hit modified</param>
        /// <param name="target">The NPC hit</param>
        /// <param name="modifiers">Modifiers for modifying the hit</param>
        public virtual void ModifyApiaryHit(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 2f;
        }

        /// <summary>
        /// Called when a bee being controlled by an apiary hits an NPC. Mirrors Projectile.OnHitNPC
        /// </summary>
        /// <param name="projectile">The projectile that hit</param>
        /// <param name="target">The NPC hit</param>
        public virtual void OnApiaryHit(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {

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

        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }
    }

    public class ApiaryPlayer : ModPlayer
    {
        public bool apiaryActive;
        public int holdTimer;

        public ApiaryItem CurrentApiary => apiaryActive ? Player.HeldItem.ModItem as ApiaryItem : null;

        public override void ResetEffects()
        {
            
        }

        public override void PreUpdate()
        {
            apiaryActive = false;

            Player.TryGetModPlayer(out ControlsPlayer controlsPlayer);
            controlsPlayer.rightClickListener = true;

            if (Player.HeldItem.ModItem != null && Player.HeldItem.ModItem is ApiaryItem && controlsPlayer.mouseRight && Main.projectile.Any(p => p.active && p.owner == Player.whoAmI && p.ModProjectile != null && p.ModProjectile is ApiaryHoldout))
            {
                apiaryActive = true;
                if (holdTimer < 20)
                    holdTimer++;
            }
            else if (holdTimer > 0)
                holdTimer--;
        }
    }

    class ApiaryGlobalProjectile : GlobalProjectile
    {
        public bool fromApiary;
        public int maxTimeleft;

        private ApiaryItem apiaryParent;
        public override bool InstancePerEntity => true;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            bool vanillaProjectile = projectile.type == ProjectileID.Bee || projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.Wasp;

            if (projectile.ModProjectile is BaseBeeProjectile || vanillaProjectile)
            {
                if (source is EntitySource_ItemUse_WithAmmo { Entity: Player p, Item: Item i })
                {
                    if (i.ModItem is ApiaryItem)
                    {
                        apiaryParent = (ApiaryItem)i.ModItem;
                        fromApiary = true;
                        maxTimeleft = projectile.timeLeft;
                    }
                }

                if (source is EntitySource_Parent { Entity: Projectile proj})
                {
                    if (proj.ModProjectile is ApiaryHoldout)
                    {
                        apiaryParent = Main.player[proj.owner].HeldItem.ModItem as ApiaryItem;
                        fromApiary = true;
                        maxTimeleft = projectile.timeLeft;
                    }
                }
            }        
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (fromApiary)
            {
                Player player = Main.player[projectile.owner];

                apiaryParent.PreDrawApiaryBees(projectile, ref lightColor, player.GetModPlayer<ApiaryPlayer>().apiaryActive);
            }

            return base.PreDraw(projectile, ref lightColor);
        }

        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (fromApiary)
            {
                Player player = Main.player[projectile.owner];

                apiaryParent.PostDrawApiaryBees(projectile, lightColor, player.GetModPlayer<ApiaryPlayer>().apiaryActive);
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (fromApiary && projectile.timeLeft < maxTimeleft - 20)
            {
                Player player = Main.player[projectile.owner];

                if (player.GetModPlayer<ApiaryPlayer>().apiaryActive)
                {
                    apiaryParent.HoldAI(projectile);

                    return false;
                }                     
            }

            return true;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (fromApiary && Main.player[projectile.owner].GetModPlayer<ApiaryPlayer>().apiaryActive)
            {
                Main.player[projectile.owner].GetModPlayer<ApiaryPlayer>().CurrentApiary.ModifyApiaryHit(projectile, target, ref modifiers);
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (fromApiary && Main.player[projectile.owner].GetModPlayer<ApiaryPlayer>().apiaryActive)
            {
                Main.player[projectile.owner].GetModPlayer<ApiaryPlayer>().CurrentApiary.OnApiaryHit(projectile, target, hit, damageDone);
            }
        }
    }
}
