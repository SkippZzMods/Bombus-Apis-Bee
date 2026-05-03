using BombusApisBee.BeeHelperProj;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Wulfrum
{
    public class WulfrumHoneycomb : CalamityDamageItem
    {
        internal static readonly SoundStyle prosthesisSound = CalamityMod.Items.Weapons.Magic.WulfrumProsthesis.ShootSound with { Pitch = 0.25f };

        public float shootRotation;
        public int shootDirection;

        public bool justAltUsed; // to prevent custom recoil anim

        public bool playedSound;

        public int chargeTimer;

        public Color ChargeColor => chargeTimer > 0 ? new Color(55, 180, 220) : new Color(130, 200, 70);
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Press <right> to empower the machination for a short time, at the cost of 20 honey");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 5;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<WulfrumBeeProjectile>();
            Item.shootSpeed = 6f;
            Item.UseSound = prosthesisSound;
            honeyCost = 4;
            altHoneyCost = 20;
            Item.noUseGraphic = true;
        }

        public override void UpdateInventory(Player player)
        {
            if (chargeTimer > 0)
                chargeTimer--;
        }

        public override bool CanUseItem(Player Player)
        {
            if (Player.altFunctionUse == 2)
            {
                Item.useTime = 75;
                Item.useAnimation = 75;
                Item.UseSound = null;

                justAltUsed = true;
                playedSound = false;
            }
            else
            {
                Item.useTime = 40;
                Item.useAnimation = 40;
                Item.UseSound = prosthesisSound;

                justAltUsed = false;
            }

            shootRotation = (Player.Center - Main.MouseWorld).ToRotation();
            shootDirection = Main.MouseWorld.X < Player.Center.X ? -1 : 1;

            return base.CanUseItem(Player);
        }

        public override bool AltFunctionUse(Player player) => chargeTimer <= 0 && player.Hymenoptra().BeeResourceCurrent >= 20;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (Item.noUseGraphic) // the item draws wrong for the first frame it is drawn when you switch directions for some odd reason, this plus setting it to true in shoot makes it not draw for the first frame.
                Item.noUseGraphic = false;

            float animProgress = 1f - player.itemTime / (float)player.itemTimeMax;

            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float itemRotation = player.compositeFrontArm.rotation + 1.5707964f * player.gravDir;
            Vector2 itemPosition = player.MountedCenter;

            if (justAltUsed)
            {
                float lerper = animProgress / 1f;
                itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(5f, -5f, EaseFunction.EaseQuinticInOut.Ease(lerper));

                itemPosition += Main.rand.NextVector2Circular(1f, 1f);

                float radius = 50f * (1f - lerper);
                Dust.NewDustPerfect(itemPosition + new Vector2(10f, -4f * player.direction).RotatedBy(itemRotation) + Main.rand.NextVector2CircularEdge(radius, radius), DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, ChargeColor, 0.35f);

                if (!playedSound && lerper > 0.9f && lerper < 1)
                {
                    CalamityMod.Items.Accessories.RoverDrive.BreakSound.PlayWith(player.Center);

                    for (int i = 0; i < 6; i++)
                    {
                        Dust.NewDustPerfect(itemPosition + new Vector2(10f, -4f * player.direction).RotatedBy(itemRotation), DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(9f, 9f), 0, ChargeColor, 0.45f);

                        Dust.NewDustPerfect(itemPosition + new Vector2(10f, -4f * player.direction).RotatedBy(itemRotation), DustType<ImpactLineDust>(), Main.rand.NextVector2Circular(8f, 8f), 0, ChargeColor with { A = 0 }, 0.1f);
                    }

                    player.Bombus().AddShake(4);

                    playedSound = true;
                }
            }
            else
            {
                if (animProgress < 0.05f)
                {
                    float lerper = animProgress / 0.05f;
                    itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(0f, -5f, EaseFunction.EaseCircularOut.Ease(lerper));
                }
                else
                {
                    float lerper = (animProgress - 0.05f) / 0.95f;
                    itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(-5f, 0f, EaseFunction.EaseBackInOut.Ease(lerper));
                }
            }

            Vector2 itemSize = new Vector2(28f, 30f);
            Vector2 itemOrigin = new Vector2(-10f, 4f);

            BeeUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, new Vector2?(itemOrigin), false, false, true);
        }

        public override void UseItemFrame(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float animProgress = 1f - player.itemTime / (float)player.itemTimeMax;
            float rotation = shootRotation * player.gravDir + 1.5707964f;

            if (!justAltUsed)
            {
                if (animProgress < 0.05f)
                {
                    float lerper = animProgress / 0.05f;
                    rotation += MathHelper.Lerp(0f, -.1f, EaseFunction.EaseCircularOut.Ease(lerper)) * player.direction;
                }
                else
                {
                    float lerper = (animProgress - 0.05f) / 0.95f;
                    rotation += MathHelper.Lerp(-.1f, 0, EaseFunction.EaseBackInOut.Ease(lerper)) * player.direction;
                }
            }

            Player.CompositeArmStretchAmount stretch = Player.CompositeArmStretchAmount.Full;

            if (justAltUsed)
            {
                if (animProgress > 0.85f)
                    stretch = Player.CompositeArmStretchAmount.None;
                else if (animProgress > 0.6f)
                    stretch = Player.CompositeArmStretchAmount.Quarter;
                else if (animProgress > 0.45f)
                    stretch = Player.CompositeArmStretchAmount.ThreeQuarters;
            }
            else
            {
                if (animProgress < 0.5f)
                    stretch = Player.CompositeArmStretchAmount.None;
                else if (animProgress < 0.75f)
                    stretch = Player.CompositeArmStretchAmount.ThreeQuarters;
            }

            player.SetCompositeArmFront(true, stretch, rotation);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item.noUseGraphic = true;

            if (player.altFunctionUse == 2)
            {
                //player.UseBeeResource(honeyCost * 4);
                chargeTimer = 600;
                return false;
            }

            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.3f), type, damage, knockback, player.whoAmI, ai2: chargeTimer > 0 ? 1f : 0f);
            }

            if (chargeTimer > 0)
            {
                for (int i = -1; i < 2; i++)
                {
                    Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(i * 0.2f) * 2.5f, ProjectileType<WulfrumHoneycombLaser>(), damage * 4, knockback, player.whoAmI);
                }
            }

            Vector2 barrelPos = position + new Vector2(16f, -4f * player.direction).RotatedBy(velocity.ToRotation());

            for (int i = 0; i < (chargeTimer > 0 ? 13 : 8); i++)
            {
                Dust.NewDustPerfect(barrelPos, DustType<GlowFastDecelerate>(), velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(1.35f), 0, ChargeColor, 0.4f);

                Dust.NewDustPerfect(barrelPos, DustType<ImpactLineDust>(), velocity.RotatedByRandom(0.8f) * Main.rand.NextFloat(0.45f, 1.5f), 0, ChargeColor with { A = 0 }, 0.1f);
            }

            //Dust.NewDustPerfect(barrelPos, ModContent.DustType<WulfrumSmokeDust>(), velocity * 0.25f, 135, ChargeColor with { A = 0 }, Main.rand.NextFloat(0.1f, 0.15f));

            player.Bombus().AddShake(2);

            return false;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class WulfrumBeeProjectile : BaseBeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public bool Supercharged => Projectile.ai[2] == 1f;
        public Color SuperchargeColor => Supercharged ? new Color(55, 180, 220) : new Color(130, 200, 70);
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Bee");
            Main.projFrames[Type] = 4;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, SuperchargeColor, 0.35f);

                Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), Main.rand.NextVector2CircularEdge(2.5f, 2.5f), 0, SuperchargeColor with { A = 0 }, 0.05f);
            }

            //Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<WulfrumSmokeDust>(), Main.rand.NextVector2Circular(.5f, .5f), 150, SuperchargeColor with { A = 0 }, Main.rand.NextFloat(0.1f, 0.15f));
        }

        public override bool SafePreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(Texture + (Supercharged ? "_Empowered" : "")).Value;
            Texture2D texBlur = Request<Texture2D>(Texture + (Supercharged ? "_Empowered" : "") + "_Blur").Value;
            Texture2D giantTex = Request<Texture2D>(Texture + (Supercharged ? "_Empowered" : "") + "_Giant").Value;
            Texture2D giantTexBlur = Request<Texture2D>(Texture + (Supercharged ? "_Empowered" : "") + "_Blur_Giant").Value;
            Texture2D drawTex = Giant ? giantTex : tex;
            Texture2D drawTexBlur = Giant ? giantTexBlur : texBlur;
            Texture2D glowTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, SuperchargeColor with { A = 0 }, 0f, glowTex.Size() / 2f, 0.25f, 0f, 0f);

            Rectangle sourceRectangle = drawTex.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
            Main.spriteBatch.Draw(drawTex, Projectile.Center - Main.screenPosition, sourceRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, sourceRectangle.Size() / 2f,
                Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            sourceRectangle = drawTexBlur.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
            Main.spriteBatch.Draw(drawTexBlur, Projectile.Center - Main.screenPosition, sourceRectangle, Projectile.GetAlpha(lightColor) with { A = 0 }, Projectile.rotation, sourceRectangle.Size() / 2f,
                Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            return false;
        }

        public override void SafeModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Supercharged)
            {
                modifiers.ArmorPenetration += 10;
                modifiers.SourceDamage *= 1.25f;
            }
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class WulfrumHoneycombLaser : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        internal Color PrimColorMult = Color.White;

        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Laser");

            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 4;
            Projectile.alpha = 255;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 100;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.965f;

            if (Main.rand.NextBool(10))
            {
                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(new CalamityMod.Particles.TechyHoloysquareParticle(Projectile.Center, Vector2.One.RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver4) * Main.rand.NextFloat(1f, 5f), Main.rand.NextFloat(2f, 3f), Main.rand.NextBool() ? new Color(99, 255, 229) : new Color(25, 132, 247), 25, 1f));

                Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), Vector2.One.RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver4) * Main.rand.NextFloat(1f, 4f), 0, new Color(55, 180, 220), 0.5f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(2, 5); i++)
            {
                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(new CalamityMod.Particles.TechyHoloysquareParticle(Projectile.Center, Main.rand.NextVector2CircularEdge(2.5f, 2.5f), Main.rand.NextFloat(2f, 3f), Main.rand.NextBool() ? new Color(99, 255, 229) : new Color(25, 132, 247), 25, 1f));

                Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), Main.rand.NextVector2CircularEdge(3f, 3f), 0, new Color(55, 180, 220, 0), 0.085f);

                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(new CalamityMod.Particles.TechyHoloysquareParticle(Projectile.Center, Vector2.One.RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver4).RotatedByRandom(0.3f) * Main.rand.NextFloat(1f, 6f), Main.rand.NextFloat(2f, 3f), Main.rand.NextBool() ? new Color(99, 255, 229) : new Color(25, 132, 247), 25, 1f));

                Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), Vector2.One.RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver4).RotatedByRandom(0.4f) * Main.rand.NextFloat(1f, 6f), 0, new Color(55, 180, 220, 0), 0.085f);
            }

            (CalamityMod.Items.Weapons.Magic.WulfrumProsthesis.HitSound with { MaxInstances = 0 }).PlayWith(Projectile.Center, volume: 0.45f);
        }

        // Credits to Calamity Mod
        internal Color ColorFunction(float completionRatio, Vector2 vertexPos)
        {
            float fadeOpacity = (float)Math.Sqrt((double)(1f - completionRatio));
            return Color.DeepSkyBlue.MultiplyRGB(PrimColorMult) * fadeOpacity;
        }

        // Credits to Calamity Mod
        internal float WidthFunction(float completionRatio, Vector2 vertexPos) => 9.4f;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glowTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowSoft").Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/Trails/BasicTrail", (ReLogic.Content.AssetRequestMode)2), 1);

            // Credit to Calamity Mod, WulfrumBolt projectile for trail drawing code

            CalamityUtils.DrawChromaticAberration(Vector2.UnitX, 3.5f, delegate (Vector2 offset, Color colorMod)
            {
                PrimColorMult = colorMod;
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new PrimitiveSettings(new PrimitiveSettings.VertexWidthFunction(WidthFunction), new PrimitiveSettings.VertexColorFunction(ColorFunction), (float x, Vector2 pos) => Projectile.Size * 0.5f + offset, true, false, GameShaders.Misc["CalamityMod:TrailStreak"], false), 30);
            });

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(55, 180, 220), 0f, glowTex.Size() / 2f, 0.35f, 0f, 0f);
            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(55, 180, 220) * 0.4f, 0f, glowTex.Size() / 2f, 0.75f, 0f, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center + new Vector2(0f, 5f * Projectile.direction).RotatedBy(Projectile.velocity.ToRotation()) - Main.screenPosition, null, Color.Blue, 0f, glowTex.Size() / 2f, 0.35f, 0f, 0f);
            Main.spriteBatch.Draw(glowTex, Projectile.Center + new Vector2(0f, 5f * Projectile.direction).RotatedBy(Projectile.velocity.ToRotation()) - Main.screenPosition, null, Color.Blue * 0.4f, 0f, glowTex.Size() / 2f, 0.75f, 0f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
