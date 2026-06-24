using BombusApisBee.Content.Crossmod.Calamity.Dusts;
using BombusApisBee.Content.Crossmod.Calamity.NPCs.Enemies.Wulfrum;
using BombusApisBee.Core.Systems.PrimitiveSystem;
using CalamityMod.DataStructures;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Desert
{
    public class ScourgedHoneycomb : CalamityDamageItem
    {
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<ScourgedHoneycombHoldout>() <= 0;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Hold <left> to charge up a powerful snapjaw attack\nFires a shotgun of deadly water when fully charged\n'Imbued with the power of the Scourge'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 22;
            Item.knockBack = 5f;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<ScourgedHoneycombHoldout>();
            Item.shootSpeed = 15f;
            honeyCost = 8;
            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class ScourgedHoneycombHoldout : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public bool flashed;

        public int flashTimer;

        public float shootingTimer;
        public bool CanHold => Owner.HeldItem.ModItem is ScourgedHoneycomb && Owner.channel && !Owner.CCed && !Owner.noItems;
        public ref float CurrentCharge => ref Projectile.ai[0];
        public ref float MaxCharge => ref Projectile.ai[1];
        public bool Shooting { get => Projectile.ai[2] == 1f; set { Projectile.ai[2] = value ? 1f : 0f; } }
        public Player Owner => Main.player[Projectile.owner];
        public Vector2? OwnerMouse => Main.myPlayer == Owner.whoAmI ? Main.MouseWorld : null;
        public override string Texture => "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/Desert/ScourgedHoneycomb";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scourged Honeycomb");
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.width = 28;
            Projectile.height = 36;

            Projectile.tileCollide = false;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity = Owner.DirectionTo(OwnerMouse.Value);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.netUpdate = true;

            MaxCharge = Owner.itemTime * 4;
        }

        public override void AI()
        {
            if (flashTimer > 0)
                flashTimer--;

            if (!CanHold && CurrentCharge >= 45)
            {
                if (!Shooting)
                {
                    Owner.Bombus().AddShake(8);
                    Owner.UseBeeResource(5);

                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Projectile.velocity, ProjectileType<ScourgedHoneycombHoldoutJaw>(), Projectile.damage, Projectile.knockBack, Projectile.owner, -1, CurrentCharge / MaxCharge);
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Projectile.velocity, ProjectileType<ScourgedHoneycombHoldoutJaw>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1, CurrentCharge / MaxCharge);

                        if (CurrentCharge >= MaxCharge)
                            for (int i = 0; i < 4; i++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Projectile.velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(6f, 10f), ProjectileType<ScourgedHoneycombBolt>(), (int)(Projectile.damage * 1.33f), Projectile.knockBack * 0.66f, Projectile.owner);
                            }
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 pos = Owner.Center + new Vector2(-60f, -40f).RotatedBy(Projectile.velocity.ToRotation());

                        Dust.NewDustPerfect(pos, DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(5f, 5f), 0, Color.DarkCyan, 0.45f);
                        Dust.NewDustPerfect(pos, DustType<ImpactLineDust>(), Main.rand.NextVector2Circular(8f, 8f), 0, Color.DarkCyan with { A = 0 }, 0.1f);

                        pos = Owner.Center + new Vector2(-60f, 40f).RotatedBy(Projectile.velocity.ToRotation());

                        Dust.NewDustPerfect(pos, DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(5f, 5f), 0, Color.DarkCyan, 0.45f);
                        Dust.NewDustPerfect(pos, DustType<ImpactLineDust>(), Main.rand.NextVector2Circular(8f, 8f), 0, Color.DarkCyan with { A = 0 }, 0.1f);
                    }

                    new SoundStyle("CalamityMod/Sounds/Custom/DesertScourge/DesertScourgeRoar").PlayWith(Owner.Center, 0.35f, 0f, 1.25f);

                    Shooting = true;
                }
            }
            else if (!Shooting && CurrentCharge < MaxCharge)
                CurrentCharge++;

            float lerper = CurrentCharge / MaxCharge;

            if (Shooting)
            {
                lerper = shootingTimer / 15f;
                if (lerper >= 1f)
                {
                    Projectile.Kill();
                    return;
                }
                else
                {
                    shootingTimer++;
                    Projectile.timeLeft = 2;
                    Projectile.Center = Owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * MathHelper.Lerp(MathHelper.Lerp(15f, 5f, CurrentCharge / MaxCharge), 15f, EaseFunction.EaseCubicInOut.Ease(lerper));
                }
            }
            else
            {
                float radius = 50f * (1f - lerper);

                Dust.NewDustPerfect(Owner.Center + new Vector2(-60f, -40f).RotatedBy(Projectile.velocity.ToRotation()) + Main.rand.NextVector2CircularEdge(radius, radius), DustID.Water, Main.rand.NextVector2Circular(3f, 3f), Main.rand.Next(150, 225), new Color(50, 255, 255, 0), 1.25f);
                Dust.NewDustPerfect(Owner.Center + new Vector2(-60f, 40f).RotatedBy(Projectile.velocity.ToRotation()) + Main.rand.NextVector2CircularEdge(radius, radius), DustID.Water, Main.rand.NextVector2Circular(3f, 3f), Main.rand.Next(150, 225), new Color(50, 255, 255, 0), 1.25f);

                Dust.NewDustPerfect(Owner.Center + new Vector2(-60f, -40f).RotatedBy(Projectile.velocity.ToRotation()) + Main.rand.NextVector2CircularEdge(radius, radius), DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, Color.DarkCyan, 0.45f);
                Dust.NewDustPerfect(Owner.Center + new Vector2(-60f, 40f).RotatedBy(Projectile.velocity.ToRotation()) + Main.rand.NextVector2CircularEdge(radius, radius), DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, Color.DarkCyan, 0.45f);

                Projectile.timeLeft = 2;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.DirectionTo(OwnerMouse.Value), 0.1f);
                Projectile.Center = Owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * MathHelper.Lerp(15f, 5f, lerper);

                if (lerper >= 1f && !flashed)
                {
                    flashed = true;
                    flashTimer = 15;
                    SoundID.MaxMana.PlayWith(Projectile.Center);
                }
            }

            Owner.ChangeDir(OwnerMouse.Value.X < Owner.Center.X ? -1 : 1);
            Owner.heldProj = Projectile.whoAmI;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D texGlow = Request<Texture2D>(Texture + "_Glowy").Value;
            Texture2D bloomTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            float fade = CurrentCharge / MaxCharge;
            if (Shooting)
                fade = MathHelper.Lerp(CurrentCharge / MaxCharge, 0f, shootingTimer / 15f);

            Main.spriteBatch.Draw(texGlow, Projectile.Center + new Vector2(0f, Owner.gfxOffY) - Main.screenPosition, null, Color.Blue with { A = 0 } * fade, Projectile.rotation, texGlow.Size() / 2f, 1f, 0f, 0f);

            float mainFade = 1f;
            if (Shooting)
                mainFade = MathHelper.Lerp(CurrentCharge / MaxCharge, 0f, shootingTimer / 15f);

            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(0f, Owner.gfxOffY) - Main.screenPosition, null, lightColor * mainFade, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            if (Shooting)
                Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(0f, Owner.gfxOffY) - Main.screenPosition, null, lightColor * mainFade, Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 2f, EaseFunction.EaseCircularInOut.Ease(shootingTimer / 15f)), 0f, 0f);

            Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

            effect.Parameters["offset"].SetValue(new Vector2(0.001f));
            effect.Parameters["repeats"].SetValue(2);
            effect.Parameters["uImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
            effect.Parameters["uImage2"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            Color color = Color.Blue with { A = 0 } * fade;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.Parameters["noiseImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(texGlow, Projectile.Center + new Vector2(0f, Owner.gfxOffY) - Main.screenPosition, null, Color.White, Projectile.rotation, texGlow.Size() / 2f, 1f, 0f, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center + new Vector2(0f, Owner.gfxOffY) - Main.screenPosition, null, Color.White, Projectile.rotation, bloomTex.Size() / 2f, 0.5f, 0f, 0f);

            Main.spriteBatch.Draw(bloomTex, Owner.Center + new Vector2(-60f, -40f).RotatedBy(Projectile.velocity.ToRotation()) - Main.screenPosition, null, Color.White, Projectile.rotation, bloomTex.Size() / 2f, MathHelper.Lerp(1f, 0.5f, fade), 0f, 0f);
            Main.spriteBatch.Draw(bloomTex, Owner.Center + new Vector2(-60f, 40f).RotatedBy(Projectile.velocity.ToRotation()) - Main.screenPosition, null, Color.White, Projectile.rotation, bloomTex.Size() / 2f, MathHelper.Lerp(1f, 0.5f, fade), 0f, 0f);

            color = Color.Cyan with { A = 0 } * fade;

            effect.Parameters["uColor"].SetValue(color.ToVector4());

            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(texGlow, Projectile.Center + new Vector2(0f, Owner.gfxOffY) - Main.screenPosition, null, Color.White, Projectile.rotation, texGlow.Size() / 2f, 1f, 0f, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center + new Vector2(0f, Owner.gfxOffY) - Main.screenPosition, null, Color.White, Projectile.rotation, bloomTex.Size() / 2f, 0.5f, 0f, 0f);

            Main.spriteBatch.Draw(bloomTex, Owner.Center + new Vector2(-60f, -40f).RotatedBy(Projectile.velocity.ToRotation()) - Main.screenPosition, null, Color.White, Projectile.rotation, bloomTex.Size() / 2f, MathHelper.Lerp(1f, 0.5f, fade), 0f, 0f);
            Main.spriteBatch.Draw(bloomTex, Owner.Center + new Vector2(-60f, 40f).RotatedBy(Projectile.velocity.ToRotation()) - Main.screenPosition, null, Color.White, Projectile.rotation, bloomTex.Size() / 2f, MathHelper.Lerp(1f, 0.5f, fade), 0f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            if (flashTimer > 0)
                Main.spriteBatch.Draw(texGlow, Projectile.Center + new Vector2(0f, Owner.gfxOffY) - Main.screenPosition, null, Color.Blue with { A = 0 } * (flashTimer / 15f), Projectile.rotation, texGlow.Size() / 2f, 1f, 0f, 0f);

            return false;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class ScourgedHoneycombHoldoutJaw : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public Vector2 originalCenter;
        public float Direction => Projectile.ai[0];
        public float ChargePercent => Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];
        public Vector2? OwnerMouse => Main.myPlayer == Owner.whoAmI ? Main.MouseWorld : null;
        public override string Texture => BombusApisBee.Invisible;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scourged Honeycomb");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.width = 48;
            Projectile.height = 48;

            Projectile.tileCollide = false;
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.timeLeft = 30;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void OnSpawn(IEntitySource source)
        {
            originalCenter = Owner.Center;
        }

        public override void AI()
        {
            float lerper = 1f - Projectile.timeLeft / 30f;

            Projectile.Center = GetBezierCurve().Evaluate(EaseFunction.EaseCubicInOut.Ease(lerper));

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustID.Water, Main.rand.NextVector2Circular(1f, 1f), Main.rand.Next(200), new Color(50, 255, 255, 0), 1.25f).noGravity = true;
            }

            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, Color.DarkCyan, 0.35f).noGravity = true;

            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, Color.Cyan, 0.35f).noGravity = true;

            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustType<ImpactLineDust>(), Projectile.oldPosition.DirectionTo(Projectile.position) * -Main.rand.NextFloat(2f), 0, Color.DarkCyan with { A = 0 }, 0.085f).noGravity = true;

            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustID.WaterCandle, Main.rand.NextVector2Circular(1f, 1f), Main.rand.Next(200), new Color(50, 255, 255, 0), 1.25f).noGravity = true;

            if (Main.rand.NextBool(5))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustType<WaterBubble>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(50, 255, 255, 0), Main.rand.NextFloat(0.25f, 0.5f));

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Owner.Bombus().AddShake(5);

            new SoundStyle("BombusApisBee/Sounds/Item/Impale").PlayWith(Projectile.Center, 0, 0f, 1f);

            for (int i = 0; i < 7; i++)
            {
                Vector2 velocity = Projectile.position.DirectionTo(Projectile.oldPosition).RotatedByRandom(0.5f);

                Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), velocity * Main.rand.NextFloat(15f), 0, Color.DarkCyan with { A = 0 }, 0.15f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustType<Glow>(), velocity * Main.rand.NextFloat(10f), 0, Color.DarkCyan, 0.65f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.Water, velocity * Main.rand.NextFloat(9f), Main.rand.Next(200), new Color(50, 255, 255, 0), 1.25f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Direction == -1)
            {
                new SoundStyle("BombusApisBee/Sounds/Item/LightSplash").PlayWith(Owner.Center, 0.1f, 0.25f, 1.25f);

                Owner.Bombus().AddShake(15);

                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(5f, 5f), 0, Color.DarkCyan, 0.5f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(5f, 5f), 0, Color.Cyan, 0.5f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), Main.rand.NextVector2Circular(8f, 8f), 0, Color.DarkCyan with { A = 0 }, 0.075f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), Main.rand.NextVector2Circular(8f, 8f), 0, Color.Blue with { A = 0 }, 0.075f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustType<WaterBubble>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(50, 255, 255, 0), Main.rand.NextFloat(0.25f, 0.5f));
                }

                for (int i = 0; i < 35; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustID.Water, Main.rand.NextVector2Circular(5f, 5f), Main.rand.Next(200), new Color(50, 255, 255, 0), 1.25f);
                }

                Dust.NewDustPerfect(Projectile.Center, DustType<WulfrumSmokeDust>(), Main.rand.NextVector2Circular(1f, 1f), 135, Color.DarkCyan with { A = 0 }, Main.rand.NextFloat(0.1f, 0.15f));

                Dust.NewDustPerfect(Projectile.Center, DustType<WulfrumSmokeDust>(), Main.rand.NextVector2Circular(1f, 1f), 135, Color.Blue with { A = 0 }, Main.rand.NextFloat(0.1f, 0.15f));

                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ProjectileType<ScourgedHoneycombExplosion>(), Projectile.damage * 2, Projectile.knockBack * 2, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bloomTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

            effect.Parameters["offset"].SetValue(new Vector2(0.001f));
            effect.Parameters["repeats"].SetValue(2);
            effect.Parameters["uImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
            effect.Parameters["uImage2"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
            effect.Parameters["noiseImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float lerper = i / (float)Projectile.oldPos.Length;

                Color color = Color.Blue with { A = 0 } * (1f - lerper);

                effect.Parameters["uColor"].SetValue(color.ToVector4());
                effect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(bloomTex, Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition, null, Color.White,
                    Projectile.rotation * i, bloomTex.Size() / 2f, MathHelper.Lerp(1f, 0.01f, lerper), 0, 0);

                color = Color.Cyan with { A = 0 } * (1f - lerper);

                effect.Parameters["uColor"].SetValue(color.ToVector4());
                effect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(bloomTex, Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition, null, Color.White,
                    Projectile.rotation * i, bloomTex.Size() / 2f, MathHelper.Lerp(0.65f, 0.25f, lerper), 0, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        private CalamityMod.DataStructures.BezierCurve GetBezierCurve()
        {
            Vector2[] curvePoints =
            {
                originalCenter + new Vector2(-60f, -40f * Direction).RotatedBy(Projectile.velocity.ToRotation()),
                originalCenter + new Vector2(0f, -60f * Direction).RotatedBy(Projectile.velocity.ToRotation()),
                originalCenter + new Vector2(50f, -80f * Direction).RotatedBy(Projectile.velocity.ToRotation()),
                originalCenter + new Vector2(MathHelper.Lerp(100f, 200f, ChargePercent), 0f).RotatedBy(Projectile.velocity.ToRotation())
            };

            CalamityMod.DataStructures.BezierCurve curve = new CalamityMod.DataStructures.BezierCurve(curvePoints);

            return curve;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class ScourgedHoneycombExplosion : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override string Texture => BombusApisBee.Invisible;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scourged Honeycomb Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.width = 96;
            Projectile.height = 96;

            Projectile.tileCollide = false;
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.timeLeft = 10;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class ScourgedHoneycombBolt : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        private List<Vector2> cache;
        private Trail trail;

        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 2;
            Projectile.alpha = 255;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water Shot");
        }

        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(9f, 9f), DustID.Water, -Projectile.velocity * 0.1f, Main.rand.Next(200), new Color(50, 255, 255, 0), 1.25f).noGravity = true;

            if (Main.rand.NextBool(7))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(9f, 9f), DustType<WaterBubble>(), -Projectile.velocity * 0.1f, 0, new Color(50, 255, 255, 0), Main.rand.NextFloat(0.25f, 0.5f));

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void OnKill(int timeLeft)
        {
            new SoundStyle("CalamityMod/Sounds/Custom/PistolShrimpBubbleBurst").PlayWith(Projectile.Center);

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, Color.DarkCyan, 0.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, Color.Cyan, 0.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), Main.rand.NextVector2Circular(4f, 4f), 0, Color.DarkCyan with { A = 0 }, 0.075f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), Main.rand.NextVector2Circular(4f, 4f), 0, Color.Blue with { A = 0 }, 0.075f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustType<WaterBubble>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(50, 255, 255, 0), Main.rand.NextFloat(0.25f, 0.5f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();

            Texture2D glowTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(0, 0, 255, 0) * 0.5f, 0f, glowTex.Size() / 2f, 0.3f, 0f, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(0, 50, 255, 0), 0f, glowTex.Size() / 2f, 0.25f, 0f, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(0, 100, 255, 0), 0f, glowTex.Size() / 2f, 0.15f, 0f, 0f);

            return false;
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 20; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center + Projectile.velocity);

            while (cache.Count > 20)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 20, new TriangularTip(1), factor => 8f, factor =>
            {
                return Color.Lerp(Color.Blue, Color.Cyan with { A = 0 }, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;
        }

        public void DrawPrimitives()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(2f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/LiquidTrail").Value);

            trail?.Render(effect);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
