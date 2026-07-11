using BombusApisBee.Assets;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Content.Jungle.Items.HiveBandAccessory;
using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Systems.ParticleSystem;
using BombusApisBee.Core.Systems.PixelationSystem;
using BombusApisBee.Core.Systems.PrimitiveSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Forest.Items.HoneyFlareGun
{
    public class HoneyFlareNPC : GlobalNPC
    {
        public List<HoneyFlare> flares = [];
        public int explosionDelay;
        public int playerIndexWhoTriggeredExplosion;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.CanBeChasedBy();
        }

        public override void PostAI(NPC npc)
        {
            if (explosionDelay > 0)
            {
                explosionDelay--;

                if (explosionDelay == 0)
                    ExplodeSelf(npc, playerIndexWhoTriggeredExplosion);
            }
        }

        public void ExplodeSelf(NPC n, int playerIndex)
        {
            if (n.TryGetGlobalNPC<HoneyFlareNPC>(out var gnpc))
            {
                int count = 0;
                int damage = 0;

                foreach (HoneyFlare flare in gnpc.flares)
                {
                    var projectile = flare.Projectile;

                    if (flare is null || !projectile.active || projectile.owner != playerIndex)
                        continue;

                    count++;

                    if (count < 9)
                        damage += projectile.damage / 2;

                    projectile.active = false;
                }

                float strength = MathHelper.Lerp(0.33f, 1f, count / 9f);

                if (strength > 1f)
                    strength = 1f;

                if (Main.myPlayer == playerIndex)
                    Projectile.NewProjectile(n.GetSource_Misc("BombusApisBee: Honey Flare Explosion Spawn"), n.Center, Vector2.Zero, ProjectileType<HoneyFlareExplosion>(), damage, 5f * strength, playerIndex, 70 * strength);

                Main.player[playerIndex].Bombus().AddShake((int)(9 * strength));
                SoundID.DD2_ExplosiveTrapExplode.PlayWith(n.position, 0, 0.2f, 0.33f);
                BombusApisBee.HoneycombWeapon.PlayWith(n.position, 0.1f, 0.1f, 0.6f);

                for (int i = 0; i < (int)(50 * strength); i++)
                {
                    ParticleHandler.SpawnParticle(new CompositeSmoke(n.Center + Main.rand.NextVector2Circular(25f, 25f) * strength, Main.rand.NextVector2Circular(9f, 9f) * strength, new Color(255, Main.rand.Next(170, 190), 20), 50, false, true, SmokeUpdate));

                    ParticleHandler.SpawnParticle(new SmallCompositeSmoke(n.Center + Main.rand.NextVector2Circular(25f, 25f) * strength, Main.rand.NextVector2Circular(9f, 9f) * strength, new Color(255, Main.rand.Next(190, 210), 50), 60, false, true, SmokeUpdate));

                    Dust.NewDustPerfect(n.Center, DustID.Honey2, Main.rand.NextVector2Circular(8f, 8f) * strength, 110, default, 1.25f);

                    Dust.NewDustPerfect(n.Center, DustID.Honey2, Main.rand.NextVector2Circular(25f, 25f) * strength, 110, default, 1.75f).noGravity = true;
                }

                for (int i = 0; i < (int)(15 * strength); i++)
                {
                    Dust.NewDustPerfect(n.Center, DustType<BandHoneyDust>(), Main.rand.NextVector2Circular(9f, 9f) * strength, 100, default, 2.5f * strength);

                    Vector2 velocity = Main.rand.NextVector2CircularEdge(12f, 12f) * Main.rand.NextFloat(0.9f, 1f) * strength;

                    ParticleHandler.SpawnParticle(new GlowLineParticle(n.Center, velocity,
                        new Color(255, 150, 50), velocity.ToRotation(), new Vector2(0.5f, 2f) * 0.6f, 30 + Main.rand.Next(10, 20), false, ExtraUpdate));
                }

                if (count > 10)
                    count = 10;

                for (int i = 0; i < count * 2; i++)
                {
                    ParticleHandler.SpawnParticle(new HoneyFlareParticle(n.Center + Main.rand.NextVector2Circular(35f, 35f), Main.rand.NextVector2Circular(15f, 15f) * strength, Main.rand.NextFloat(0.9f, 1.2f), Main.rand.Next(50, 80), i % 2 == 0 ? 1 : 0));
                }

                static void ExtraUpdate(Particle p)
                {
                    p.Velocity *= 0.92f;
                }

                static void SmokeUpdate(Particle p)
                {
                    p.Velocity *= 0.95f;
                    p.Velocity.Y -= 0.05f;
                }

                gnpc.flares.Clear();
            }
        }

        public static void ExplodeAllStuckNPCs(int playerIndex)
        {
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.TryGetGlobalNPC<HoneyFlareNPC>(out var gnpc))
                {
                    if (gnpc.flares.Count <= 0)
                        continue;

                    gnpc.explosionDelay = Main.rand.Next(5, 15);
                    gnpc.playerIndexWhoTriggeredExplosion = playerIndex;
                }
            }
        }
    }

    class HoneyFlareExplosion : ModProjectile
    {
        private List<Vector2> cache;

        private Trail trail;
        private Trail trail2;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        private float Progress => 1 - Projectile.timeLeft / 20f;

        private float Radius => Projectile.ai[0] * EaseFunction.EaseQuinticOut.Ease(Progress);

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shockwave");
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            for (int k = 0; k < 6; k++)
            {
                float rot = Main.rand.NextFloat(0, 6.28f);

                Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * Radius, DustID.Honey2,
                    Vector2.One.RotatedBy(rot) * 0.5f, 150, default, 1.2f).noGravity = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 line = targetHitbox.Center.ToVector2() - Projectile.Center;
            line.Normalize();
            line *= Radius;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + line);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            return false;
        }

        private void ManageCaches()
        {
            if (cache is null)
            {
                cache = new List<Vector2>();

                for (int i = 0; i < 40; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            for (int k = 0; k < 40; k++)
            {
                cache[k] = Projectile.Center + Vector2.One.RotatedBy(k / 39f * 6.28f) * Radius;
            }

            while (cache.Count > 40)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 15 * (1f - Progress), factor =>
            {
                return new Color(255, 130, 25);
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 8 * (1f - Progress), factor =>
            {
                return new Color(255, 190, 50);
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[39];

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = cache[39];
        }

        public void DrawPrimitives()
        {
            GetInstance<PixelationSystem>().QueueRenderAction("UnderTiles", () =>
            {
                Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.EffectMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.01f);
                effect.Parameters["repeats"].SetValue(5f);
                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

                trail?.Render(effect);
                trail2?.Render(effect);
            });
        }
    }

    class HoneyFlareParticle : Particle
    {
        internal int _variant;
        public override ParticleDrawType DrawType => ParticleDrawType.Custom;
        public HoneyFlareParticle(Vector2 position, Vector2 velocity, float scale, int maxTime, int variant)
        {
            Position = position;
            Velocity = velocity;
            Rotation = Main.rand.NextFloat(6.28f);
            Scale = scale;
            MaxTime = maxTime;
            Color = Color.White;

            _variant = variant;
        }

        public override void Update()
        {
            Velocity.X *= 0.96f;
            Velocity.Y += 0.2f;     
            Rotation += Velocity.Length() * 0.1f;

            if (Main.rand.NextBool(5))
                Dust.NewDustPerfect(Position, DustID.Honey2, Velocity.RotatedByRandom(0.3f) * 0.5f, 150, default, 1.2f * (1f - Progress)).noGravity = true;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            var texture = Texture;

            var frame = texture.Frame(1, 2, 0, _variant);

            float progress = Progress;

            float fadeIn;

            if (progress < 0.25f)
                fadeIn = EaseBuilder.EaseCircularOut.Ease(progress / 0.25f);
            else
                fadeIn = EaseBuilder.EaseCircularIn.Ease(1f - (progress - 0.25f) / 0.75f);

            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, Color * fadeIn, Rotation, frame.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }

    public class HoneyFlareGun : BeekeeperWeapon
    {
        public int MAX_SHOTS = 15;
        public int shots;

        bool reload;
        
        public float shootRotation;
        public int shootDirection;

        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Sugar Shot");
            Tooltip.SetDefault($"Can fire up to {MAX_SHOTS} sticky flares before needing to reload\nUpon reloading, detonate all existing flares");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 14;

            Item.noMelee = true;
            Item.width = 25;
            Item.height = 25;

            Item.useTime = 24;  
            Item.useAnimation = 24;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(0, 1, 25, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<HoneyFlare>();
            Item.shootSpeed = 19;
            Item.UseSound = null;

            honeyCost = 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.FlareGun, 1).
                AddIngredient(ItemID.BottledHoney, 5).
                AddIngredient(ItemType<PollenItem>(), 20).
                AddTile(TileID.Anvils).
                Register();
        }

        public override bool SafeCanUseItem(Player player)
        {

            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 35f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            float animProgress = 1f - ((float)player.itemTime / (float)player.itemTimeMax);

            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float itemRotation = player.compositeFrontArm.rotation + 1.5707964f * player.gravDir;
            Vector2 itemPosition = player.MountedCenter;

            if (animProgress < 0.2f)
            {
                float lerper = animProgress / 0.2f;
                itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(0f, -5f, EaseBuilder.EaseCircularOut.Ease(lerper));
            }
            else
            {
                float lerper = (animProgress - 0.2f) / 0.8f;
                itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(-5f, 0f, EaseBuilder.EaseBackInOut.Ease(lerper));
            }

            Vector2 itemSize = new Vector2(34f, 24f);
            Vector2 itemOrigin = new Vector2(-35f, 5f);

            BeeUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, new Vector2?(itemOrigin), false, false, true);
        }

        public override void UseItemFrame(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float animProgress = 1f - ((float)player.itemTime / (float)player.itemTimeMax);
            float rotation = shootRotation * player.gravDir + 1.5707964f;

            if (animProgress < 0.15f)
            {
                float lerper = animProgress / 0.15f;
                rotation += MathHelper.Lerp(0f, -.5f, EaseBuilder.EaseCircularOut.Ease(lerper)) * player.direction;
            }
            else
            {
                float lerper = (animProgress - 0.15f) / 0.85f;
                rotation += MathHelper.Lerp(-.5f, 0, EaseBuilder.EaseBackInOut.Ease(lerper)) * player.direction;
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (++shots > MAX_SHOTS)
            {
                Item.noUseGraphic = true;

                Projectile.NewProjectile(source, position, Vector2.UnitX * Math.Sign(velocity.X), ProjectileType<HoneyFlareGunReload>(), damage, knockback, player.whoAmI);

                SoundID.Unlock.PlayWith(position, 0, 0.2f, 1.5f);

                shots = 0;

                return false;
            }

            Item.noUseGraphic = false;

            Vector2 visualPosition = position + new Vector2(0, -5 * player.direction).RotatedBy(velocity.ToRotation());

            Projectile.NewProjectile(source, visualPosition, velocity, type, damage, knockback, player.whoAmI);

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(visualPosition + (Vector2.UnitY.RotatedBy(velocity.ToRotation()) * -5f) * player.direction, ModContent.DustType<HoneyDust>(), velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.25f), Main.rand.Next(100)).noGravity = true;
                Dust.NewDustPerfect(visualPosition + (Vector2.UnitY.RotatedBy(velocity.ToRotation()) * -5f) * player.direction, DustID.Honey2, velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.25f), Main.rand.Next(100)).noGravity = true;
            }

            SoundID.Item11.PlayWith(position, 0, 0.35f, 1f);

            shootRotation = (player.Center - Main.MouseWorld).ToRotation();
            shootDirection = (Main.MouseWorld.X < player.Center.X) ? -1 : 1;

            
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-11, 0);
        }
    }

    public class HoneyFlareGunReload : ModProjectile
    {
        Vector2 offset;
        float muzzleRotation;
        float flareRotation;

        int flashTimer;

        bool activatedExplosions;

        public override string Texture => "BombusApisBee/Content/Forest/Items/HoneyFlareGun/HoneyFlareGun";
        public Vector2 ArmPosition => Owner.RotatedRelativePoint(Owner.MountedCenter, true) + new Vector2((12f + offset.X) * Projectile.direction, -4f + offset.Y).RotatedBy((Projectile.velocity * Projectile.direction).ToRotation());
        public float Progress => 1f - Projectile.timeLeft / 110f;
        public ref float Timer => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 26;
            
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 110;
        }

        public override void AI()
        {
            if (flashTimer > 0)
                flashTimer--;

            if (Timer == 23)
            {
                SoundID.DD2_MonkStaffSwing.PlayWith(Owner.Center, 0, 0.2f, 1f);
            }

            if (Timer == 65)
            {
                BombusApisBee.HoneycombWeapon.PlayWith(Owner.Center, 0, 0.2f, 0.35f);
            }

            if (Timer == 85)
            {
                SoundID.Item11.PlayWith(Owner.Center, -0.5f, 0.2f, 1f);
                SoundID.DD2_MonkStaffSwing.PlayWith(Owner.Center, 0, 0.2f, 1f);
            }

            if (Progress < 0.15f)
            {
                float lerp = EaseBuilder.EaseCircularOut.Ease(Progress / 0.15f);

                Projectile.rotation = Utils.AngleLerp(0, -0.55f, lerp) * Projectile.direction;
                offset = Vector2.Lerp(new Vector2(0, 0), new Vector2(-4, -6), lerp);
            }
            else if (Progress < 0.5f)
            {
                float lerp = EaseBuilder.EaseCircularInOut.Ease((Progress - 0.15f) / 0.35f);

                Projectile.rotation = Utils.AngleLerp(-0.55f, 1.15f, lerp) * Projectile.direction;
                offset = Vector2.Lerp(new Vector2(-4, -6), new Vector2(-4, 12), lerp);

                muzzleRotation = Utils.AngleLerp(0f, 0.5f, lerp) * Projectile.direction;

                Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + Utils.AngleLerp(0, -3, lerp) * Projectile.direction);
            }
            else
            {
                if (!activatedExplosions && Progress > 0.65f)
                {
                    flashTimer = 20;
                    
                    for (int i = 0; i < 6; i++)
                    {
                        Dust.NewDustPerfect(ArmPosition + new Vector2(8f * Projectile.direction, 0f), DustID.Honey2, Main.rand.NextVector2Circular(3f, 3f), 90, default, 1.25f).noGravity = true;
                    }

                    HoneyFlareNPC.ExplodeAllStuckNPCs(Owner.whoAmI);
                    activatedExplosions = true;
                }

                if (Progress > 0.75f)
                {
                    float lerp = EaseBuilder.EaseBackInOut.Ease((Progress - 0.75f) / 0.25f);

                    Projectile.rotation = Utils.AngleLerp(1.15f, 0, lerp) * Projectile.direction;
                    offset = Vector2.Lerp(new Vector2(-4, 12), new Vector2(0, 0), lerp);

                    muzzleRotation = Utils.AngleLerp(0.5f, 0f, lerp) * Projectile.direction;
                }
                else
                {
                    float otherLerp = EaseBuilder.EaseQuinticIn.Ease((Progress - 0.5f) / 0.25f);

                    Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + Utils.AngleLerp(-3f, 0f, otherLerp) * Projectile.direction);
                }
            }

            Timer++;

            UpdateHeldProjectile();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer <= 2)
                return false;

            var texMuzzle = Request<Texture2D>(Texture + "_Muzzle").Value;
            var texGrip = Request<Texture2D>(Texture + "_Grip").Value;
            
            var flareTexture = Request<Texture2D>("BombusApisBee/Content/Forest/Items/HoneyFlareGun/HoneyFlare").Value;
            var star = Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/StarAlpha").Value;

            SpriteEffects spriteEffects = Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 position = ArmPosition - Main.screenPosition;

            float rotation = Projectile.rotation;
           
            Main.spriteBatch.Draw(texGrip, position, null, lightColor * (Timer < 15 ? Timer / 15f : 1f), rotation, texGrip.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            if (Progress > 0.15f && Progress < 0.75f)
            {
                float flareRotation = 0f;
                Vector2 flarePosition = Vector2.Zero;
                float fadeIn = 1f;

                if (Progress < 0.5f)
                {
                    float lerp = EaseBuilder.EaseCircularInOut.Ease((Progress - 0.15f) / 0.35f);

                    flarePosition = position + new Vector2(8 * Projectile.direction, MathHelper.Lerp(15, -15, lerp));
                    flareRotation = 2f * lerp;

                    fadeIn = lerp;
                }
                else
                {
                    float lerp = EaseBuilder.EaseQuinticIn.Ease((Progress - 0.5f) / 0.25f);

                    flarePosition = position + new Vector2(8 * Projectile.direction, MathHelper.Lerp(-15, 5, lerp));
                    flareRotation = 2f;
                }

                Main.spriteBatch.Draw(flareTexture, flarePosition, null, lightColor * fadeIn, rotation + flareRotation * Projectile.direction, flareTexture.Size() / 2f, Projectile.scale, spriteEffects, 0f);
            }

            Main.spriteBatch.Draw(texMuzzle, position, null, lightColor * (Timer < 15 ? Timer / 15f : 1f), rotation + muzzleRotation, texMuzzle.Size() / 2f, Projectile.scale, spriteEffects, 0f);    

            if (flashTimer > 0)
            {
                float lerp = flashTimer / 20f;

                Main.spriteBatch.Draw(star, position + new Vector2(8 * Projectile.direction, 0), null, new Color(255, 200, 50, 0) * lerp, 0f, star.Size() / 2f, new Vector2(MathHelper.Lerp(0.3f, 0.6f, 1f - lerp), MathHelper.Lerp(0.5f, 0.3f, 1f - lerp)), 0f, 0f);

                Main.spriteBatch.Draw(star, position + new Vector2(8 * Projectile.direction, 0), null, new Color(255, 255, 150, 0) * lerp, 0f, star.Size() / 2f, new Vector2(MathHelper.Lerp(0.3f, 0.6f, 1f - lerp), MathHelper.Lerp(0.5f, 0.3f, 1f - lerp)) * 0.6f, 0f, 0f);
            }

            return false;
        }

        /// <summary>
        /// Updates the basic variables needed for a held projectile
        /// </summary>
        protected virtual void UpdateHeldProjectile()
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;

            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2 + (Projectile.direction == -1 ? MathHelper.Pi : 0f));

            Projectile.position = ArmPosition - Projectile.Size * 0.5f;
        }
    }
}