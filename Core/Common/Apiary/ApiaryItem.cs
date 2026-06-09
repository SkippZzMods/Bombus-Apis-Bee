using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Common.BeeProjectile;
using CalamityMod;
using Terraria.DataStructures;

namespace BombusApisBee.Core.Common.Apiary
{
    public abstract class ApiaryItem : BeekeeperWeapon
    {
        public virtual int BaseUseTime => 20;
        public virtual int AltUseTime => 40;

        /// <summary>
        /// PreAI method for bees spawned from an apiary
        /// Modifiers for speed and range should go here
        /// </summary>
        public virtual void PreApiaryAI(Projectile projectile)
        {

        }

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
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)(Projectile.spriteDirection == 1 ? 0f : Math.PI) + Projectile.spriteDirection * MathHelper.ToRadians(45f);

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            bool modded = Projectile.ModProjectile != null && Projectile.ModProjectile is CommonBeeProjectile;

            float speed = 7f;
            if (modded)
                speed = Math.Max((Projectile.ModProjectile as CommonBeeProjectile).Speed, 1f);

            Projectile.velocity = (Projectile.velocity * 35f + (controlsPlayer.mouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * speed) / 36f;

            if (Projectile.velocity.Length() < 0.1f && Vector2.DistanceSquared(Projectile.Center, controlsPlayer.mouseWorld) < 5 * 5)
                Projectile.velocity += Main.rand.NextVector2Circular(4f, 4f);               
        }

        /// <summary>
        /// Called on the player when they are using an apiary
        /// </summary>
        /// <param name="Player">The player using the apiary</param>
        /// <param name="altUse">Whether or not the player is using the alt use</param>
        public virtual void UpdateApiaryPlayer(Player Player, bool altUse)
        {

        }

        /// <summary>
        /// Modifies the hit of a bee being controlled by an apiary. Mirrors Projectile.ModifyHitNPC
        /// </summary>
        /// <param name="projectile">The projectile that is having its hit modified</param>
        /// <param name="target">The NPC hit</param>
        /// <param name="modifiers">Modifiers for modifying the hit</param>
        public virtual void ModifyApiaryHit(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            
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

            Item.useTime = BaseUseTime;
            Item.useAnimation = BaseUseTime;

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

        public override float UseAnimationMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                float leftClickTime = BaseUseTime;
                float rightClickTime = AltUseTime;

                return rightClickTime / leftClickTime;
            }

            return 1f;
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                float leftClickTime = BaseUseTime;
                float rightClickTime = AltUseTime;

                return rightClickTime / leftClickTime;
            }

            return 1f;
        }

    }

    public class ApiaryPlayer : ModPlayer
    {
        public bool apiaryActive;
        public int holdTimer;
        public int apiaryVisualTimer;
        public int maxVisualTimer;

        public ApiaryItem CurrentApiary => apiaryActive ? Player.HeldItem.ModItem as ApiaryItem : null;

        public override void ResetEffects()
        {
            if (apiaryVisualTimer <= 0)
                maxVisualTimer = 20;
        }

        public override void PreUpdate()
        {
            apiaryActive = false;

            Player.TryGetModPlayer(out ControlsPlayer controlsPlayer);
            controlsPlayer.rightClickListener = true;

            if (Player.HeldItem.ModItem != null && Player.HeldItem.ModItem is ApiaryItem && controlsPlayer.mouseRight && Main.projectile.Any(p => p.active && p.owner == Player.whoAmI && p.ModProjectile != null && p.ModProjectile is ApiaryHoldout))
            {
                if (apiaryVisualTimer < maxVisualTimer)
                    apiaryVisualTimer++;

                if (holdTimer < 20)
                    holdTimer++;

                apiaryActive = true;
            }
            else
            {
                if (holdTimer > 0)
                    holdTimer--;

                if (apiaryVisualTimer > 0)
                    apiaryVisualTimer--;
            } 
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

            if (projectile.ModProjectile is CommonBeeProjectile || vanillaProjectile)
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

                if (source is EntitySource_Parent { Entity: Projectile proj })
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
                    if (projectile.ModProjectile != null)
                        (projectile.ModProjectile as CommonBeeProjectile).speedMultiplier = 1f + player.Beekeeper().BeeSpeedMultiplier;

                    apiaryParent.PreApiaryAI(projectile);
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
